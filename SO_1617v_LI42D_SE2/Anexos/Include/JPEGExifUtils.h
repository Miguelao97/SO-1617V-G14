#ifndef _JPEGEXIFUTILS_H
#define _JPEGEXIFUTILS_H

#include <Windows.h>

// Set unaligned struct fields
#pragma pack(push, 1)


// Unset unaligned struct fields
#pragma pack(pop)

typedef BOOL(*PROCESS_EXIF_TAG)(LPCVOID ctx, DWORD tag, LPVOID value);

#ifdef JPEGEXIFUTILS_DLL_EXPORT
#define JPEGEXIFUTILS_DLL_API __declspec(dllexport)
#else
#define JPEGEXIFUTILS_DLL_API __declspec(dllimport)
#endif

#ifdef __cplusplus
extern "C" {
#endif

	JPEGEXIFUTILS_DLL_API VOID JPEG_PrintMetadataA(PCHAR fileimage);
	JPEGEXIFUTILS_DLL_API VOID JPEG_PrintMetadataW(PWCHAR fileimage);

	/*
	 * Call processor for each exif tag in fileimage while processor returns true. Return immediatly after processor returns false.
	 */
	JPEGEXIFUTILS_DLL_API VOID JPEG_ProcessExifTagsA(PCHAR fileimage, PROCESS_EXIF_TAG processor, LPCVOID ctx);
	JPEGEXIFUTILS_DLL_API VOID JPEG_ProcessExifTagsW(PWCHAR fileimage, PROCESS_EXIF_TAG processor, LPCVOID ctx);

#ifdef UNICODE
#define JPEG_PrintMetadata JPEG_PrintMetadataW
#define JPEG_ProcessExifTags JPEG_ProcessExifTagsW
#else
#define JPEG_PrintMetadata JPEG_PrintMetadataA
#define JPEG_ProcessExifTags JPEG_ProcessExifTagsA
#endif

#ifdef __cplusplus
}
#endif

#endif/*_JPEGEXIFUTILS_H*/