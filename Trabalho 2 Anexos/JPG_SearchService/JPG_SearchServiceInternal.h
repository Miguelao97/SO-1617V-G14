// for service internal use

#pragma once

#include <Windows.h>

// Dimensions

#define MAX_MSG_SIZE 127
#define MAX_ANSWER_SIZE (MAX_MSG_SIZE + 50)
#define MAX_SLOTS 128

// Internal commands and command args
#define SEARCH_CMD		1	
#define FREE_MAP_CMD	2	

typedef struct searchCmdArgs {
	PCSTR repository, filter;
} SEARCH_CMD_ARGS, *PSEARCH_CMD_ARGS;


// the request
typedef struct JPG_SearchServiceRequest {
	WORD	Cmd;
	union {	// cmds args
		struct {
			CHAR	Repository[MAX_ANSWER_SIZE];
			CHAR	Filter[MAX_ANSWER_SIZE];
		};
		HANDLE	MapHandle;
	};

	DWORD   AnswerIndex;
	HANDLE	ClientEvent;
} JPG_SEARCH_SERVICE_REQUEST, *PJPG_SEARCH_SERVICE_REQUEST;


typedef struct JPG_SearchServiceAnswer {
	BOOL Used;
	HANDLE MapFile;	 // Server side: Map section object to answer
}  JPG_SEARCH_SERVICE_ANSWER, *PJPG_SEARCH_SERVICE_ANSWER;



/* this is in shared memory */
typedef struct JPG_SearchServiceShared {
	/* identificador do processo servidor */
	DWORD serverProcId;
	int putIndex;
	int getIndex;
	JPG_SEARCH_SERVICE_REQUEST entries[MAX_SLOTS];
	JPG_SEARCH_SERVICE_ANSWER  answers[MAX_SLOTS];
} JPG_SEARCH_SERVICE_SHARED, *PJPG_SEARCH_SERVICE_SHARED;
