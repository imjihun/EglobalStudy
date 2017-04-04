#include "printLog.h"

void printView(char *buf)
{
    puts(buf);
}
void printFile(char *buf)
{
	FILE *fp;
	char filename[256];
	time_t timer;
	struct tm *t;

	timer = time(NULL);
	t = localtime(&timer);
	sprintf(filename, "log/%d_%d_%d_%d.txt", t->tm_year + 1900, t->tm_mon, t->tm_mday, t->tm_hour);
	fp = fopen(filename, "a");

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
        strcat(buf, " : ");
        strcat(buf, strerror(errno));
    }

#ifdef LOG_VIEW
    printView(buf);
#endif

#ifdef LOG_FILE
    printFile(buf);
#endif
}
