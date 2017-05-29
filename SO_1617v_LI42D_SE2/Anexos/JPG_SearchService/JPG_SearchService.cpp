// JPG_SearchService.cpp : Defines the exported functions for the DLL application.
//
#include <windows.h>
#include <crtdbg.h>
#include <stdlib.h>
#include <stdio.h>
#include "../Include/JPG_SearchService.h"
#include "JPG_SearchServiceInternal.h"

/* Name prefix for named  synchronization Objects */


// avaiable answer space semaphore
#define AVAIL_ANSWERSPACE_SEM "Sem_AvailAnswerSpace"

// avaiable req semaphore
#define AVAIL_REQ_SEM "Sem_AvailReq"

// avaiable req space semaphore
#define AVAIL_REQSPACE_SEM "Sem_AvailReqSpace"

// mutex for shared data exlusive access
#define LOG_MUTEX "Mutex"
 
/*auxiliary methods for create/open synchronizers */
static
FORCEINLINE
HANDLE CreateSem(CHAR *echoServiceName, CHAR* semName, ULONG init, ULONG max) {
	CHAR name[MAX_PATH];
 
	sprintf_s(name, MAX_PATH, "%s_%s", echoServiceName, semName);
	return CreateSemaphore(NULL, init, max, name);
}

static
FORCEINLINE
HANDLE OpenSem(CHAR *echoServiceName, CHAR* semName) {
	CHAR name[MAX_PATH];
 
	sprintf_s(name, MAX_PATH, "%s_%s", echoServiceName, semName);
	return OpenSemaphore(SEMAPHORE_ALL_ACCESS , FALSE, name);
}

static
FORCEINLINE
HANDLE CreateMtx(CHAR *echoServiceName, CHAR* mutexName) {
	CHAR name[MAX_PATH];
	 
	sprintf_s(name, MAX_PATH, "%s_%s", echoServiceName, mutexName);
	return CreateMutex(NULL,0, name);
}

static
FORCEINLINE
HANDLE OpenMtx(CHAR *echoServiceName, CHAR* mutexName) {
	CHAR name[MAX_PATH];
 
	sprintf_s(name, MAX_PATH, "%s_%s", echoServiceName, mutexName);
	return OpenMutex(MUTEX_ALL_ACCESS, FALSE, name);
}

static VOID JPG_SearchServiceDestroy(PJPG_SEARCH_SERVICE service) {
	if (service->answerEvent != NULL) CloseHandle(service->answerEvent);
	if (service->mapHandle != NULL) CloseHandle(service->mapHandle);
	if (service->shared != NULL) UnmapViewOfFile(service->shared);
	if (service->avaiableRequests != NULL) CloseHandle(service->avaiableRequests);
	if (service->serverProcHandle != NULL) CloseHandle(service->serverProcHandle);
	if (service->mutex != NULL) CloseHandle(service->mutex);
	service->opened = FALSE;
	free(service);
}

static DWORD AllocateAnswerSlot(PJPG_SEARCH_SERVICE service)
{
	PJPG_SEARCH_SERVICE_SHARED buf = (PJPG_SEARCH_SERVICE_SHARED) service->shared;
	PJPG_SEARCH_SERVICE_ANSWER answers = buf->answers;
	DWORD freeSlot = -1;

	// wait for free slot
	WaitForSingleObject(service->avaiableAnswerSpace, INFINITE);

	// select free slot 
	WaitForSingleObject(service->mutex, INFINITE);
	for (int i = 0; i < MAX_SLOTS; ++i)
	{
		if (!answers[i].Used)
		{
			freeSlot = i;
			answers[i].Used = TRUE;
			break;
		}
	}
	ReleaseMutex(service->mutex);
	_ASSERTE(freeSlot != -1);
	return freeSlot;
}


VOID FreeAnswerSlot(PJPG_SEARCH_SERVICE service, DWORD answerIndex)
{
	PJPG_SEARCH_SERVICE_SHARED buf = (PJPG_SEARCH_SERVICE_SHARED)service->shared;
	PJPG_SEARCH_SERVICE_ANSWER answers = buf->answers;
	_ASSERT(answers[answerIndex].Used == TRUE);

	answers[answerIndex].Used = FALSE;
	ReleaseSemaphore(service->avaiableAnswerSpace, 1, NULL);
}



