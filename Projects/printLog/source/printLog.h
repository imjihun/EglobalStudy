
#ifndef PRINT_LOG
#define PRINT_LOG

#include <string.h>
#include <stdio.h>
#include <errno.h>
#include <time.h>
#include <stdarg.h>

void printView(char *buf);
void printFile(char *buf);
void printLog(const char *Format, ...);

#define LOG_VIEW
//#define LOG_FILE
#endif /* PRINT_LOG */
