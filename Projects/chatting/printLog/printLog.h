/* 2017-04-04 : created by JHLim */
/***************************************************************************
* Copyright (c) 2004-2016, eGlobal Systems, Co., Ltd.
* Seoul, Republic of Korea
* All Rights Reserved.

* Description : prototype.h
*		head file for *.c : XXX
*
***************************************************************************/
#ifndef _PRINT_LOG_H
#define _PRINT_LOG_H

/*************** Header files *********************************************/

#include <string.h>
#include <stdio.h>
#include <errno.h>
#include <time.h>
#include <stdarg.h>

/*************** Assertions ***********************************************/

/*************** Macros ***************************************************/

//#define LOG_VIEW
//#define LOG_FILE

/*************** Definitions / Macros *************************************/

/*************** New Data Types *******************************************/

/*************** Constant *************************************************/

/*************** typedef  *************************************************/

/*************** Prototypes ***********************************************/

void printView(char *buf);
void printFile(char *buf);
void printLog(const char *Format, ...);

#endif /* _PRINT_LOG_H */
/*************** END OF FILE **********************************************/