/* Add a request */
static INT JPG_SearchServiceProcessRepository_internalCmd(PJPG_SEARCH_SERVICE service, int cmd, PVOID args) {
	DWORD answerIndex = 0;
	PJPG_SEARCH_SERVICE_REQUEST request;
	PJPG_SEARCH_SERVICE_SHARED buf = (PJPG_SEARCH_SERVICE_SHARED) service->shared;

	

	if (cmd == SEARCH_CMD) {	// request needs answer, allocate one answer slot!
		answerIndex = AllocateAnswerSlot(service);
	}

	WaitForSingleObject(service->avaiableReqSpace, INFINITE);
	WaitForSingleObject(service->mutex, INFINITE);

	request = &buf->entries[buf->putIndex];

	buf->putIndex = (buf->putIndex + 1) % MAX_SLOTS;

	switch (cmd) {
		case SEARCH_CMD: {
			PSEARCH_CMD_ARGS sargs = (PSEARCH_CMD_ARGS)args;
			request->AnswerIndex = answerIndex;
			strcpy_s(request->Repository, MAX_MSG_SIZE, sargs->repository);
			strcpy_s(request->Filter, MAX_MSG_SIZE, sargs->filter);
			break;
		}
		case FREE_MAP_CMD:
			break;
	}
	request->Cmd = cmd;
	request->ClientEvent = service->answerEventForServer;

	ReleaseMutex(service->mutex);
	ReleaseSemaphore(service->avaiableRequests, 1, NULL);
	return answerIndex;
}

static VOID FreeAnswerCmd(PJPG_SEARCH_SERVICE service, HANDLE MapHandle) {
	JPG_SearchServiceProcessRepository_internalCmd(service, FREE_MAP_CMD, MapHandle);
}

PCHAR RetrieveAnswer(PJPG_SEARCH_SERVICE service, int answerIndex) {
	PJPG_SEARCH_SERVICE_SHARED buf = (PJPG_SEARCH_SERVICE_SHARED) service->shared;
	DWORD res = 0;

	// wait for answer
	WaitForSingleObject(service->answerEvent, INFINITE);
	
	FreeAnswerSlot(service, answerIndex);

	return NULL;
}



PCHAR JPG_SearchServiceProcessRepository(PJPG_SEARCH_SERVICE service, PCSTR repository, PCSTR filter) {
	PJPG_SEARCH_SERVICE_SHARED buf = (PJPG_SEARCH_SERVICE_SHARED) service->shared;
	SEARCH_CMD_ARGS args = { repository, filter };

	// validate server state
	if (!service->opened || buf->serverProcId == 0) return NULL;

	// validate msg parameters
	if (repository == NULL || strlen(repository) > MAX_MSG_SIZE) return FALSE;

	INT answerIndex = JPG_SearchServiceProcessRepository_internalCmd(service, SEARCH_CMD, &args);

	// wait and return response
	return RetrieveAnswer(service, answerIndex);
}
 

/* Access  echo service */
PJPG_SEARCH_SERVICE JPG_SearchServiceOpen(PCHAR serviceName) {
	HANDLE procHandle=NULL;
	PJPG_SEARCH_SERVICE service = (PJPG_SEARCH_SERVICE)calloc(1, sizeof(JPG_SEARCH_SERVICE));

	if (service == NULL) return NULL;
	 
	// Open shared memory region
	service->mapHandle = OpenFileMapping(FILE_MAP_ALL_ACCESS, FALSE, serviceName);
	if (service->mapHandle == NULL)
		goto error;
	 
	if ((service->shared =  
		MapViewOfFile(service->mapHandle, FILE_MAP_WRITE, 0, 0, 0)) == NULL)
		goto error;

	PJPG_SEARCH_SERVICE_SHARED buf = (PJPG_SEARCH_SERVICE_SHARED)service->shared;

	// service is not activated
	if (buf->serverProcId == 0)  
		goto error;

	//Open Avaiable Items Semaphore
	if ((service->avaiableRequests = OpenSem(serviceName, AVAIL_REQ_SEM)) == NULL)
		goto error;
	
	//Open Avaiable Space Semaphore
	if ((service->avaiableReqSpace = OpenSem(serviceName, AVAIL_REQSPACE_SEM)) == NULL)
		goto error;

	// Create Mutex
	if ((service->mutex = OpenMtx(serviceName, LOG_MUTEX)) == NULL)
		goto error;


	// Create answer Event
	if ((service->answerEvent = CreateEvent(NULL,FALSE,FALSE,NULL)) == NULL)
		goto error;
	// Create Handle to server
	if ((procHandle=
		OpenProcess(PROCESS_DUP_HANDLE, FALSE, buf->serverProcId)) == NULL)
			goto error;
	// Save server proc handle
	service->serverProcHandle = procHandle;

	// Create answer space semaphore
	if ((service->avaiableAnswerSpace = CreateSem(serviceName, AVAIL_REQSPACE_SEM, MAX_SLOTS, MAX_SLOTS)) == NULL)
		goto error;

	// duplicate handle to answer event used by server 
	if (!DuplicateHandle(
			GetCurrentProcess(),			// original process
			service->answerEvent,			// original handle
		    procHandle,						// destination process (server)
			&service->answerEventForServer, // event handle for server
			0,								// desired access
			FALSE,							// not inheritable
			DUPLICATE_SAME_ACCESS))			// same access permissions as original
		goto error;

	// Mark service as opened!
	service->opened = TRUE;

	CloseHandle(procHandle);
	return service;

error:
	JPG_SearchServiceDestroy(service);
	return NULL;
}


