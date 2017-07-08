/*  
 * This module contains the  service comand handlers 
 */

#include "stdafx.h"

#include "../Include/list.h"
#include "Connection.h"

#include "JPEGExifUtils.h"
#include "tagEvaluators.h"

extern PCHAR Repository; // the repository folder

// maximum line size
#define MAXSIZE 1024

typedef BOOL(*MessageProcessor)(PCONNECTION cn);

static void ResponseError(PCONNECTION cn, int status);
static void ResponseValid(PCONNECTION cn, PLIST_ENTRY filesList);
static void ResponseValid(PCONNECTION cn);
VOID SearchFileDir(PCSTR path, PROCESS_EXIF_TAG processor, LPVOID ctx);

// Construct a SEARCH_EXP struct  
VOID InitSearchExpr(PSEARCH_EXP exp, DWORD tag, LPCSTR oper, LPCSTR value1, LPCSTR value2) {
	exp->tagNumber = tag;
	strcpy(exp->oper, oper);
	if (value1 != NULL) strcpy(exp->value1, value1);
	if (value2 != NULL) strcpy(exp->value2, value2);
}

// Create a jpg search context
POP_CTX CreateJPGContext() {
	POP_CTX ctx = (POP_CTX) malloc(sizeof(OP_CTX));
	InitializeListHead(&ctx->filesList);
	ctx->totalFiles = 0;
	return ctx;
}

// Destroy a jpg search context
static VOID DestroyJPGContext(POP_CTX ctx) {
	// destroy filesList
	DestroyList(&ctx->filesList, FILENODE, link);

}

// add a file to the result list of the query command
VOID appendFile(PLIST_ENTRY filesList, PCSTR file) {
	PFILENODE node = (PFILENODE)malloc(sizeof(FILENODE));

	strcpy(node->file,file+strlen(Repository));
	InsertTailList(filesList, &node->link);
}



// the supported tags registry
// For now just two tags supported
#define TAG_DATE_TAKEN 306  
#define TAG_APERTURE 33437

static struct EvaluatorsRegistration {
	DWORD tag;
	TagEval *processor;
}  evaluatorRegistry[] = {
	{ TAG_DATE_TAKEN, new TagDateTaken() },
	{ TAG_APERTURE, new TagAperture() },
	{ NULL, NULL }
};


// Try to evaluate search expression on the specified tag/value pair
BOOL select(INT tag, LPVOID value, PSEARCH_EXP oper) {
	struct EvaluatorsRegistration *curr = evaluatorRegistry;
	while (curr->processor != NULL) {
		if (curr->tag == tag) {
			return curr->processor->process(value, oper);
		}
		curr++;
	}
	return false;
}

// Callback to JPG_ProcessExifTags
BOOL ProcessExifTag(LPCVOID ctx, DWORD tag, LPVOID value) {
	PCSTR filePath = ((PJPG_CTX)ctx)->filepath;
	POP_CTX opCtx = (POP_CTX) ((PJPG_CTX)ctx)->opCtx;	
	PSEARCH_EXP query = &opCtx->query;
	 
	if (query->tagNumber == tag && select(tag, value, query)) {
		appendFile(&opCtx->filesList, filePath);
	}
	return TRUE;
}

 
/*
 * Handle to process a Query Command
 * Format:
 *   QUERY_PHOTOS
 *   <tagNumber> { contains | { = | > | < } <value>  | { between <value1> <value2> } } CRLF  
 * Response:
 *   { <filename> CRLF }*
 *   CRLF
 */
static BOOL ProcessQueryCmd(PCONNECTION cn) {
	char line[MAXSIZE];
	int lineSize;

	if ((lineSize = ConnectionGetLineAsync(cn, line, MAXSIZE)) > 0) {
		POP_CTX ctx = CreateJPGContext();
		char *parms[4] = { 0 };
		int tag;

		int nParms = SplitLine(line, parms, ' ', 4);
		if (nParms < 2 || (tag = atoi(parms[0])) == 0) {
			DestroyJPGContext(ctx);
			ResponseError(cn, STATUS_INVALID_CMD);
			return FALSE;
		}
#ifdef _DEBUG
		printf("Start processing cmd Query_Photos\n");
#endif
		InitSearchExpr(&ctx->query, tag, parms[1], parms[2], parms[3]);
		SearchFileDir(Repository, ProcessExifTag, ctx);

		ResponseValid(cn, &ctx->filesList);
		DestroyJPGContext(ctx);
		return TRUE;
	}
	return FALSE;	
}

