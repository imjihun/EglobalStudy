/* 2017-04-04 : created by JHLim */
/***************************************************************************
* Copyright (c) 2004-2016, eGlobal Systems, Co., Ltd.
* Seoul, Republic of Korea
* All Rights Reserved.

* Description : macroFile.h
*		head file for *.c : XXX
*
***************************************************************************/
#ifndef _MACRO_FILE_H
#define _MACRO_FILE_H

/*************** Header files *********************************************/

/*************** Assertions ***********************************************/

/*************** Macros ***************************************************/

#define MAX_CLIENT				1024
#define MAX_ROOM				1024

#define SIZE_BUFFER				4096
#define PORT_SERVER				9000


/************************ aes **************************/
#define SIZE_SECRET_KEY			16
#define TYPE_SECRET_KEY			unsigned char
#define MAX_TYPE_SECRET_KEY		(TYPE_SECRET_KEY)255
/********************** end aes ************************/



/***********************protocol***********************/
/* 
	CMD[2] : LENGTH[4] : DATA 
*/

#define TYPE_CMD					unsigned short
#define SIZE_CMD					2
#define TYPE_PACKET_LENGTH			int
#define SIZE_PACKET_LENGTH			4

#define SIZE_HEADER					SIZE_CMD + SIZE_PACKET_LENGTH


// Max 15bytes
#define SIZE_ID						16
#define SIZE_ROOM_STATUS			1

// Max 63bytes
#define SIZE_ROOM_SUBJECT			64


#define ROOM_INFO_STATUS_SECRET			'S'
#define ROOM_INFO_STATUS_NORMAL			'N'
#define ROOM_INFO_STATUS_NOT_EXIST		0xFF

#define TYPE_ROOM_NUMBER			int
#define SIZE_ROOM_NUMBER			4


#define MAX_ONCE_LOAD_CHATTING_LOG	1024
#define SIZE_CHATTING_LOG			1024

#define NOT_DEFINE_CMD				-500
#define CMD_FAIL					10000

/*
	C->S # CMD[2] : LENGTH[4] : ID
	S->C # CMD[2] : LENGTH[4] : ID or CMD_FAIL
*/
#define CMD_CREATE_ID				10

/*
	C->S # CMD[2] : LENGTH[4] : ID : IS_SECRET : SUBJECT
	S->C # CMD[2] : LENGTH[4] : ID or CMD_FAIL : ROOM_NUMBER : STATUS : SUBJECT : SECRETKEY
*/
#define CMD_CREATE_ROOM				20

/*
	C->S # CMD[2] : LENGTH[4] : ID : ROOM_NUMBER
	S->C # CMD[2] : LENGTH[4] : ID or CMD_FAIL : ROOM_NUMBER : STATUS : SUBJECT : SECRETKEY : COUNT_MEMBER
*/
#define CMD_ENTER_ROOM				30

/*
	C->S # CMD[2] : LENGTH[4] : ID : ROOM_NUMBER
	S->C # CMD[2] : LENGTH[4] : ID or CMD_FAIL : ROOM_NUMBER
*/
#define CMD_LEAVE_ROOM				40

/*
	C->S # CMD[2] : LENGTH[4] : ID : ROOM_NUMBER
	S->C # CMD[2] : LENGTH[4] : ID or CMD_FAIL : ROOM_NUMBER : CHATTING_LOG
*/
#define CMD_VIEW_ROOM				50

/*
	C->S # CMD[2] : LENGTH[4] : ID
	S->C # CMD[2] : LENGTH[4] : ID or CMD_FAIL :ROOM_NUMBER : STATUS : SUBJECT : COUNT_MEMBER
	... room count ...
	S->C # CMD[2] : LENGTH[4] : ID or CMD_FAIL :ROOM_NUMBER : STATUS : SUBJECT : COUNT_MEMBER
*/
#define CMD_TOTAL_ROOM_LIST			60

/*
	C->S # CMD[2] : LENGTH[4] : ID
	S->C # CMD[2] : LENGTH[4] : ID or CMD_FAIL : ROOM_NUMBER : STATUS : SUBJECT : SECRETKEY : COUNT_MEMBER : CHATTING_LOG
	... room count ...
	S->C # CMD[2] : LENGTH[4] : ID or CMD_FAIL : ROOM_NUMBER : STATUS : SUBJECT : SECRETKEY : COUNT_MEMBER : CHATTING_LOG
*/
#define CMD_MY_ROOM_LIST			70

/*
	C1->S # CMD[2] : LENGTH[4] : ID : ROOM_NUMBER : MESSAGE
	S->C1 # CMD[2] : LENGTH[4] : ID or CMD_FAIL : ROOM_NUMBER : MESSAGE
	... user count ...
	S->Cn # CMD[2] : LENGTH[4] : ID or CMD_FAIL : ROOM_NUMBER : MESSAGE
*/
#define CMD_CHATTING_MESSAGE		80

/*
	C1->S # CMD[2] : LENGTH[4] : MYID : YOURID : ROOM_NUMBER
	S->C1 # CMD[2] : LENGTH[4] : MYID or CMD_FAIL
	S->C2 # CMD[2] : LENGTH[4] : YOURID or CMD_FAIL : ROOM_NUMBER
*/
#define CMD_INVITE					90

/*********************end protocol*********************/

/*************** Definitions / Macros *************************************/

/*************** New Data Types *******************************************/

typedef struct _sockinfo
{
    int sockfd;
    char id[SIZE_ID];
    char buffer[SIZE_BUFFER];
    int cnt_read;
} socket_info;

typedef struct _listnode
{
    socket_info *p_socket_info;
    struct _listnode *next;
} list_socket;

typedef struct _roominfo
{
	TYPE_ROOM_NUMBER room_number;
    unsigned char status;
    char subject[SIZE_ROOM_SUBJECT];

    TYPE_SECRET_KEY key[SIZE_SECRET_KEY];
    
    int count_member;
} room_info;

/*************** Constant *************************************************/

/*************** typedef  *************************************************/

/*************** Prototypes ***********************************************/

#endif	/*_MACRO_FILE_H */
/*************** END OF FILE **********************************************/

