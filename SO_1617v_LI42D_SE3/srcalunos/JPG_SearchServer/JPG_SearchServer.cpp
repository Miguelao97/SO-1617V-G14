// SocketServer.cpp : Defines the entry point for the console application.
// A synchronous server exemplifying use of sockets  

#include "stdafx.h"


#include "../Include/jpg_searchservice.h"
#include "Connection.h"

// this is a "black magic" alternative to explicit link with sockets library
#pragma comment (lib, "Ws2_32.lib")

PCHAR Repository;
 
UINT WINAPI ProcessConnection(LPVOID arg) {
	SOCKET connectSocket = (SOCKET)arg;
	PCONNECTION connection;

	connection = ConnectionCreate(connectSocket);
	
	while (ProcessRequest(connection));
	
	ConnectionDestroy(connection);
	
	return 0;
}

int main(int argc, char* argv[])
{
	/* Server listening and connected sockets. */

	SOCKET SrvSock = INVALID_SOCKET, connectSock = INVALID_SOCKET;
	int addrLen;
	struct sockaddr_in srvSAddr;		/* Server's Socket address structure */
	struct sockaddr_in connectSAddr;	/* Connected socket with client details   */
	WSADATA WSStartData;				/* Socket library data structure   */
	BOOL terminate = FALSE;

	if (argc != 2) {
		printf("usage: JPG_SearchServer <repository_path/>\n");
		return 1;
	}

	Repository = argv[1];
	/*	Initialize the WS library. Ver 2.0 */

	if (WSAStartup(MAKEWORD(2, 0), &WSStartData) != 0) {
		_tprintf(_T("Cannot support sockets"));
		return 1;
	}

	/*	Follow the standard server socket/bind/listen/accept sequence */
	SrvSock = socket(AF_INET, SOCK_STREAM, 0);
	if (SrvSock == INVALID_SOCKET) {
		_tprintf(_T("Failed server socket() call"));
		return 1;
	}

	/*	Prepare the socket address structure for binding the
	server socket to port number "reserved" for this service. */

	srvSAddr.sin_family = AF_INET;
	srvSAddr.sin_addr.s_addr = htonl(INADDR_ANY);
	srvSAddr.sin_port = htons(SERVER_PORT);
	if (bind(SrvSock, (struct sockaddr *)&srvSAddr, sizeof(srvSAddr)) == SOCKET_ERROR) {
		printf("Failed server bind() call");
		return 1;
	}

	// define the accept queue length
	if (listen(SrvSock, SOMAXCONN) != 0) {
		printf("Server listen() error");
		return 1;
	}

	/* Main thread becomes listening/connecting/monitoring thread */
	while (!terminate) {
		addrLen = sizeof(connectSAddr);
		_tprintf(_T("waiting  connection..\n"));
		/* Accept requests from any client machine.  */
		connectSock = accept(SrvSock,
			(struct sockaddr *)&connectSAddr, &addrLen);
		if (connectSock == INVALID_SOCKET) {
			printf("accept: invalid socket error");
			terminate = TRUE;
			continue;
		}
#ifdef _DEBUG
		printf("connected with %X, port %d..\n", connectSAddr.sin_addr, connectSAddr.sin_port);
#endif
		// Process connection in a dedicated thread
		_beginthreadex(NULL, 0, ProcessConnection, (LPVOID)connectSock, 0, NULL);
	}

	// Cleanup
	shutdown(SrvSock, SD_BOTH); /* Disallow sends and receives */
	closesocket(SrvSock);
	WSACleanup();

	return 0;
}


