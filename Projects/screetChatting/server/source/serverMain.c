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

#include <mysql.h>

#include "../../printLog/printLog.h"
#include "../../macroFile.h"

/*************** New Data Types *******************************************/
typedef struct _sockinfo
{
    int sockfd;
    char id[SIZE_ID];
    char buffer[SIZE_BUFFER];
    int cnt_read;
} socket_info;

/*************** Definitions / Macros  ************************************/

/*************** Global Variables *****************************************/
int PORT = PORT_SERVER;
socket_info *g_arr_socket_info[MAX_CLIENT];
int g_cnt_socket = 0;

/*************** Prototypes ***********************************************/
int initListenfd(int *listenfd, int port);
int initEpoll(int *epollfd);
int addSocketEpoll(int epollfd, int sockfd);
int delSocketEpoll(int epollfd, socket_info * sockfd);
int mainChattingServer();
int recvProcessing(int epollfd, socket_info * sockfd);
int packetProcessing(char *buffer);
int reuse(int sockfd);

int addSocketInfo(socket_info *p_socket_info);
int delSocketInfo(socket_info *p_socket_info);

int requestDB(const char *query)
{

    return 0;
}
int sendMessage(char *buffer, TYPE_PACKET_LENGTH size_packet)
{
    int i;
    int cnt_write;

    if(requestDB("") != 0)
        return -1;


    for(i = 1; i < g_cnt_socket; i++)
    {
        cnt_write = send(g_arr_socket_info[i]->sockfd, buffer, size_packet, 0);
        if(size_packet != cnt_write)
        {
            printLog("[recv %d : send %d] write error\n", size_packet, cnt_write);
            return -1;
        }
    }

    return 0;
}
/*************** Function *************************************************/

int main(int argc, char **argv)
{
    return mainChattingServer();
}


int packetProcessing(char *buffer)
{
    int retval = 0;
    TYPE_CMD cmd = *(TYPE_CMD*)buffer;
    TYPE_PACKET_LENGTH size_packet = *(TYPE_PACKET_LENGTH *)(buffer + SIZE_CMD);

    buffer[size_packet] = 0;
    
    switch(cmd)
    {
    case CMD_CREATE_ID:
        printLog("[server] [create id] %s", buffer + SIZE_HEADER);
        break;

    case CMD_CREATE_ROOM:
        printLog("[server] [create room] %s", buffer + SIZE_HEADER);
        break;

    case CMD_ENTER_ROOM:
        printLog("[server] [enter room] %s", buffer + SIZE_HEADER);
        break;

    case CMD_LEAVE_ROOM:
        printLog("[server] [leave room] %s", buffer + SIZE_HEADER);
        break;

    case CMD_VIEW_ROOM:
        printLog("[server] [view room] %s", buffer + SIZE_HEADER);
        break;

    case CMD_TOTAL_ROOM_LIST:
        printLog("[server] [total room list] %s", buffer + SIZE_HEADER);
        break;

    case CMD_MY_ROOM_LIST:
        printLog("[server] [my room list] %s", buffer + SIZE_HEADER);
        break;

    case CMD_MESSAGE:
        printLog("[server] [message] %s", buffer + SIZE_HEADER);
        retval = sendMessage(buffer, size_packet);
        break;

    default:
        retval = -1;
        break;
    }

    return retval;
}
int recvProcessing(int epollfd, socket_info *p_socket_info)
{
    int retval;

    int sockfd = p_socket_info->sockfd;
    char *buffer = p_socket_info->buffer;

    TYPE_PACKET_LENGTH size_packet;

    retval = recv(sockfd, buffer + p_socket_info->cnt_read, SIZE_BUFFER - p_socket_info->cnt_read - 1, 0);
    if(retval <= 0)
    {
        if(delSocketEpoll(epollfd, p_socket_info) != 0)
            return -1;

        return -1;
    }
    p_socket_info->cnt_read += retval;

    if(p_socket_info->cnt_read < SIZE_HEADER)
        return 1;

    size_packet = *(TYPE_PACKET_LENGTH *)(buffer + SIZE_CMD);
    if(size_packet > p_socket_info->cnt_read)
        return 1;

    if(packetProcessing(buffer) != 0)
    {
        return -1;
    }

    p_socket_info->cnt_read = 0;
    return 0;
}
int mainChattingServer()
{
    int listenfd;
    int epollfd;
    struct epoll_event arr_return_event[MAX_CLIENT];

    int sockfd;
    int i;
    int cnt_return_event;

    socket_info *p_socket_info;


    if(initListenfd(&listenfd, PORT) != 0)
        return -1;

    if(initEpoll(&epollfd) != 0)
        return -1;

    if(addSocketEpoll(epollfd, listenfd) != 0)
        return -1;

    while(1)
    {
        cnt_return_event = epoll_wait(epollfd, arr_return_event, MAX_CLIENT, -1);
        if(cnt_return_event < 0)
        {
            printLog("epoll_wait()");
            return -1;
        }

        for(i = 0; i < cnt_return_event; i++)
        {
            p_socket_info = (socket_info*)arr_return_event[i].data.ptr;
            if(p_socket_info->sockfd == listenfd)
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
                if(recvProcessing(epollfd, p_socket_info) != 0)
                    ;
            }
        }
    }

    return 0;
}

int reuse(int sockfd)
{
    int enable = 1;
    if (setsockopt(sockfd, SOL_SOCKET, SO_REUSEADDR, &enable, sizeof(int)) < 0)
    {
        printLog("setsockopt(SO_REUSEADDR) failed");
        return -1;
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
    if(reuse((*listenfd)) != 0)
        return -1;

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

    socket_info *p_socket_info = (socket_info*)malloc(sizeof(socket_info));
    if(p_socket_info == NULL)
    {
        printLog("malloc()");
        return -1;
    }
    p_socket_info->sockfd = sockfd;
    p_socket_info->cnt_read = 0;

    tmp_event.events = EPOLLIN | EPOLLHUP;
    tmp_event.data.ptr = (void*)p_socket_info;
    if(epoll_ctl(epollfd, EPOLL_CTL_ADD, sockfd, &tmp_event) < 0)
    {
        printLog("epoll_ctl(), adding sockfd");
        return -1;
    }

    if(addSocketInfo(p_socket_info) != 0)
        return -1;

    return 0;
}

int delSocketEpoll(int epollfd, socket_info *p_socket_info)
{
    if(epoll_ctl(epollfd, EPOLL_CTL_DEL, p_socket_info->sockfd, NULL) != 0)
    {
        printLog("epoll_ctl(), deleting sockfd");
        return -1;
    }

    if(close(p_socket_info->sockfd) != 0)
    {
        printLog("close()");
        return -1;
    }

    free(p_socket_info);

    if(delSocketInfo(p_socket_info) != 0)
        return -1;

    return 0;   
}

int addSocketInfo(socket_info *p_socket_info)
{
    if(g_cnt_socket >= MAX_CLIENT)
        return -1;

    g_arr_socket_info[g_cnt_socket++] = p_socket_info;
    return 0;
}

int delSocketInfo(socket_info *p_socket_info)
{
    int i;
    for(i = 0; i < g_cnt_socket; i++)
    {
        if(g_arr_socket_info[i] == p_socket_info)
            break;
    }
    if(i >= g_cnt_socket)
        return -1;

    g_cnt_socket--;
    for(; i < g_cnt_socket; i++)
    {
        g_arr_socket_info[i] = g_arr_socket_info[i + 1];
    }

    return 0;
}
/*************** END OF FILE **********************************************/





