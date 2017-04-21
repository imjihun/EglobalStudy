/* 2017-04-04 : created by JHLim */
/***************************************************************************
* Copyright (c) 2004-2016, eGlobal Systems, Co., Ltd.
* Seoul, Republic of Korea
* All Rights Reserved.

* Description : prototype.h
*		head file for *.c : XXX
*
***************************************************************************/
#ifndef _CHATTING_LOG_H
#define _CHATTING_LOG_H

/*************** Header files *********************************************/
#include "connectDB.h"
#include "../../macroFile.h"
#include "../../printLog/printLog.h"

#include "encrypt.h"
#include "../../aes/source/aes.h"

#include <stdio.h>
#include <unistd.h>
#include <sys/stat.h>
#include <sys/types.h>
#include <pwd.h>
#include <errno.h>
#include <stdlib.h>
#include <dirent.h>

/*************** Assertions ***********************************************/

/*************** Macros ***************************************************/
#define DIRNAME_CHATTINGLOG			"chattinglog"
#define SIZE_DIRNAME_CHATTINGLOG	11

/*************** Definitions / Macros *************************************/

/*************** New Data Types *******************************************/

/*************** Constant *************************************************/

/*************** typedef  *************************************************/

/*************** Prototypes ***********************************************/
int initPath();
int setPath(char *path);
int viewList();
int viewFile(TYPE_ROOM_NUMBER room_number);
int removeAllFiles();

int resetFile(room_info *p_room_info);
int writeChattingLog(room_info *p_room_info, char *id, char *message, size_t size_message);
size_t readChattingLog(room_info *p_room_info, char *id, char *buffer_ret, size_t size_buffer_ret);

#endif	/*_CHATTING_LOG_H */
/*************** END OF FILE **********************************************/

