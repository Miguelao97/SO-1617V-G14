// Exercicio3.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <Windows.h>
#include <Psapi.h>
#include <stdio.h>
#include <iostream>

static char* getProcessName(char* out){
	char* aux = out;
	while (*out != '\0' || *(out + 1) != '\0'){
		if (*out == '\\'){
			aux = out;
			aux++;
		}
		out++;
	}
	return ++aux;
}

static void printProcessName(char* out){
	out = getProcessName(out);

	printf("Process Name: ");

	while (*out != '\0' || *(out + 1) != '\0'){
		printf("%s", out);
		out++;
	}
	printf("\n");
}

int _tmain(int argc, _TCHAR* argv[]){

	const DWORD processID = 5876;
	HANDLE h = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, false, processID);
	const DWORD FC = 10;

	PMEMORY_BASIC_INFORMATION buffer = (PMEMORY_BASIC_INFORMATION)malloc(sizeof(MEMORY_BASIC_INFORMATION));

	LPSYSTEM_INFO systemInfo = (LPSYSTEM_INFO)malloc(sizeof(SYSTEM_INFO));
	GetSystemInfo(systemInfo);

	SIZE_T size = VirtualQueryEx(h, systemInfo->lpMinimumApplicationAddress, buffer, sizeof(MEMORY_BASIC_INFORMATION));
	DWORD maxReserved = 0;
	PVOID baseAddress = NULL;
	PCHAR aux = 0;

	while (size != 0){
		if (buffer->State == MEM_RESERVE && buffer->RegionSize > maxReserved){
			maxReserved = buffer->RegionSize;
			baseAddress = buffer->BaseAddress;
		}
		aux = (PCHAR)((ULONG)buffer->BaseAddress + buffer->RegionSize);
		size = VirtualQueryEx(h, (PCHAR)((ULONG)(buffer->BaseAddress) + buffer->RegionSize), buffer, sizeof(MEMORY_BASIC_INFORMATION));
	}
	char filename[1024] = {};
	DWORD name = GetModuleFileNameEx(h, 0, (LPTSTR)filename, sizeof(filename)-1);

	printProcessName(filename);
	printf("Max Reserved = %lu\n", maxReserved);
	printf("Base Adress = %lu\n", baseAddress);

	free(buffer);
	free(systemInfo);
	CloseHandle(h);
	return 0;
}



