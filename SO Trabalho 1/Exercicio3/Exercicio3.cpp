// Exercicio3.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <Windows.h>
#include <Psapi.h>


int main(int argc, char* argv[]){

	DWORD processID = 0;
	HANDLE h = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, false, processID);
	const DWORD FC = 10;

	PMEMORY_BASIC_INFORMATION buffer = (PMEMORY_BASIC_INFORMATION) malloc(sizeof(PMEMORY_BASIC_INFORMATION));

	SIZE_T size = VirtualQueryEx(h, NULL, buffer, sizeof(buffer));
	DWORD maxReserved = 0 ;
	PVOID baseAddress = NULL;

	while (size != 0){
		if (buffer->State == MEM_RESERVE && buffer->RegionSize > maxReserved){
			maxReserved = buffer->RegionSize;
			baseAddress = buffer->BaseAddress;
		}

		size = VirtualQueryEx(h, &buffer->BaseAddress + buffer->RegionSize, buffer, sizeof(buffer));

	}




	CloseHandle(h);	
	return 0;
}

