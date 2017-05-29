#include <Windows.h>
#include <stdio.h>
#include <assert.h>
#include "../Include/JPG_SearchService.h"
#include "../Include/JPEGExifUtils.h"

// Struct to hold context to JPG_ProcessExifTags
typedef struct {
	PCSTR filepath;				// Name to collect
	PCSTR searchArgs;			// Filter to use in tag comparation
} JPG_CTX, *PJPG_CTX;


BOOL ProcessExifTag(LPCVOID ctx, DWORD tag, LPVOID value) {
	PCSTR searchArgs = ((PJPG_CTX)ctx)->searchArgs;
	DWORD tagToFind = atoi((PCHAR)searchArgs);
	BOOL bRes = tagToFind == tag;
	if (bRes) {
		PCSTR filepath = ((PJPG_CTX)ctx)->filepath;
		printf("%s\n", filepath); 
	}
	return !bRes;
}

// @param ctx No need for this implementation.
static VOID SearchFileDir(PCSTR path, PCSTR searchArgs, LPVOID ctx) {

	CHAR buffer[MAX_PATH];		// auxiliary buffer
	// the buffer is needed to define a match string that guarantees 
	// a priori selection for all files
	sprintf_s(buffer, "%s%s", path, "*.*");

	WIN32_FIND_DATA fileData;
	HANDLE fileIt = FindFirstFile(buffer, &fileData);
	if (fileIt == INVALID_HANDLE_VALUE) return;

	// Process directory entries
	do {
		if ((fileData.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY) != 0) {
			// Not processing "." and ".." files!
			if (strcmp(fileData.cFileName, ".") && strcmp(fileData.cFileName, "..")) {
				sprintf_s(buffer, "%s%s/", path, fileData.cFileName);
				// Recursively process child directory
				SearchFileDir(buffer, searchArgs, ctx);
			}
		}
		else {
			CHAR filepath[MAX_PATH];
			sprintf_s(filepath, "%s%s", path, fileData.cFileName);
			JPG_CTX jpgCtx = { filepath, searchArgs };
			JPEG_ProcessExifTags(filepath, ProcessExifTag, &jpgCtx);
		}
	} while (FindNextFile(fileIt, &fileData) == TRUE);

	FindClose(fileIt);

}

LPVOID ProcessRepository(PCSTR repository, PCSTR filter) {
	 
	SearchFileDir(repository, filter, NULL);

	return NULL;
}

INT main(INT argc, PCHAR argv[]) {
	if (argc < 2) {
		printf("Use: %s <service name>", argv[0]);
		exit(0);
	}

	PCHAR serviceName = argv[1];

	PJPG_SEARCH_SERVICE service = JPG_SearchServiceCreate(serviceName);
	if (service == NULL) return 0;

	JPG_SearchServiceProcess(service, ProcessRepository);
	JPG_SearchServiceClose(service);

	return 0;
}