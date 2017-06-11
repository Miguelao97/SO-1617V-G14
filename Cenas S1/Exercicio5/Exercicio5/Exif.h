#pragma once
#pragma pack(1)

#ifndef EXIFDLL
#define EXIFDLL

	#include <Windows.h>

	#ifdef DLL_EXIF_EXPORTS
		#define EXIFAPI __declspec(dllexport)
	#else
		#define EXIFAPI __declspec(dllimport)
	#endif

	#ifdef __cplusplus
		extern "C" {
	#endif // __cplusplus

	#define SOIMARKER (0xFFD8)
	#define APP1MARKER (0xFFE1)
	#define EOI (0xFFD9)
	#define INTELMACRO (0x4949)
	#define EO (0x8769)
	#define GPSO (0x8825)
	#define MODEL (0x0110)
	#define ASCII_FORMAT (2)
	#define DT (0x9003)
	#define ET (0x829A)
	#define AT (0x9205)
	#define ISO (0x8827)
	#define WTH (0xa002)
	#define LTH (0xa003)
	#define LAT (0x0002)
	#define LNG (0x0004)
	#define LNGREF (0x0003)
	#define LATREF (0x0001)
	#define ALT (0x0006)

	typedef struct entry {
		short tag;
		short format;
		int num_comp;
		int data_value;
	}ENTRY, *PENTRY;

	typedef struct ifd {
		short num_dir;
		ENTRY entry[1];
	}IFD, *PIFD;

	typedef struct tiffhead {
		short byte_align;
		short tag_mark;
		int ifd_offset;
	}TiffHeader, *PTiffHeader;

	typedef struct exifhead {
		char data[6];
	}ExifHeader, *PExifHeader;

	typedef struct exif {
		short marker;
		short dataSize;
		ExifHeader exhead;
		TiffHeader tiffhead;
	}EXIIF, *PEXIF;

	#ifdef UNICODE
		#define printTags printTagsW
	#else 
		#define printTags printTagsA
	#endif

	EXIFAPI int AlignByte(PVOID pos, size_t nbytes);
	EXIFAPI int Align(int* pos, size_t nbytes);
	EXIFAPI PEXIF get_exif_from_file(HANDLE file);
	EXIFAPI ENTRY getEntryAleign(short mark, PIFD base);
	EXIFAPI VOID printTagsW(LPCWSTR name);
	EXIFAPI VOID printTagsA(LPCSTR name);

	#ifdef __cplusplus
		}
	#endif
#endif