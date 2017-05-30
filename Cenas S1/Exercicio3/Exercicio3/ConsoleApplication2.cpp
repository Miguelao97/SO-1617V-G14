// ConsoleApplication2.cpp : Defines the entry point for the console application.
//

/*EXERCICIO 3*/

#include "stdafx.h"
#include "windows.h"
#include "tchar.h"
#include "stdio.h"
#include "psapi.h"
#include "winnt.h"


void main()
{
	DWORD procId = 20188;	//this needs to come in the form of args but for now it is what it is
	HANDLE procHandle = OpenProcess(PROCESS_ALL_ACCESS, false, procId);
	TCHAR processName[MAX_PATH] = TEXT("<n/a>");	//buffer that holds process info
	LPSYSTEM_INFO system_info = (LPSYSTEM_INFO)malloc(sizeof(LPSYSTEM_INFO));
	if (procHandle) {
		MEMORY_BASIC_INFORMATION memBuffer;	//buffer that holds memory info
		HMODULE hMod;
		DWORD sizeBytes;
		
		GetSystemInfo(system_info);
		
		
		LPVOID maxReservedAdd = 0;
		size_t virtualQuery;
		size_t maxRegSize = 0;
		size_t aux = 0;
		for (LPVOID curMinAddress = system_info->lpMinimumApplicationAddress; 
			aux=VirtualQueryEx(procHandle, curMinAddress, &memBuffer, sizeof(MEMORY_BASIC_INFORMATION))!=0; )
		{
	

			if (memBuffer.State == MEM_RESERVE && maxRegSize < memBuffer.RegionSize) {
				maxRegSize = memBuffer.RegionSize;
				maxReservedAdd = curMinAddress;

			}
				
			ULONG sz = (ULONG)memBuffer.BaseAddress;
			size_t region = (memBuffer.RegionSize);
			curMinAddress = (PCHAR)(sz + region);
		}

		DWORD procModule = EnumProcessModules(procHandle, &hMod, sizeof(hMod), &sizeBytes);
		

		
		if (procModule) {
			DWORD size = sizeof(processName) / sizeof(TCHAR);
			GetModuleBaseName(procHandle, hMod, processName, size);
		}
		
		
		printf("%p \n", maxReservedAdd);
		printf("%d \n", maxRegSize);
		
	}
	_tprintf(TEXT("%s  (PID: %u)\n"), processName, procId);
	
	free(system_info);
	CloseHandle(procHandle);


	
}

