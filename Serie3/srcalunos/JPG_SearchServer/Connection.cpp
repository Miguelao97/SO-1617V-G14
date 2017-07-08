#include "stdafx.h"

#include "connection.h"


/*
* Used on socket buffered I/O
* fill receive buffer
*/
void ConnectionFillBufferFromSocket(PCONNECTION c) {
	c->len = recv(c->socket, c->bufferIn, BUFFERSIZE, 0);
	c->rPos = 0;
}

/*
* Used on socket buffered I/O
* flush send buffer to socket
*/
void ConnectionFlushBufferToSocket(PCONNECTION c) {
	if (c->wPos > 0) {
		send(c->socket, c->bufferOut, c->wPos, 0);
		c->wPos = 0;
	}
}

/*
* reads a line (terminate with \r\n pair) from the connection socket
* using the buffer the I/O buffer in PHttpConnection "cn"
*
* Return the number of line readed
*/
int ConnectionGetLine(PCONNECTION cn, char *buffer, int bufferSize) {
	int i = 0;
	int c;

	while (i < bufferSize - 1 && (c = CGetChar(cn)) != -1 && c != '\r')
		buffer[i++] = c;
	if (c == -1)
		return -1;
	if (c == '\r')
		c = CGetChar(cn); /* read line feed */
	buffer[i] = 0;
	return i;
}

int ConnectionGetLineAsync(PCONNECTION cn, char *buffer, int bufferSize) {
	int i = 0;
	int c;

	while (i < bufferSize - 1 && (c = cn->bufferIn[cn->rPos++]) != -1 && c != '\r')
		buffer[i++] = c;
	if (c == -1)
		return -1;
	if (c == '\r')
		cn->rPos++; /* read line feed */
	buffer[i] = 0;
	return i;
}


/*
* Output formatters
*/

void ConnectionPutLineEnd(PCONNECTION cn) {
	CPutChar(cn, '\r'); // CR
	CPutChar(cn, '\n'); // LF
}

VOID ConnectionPutString(PCONNECTION cn, const char *str) {
	int c;

	while ((c = *str++) != 0) CPutChar(cn, c);
}

VOID ConnectionPutStringFromWString(PCONNECTION cn, wchar_t *str) {
	int c;

	while ((c = *str++) != 0) CPutChar(cn, c);
}

void ConnectionPutInt(PCONNECTION cn, int num) {
	char ascii[32];

	ConnectionPutString(cn, _itoa(num, ascii, 10));
}

void ConnectionPutBytes(PCONNECTION cn, PCHAR buf, DWORD size) {
	send(cn->socket, buf, size, 0);
}

BOOL ConnectionCopyFile(PCONNECTION cn, HANDLE hFile, int fSize) {
	CHAR data[BUFFERSIZE];
	// first send the file size in plain text
	ConnectionPutInt(cn, fSize);
	ConnectionPutLineEnd(cn);

	// flush buffer from previous writes
	ConnectionFlushBufferToSocket(cn);

	// Now copy the file in non-buffered mode
	while (fSize > 0) {
		DWORD toRead = min(BUFFERSIZE, fSize), read;
		if (!ReadFile(hFile, data, toRead, &read, NULL)) // synchronous read
			return FALSE; 
		ConnectionPutBytes(cn, data, read);
		fSize -= read;
	}

	return TRUE;

}

/*
 * A generic ( a la printf) generic output formatter
 */
void ConnectionPut(PCONNECTION cn, char* format, ...) {
	va_list ap;
	char *pcurr = format, c;

	va_start(ap, format);

	while ((c = *pcurr) != 0) {
		if (c != '%') {
			if (c != '\\')
				CPutChar(cn, c);
			else {
				c = *++pcurr;
				if (c == 0) break;
				if (c == 't')
					CPutChar(cn, '\t');
				else if (c == 'n')
					CPutChar(cn, '\n');
				else if (c == 'r')
					CPutChar(cn, '\r');
				else if (c == '0')
					CPutChar(cn, '\0');
			}
		}
		else {
			c = *++pcurr;
			if (c == 0) break;
			switch (c) {
			case 'd':
				ConnectionPutInt(cn, va_arg(ap, int));
				break;
			case 's':
				ConnectionPutString(cn, va_arg(ap, char *));
				break;
			case 'S':
				ConnectionPutStringFromWString(cn, va_arg(ap, wchar_t *));
				break;
			default:
				break;
			}

		}
		pcurr++;
	}
}


/*
* Connection initialization
*/

// Initialize a connection
VOID  ConnectionInit(PCONNECTION c, SOCKET s) {
	ZeroMemory(c, sizeof(Connection));
	c->socket = s;
}

// Create (and initialize) new connection
PCONNECTION ConnectionCreate(SOCKET s) {
	PCONNECTION cn = (PCONNECTION)malloc(sizeof(CONNECTION));
	ConnectionInit(cn, s);
	return cn;
}

// destroy a connection
VOID ConnectionDestroy(PCONNECTION cn) {
	shutdown(cn->socket, SD_BOTH);
	closesocket(cn->socket);
	free(cn);
}

 