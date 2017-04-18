/* 2017-04-04 : created by JHLim */
/***************************************************************************
* Copyright (c) 2004-2016, eGlobal Systems, Co., Ltd.
* Seoul, Republic of Korea
* All Rights Reserved.

* Description : prototype.h
*		head file for *.c : XXX
*
***************************************************************************/
#ifndef _CONNECT_DB_H
#define _CONNECT_DB_H

/*************** Header files *********************************************/
#include <mysql.h>
#include <string.h>
#include <stdio.h>
#include <stdlib.h>

#include "../../macroFile.h"

/*************** Assertions ***********************************************/

/*************** Macros ***************************************************/

#define DB_HOST "127.0.0.1"
#define DB_USER "root"
#define DB_PASS "root00"
#define DB_NAME "test"

#define SIZE_QUERY  1024

/*************** Definitions / Macros *************************************/

/*************** New Data Types *******************************************/

/*************** Constant *************************************************/

/*************** typedef  *************************************************/

/*************** Prototypes ***********************************************/

int dbOpen();
int dbClose();
int dbCreateAllTable();
int dbDropAllTable();
int viewAllTable();

int dbInsertUserinfo(char *id);
int dbInsertRoominfo(room_info *p_room_info);
int dbDeleteRoominfo(TYPE_ROOM_NUMBER room_number);
int dbInsertRoomUser(TYPE_ROOM_NUMBER room_number, char *id);
int dbDeleteRoomUser(TYPE_ROOM_NUMBER room_number, char *id);

int dbSelectAllRoom(room_info *arr_room_info_ret, int size_arr);
int dbSelectUserInRoom(int room_number, char** arr_id_ret, int size_arr_id);
int dbSelectRoomOfUser(char *id, room_info *arr_room_info_ret, int size_arr);

int dbSelectKeyOfRoom(TYPE_ROOM_NUMBER room_number, TYPE_SECRET_KEY *key);
#endif	/*_CONNECT_DB_H */
/*************** END OF FILE **********************************************/

