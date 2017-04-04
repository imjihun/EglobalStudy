/* 2017-04-04 : created by JHLim */
/***************************************************************************
* Copyright (c) 2004-2016, eGlobal Systems, Co., Ltd.
* Seoul, Republic of Korea
* All Rights Reserved.
*
* Description : processManager.c
*       xxxx
*
***************************************************************************/

/*************** Header files *********************************************/
#include <sys/time.h>
#include <sys/types.h>
#include <unistd.h>

#include <sys/socket.h>
#include <sys/types.h>
#include <sys/stat.h> 
#include <stdlib.h> 
#include <sys/poll.h> 

#include <netinet/in.h>
#include <arpa/inet.h>

#include <stdio.h>

#include "myProcess.h"

/*************** New Data Types *******************************************/

/*************** Definitions / Macros  ************************************/

/*************** Global Variables *****************************************/

int PORT = PROCESS_MANAGER_PORT;
char MAIN_PROC_PATH[256] = "./processMain";

/*************** Prototypes ***********************************************/

int writeAll(struct pollfd *arr_client, char *buf);
int startMainProc(int port);
int initServer(int *listenfd, int port);
int acceptSockfd(int listenfd, struct pollfd *arr_client);
int readWrite(struct pollfd *arr_client);
int mainProcessManager(int port_mainproc);

/*************** Function *************************************************/
int main(int argc, char **argv)
{
    int port_mainproc = MAIN_PROCESS_PORT;

    if(argc == 2)
        port_mainproc = atoi(argv[1]);

    return mainProcessManager(port_mainproc);
}

int mainProcessManager(int port_mainproc)
{
    int listenfd;

    int retval_poll;
    struct pollfd arr_client[MAX_CLIENT];
    int i;

    if(initServer(&listenfd, PORT) == -1)
        return -1;
    
    arr_client[0].fd = listenfd;
    arr_client[0].events = POLLIN;
    for (i = 1; i < MAX_CLIENT; i++)
    {
        arr_client[i].fd = -1;
    }

    if(startMainProc(port_mainproc) == -1)
        return -1;

    while(1)
    {
        retval_poll = poll(arr_client, MAX_CLIENT, -1);

        if(retval_poll == 0)
            continue;
        else if(retval_poll < 0)
        {
            printLog("poll error");
            return -1;
        }

        if (arr_client[0].revents & POLLIN)
        {
            if(acceptSockfd(listenfd, arr_client) != 0)
            {
                return -1;
            }
            if (--retval_poll <= 0)
                continue;
        }

        readWrite(arr_client);
    }
}


int initServer(int *listenfd, int port)
{
    struct sockaddr_in server_addr;

    if(((*listenfd) = socket(AF_INET, SOCK_STREAM, 0)) == -1)
    {
        printLog("socket error");
        return -1;
    }   
    memset((void *)&server_addr, 0x00, sizeof(server_addr));
    server_addr.sin_family = AF_INET;
    server_addr.sin_addr.s_addr = htonl(INADDR_ANY);
    server_addr.sin_port = htons(port);
    
    if(bind((*listenfd), (struct sockaddr *)&server_addr, sizeof(server_addr)) == -1)
    {
        printLog("bind error");
        return -1;
    }   
    if(listen((*listenfd), 5) == -1)
    {
        printLog("listen error");
        return -1;
    }
    return 0;
}
int startMainProc(int port)
{
    int pid;
    char arg[256];

    // child
    if ((pid = fork()) == 0) 
    {
        sprintf(arg, "%d", port);
        execl(MAIN_PROC_PATH, MAIN_PROC_PATH, arg, NULL);
    }
    // parent
    else if(pid > 0)
    {
        return 0;
    }
    else
    {
        printLog("fork() error");
        return -1;
    }
    return 0;
}


int writeAll(struct pollfd *arr_client, char *buf)
{
    int i;
    for(i = 1; i < MAX_CLIENT; i++)
    {
        if(arr_client[i].fd != -1)
            write(arr_client[i].fd, buf, strlen(buf));
    }
    return 0;
}

int readWrite(struct pollfd *arr_client)
{
    int cur_sockfd;
    int recv_cnt = 0;
    char buf[BUF_SIZE];
    int i;

    for (i = 1; i <= MAX_CLIENT; i++)
    {
        if ((cur_sockfd = arr_client[i].fd) < 0)
        {
            continue;
        }
        if (arr_client[i].revents & (POLLIN | POLLERR))
        {
            if( (recv_cnt = read(cur_sockfd, buf, BUF_SIZE-1)) <= 0)
            {
                printLog("close [%d]\n", cur_sockfd);
                close(cur_sockfd);
                arr_client[i].fd = -1;
            }
            else
            {
                buf[recv_cnt] = '\0';
                printLog("[%d]recv = %s", cur_sockfd, buf);
                writeAll(arr_client, buf);
                printLog("send = %s", buf);
            }
        }
    }
    return 0;
}

int acceptSockfd(int listenfd, struct pollfd *arr_client)
{
    int client_fd;
    struct sockaddr_in client_addr;
    socklen_t addrlen;
    int i;

    addrlen = sizeof(client_addr);
    client_fd = accept(listenfd,
            (struct sockaddr *)&client_addr, &addrlen);
    for (i = 1; i < MAX_CLIENT; i++)
    {
        if (arr_client[i].fd < 0)
        {
            arr_client[i].fd = client_fd;
            break;
        }
    }

    if (i == MAX_CLIENT)
    {
        printLog("too many clients : ");
        return -1;
    }

    arr_client[i].events = POLLIN;
    return 0;
}
/*************** END OF FILE **********************************************/





