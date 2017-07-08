#include "stdafx.h"

#include "JPEGExifUtils.h"

// adapted from serie2 (sequential version) photo searcher
VOID SearchFileDir(PCSTR path, PROCESS_EXIF_TAG processor, LPVOID ctx) {

	CHAR buffer[MAX_PATH];		// auxiliary buffer
								// the buffer is needed to define a match string that guarantees 
								// a priori selection for all files
	sprintf(buffer, "%s%s", path, "*.*");

	WIN32_FIND_DATAA fileData;
	HANDLE fileIt = FindFirstFileA(buffer, &fileData);
	if (fileIt == NULL) return;

	// Process directory entries
	do {
		if ((fileData.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY) != 0) {
			// Not processing "." and ".." files!
			if (strcmp(fileData.cFileName, ".") && strcmp(fileData.cFileName, "..")) {
				sprintf(buffer, "%s%s/", path, fileData.cFileName);
				// Recursively process child directory
				SearchFileDir(buffer, processor, ctx);
			}
		}
		else {
			// Process file archive
			CHAR filepath[MAX_PATH];
			sprintf(filepath, "%s%s", path, fileData.cFileName);
			JPG_CTX jpgCtx = { filepath, ctx };
			JPEG_ProcessExifTagsA(filepath, processor, &jpgCtx);
		}
	} while (FindNextFileA(fileIt, &fileData) == TRUE);

	FindClose(fileIt);

}

 

