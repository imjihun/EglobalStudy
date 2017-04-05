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
#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <unistd.h>
#include <fcntl.h>
#include <signal.h>

#include <sys/epoll.h>

#include <sys/socket.h>
#include <netinet/in.h>

#include <sys/timeb.h>
#include <sys/types.h>
#include <sys/wait.h>

#include "../../printLog/printLog.h"
#include "../../macroFile.h"

/*************** New Data Types *******************************************/
typedef struct _sockinfo
{
    int sockfd;
    char buffer[SIZE_BUFFER];
} sockinfo;
/*************** Definitions / Macros  ************************************/

/*************** Global Variables *****************************************/
int PORT = PORT_SERVER;

/*************** Prototypes ***********************************************/
int initListenfd(int *listenfd, int port);
int initEpoll(int *epollfd);
int addSocketEpoll(int epollfd, int sockfd);
int delSocketEpoll(int epollfd, int sockfd);
int mainChattingServer();
int recvProcessing(int epollfd, int sockfd);
/*************** Function *************************************************/

int main(int argc, char **argv)
{
    return mainChattingServer();
}

int recvProcessing(int epollfd, int sockfd)
{
    char buffer[SIZE_BUFFER];
    int cnt_read, cnt_write, retval;

    cnt_read = 0;
    retval = recv(sockfd, buffer, SIZE_BUFFER-1, 0);
    if(retval <= 0)
    {
        if(delSocketEpoll(epollfd, sockfd) != 0)
            return -1;
    }
    cnt_read += retval;

    else {
        cnt_write = send(sockfd, buffer, cnt_read, 0);
        if(cnt_read != cnt_write)
        {
            printLog("[recv %d : send %d] write error\n", cnt_read, cnt_write);
            return -1;
        }
    }
    return 0;
}
int mainChattingServer()
{
    int listenfd;
    int epollfd;
    struct epoll_event *arr_return_event[MAX_CLIENT];

    int sockfd;
    int i;
    int cnt_return_event;



    if(initListenfd(&listenfd, PORT) != 0)
        return -1;

    if(initEpoll(&epollfd) != 0)
        return -1;

    if(addSocketEpoll(epollfd, listenfd) != 0)
        return -1;

    for( ; ; )
    {
        cnt_return_event = epoll_wait(epollfd, arr_return_event, MAX_CLIENT, -1);
        if(cnt_return_event < 0)
        {
            printLog("epoll_wait()");
            return -1;
        }

        for(i = 0; i < cnt_return_event; i++)
        {
            if(arr_return_event[i].data.fd == listenfd)
            {
                sockfd = accept(listenfd, NULL, NULL);
                if(sockfd <= 0)
                {
                    printLog("accept()");
                    return -1;
                }
                if(addSocketEpoll(epollfd, sockfd) != 0)
                    return -1;
            }
            else {
                if(recvProcessing(epollfd, arr_return_event[i].data.fd) != 0)
                    ;
            }
        }
    }

    return 0;
}

int initListenfd(int *listenfd, int port)
{
    struct sockaddr_in listen_addr;

    if( ((*listenfd) = socket(AF_INET, SOCK_STREAM, 0)) < 0)
    {
        printLog("socket()");
        return -1;
    }

    memset(&listen_addr, 0, sizeof(listen_addr));
    listen_addr.sin_family = AF_INET;
    listen_addr.sin_addr.s_addr = INADDR_ANY;
    listen_addr.sin_port = htons(port);

    if( bind((*listenfd), (struct sockaddr *) &listen_addr, sizeof(listen_addr)) < 0)
    {
        printLog("bind()");
        return -1;
    }

    if( listen((*listenfd), SOMAXCONN) != 0)
    {
        printLog("listen()");
        return -1;
    }

    return 0;
}
int initEpoll(int *epollfd)
{
    (*epollfd) = epoll_create(MAX_CLIENT);
    if(!(*epollfd))
    {
        printLog("epoll_create()");
        return -1;
    }

    return 0;
}
int addSocketEpoll(int epollfd, int sockfd)
{
    struct epoll_event tmp_event;

    tmp_event.events = EPOLLIN | EPOLLHUP;
    tmp_event.data.fd = sockfd;
    if(epoll_ctl(epollfd, EPOLL_CTL_ADD, sockfd, &tmp_event) < 0)
    {
        printLog("epoll_ctl(), adding sockfd");
        return -1;
    }
    return 0;
}

int delSocketEpoll(int epollfd, int sockfd)
{
    if(epoll_ctl(epollfd, EPOLL_CTL_DEL, sockfd, NULL) != 0)
    {
        printLog("epoll_ctl(), deleting sockfd");
        return -1;
    }

    if(close(sockfd) != 0)
    {
        printLog("epoll_ctl(), deleting sockfd");
        return -1;
    }
    return 0;   
}
/*************** END OF FILE **********************************************/