VOID JPG_SearchServiceProcess(PJPG_SEARCH_SERVICE service, PROCESS_ENTRY_FUNC processor) {
	PJPG_SEARCH_SERVICE_SHARED buf = (PJPG_SEARCH_SERVICE_SHARED) service->shared;

	while (TRUE) {
		WaitForSingleObject(service->avaiableRequests, INFINITE);
		WaitForSingleObject(service->mutex, INFINITE);
		// Makes a copy of request to get out of exclusion quickly (before processing request)
		JPG_SEARCH_SERVICE_REQUEST _req = buf->entries[buf->getIndex];
		PJPG_SEARCH_SERVICE_REQUEST req = &_req;
		buf->getIndex = (buf->getIndex + 1) % MAX_SLOTS;

		ReleaseMutex(service->mutex);
		ReleaseSemaphore(service->avaiableReqSpace, 1, NULL);

		// process commands
		if (req->Cmd == SEARCH_CMD) {
			// Normal resquest; process request
			PCHAR answer =	(PCHAR)processor(req->Repository,req->Filter);
			// Signal client that has answer
			SetEvent(req->ClientEvent);
			// Free resources 
			CloseHandle(req->ClientEvent);
			free(answer);
		}
		else if (req->Cmd == FREE_MAP_CMD) {
			;
		}
	}
}

VOID JPG_SearchReleaseAnswer(PCHAR answer) {
	free(answer);
}

PJPG_SEARCH_SERVICE JPG_SearchServiceCreate(PCHAR serviceName) {
	PJPG_SEARCH_SERVICE service = (PJPG_SEARCH_SERVICE)calloc(1, sizeof(JPG_SEARCH_SERVICE));
	if (service == NULL) return NULL;

	// Create shared memory region
	service->mapHandle = CreateFileMapping(INVALID_HANDLE_VALUE,
		NULL,
		PAGE_READWRITE,
		0,
		sizeof(JPG_SEARCH_SERVICE_SHARED),
		serviceName);
	if (service->mapHandle == NULL)
			goto error;

	service->shared =  MapViewOfFile(service->mapHandle, FILE_MAP_WRITE, 0, 0, 0);
	if (service->shared == NULL)
		goto error;
	PJPG_SEARCH_SERVICE_SHARED buf = (PJPG_SEARCH_SERVICE_SHARED)service->shared;

	//Create Avaiable Items Semaphore
	if ((service->avaiableRequests = CreateSem(serviceName, AVAIL_REQ_SEM, 0, MAX_SLOTS)) == NULL)
		goto error;
	 
	//Create Avaiable Space Semaphore
	if ((service->avaiableReqSpace = CreateSem(serviceName, AVAIL_REQSPACE_SEM, MAX_SLOTS, MAX_SLOTS)) == NULL)
		goto error;

	// Create Mutex
	if ((service->mutex = CreateMtx(serviceName, LOG_MUTEX)) == NULL)
		goto error;
    
	service->answerEvent = NULL;
	service->answerEventForServer = NULL;
	service->avaiableAnswerSpace = NULL;
	buf->serverProcId = GetCurrentProcessId();
	service->opened=TRUE;
	return service;
error:
	JPG_SearchServiceDestroy(service);
	return NULL;
}

VOID JPG_SearchServiceClose(PJPG_SEARCH_SERVICE service) {
	PJPG_SEARCH_SERVICE_SHARED buf = (PJPG_SEARCH_SERVICE_SHARED)service->shared;

	if (!service->opened) return;
	if (GetCurrentProcessId() == buf->serverProcId) {
		buf->serverProcId = 0;
	}
	JPG_SearchServiceDestroy(service);
}
