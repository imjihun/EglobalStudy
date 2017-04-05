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
#define SIZE_BUFFER				4096
#define PORT_SERVER				9000

/***********************protocol***********************/
/* CMD[2] : LENGTH[4] : DATA */

#define TYPE_CMD					unsigned short
#define SIZE_CMD					2
#define TYPE_PACKET_LENGTH			unsigned int
#define SIZE_PACKET_LENGTH			4

#define SIZE_HEADER					SIZE_CMD + SIZE_PACKET_LENGTH



#define CMD_FAIL					1

#define SIZE_ID						16
#define MAX_ONCE_LOAD_CHATTING_LOG	1024

/*
	C->S # CMD[2] : LENGTH[4] : ID
	S->C # CMD[2] : LENGTH[4] : ID or CMD_FAIL
*/
#define CMD_CREATE_ID				10

/*
	C->S # CMD[2] : LENGTH[4] : ID : IS_SECRET : SUBJECT
	S->C # CMD[2] : LENGTH[4] : ID or CMD_FAIL : ROOM_NUMBER : SECRETKEY
*/
#define CMD_CREATE_ROOM				20

/*
	C->S # CMD[2] : LENGTH[4] : ID : ROOM_NUMBER
	S->C # CMD[2] : LENGTH[4] : ID or CMD_FAIL : ROOM_NUMBER
*/
#define CMD_ENTER_ROOM				30

/*
	C->S # CMD[2] : LENGTH[4] : ID : ROOM_NUMBER
	S->C # CMD[2] : LENGTH[4] : ID or CMD_FAIL : ROOM_NUMBER
*/
#define CMD_LEAVE_ROOM				40

/*
	C->S # CMD[2] : LENGTH[4] : ID : ROOM_NUMBER
	S->C # CMD[2] : LENGTH[4] : ID or CMD_FAIL : CHATTING_LOG
*/
#define CMD_VIEW_ROOM				50

/*
	C->S # CMD[2] : LENGTH[4] : ID
	S->C # CMD[2] : LENGTH[4] : ID or CMD_FAIL : ROOM_LIST[room_number : subject]
*/
#define CMD_TOTAL_ROOM_LIST			60

/*
	C->S # CMD[2] : LENGTH[4] : ID
	S->C # CMD[2] : LENGTH[4] : ID or CMD_FAIL : ROOM_LIST[room_number : subject]
*/
#define CMD_MY_ROOM_LIST			70

/*
	C->S # CMD[2] : LENGTH[4] : ID : ROOM_NUMBER : MESSAGE
	S->C # CMD[2] : LENGTH[4] : ID or CMD_FAIL : ROOM_NUMBER : MESSAGE
*/
#define CMD_MESSAGE					80

/*********************end protocol*********************/

/*************** Definitions / Macros *************************************/

/*************** New Data Types *******************************************/

/*************** Constant *************************************************/

/*************** typedef  *************************************************/

/*************** Prototypes ***********************************************/

#endif	/*_MACRO_FILE_H */
/*************** END OF FILE **********************************************/

