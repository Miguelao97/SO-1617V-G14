#pragma once
 
/*
channel format:

QueryPhotos (text):
QUERY_PHOTOS
<tagNumber> { contains | { = | ">" | "<" } <value>  | { between <value1> <value2> } } CRLF  
CRLF

Response (text):
<response_status> CRLF
{<fileurl> CRLF }*
CRLF

FileCmd (text):
GET_PHOTO
<fileurl> CRLF

Response (header-text, content-binary):
<response_status> CRLF
<file size> CRLF
<file bytes...>

*/


#define OP_EQUAL "="
#define OP_GREATER_THAN ">"
#define OP_LESS_THAN  "<"
#define OP_BETWEEN "between"
#define OP_CONTAINS "contains"

// these are the accepted commands
#define QUERY_CMD "QUERY_PHOTOS"
#define GET_PHOTO_CMD "GET_PHOTO"

// response status
#define STATUS_OK 0
#define STATUS_INVALID_CMD 1
#define STATUS_PHOTO_NOT_FOUND 2
#define STATUS_INVALID_TAG 3
#define STATUS_INTERNAL_ERROR 4

