#include <Windows.h>
#include <tchar.h>
#include <stdio.h>
#include <assert.h>
#include <process.h>
#include "../Include/JPEGExifUtils.h"
#include "../Include/PrintUtils.h"

// Struct to hold context to JPG_ProcessExifTags
typedef struct {				
	PCTSTR filepath;
	PCTSTR searchArgs;
} JPG_CTX, *PJPG_CTX;

// Callback to JPG_ProcessExifTags
BOOL ProcessExifTag(LPCVOID ctx, DWORD tag, LPVOID value) {
	PCTSTR searchArgs = ((PJPG_CTX)ctx)->searchArgs;
	DWORD tagToFind = _ttoi(searchArgs);
	BOOL bRes = tagToFind == tag;
	if (bRes) {
		PCTSTR filepath = ((PJPG_CTX)ctx)->filepath;
		_tprintf("%s\n", filepath); 
	}
	return !bRes;
}

VOID SearchFileDir(PCTSTR path, PCTSTR searchArgs, LPVOID ctx) {

	TCHAR buffer[MAX_PATH];		// auxiliary buffer
	// the buffer is needed to define a match string that guarantees 
	// a priori selection for all files
	_stprintf_s(buffer, _T("%s%s"), path, _T("*.*"));

	WIN32_FIND_DATA fileData;
	HANDLE fileIt = FindFirstFile(buffer, &fileData);
	if (fileIt == INVALID_HANDLE_VALUE) return;

	// Process directory entries
	do {
		if ((fileData.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY) != 0) {
			// Not processing "." and ".." files!
			if (_tcscmp(fileData.cFileName, _T(".")) && _tcscmp(fileData.cFileName, _T(".."))) {
				_stprintf_s(buffer, _T("%s%s/"), path, fileData.cFileName);
				// Recursively process child directory
				SearchFileDir(buffer, searchArgs, ctx);
			}
		}
		else {
			// Process file archive
			TCHAR filepath[MAX_PATH];
			_stprintf_s(filepath, "%s%s", path, fileData.cFileName);
			JPG_CTX jpgCtx = { filepath, searchArgs };
			JPEG_ProcessExifTags(filepath, ProcessExifTag, &jpgCtx);
		}
	} while ( FindNextFile(fileIt, &fileData) == TRUE );

	FindClose(fileIt);

}

DWORD _tmain(DWORD argc, PTCHAR argv[]) {

	if (argc < 3) {
		_tprintf(_T("Use: %s <repository path> <tag exif>"), argv[0]);
		exit(0);
	}

	SearchFileDir(argv[1], argv[2], NULL);

	PRESS_TO_FINISH("");

	return 0;

}

