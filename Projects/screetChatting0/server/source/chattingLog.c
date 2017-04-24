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

int setPath(char *path)
{
	sprintf(g_path_logdir, "%s", path);
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

	printf("[list] [%s] =>\n", g_path_logdir);
	system(buf);
	return 0;
}

int viewFile(TYPE_ROOM_NUMBER room_number)
{
	FILE *fp;
	char filefullname[1024];
	int retval = 0;

	room_info room;

	char buf[SIZE_BUFFER];
	char buf_plain[SIZE_BUFFER];

	// get room info
    if(dbOpen() != 0)
    {
		printLog("dbOpen() error\n");
		return -1;
    }
	if(dbSelectRoomOfRoomNumber(room_number, &room) != 0)
    {
		printLog("dbSelectRoomOfRoomNumber() error\n");
		return -1;
    }
	if(dbClose() != 0)
    {
		printLog("dbClose() error\n");
		return -1;
    }

    // get file path
	sprintf(filefullname, "%s/%09d.log", g_path_logdir, room_number);

	// open file
	fp = fopen(filefullname, "r");
	if(fp == NULL)
	{
		printLog("fopen() error\n");
		return -1;
	}


	printf("[view] [%s] =>\n", filefullname);

	while((retval = fread(buf, 1, SIZE_BUFFER - 1, fp)) > 0)
	{
		if(room.status == ROOM_INFO_STATUS_NORMAL)
		{
			// normal file
			buf[retval] = '\0';
			printf("%s", buf);
		}
		else if(room.status == ROOM_INFO_STATUS_SECRET)
		{
			// secret file
		    retval = decryptBuffer(
		        // key
		        room.key,
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
		}
	}

	return 0;
}

int removeAllFiles()
{
	char buf[1024];
	sprintf(buf, "rm -rf %s", g_path_logdir);
	system(buf);
	return 0;
}
int resetFile(room_info *p_room_info)
{
    FILE *fp;
    char file_path[256];

    //sprintf(file_name, "log/%d_%d_%c.txt", g_arr_room_info[room_number].index_chatting_log, room_number, g_arr_room_info[room_number].status);
    sprintf(file_path, "%s/%09d.log", g_path_logdir, p_room_info->room_number);

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
    sprintf(file_path, "%s/%09d.log", g_path_logdir, p_room_info->room_number);

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
    sprintf(file_path, "%s/%09d.log", g_path_logdir, p_room_info->room_number);

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
