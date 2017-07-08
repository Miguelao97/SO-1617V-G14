#pragma once

#include "../Include/jpg_searchservice.h"

#define MAX_OPER_SIZE 16
#define MAX_VALUE_SIZE	32

// struct describing a search expression
typedef struct SearchExp {
	DWORD tagNumber;
	CHAR oper[MAX_OPER_SIZE];
	CHAR value1[MAX_VALUE_SIZE];
	CHAR value2[MAX_VALUE_SIZE];
} SEARCH_EXP, *PSEARCH_EXP;

// QueryCmd context for JPG_ProcessExifTags use
typedef struct {
	LONG totalFiles;
	LIST_ENTRY filesList;
	SEARCH_EXP query;
} OP_CTX, *POP_CTX;

// the query_photos response list node
typedef struct fileNode {
	CHAR file[MAX_PATH];
	LIST_ENTRY link;
} FILENODE, *PFILENODE;

// destroy a list of generic elements
#define DestroyList(ListHead, NodeType, MemberName) do { \
	PLIST_ENTRY curr = (ListHead)->Flink; \
	while (curr != (ListHead)) { \
		PLIST_ENTRY next = curr->Flink; \
		free(CONTAINING_RECORD(curr, NodeType, MemberName)); \
		curr = next; \
	} \
} while(0)