#pragma once
#include <WinSock2.h>


#define BUFFERSIZE 4096		/* I/O buffer size used during server request processing */
#define SERVER_PORT 8888   /* Well known server port. */
#define MAX_PENDING_CONNECTIONS 1024

/*
 * the connection
 */
typedef struct Connection {
	CHAR bufferIn[BUFFERSIZE];	/* buffer used on connection reads */
	CHAR bufferOut[BUFFERSIZE];	/* buffer used on buffered connection writes */
	int wPos;					/* buffered write position */
	int rPos;					/* read position */
	int len;					/* buffer content size */
	SOCKET socket;				/* the connection socket */
} CONNECTION, *PCONNECTION;


VOID ConnectionInit(PCONNECTION c, SOCKET s);
PCONNECTION ConnectionCreate(SOCKET s);
VOID ConnectionDestroy(PCONNECTION cn);

void ConnectionFillBufferFromSocket(PCONNECTION c);
void ConnectionFlushBufferToSocket(PCONNECTION c);

/* I/O Formatters */
INT ConnectionGetLine(PCONNECTION cn, char *buffer, int bufferSize);
VOID ConnectionPutString(PCONNECTION cn, const char *str);
VOID ConnectionPutInt(PCONNECTION cn, int num);
VOID ConnectionPut(PCONNECTION cn, char* format, ...);
VOID ConnectionPutStringFromWString(PCONNECTION cn, wchar_t *str);
VOID ConnectionClearBuffer(PCONNECTION c);
VOID ConnectionPutLineEnd(PCONNECTION cn);
BOOL ConnectionCopyFile(PCONNECTION cn, HANDLE hFile, int fSize);

/* inline functions for buffered char input/output */
CHAR FORCEINLINE CGetChar(PCONNECTION cn) {
	return (cn->rPos == cn->len) ?
		(ConnectionFillBufferFromSocket(cn), (cn->len <= 0 ? -1 : cn->bufferIn[cn->rPos++])) :
		cn->bufferIn[(cn)->rPos++];
}

VOID FORCEINLINE CPutChar(PCONNECTION cn, CHAR c) {
	if (cn->wPos == BUFFERSIZE)
		ConnectionFlushBufferToSocket(cn);
	cn->bufferOut[cn->wPos++] = c;
}
		 

/* utilitary functions */
int SplitLine(char *line, char *words[], char delim, int nlines);
BOOL Char2Wchar(TCHAR* pDest, char* pSrc, int dstLen);
VOID ToUpper(char *str);


/* Handler entry point */
BOOL ProcessRequest(PCONNECTION cn);