/*
* Handle to process an GetPhoto Command.
* Format:
*   GET_PHOTO
*   <fileurl> CRLF
* Response:
*   <status> CRLF
*   <file size> CRLF
*   <file bytes...>
*/
static BOOL ProcessGetPhotoCmd(PCONNECTION cn) {
	char line[MAXSIZE];
	char filePath[MAX_PATH];
	int lineSize;

	if ((lineSize = ConnectionGetLine(cn, line, MAXSIZE)) > 0)
	{
#ifdef _DEBUG
		printf("Start processing cmd Get_Photo\n");
#endif
		sprintf(filePath, "%s%s", Repository, line);
		HANDLE fh = CreateFileA(
					filePath,
					GENERIC_READ, 
					FILE_SHARE_READ, 
					NULL, 
					OPEN_EXISTING,
					FILE_ATTRIBUTE_NORMAL, 
					NULL);
		if (fh == INVALID_HANDLE_VALUE) {
			ResponseError(cn, STATUS_PHOTO_NOT_FOUND);
			return FALSE;
		}
		LARGE_INTEGER fs;
		if (!GetFileSizeEx(fh, &fs)) {
			CloseHandle(fh); 
			ResponseError(cn, STATUS_INTERNAL_ERROR);
			return FALSE;
		}
		ResponseValid(cn);
		BOOL res= ConnectionCopyFile(cn, fh, fs.LowPart); // for files until 2GB
		CloseHandle(fh);
		return res;
	}
	return TRUE;
}

/**
* Response for errors
*/
static void ResponseError(PCONNECTION cn, int status) {
#ifdef _DEBUG
	printf("Response error %d returned!\n", status);
#endif
	ConnectionPutInt(cn, status);
	ConnectionPutLineEnd(cn);
	ConnectionFlushBufferToSocket(cn);
}

/*
 * Response with file lsit
 */
static void ResponseValid(PCONNECTION cn, PLIST_ENTRY filesList) {
#ifdef _DEBUG
	printf("A (possibly empty) list returned for query cmd!\n");
#endif
	ConnectionPutInt(cn, STATUS_OK);
	ConnectionPutLineEnd(cn);

	PLIST_ENTRY curr =  filesList->Flink;
	while (curr != filesList) {
		PFILENODE node = CONTAINING_RECORD(curr, FILENODE, link);
		ConnectionPutString(cn, node->file);
		ConnectionPutLineEnd(cn);
		curr = curr->Flink;
	}
	ConnectionPutLineEnd(cn);
	ConnectionFlushBufferToSocket(cn);
}

static void ResponseValid(PCONNECTION cn) {
#ifdef _DEBUG
	printf("Response Ok!\n");
#endif
	ConnectionPutInt(cn, STATUS_OK);
	ConnectionPutLineEnd(cn);
	ConnectionFlushBufferToSocket(cn);
}

// the commands handler registry
static struct ProcessorRegistration {
	char *msgType;
	MessageProcessor processor;
}  processorRegistry[] = {
	{ QUERY_CMD, ProcessQueryCmd },
	{ GET_PHOTO_CMD, ProcessGetPhotoCmd },
	{ NULL, NULL }
};

static MessageProcessor processorForMessageType(char *msgType) {
	struct ProcessorRegistration *currRegist = processorRegistry;

	while (currRegist->msgType != NULL) {
		if (!strcmp(msgType, currRegist->msgType))
			return currRegist->processor;
		currRegist++;
	}
	return NULL;
}


BOOL ProcessRequest(PCONNECTION cn) {
	char requestType[MAXSIZE];

	int lineSize;
	
	if ((lineSize = ConnectionGetLineAsync(cn, requestType, MAXSIZE)) > 0)
	{
		MessageProcessor processor;
		ToUpper(requestType);
		if ((processor = processorForMessageType(requestType)) == NULL || !processor(cn))
		{
#ifdef _DEBUG
			printf("Invalid command received: %s\n", requestType);
#endif
			cn->rPos = 0;
			return FALSE;
		}
	 
	}
	return lineSize > 0;
}