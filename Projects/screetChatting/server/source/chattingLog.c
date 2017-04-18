/* 2017-04-04 : created by JHLim */
/***************************************************************************
* Copyright (c) 2004-2016, eGlobal Systems, Co., Ltd.
* Seoul, Republic of Korea
* All Rights Reserved.
*
* Description : prototype.c
*		xxxx
*
***************************************************************************/

/*************** Header files *********************************************/
#include "chattingLog.h"

/*************** New Data Types *******************************************/

/*************** Definitions / Macros  ************************************/

/*************** Global Variables *****************************************/
char g_path_logdir[1024];

/*************** Prototypes ***********************************************/

/*************** Function *************************************************/

int initPath()
{
	struct passwd pwd;
	struct passwd *result;
	char *buf;
	size_t bufsize;
	int s;

	bufsize = sysconf(_SC_GETPW_R_SIZE_MAX);
	if (bufsize == -1)
	bufsize = 16384;
	buf = malloc(bufsize);
	if (buf == NULL) {
		printLog("malloc()");
		return -1;
	}

	s = getpwuid_r(getuid(), &pwd, buf, bufsize, &result);
	if (result == NULL) {
		if (s == 0)
		    printLog("in initPath() Not found\n");
		else {
		    errno = s;
		    printLog("getpwnam_r()");
		}
		return -1;
	}

	const char *homedir = result->pw_dir;
	if(access(homedir, 2) != 0)
		return -1;

	sprintf(g_path_logdir, "%s/%s", homedir, DIRNAME_CHATTINGLOG);

	if(access(g_path_logdir, 0) == -1)
	{
		if(mkdir(g_path_logdir, 0775) != 0)
		{
			return -1;
		}
	}

	return 0;
}
int viewList()
{
	/*
	DIR 			*dir_info;
	struct dirent 	*dir_entry;

	dir_info = opendir(g_path_logdir);              // 현재 디렉토리를 열기

	printf("[%s] >\n", g_path_logdir);
	if (NULL != dir_info)
	{
		while(dir_entry = readdir(dir_info))  // 디렉토리 안에 있는 모든 파일과 디렉토리 출력
		{
			printf("%s\n", dir_entry->d_name);
		}
		closedir(dir_info);
	}
	*/
	char buf[1024];
	sprintf(buf, "ls %s", g_path_logdir);

	printf("[list]%s =>\n", g_path_logdir);
	system(buf);
	return 0;
}
int viewFile(char* filename)
{
	int i;
	int size_filename;
	FILE *fp;
	char buf[SIZE_BUFFER];
	char buf_plain[SIZE_BUFFER];
	char filefullname[1024];
	int retval = 0;

	TYPE_ROOM_NUMBER room_number = 0;
	TYPE_SECRET_KEY key[SIZE_SECRET_KEY];

	size_filename = strlen(filename);
	for(i = 0; i < size_filename; i++)
	{
		if(filename[i] == '_')
		{
			filename[i] = '\0';
			room_number = atoi(filename);
			i++;
			break;
		}
	}

	sprintf(filefullname, "%s/%s_%s", g_path_logdir, filename, filename + i);
	fp = fopen(filefullname, "r");
	if(fp == NULL)
	{
		printLog("fopen() error\n");
		return -1;
	}


	printf("[view]%s =>\n", filefullname);

	// secret file
	if(filename[i] == ROOM_INFO_STATUS_SECRET)
	{
		if((retval = fread(buf, 1, SIZE_BUFFER - 1, fp)) < 1)
		{
			printLog("fread() error\n");
			return -1;
		}

	    if(dbOpen() != 0)
	    {
			printLog("dbOpen() error\n");
			return -1;
	    }
		if(dbSelectKeyOfRoom(room_number, key) != 0)
	    {
			printLog("dbSelectKeyOfRoom() error\n");
			return -1;
	    }
		if(dbClose() != 0)
	    {
			printLog("dbClose() error\n");
			return -1;
	    }

	    retval = decryptBuffer(
	        // key
	        key,
	        // input
	        buf,
	        // length_buffer
	        retval,
	        // output
	        buf_plain);
	    if(retval == -1)
	    {
			printLog("decryptBuffer()\n");
			return -1;
	    }
		buf_plain[retval] = '\0';
		printf("%s\n", buf_plain);
		/*
		for(i = 0; i < retval; i++)
			printf("%2x", *(unsigned char *)(buf_plain + i));
		printf("\n");
		*/
		return 0;
	}

	// normal file
	while((retval = fread(buf, 1, SIZE_BUFFER - 1, fp)) == SIZE_BUFFER -1)
	{
		buf[retval] = '\0';
		printf("%s", buf);
	}
	buf[retval] = '\0';
	printf("%s", buf);

	return 0;
}

int resetFile(room_info *p_room_info)
{
    FILE *fp;
    char file_path[256];

    //sprintf(file_name, "log/%d_%d_%c.txt", g_arr_room_info[room_number].index_chatting_log, room_number, g_arr_room_info[room_number].status);
    sprintf(file_path, "%s/%d_%c.log", g_path_logdir, p_room_info->room_number, p_room_info->status);

    //realpath(file_name, file_path);

    fp = fopen(file_path, "w");
    if (fp == NULL)
    {
        printLog("fopen");
        return -1;
    }

    if (fclose(fp) != 0)
    {
        printLog("fclose");
        return -1;
    }

    return 0;
}
int writeChattingLog(room_info *p_room_info, char *id, char *message, size_t size_message)
{
    FILE *fp;
    char file_path[256];

    //sprintf(file_name, "log/%d_%d_%c.txt", g_arr_room_info[room_number].index_chatting_log, room_number, g_arr_room_info[room_number].status);
    sprintf(file_path, "%s/%d_%c.log", g_path_logdir, p_room_info->room_number, p_room_info->status);

    // realpath(file_name, file_path);

    fp = fopen(file_path, "a");
    if (fp == NULL)
    {
        printLog("fopen");
        return -1;
    }

    fwrite(message, 1, size_message, fp);

    if (fclose(fp) != 0)
    {
        printLog("fclose");
        return -1;
    }

    return 0;
}
size_t readChattingLog(room_info *p_room_info, char *id, char *buffer_ret, size_t size_buffer_ret)
{
    FILE *fp;
    char file_path[256];

    size_t retval;

    //sprintf(file_name, "log/%d_%d_%c.txt", g_arr_room_info[room_number].index_chatting_log, room_number, g_arr_room_info[room_number].status);
    sprintf(file_path, "%s/%d_%c.log", g_path_logdir, p_room_info->room_number, p_room_info->status);

    // realpath(file_name, file_path);

    fp = fopen(file_path, "r");
    if (fp == NULL)
    {
        printLog("fopen");
        return -1;
    }

    fseek(fp, 0, SEEK_END);
    if (ftell(fp) > SIZE_CHATTING_LOG)
        fseek(fp, -SIZE_CHATTING_LOG, SEEK_END);
    else
        fseek(fp, 0, SEEK_SET);

    retval = fread(buffer_ret, 1, size_buffer_ret, fp);
    //printLog("[message log read] [%s] [[%d/%d]%s]", file_path, retval, size_buffer_ret, buffer_ret);

    if (fclose(fp) != 0)
    {
        printLog("fclose");
        return -1;
    }

    return retval;
}

/*************** END OF FILE **********************************************/

