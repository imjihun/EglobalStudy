/* 2017-04-04 : created by JHLim */
/***************************************************************************
* Copyright (c) 2004-2016, eGlobal Systems, Co., Ltd.
* Seoul, Republic of Korea
* All Rights Reserved.
*
* Description : prototype.c
*       xxxx
*
***************************************************************************/

/*************** Header files *********************************************/
#include "printLog.h"

/*************** New Data Types *******************************************/

/*************** Definitions / Macros  ************************************/

/*************** Global Variables *****************************************/

/*************** Prototypes ***********************************************/

/*************** Function *************************************************/

void printView(char *buf)
{
    puts(buf);
}
void printFile(char *buf)
{
    FILE *fp;
    char filename[256];
    char filepath[256];
    time_t timer;
    struct tm *t;

    timer = time(NULL);
    t = localtime(&timer);
    sprintf(filename, "%s/%d_%d_%d_%d.txt", FILENAME_SERVERLOG, t->tm_year + 1900, t->tm_mon + 1, t->tm_mday, t->tm_hour);

    realpath(filename, filepath);

    fp = fopen(filepath, "a");
    if(fp == NULL)
        perror("fopen");

    fprintf(fp, "[%d : %d] %s\n", t->tm_min, t->tm_sec, buf);

    fclose(fp);
}

void printLog(const char *Format, ...)
{
    char buf[1024] = {0,}; 
    va_list ap; 
    
    strcpy (buf, ""); 
    
    va_start(ap, Format); 
    vsprintf(buf + strlen(buf), Format, ap); 
    va_end(ap); 
    
    if(errno != 0)
    {
        strcat(buf, " [error] ");
        strcat(buf, strerror(errno));
    }

#ifdef LOG_VIEW
    printView(buf);
#endif

#ifdef LOG_FILE
    printFile(buf);
#endif
}

/*************** END OF FILE **********************************************/


