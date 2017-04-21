// Exercicio5.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <Windows.h>
#include "Exif.h"


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
#define ALT (0x0006)

static short ALIGNMENT;

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

DWORD main(DWORD argc, PCSTR argv[])
{
	printTags(argv[1]);
    return 0;
}

void PrintError() {
	printf("Error in file\n");
	return;
}

PVOID getPointerExif(short marker, HANDLE h) {
	for (short* iter = (short*)h; *iter != EOI; iter++)
	{
		if (*iter == marker || Align((int*)iter, sizeof(short)) == marker)
			return iter;
	}
	return NULL;
}

int Align(int* pos, size_t nbytes) {
	return *pos == INTELMACRO ? *pos : AlignByte(pos, nbytes);
}

int AlignByte(PVOID pos, size_t nbytes) {
	BYTE *bytes = (PBYTE)pos, ret[4] = { 0 };
	while (nbytes--) ret[nbytes] = *bytes++;
	return (int)*bytes;
}

PEXIF get_exif_from_file(HANDLE h) {
	PEXIF ret = (PEXIF)getPointerExif(APP1MARKER, h);
	//alignment (to know if its intel or motorola)
	ALIGNMENT = ret->tiffhead.byte_align;
	return ret;
}

PVOID getAdress(PBYTE base, int offset) {
	return (PVOID)(base + offset);
}



static ENTRY getEntry(short tag, PIFD pifd){
	int noEntries = Align((int*)(&pifd->num_dir),sizeof(pifd->num_dir));
	PENTRY entry = pifd->entry;
	for (short i = 0, value; i < noEntries; i++) {
		if ( Align((int*)(&entry[i].tag),sizeof(entry[i].tag)) == tag) {
			return entry[i];
		}
	}
	return {};
}

VOID getAlignedRational(PLONG toAlign, PLONG aligned, DWORD n) {
	memcpy(aligned, toAlign, sizeof(LONG)*(n *= 2));
	for (; n--; ++aligned)
		*aligned = Align((int*)aligned, sizeof(LONG));
}

short getFormatSize(short format) {
	switch (format) {
	case 1: return sizeof(BYTE);
	case 2: return sizeof(CHAR);
	case 3: return sizeof(SHORT);
	case 4: return sizeof(LONG);
	case 5: return 2 * sizeof(LONG);
	default: return 0;
	}
}

ENTRY getEntryAleign(short mark, PIFD base) {
	ENTRY entry = getEntry(mark, base);
	entry.tag = Align((int*)(&entry.tag),sizeof(entry.tag));
	entry.format = Align((int*)(&entry.format), sizeof(entry.format));
	entry.num_comp = Align((int*)(&entry.num_comp), sizeof(entry.num_comp));
	if (getFormatSize(entry.format)*entry.num_comp > sizeof(entry.data_value)) {
		entry.data_value = Align((int*)(&entry.data_value), sizeof(entry.data_value));
	}
	else if (entry.format != ASCII_FORMAT) {
		entry.data_value = Align((int*)(&entry.data_value), getFormatSize(entry.format)*entry.num_comp);
	}
	return entry;
}

void printTags(LPCSTR nameFile) {
	HANDLE file = CreateFileA(nameFile, GENERIC_READ, 0, NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL);

	if (file == INVALID_HANDLE_VALUE) {
		PrintError();
		goto ret;
	}

	HANDLE mapFile = CreateFileMappingA(file, NULL, PAGE_READONLY, 0, 0, NULL);
	if (mapFile == INVALID_HANDLE_VALUE)
		goto retFile;

	HANDLE viewExif = MapViewOfFile(mapFile, FILE_MAP_READ, 0, 0, NULL);

	if (viewExif == INVALID_HANDLE_VALUE) {
		goto retMap;
	}

	PEXIF exifFile = get_exif_from_file(viewExif);
	PTiffHeader tiff = &(exifFile->tiffhead);
	PBYTE btif = (PBYTE)tiff;

	PIFD ifd0 = (PIFD)getAdress(btif, Align(&tiff->ifd_offset, sizeof(&tiff->ifd_offset)));
	ENTRY exifOff = getEntryAleign(EO, ifd0),
		gpsOff = getEntryAleign(GPSO, ifd0),
		model = getEntryAleign(MODEL, ifd0);

	PIFD subIFD0 = (PIFD)getAdress(btif, exifOff.data_value);
	ENTRY dateTime = getEntryAleign(DT, subIFD0),
		exposureTime = getEntryAleign(ET, subIFD0),
		apertureTime = getEntryAleign(AT, subIFD0),
		iso = getEntryAleign(ISO, subIFD0),
		width = getEntryAleign(WTH, subIFD0),
		length = getEntryAleign(LTH, subIFD0);

	PIFD gpsInfo = (PIFD)getAdress(btif, gpsOff.data_value);
	ENTRY latitude = getEntryAleign(LAT, subIFD0),
		longitude_reference = getEntryAleign(LNGREF, subIFD0),
		longitude = getEntryAleign(LNG, subIFD0),
		altitude = getEntryAleign(ALT, subIFD0);

	long lat[6], lon[6], alt[6], ext[6], apv[6];
	getAlignedRational((PLONG)getAdress(btif, latitude.data_value), lat, latitude.num_comp);
	getAlignedRational((PLONG)getAdress(btif, longitude.data_value), lon, longitude.num_comp);
	getAlignedRational((PLONG)getAdress(btif, altitude.data_value), alt, altitude.num_comp);
	getAlignedRational((PLONG)getAdress(btif, exposureTime.data_value), ext, exposureTime.num_comp);
	getAlignedRational((PLONG)getAdress(btif, apertureTime.data_value), apv, apertureTime.num_comp);

	if (&model != NULL)printf("Model: %s\n", (PCHAR)getAdress(btif, model.data_value));
	if (&width != NULL && &length != NULL)printf("Dimensão : %u px x %u px\n", width.data_value, length.data_value);
	if (&dateTime != NULL)printf("Data: %s\n", (PCHAR)getAdress(btif, dateTime.data_value));
	if (&iso != NULL) printf("ISO : %u\n", iso.data_value);
	if (&exposureTime != NULL)printf("Velocidade : %lu / %lu s\n", ext[0], ext[1]);
	if (&apertureTime != NULL)printf("Abertura : %f\n", (FLOAT)apv[0] / apv[1]);
	if (&latitude != NULL)printf("Latitude : %lu; %lu; %f %s\n", lat[0] / lat[1], lat[2] / lat[3], (FLOAT)lat[4] / lat[5], (PCHAR)&latitude.data_value);
	if (&longitude != NULL && &longitude_reference != NULL)printf("Longitude : %lu; %lu; %f %s\n", lon[0] / lon[1], lon[2] / lon[3], (FLOAT)lon[4] / lon[5], (PCHAR)&longitude_reference.data_value);
	if (&altitude != NULL)printf("Altitude : %f\n", (FLOAT)alt[0] / alt[1]);


	UnmapViewOfFile(viewExif);
retMap:	CloseHandle(mapFile);
retFile: CloseHandle(file);
ret: return;
}

