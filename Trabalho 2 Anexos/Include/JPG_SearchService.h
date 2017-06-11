// publi service API

#pragma once

#include <Windows.h>

#ifdef  JPG_SEARCH_SERVICE_EXPORTS
#define JPG_SEARCH_API __declspec(dllexport)
#else
#define JPG_SEARCH_API __declspec(dllimport)
#endif



/* this is private to each client and the server */
typedef struct JPG_SearchService {
	 
	// handle for answer event used by client
    HANDLE answerEvent; 

	// handle for answer event used by server
	HANDLE answerEventForServer;

	// semaphore that indicates that there are avaiable answer space
	HANDLE avaiableAnswerSpace;

	// Handle for proce server
	HANDLE serverProcHandle;

	// tells that service is usable
	BOOL opened;

	// handle for shared memory region
	HANDLE mapHandle;

	// semaphore that indicates that there are avaiable req space
	HANDLE avaiableReqSpace;

	// semaphore that indicates that there are avaiable request items
	HANDLE avaiableRequests;

	// mutex to mutual exclusive access  to log 
	HANDLE mutex;

	//the internal shared state (in shared memory)
	PVOID  shared;
} JPG_SEARCH_SERVICE, *PJPG_SEARCH_SERVICE;

/*
process entry callback
*/
typedef LPVOID(*PROCESS_ENTRY_FUNC)(PCSTR repository, PCSTR filter);

#ifdef __cplusplus
extern "C" {
#endif

	/*
	  Funtions to JPG Search Service clients
	  */


	JPG_SEARCH_API
		PCHAR JPG_SearchServiceProcessRepository(PJPG_SEARCH_SERVICE service, PCSTR repository, PCSTR filter);

	JPG_SEARCH_API
		VOID JPG_SearchReleaseAnswer(PCHAR answer);

	JPG_SEARCH_API
		PJPG_SEARCH_SERVICE JPG_SearchServiceOpen(PCHAR serviceName);

	/*
	  Funtions to Search Service server
	  */

	JPG_SEARCH_API
		PJPG_SEARCH_SERVICE JPG_SearchServiceCreate(PCHAR serviceName);


	JPG_SEARCH_API
		VOID JPG_SearchServiceProcess(PJPG_SEARCH_SERVICE service, PROCESS_ENTRY_FUNC processor);
	/*
		Miscelaneous
		*/
	JPG_SEARCH_API
		VOID JPG_SearchServiceClose(PJPG_SEARCH_SERVICE service);
	
	

#ifdef __cplusplus
}
#endif

