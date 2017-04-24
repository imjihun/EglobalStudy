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
#include "connectDB.h"

#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <unistd.h>
#include <fcntl.h>
#include <signal.h>
#include <pwd.h>
#include <errno.h>
#include <dirent.h>

#include <sys/epoll.h>

#include <sys/socket.h>
#include <netinet/in.h>

#include <sys/timeb.h>
#include <sys/types.h>
#include <sys/stat.h>
#include <sys/wait.h>

#include "../../printLog/printLog.h"
#include "../../macroFile.h"
#include "chattingLog.h"
#include "encrypt.h"

/*************** New Data Types *******************************************/

/*************** Definitions / Macros  ************************************/

/*************** Global Variables *****************************************/
int PORT = PORT_SERVER;

socket_info *g_arr_p_socket_info[MAX_CLIENT];
int g_cnt_socket = 0;

int g_epollfd = 0;
/*************** Prototypes ***********************************************/
int initDaemon();

int initChattingServer(int *listenfd);
int runChattingServer(int listenfd);

int initListenfd(int *listenfd, int port);
int initEpoll();
int addSocketEpoll(int sockfd);
int delSocketEpoll(socket_info * sockfd);

int mainChattingServer();
int checkKnowPacket(socket_info *p_socket_info);
int recvProcessing(socket_info * sockfd);
int packetProcessing(socket_info *p_socket_info, char *packet);
int reuse(int sockfd);

int addSocketInfoArr(socket_info *p_socket_info);
int delSocketInfoArr(socket_info *p_socket_info);
int findSocketInfoArr(const char *id, socket_info **p_socket_info_ret);

int sendPacket(socket_info *p_socket_info, char *packet, int size_packet);
int sendFail(socket_info *p_socket_info, TYPE_CMD cmd);
int spridePacket(TYPE_ROOM_NUMBER room_number, char *packet, TYPE_PACKET_LENGTH size_packet);
int makeMessage(char *id, room_info *p_room, char *message, int size_message, char *message_ret);
int sprideChattingMessage(room_info *p_room, char *id, char *message, int size_message);

int cmdCreateId(socket_info *p_socket_info);
int cmdCreateRoom(socket_info *p_socket_info);
int cmdEnterRoom(socket_info *p_socket_info);
int cmdLeaveRoom(socket_info *p_socket_info);
int cmdViewRoom(socket_info *p_socket_info);
int cmdTotalRoomList(socket_info *p_socket_info);
int cmdMyRoomList(socket_info *p_socket_info);
int cmdChattingMessage(socket_info *p_socket_info);
int cmdInvite(socket_info *p_socket_info);

int viewPacket(char *packet);
int viewRecv(int sockfd, char *packet);
int viewSend(int sockfd, char *packet, int size_packet);

/*************** Function *************************************************/

int viewPacket(char *packet)
{
    TYPE_PACKET_LENGTH size = *(TYPE_PACKET_LENGTH *)(packet + SIZE_CMD);
    int i;


    printf("[cmd = %d, length = %d] [ ", *(TYPE_CMD*)packet, *(TYPE_PACKET_LENGTH *)(packet + SIZE_CMD));
    for (i = 0; i < size; i++)
    {
        printf("%2x ", *(unsigned char *)(packet + i));
    }
    printf("]\n\n");

    return 0;
}

int viewSend(int sockfd, char *packet, int size_packet)
{
    printLog("[server] [send sock(%d) size = %d] ", sockfd, size_packet);
    return viewPacket(packet);
    //return 0;
}

int viewRecv(int sockfd, char *packet)
{
    printLog("[server] [recv sock(%d)] ", sockfd);
    return viewPacket(packet);
    //return 0;
}

int main(int argc, char **argv)
{
    int retval;
    if(argc < 2 || 
        (argv[1][0] != '-' || strlen(argv[1]) != 2))
    {
        printf("usage %s <-option>\n", argv[0]);
        printf("\t -d : start deamon server\n");
        printf("\t -n : start normal server\n");
        printf("\n");
        return -1;
    }

    if(dbOpen() < 0)
    {
        printLog("dbOpen()");
        return -2;
    }
    if(dbCreateAllTableIfNotExists() < 0)
    {
        printLog("dbCreateAllTable()");
        return -2;
    }

    if(argv[1][1] == 'd')
    {
        if (initDaemon() < 0)
        {
            printLog("initDaemon()");
            return -1;
        }
    }
    retval = mainChattingServer();
    dbClose();

    return retval;
}

int packetProcessing(socket_info *p_socket_info, char *packet)
{
    int sockfd = p_socket_info->sockfd;
    int retval = 0;
    char *buffer = packet;
    TYPE_CMD cmd = *(TYPE_CMD*)buffer;
    TYPE_PACKET_LENGTH size_packet = *(TYPE_PACKET_LENGTH *)(buffer + SIZE_CMD);

    buffer[size_packet] = 0;

    viewRecv(p_socket_info->sockfd, packet);
    switch (cmd)
    {
    case CMD_CREATE_ID:
        printLog("[server] [create id] [id = %s]\n", buffer + SIZE_HEADER);
        retval = cmdCreateId(p_socket_info);
        break;

    case CMD_CREATE_ROOM:
        printLog("[server] [create room] [id = %s][%c][subject = %s]\n", buffer + SIZE_HEADER, *(buffer + SIZE_HEADER + SIZE_ID), buffer + SIZE_HEADER + SIZE_ID + SIZE_ROOM_STATUS);
        retval = cmdCreateRoom(p_socket_info);
        break;

    case CMD_ENTER_ROOM:
        printLog("[server] [enter room] [id = %s]\n", buffer + SIZE_HEADER);
        retval = cmdEnterRoom(p_socket_info);
        break;

    case CMD_LEAVE_ROOM:
        printLog("[server] [leave room] [id = %s]\n", buffer + SIZE_HEADER);
        retval = cmdLeaveRoom(p_socket_info);
        break;

    case CMD_VIEW_ROOM:
        printLog("[server] [view room] [id = %s]\n", buffer + SIZE_HEADER);
        retval = cmdViewRoom(p_socket_info);
        break;

    case CMD_TOTAL_ROOM_LIST:
        printLog("[server] [total room list] [id = %s]\n", buffer + SIZE_HEADER);
        retval = cmdTotalRoomList(p_socket_info);
        break;

    case CMD_MY_ROOM_LIST:
        printLog("[server] [my room list] [id = %s]\n", buffer + SIZE_HEADER);
        retval = cmdMyRoomList(p_socket_info);
        break;

    case CMD_CHATTING_MESSAGE:
        printLog("[server] [message] [id = %s] [%d]\n", buffer + SIZE_HEADER, size_packet - (SIZE_HEADER + SIZE_ID + SIZE_ROOM_NUMBER));
        retval = cmdChattingMessage(p_socket_info);
        break;

    case CMD_INVITE:
        printLog("[server] [invite] [id = %s] [your id = %s]\n", buffer + SIZE_HEADER, buffer + SIZE_HEADER + SIZE_ID);
        retval = cmdInvite(p_socket_info);
        break;

    default:
        if (delSocketEpoll(p_socket_info) < 0)
        {
            printLog("delSocketEpoll()");
            return NOT_DEFINE_CMD;
        }
        printLog("close sock(%d)", sockfd);
        return NOT_DEFINE_CMD;
    }

    if (retval < 0)
    {
        sendFail(p_socket_info, cmd);
        printLog("in packetProcessing()");
    }

    return retval;
}
int checkKnowPacket(socket_info *p_socket_info)
{
    TYPE_CMD cmd = *(TYPE_CMD *)p_socket_info->buffer;

    if(     cmd == CMD_CREATE_ID
        ||  cmd == CMD_CREATE_ROOM
        ||  cmd == CMD_ENTER_ROOM
        ||  cmd == CMD_LEAVE_ROOM
        ||  cmd == CMD_VIEW_ROOM
        ||  cmd == CMD_TOTAL_ROOM_LIST
        ||  cmd == CMD_MY_ROOM_LIST
        ||  cmd == CMD_CHATTING_MESSAGE
        ||  cmd == CMD_INVITE)
        return 0;
    else
    {
        printLog("[server] [not define command]\n");
        return NOT_DEFINE_CMD;
    }
}
int recvProcessing(socket_info *p_socket_info)
{
    int retval;
    int number_error = 0;

    int sockfd = p_socket_info->sockfd;
    char *buffer = p_socket_info->buffer;
    char packet[SIZE_BUFFER];

    TYPE_PACKET_LENGTH size_packet;

    retval = recv(sockfd, buffer + p_socket_info->cnt_read, SIZE_BUFFER - p_socket_info->cnt_read - 1, 0);
    if (retval <= 0)
    {
        printLog("recv()");

        if (delSocketEpoll(p_socket_info) < 0)
        {
            printLog("delSocketEpoll()");
            return -1;
        }
        printLog("close sock(%d)", sockfd);
        return -1;
    }
    p_socket_info->cnt_read += retval;

    if (p_socket_info->cnt_read < SIZE_HEADER)
        return 1;

    if (checkKnowPacket(p_socket_info) == NOT_DEFINE_CMD)
    {
        if (delSocketEpoll(p_socket_info) < 0)
        {
            printLog("delSocketEpoll()");
            return -1;
        }
        printLog("close sock(%d)", sockfd);
        return -1;
    }

    while(SIZE_HEADER <= p_socket_info->cnt_read
        && (size_packet = *(TYPE_PACKET_LENGTH *)(buffer + SIZE_CMD)) <= p_socket_info->cnt_read)
    {
        memcpy(packet, buffer, size_packet);
        retval = packetProcessing(p_socket_info, packet);
        if (retval < 0)
        {
            if(retval == NOT_DEFINE_CMD)
                return NOT_DEFINE_CMD;

            printLog("packetProcessing()");

            number_error = -1;
        }

        p_socket_info->cnt_read -= size_packet;
        if (p_socket_info->cnt_read > 0)
            memcpy(buffer, buffer + size_packet, p_socket_info->cnt_read);
    }

    return number_error;
}
int initChattingServer(int *listenfd)
{
    if (initPath() < 0)
    {
        printLog("initPath()");
        return -1;
    }

    if (initListenfd(listenfd, PORT) < 0)
    {
        printLog("initListenfd()");
        return -1;
    }

    if (initEpoll() < 0)
    {
        printLog("initEpoll()");
        return -1;
    }

    if (addSocketEpoll(*listenfd) < 0)
    {
        printLog("addSocketEpoll()");
        return -1;
    }
    return 0;
}
int runChattingServer(int listenfd)
{
    struct epoll_event arr_return_event[MAX_CLIENT];

    int sockfd;
    int i;
    int cnt_return_event;

    socket_info *p_socket_info;


    while (1)
    {
        cnt_return_event = epoll_wait(g_epollfd, arr_return_event, MAX_CLIENT, -1);
        if (cnt_return_event < 0)
        {
            printLog("epoll_wait()");
            return -1;
        }

        for (i = 0; i < cnt_return_event; i++)
        {
            p_socket_info = (socket_info*)arr_return_event[i].data.ptr;
            if (p_socket_info->sockfd == listenfd)
            {
                sockfd = accept(listenfd, NULL, NULL);
                if (sockfd <= 0)
                {
                    printLog("accept()");
                    return -1;
                }
                if (addSocketEpoll(sockfd) < 0)
                {
                    printLog("addSocketEpoll()");
                    return -1;
                }
                printLog("accept sock(%d)", sockfd);
            }
            else {
                if (recvProcessing(p_socket_info) < 0)
                {
                    printLog("recvProcessing()");
                }
            }
        }
    }
    return 0;
}
int mainChattingServer()
{
    int listenfd;
    if(initChattingServer(&listenfd) < 0)
    {
        printLog("initChattingServer()");
        return -1;
    }

    if(runChattingServer(listenfd) < 0)
    {
        printLog("runChattingServer()");
        return -1;   
    }
    return 0;
}

int reuse(int sockfd)
{
    int enable = 1;
    if (setsockopt(sockfd, SOL_SOCKET, SO_REUSEADDR, &enable, sizeof(int)) < 0)
    {
        printLog("setsockopt(SO_REUSEADDR)");
        return -1;
    }
    return 0;
}

int initListenfd(int *listenfd, int port)
{
    struct sockaddr_in listen_addr;

    if (((*listenfd) = socket(AF_INET, SOCK_STREAM, 0)) < 0)
    {
        printLog("socket()");
        return -1;
    }
    if (reuse((*listenfd)) < 0)
    {
        printLog("reuse()");
        return -1;
    }

    memset(&listen_addr, 0, sizeof(listen_addr));
    listen_addr.sin_family = AF_INET;
    listen_addr.sin_addr.s_addr = INADDR_ANY;
    listen_addr.sin_port = htons(port);

    if (bind((*listenfd), (struct sockaddr *) &listen_addr, sizeof(listen_addr)) < 0)
    {
        printLog("bind()");
        return -1;
    }

    if (listen((*listenfd), SOMAXCONN) < 0)
    {
        printLog("listen()");
        return -1;
    }

    return 0;
}
int initEpoll()
{
    g_epollfd = epoll_create(MAX_CLIENT);
    if (g_epollfd == 0)
    {
        printLog("epoll_create()");
        return -1;
    }
    return 0;
}
int addSocketEpoll(int sockfd)
{
    struct epoll_event tmp_event;

    socket_info *p_socket_info = (socket_info*)malloc(sizeof(socket_info));
    if (p_socket_info == NULL)
    {
        printLog("malloc()");
        return -1;
    }
    p_socket_info->sockfd = sockfd;
    p_socket_info->cnt_read = 0;
    memset(p_socket_info->id, 0, SIZE_ID);

    tmp_event.events = EPOLLIN | EPOLLHUP;
    tmp_event.data.ptr = (void*)p_socket_info;
    if (epoll_ctl(g_epollfd, EPOLL_CTL_ADD, sockfd, &tmp_event) < 0)
    {
        printLog("epoll_ctl(), adding sockfd");
        return -1;
    }

    if (addSocketInfoArr(p_socket_info) < 0)
    {
        printLog("addSocketInfoArr()");
        return -1;
    }

    return 0;
}

int delSocketEpoll(socket_info *p_socket_info)
{
    if (epoll_ctl(g_epollfd, EPOLL_CTL_DEL, p_socket_info->sockfd, NULL) < 0)
    {
        printLog("epoll_ctl(), deleting sockfd");
        return -1;
    }

    if (close(p_socket_info->sockfd) < 0)
    {
        printLog("close()");
        return -1;
    }

    free(p_socket_info);

    if (delSocketInfoArr(p_socket_info) < 0)
    {
        printLog("delSocketInfoArr()");
        return -1;
    }

    return 0;
}

int addSocketInfoArr(socket_info *p_socket_info)
{
    if (g_cnt_socket >= MAX_CLIENT)
    {
        printLog("socket max");
        return -1;
    }

    g_arr_p_socket_info[g_cnt_socket++] = p_socket_info;
    return 0;
}

int delSocketInfoArr(socket_info *p_socket_info)
{
    int i;
    for (i = 0; i < g_cnt_socket; i++)
    {
        if (g_arr_p_socket_info[i] == p_socket_info)
            break;
    }
    if (i >= g_cnt_socket)
    {
        printLog("socket max");
        return -1;
    }

    g_cnt_socket--;
    for (; i < g_cnt_socket; i++)
    {
        g_arr_p_socket_info[i] = g_arr_p_socket_info[i + 1];
    }

    return 0;
}

int findSocketInfoArr(const char *id, socket_info **p_socket_info_ret)
{
    int i;
    for (i = 0; i < g_cnt_socket; i++)
    {
        if (memcmp(id, g_arr_p_socket_info[i]->id, SIZE_ID) == 0)
        {
            *p_socket_info_ret = g_arr_p_socket_info[i];
            return 0;
        }
    }
    printLog("not found socket");
    return -1;
}

int sendPacket(socket_info *p_socket_info, char *packet, int size_packet)
{
    int send_cnt = 0, retval;

    while (send_cnt < size_packet)
    {
        retval = send(p_socket_info->sockfd, packet + send_cnt, size_packet - send_cnt, 0);
        if (retval <= 0)
        {
            printLog("send() sockfd = %d, send_cnt = %d, retval = %d, size_packet = %d", p_socket_info->sockfd, send_cnt, retval, size_packet);
            if(delSocketEpoll(p_socket_info) < 0)
            {
                printLog("delSocketEpoll()");
                return -1;
            }
            return -1;
        }
        send_cnt += retval;
    }
    viewSend(p_socket_info->sockfd, packet, size_packet);
    return 0;
}
int sendFail(socket_info *p_socket_info, TYPE_CMD cmd)
{
    TYPE_CMD cmd_fail = CMD_FAIL;
    char buffer_send[SIZE_BUFFER];
    TYPE_PACKET_LENGTH size_send_packet = 0;

    //cmd
    memcpy(buffer_send + size_send_packet, &cmd, SIZE_CMD);
    size_send_packet += SIZE_HEADER;

    //cmd_fail
    memcpy(buffer_send + size_send_packet, &cmd_fail, SIZE_CMD);
    size_send_packet += SIZE_CMD;

    // room number for removing myroom when not existing room
    if(cmd == CMD_ENTER_ROOM || cmd == CMD_VIEW_ROOM)
    {
        memcpy(buffer_send + size_send_packet, p_socket_info->buffer + SIZE_HEADER + SIZE_ID, SIZE_ROOM_NUMBER);
        size_send_packet += SIZE_ROOM_NUMBER;
    }

    //length
    memcpy(buffer_send + SIZE_CMD, &size_send_packet, SIZE_PACKET_LENGTH);

    // send packet
    if (sendPacket(p_socket_info, buffer_send, size_send_packet) < 0)
    {
        printLog("sendPacket()");
        return -1;
    }

    return 0;
}

int spridePacket(TYPE_ROOM_NUMBER room_number, char *packet, TYPE_PACKET_LENGTH size_packet)
{
    char arr_id[1024][SIZE_ID];
    int size_arr = 1024;
    int count;

    socket_info *p_socket_info_ret;

    int i;

    if((count = dbSelectUserInRoom(room_number, arr_id, size_arr)) < 0)
    {
        printLog("dbSelectUserInRoom()");
        return -2;
    }

    for(i = 0; i < count; i++)
    {
        if(findSocketInfoArr(arr_id[i], &p_socket_info_ret) < 0)
        {
            printLog("findSocketInfoArr()");
            continue;
        }

        if(sendPacket(p_socket_info_ret, packet, size_packet) < 0)
        {
            printLog("sendPacket()");
            continue;
        }
    }
    return 0;
}

int makeMessage(char *id, room_info *p_room, char *message, int size_message, char *message_ret)
{
    int retval;
    time_t timer;
    struct tm *t;

    char buffer_plain[SIZE_BUFFER];
    char *p_buffer = message_ret;

    timer = time(NULL);
    t = localtime(&timer);

    if (p_room->status == ROOM_INFO_STATUS_SECRET)
        p_buffer = buffer_plain;

    // make message template
    retval = sprintf(p_buffer, "[%04d_%02d_%02d %02d:%02d:%02d] %s : ", t->tm_year + 1900, t->tm_mon + 1, t->tm_mday, t->tm_hour, t->tm_min, t->tm_sec, id);
    memcpy(p_buffer + retval, message, size_message);
    retval += size_message;
    p_buffer[retval++] = '\n';
    p_buffer[retval] = '\0';

    // check encrypt & encrypt
    if (p_room->status == ROOM_INFO_STATUS_SECRET)
        retval = encryptBuffer(p_room->key, p_buffer, retval, message_ret);

    return retval;
}

int sprideChattingMessage(room_info *p_room, char *id, char *message, int size_message)
{
    char packet[SIZE_BUFFER];
    char remake_message[SIZE_BUFFER];
    int size_remake_message;
    TYPE_CMD cmd;
    TYPE_PACKET_LENGTH length;

    // make packet
    // cmd
    cmd = CMD_CHATTING_MESSAGE;
    memcpy(packet, &cmd, SIZE_CMD);
    length = SIZE_HEADER;

    // id
    memcpy(packet + length, id, SIZE_ID);
    length += SIZE_ID;

    // room number
    memcpy(packet + length, &p_room->room_number, SIZE_ROOM_NUMBER);
    length += SIZE_ROOM_NUMBER;

    // message
    size_remake_message = makeMessage(id, p_room, message, size_message, remake_message);
    memcpy(packet + length, remake_message, size_remake_message);
    length += size_remake_message;

    // length
    memcpy(packet + SIZE_CMD, &length, SIZE_PACKET_LENGTH);

    // send packet
    if(spridePacket(p_room->room_number, packet, length) < 0)
    {
        printLog("spridePacket()");
        return -1;
    }

    // write log file
    if (writeChattingLog(p_room, packet + SIZE_HEADER, remake_message, size_remake_message) < 0)
    {
        printLog("writeChattingLog()");
        return -3;
    }
    return 0;
}
/*****************************************************/
/*********************** cmd Function *****************************/
int cmdCreateId(socket_info *p_socket_info)
{
    char *buffer = p_socket_info->buffer;
    char *id = buffer + SIZE_HEADER;

    if (dbInsertUserinfo(id) < 0)
    {
        printLog("dbInsertUserinfo()");
        return -2;
    }

    memcpy(p_socket_info->id, id, SIZE_ID);

    // send packet
    if (sendPacket(p_socket_info, buffer, *(TYPE_PACKET_LENGTH *)(buffer + SIZE_CMD)) < 0)
    {
        printLog("sendPacket()");
        return -1;
    }
    return 0;
}
int cmdCreateRoom(socket_info *p_socket_info)
{
    char buffer_send[SIZE_BUFFER];
    int size_send_packet = 0;
    char *buffer = p_socket_info->buffer;

    char *id = buffer + SIZE_HEADER;
    char is_secret = *(id + SIZE_ID);
    char *room_subject = id + SIZE_ID + SIZE_ROOM_STATUS;

    int retval;

    room_info room;
    room.room_number = -1;
    room.status = is_secret;
    memcpy(room.subject, room_subject, SIZE_ROOM_SUBJECT);
    room.count_member = 1;
    if(room.status == ROOM_INFO_STATUS_SECRET)
        makeKey(room.key);

    if (dbInsertRoominfo(&room) < 0)
    {
        printLog("dbInsertRoominfo()");
        return -2;
    }
    if (dbInsertRoomUser(room.room_number, p_socket_info->id) < 0)
    {
        printLog("dbInsertRoomUser()");
        return -2;
    }
    
    if (resetFile(&room) < 0)
    {
        printLog("resetFile()");
        return -3;
    }
    
    // S->C # CMD[2] : LENGTH[4] : ID or CMD_FAIL : ROOM_NUMBER : STATUS : SUBJECT : SECRETKEY
    // make packet
    // cmd, id
    memcpy(buffer_send, buffer, SIZE_HEADER + SIZE_ID);
    size_send_packet += SIZE_HEADER + SIZE_ID;

    // room number
    memcpy(buffer_send + size_send_packet, &room.room_number, SIZE_ROOM_NUMBER);
    size_send_packet += SIZE_ROOM_NUMBER;

    // status
    memcpy(buffer_send + size_send_packet, &room.status, SIZE_ROOM_STATUS);
    size_send_packet += SIZE_ROOM_STATUS;

    // subject
    memcpy(buffer_send + size_send_packet, room.subject, SIZE_ROOM_SUBJECT);
    size_send_packet += SIZE_ROOM_SUBJECT;

    // secretKey
    memcpy(buffer_send + size_send_packet, room.key, SIZE_SECRET_KEY);
    size_send_packet += SIZE_SECRET_KEY;

    // length
    memcpy(buffer_send + SIZE_CMD, &size_send_packet, SIZE_PACKET_LENGTH);

    
    // send packet
    if (sendPacket(p_socket_info, buffer_send, size_send_packet) < 0)
    {
        printLog("sendPacket()");
        return -1;
    }


    
    // send message packet
    retval = sprintf(buffer_send, "[%s is joined]", buffer + SIZE_HEADER);
    memset(buffer_send + retval, VALUE_SYSTEM_MESSAGE_PADDING, SIZE_SYSTEM_MESSAGE_PADDING);
    retval += SIZE_SYSTEM_MESSAGE_PADDING;
    if (sprideChattingMessage(&room, buffer + SIZE_HEADER, buffer_send, retval) < 0)
    {
        printLog("sprideChattingMessage()");
        return -1;
    }

    return 0;
}

int cmdEnterRoom(socket_info *p_socket_info)
{
    char *buffer = p_socket_info->buffer;
    TYPE_ROOM_NUMBER room_number = *(TYPE_ROOM_NUMBER *)(buffer + SIZE_HEADER + SIZE_ID);

    char buffer_send[SIZE_BUFFER];
    int cnt_send;
    int retval;

    room_info room;

    if(dbCheckRoomUser(room_number, p_socket_info->id) == 0)
    {
        //printLog("already exist roomuser");
        return 1;
    }

    if (dbInsertRoomUser(room_number, p_socket_info->id) < 0)
    {
        printLog("dbInsertRoomUser()");
        return -2;
    }
    if(dbSelectRoomOfRoomNumber(room_number, &room) < 0)
    {
        printLog("dbSelectRoomOfRoomNumber()");
        return -2;
    }

    // make packet EnterRoom
    // CMD : ID : ROOM_NUMBER 
    cnt_send = SIZE_HEADER + SIZE_ID + SIZE_ROOM_NUMBER;
    memcpy(buffer_send, buffer, cnt_send);

    // status
    buffer_send[cnt_send] = room.status;
    cnt_send++;

    // subject
    memcpy(buffer_send + cnt_send, room.subject, SIZE_ROOM_SUBJECT);
    cnt_send += SIZE_ROOM_SUBJECT;

    // secretKey
    memcpy(buffer_send + cnt_send, room.key, SIZE_SECRET_KEY);
    cnt_send += SIZE_SECRET_KEY;

    // count member
    memcpy(buffer_send + cnt_send, &room.count_member, sizeof(room.count_member));
    cnt_send += sizeof(room.count_member);    

    // length
    memcpy(buffer_send + SIZE_CMD, &cnt_send, SIZE_PACKET_LENGTH);

    // send packet EnterRoom
    if(spridePacket(room_number, buffer_send, cnt_send) < 0)
    {
        printLog("spridePacket()");
        return -1;
    }

    // send message packet
    retval = sprintf(buffer_send, "[%s is joined]", buffer + SIZE_HEADER);
    memset(buffer_send + retval, VALUE_SYSTEM_MESSAGE_PADDING, SIZE_SYSTEM_MESSAGE_PADDING);
    retval += SIZE_SYSTEM_MESSAGE_PADDING;
    if (sprideChattingMessage(&room, buffer + SIZE_HEADER, buffer_send, retval) < 0)
    {
        printLog("sprideChattingMessage()");
        return -1;
    }
    return 0;
}

int cmdLeaveRoom(socket_info *p_socket_info)
{
    char *buffer = p_socket_info->buffer;
    TYPE_ROOM_NUMBER room_number = *(TYPE_ROOM_NUMBER *)(buffer + SIZE_HEADER + SIZE_ID);

    char buffer_send[SIZE_BUFFER];
    int retval;

    room_info room;

    // send packet
    if(spridePacket(room_number, buffer, *(TYPE_PACKET_LENGTH *)(buffer + SIZE_CMD)) < 0)
    {
        printLog("spridePacket()");
        return -1;
    }

    if(dbSelectRoomOfRoomNumber(room_number, &room) < 0)
    {
        printLog("dbSelectRoomOfRoomNumber()");
        return -2;
    }
    // send message packet
    retval = sprintf(buffer_send, "[%s is leaved]", buffer + SIZE_HEADER);
    memset(buffer_send + retval, VALUE_SYSTEM_MESSAGE_PADDING, SIZE_SYSTEM_MESSAGE_PADDING);
    retval += SIZE_SYSTEM_MESSAGE_PADDING;
    if (sprideChattingMessage(&room, buffer + SIZE_HEADER, buffer_send, retval) < 0)
    {
        printLog("sprideChattingMessage()");
        return -1;
    }

    if (dbDeleteRoomUser(room_number, p_socket_info->id) < 0)
    {
        printLog("dbDeleteRoominfo()");
        return -2;
    }
    return 0;
}

int cmdViewRoom(socket_info *p_socket_info)
{
    char *buffer = p_socket_info->buffer;
    TYPE_ROOM_NUMBER room_number = *(TYPE_ROOM_NUMBER *)(buffer + SIZE_HEADER + SIZE_ID);

    char buffer_send[SIZE_BUFFER];

    int size_send_packet;

    room_info room;
    if(dbSelectRoomOfRoomNumber(room_number, &room) < 0)
    {
        printLog("dbSelectRoomOfRoomNumber()");
        return -2;
    }

    // make packet
    // cmd, id, room_number
    size_send_packet = SIZE_HEADER + SIZE_ID + SIZE_ROOM_NUMBER;
    memcpy(buffer_send, buffer, size_send_packet);

    // length
    memcpy(buffer_send + SIZE_CMD, &size_send_packet, SIZE_PACKET_LENGTH);

    // send packet
    if (sendPacket(p_socket_info, buffer_send, size_send_packet) < 0)
    {
        printLog("sendPacket()");
        return -1;
    }

    return 0;
}

int cmdTotalRoomList(socket_info *p_socket_info)
{
    char *buffer = p_socket_info->buffer;

    char buffer_send[SIZE_BUFFER];
    int size_send_packet = 0;
    int count_room = -1;

    TYPE_ROOM_NUMBER i;

    room_info arr_room_info[MAX_ROOM];

    if((count_room = dbSelectAllRoom(arr_room_info, MAX_ROOM)) < 0)
    {
        printLog("dbSelectAllRoom()");
        return -2;
    }

    // make packet
    // cmd, id
    memcpy(buffer_send, buffer, SIZE_HEADER + SIZE_ID);

    // roomlist
    for (i = 0; i < count_room; i++)
    {
        size_send_packet = SIZE_HEADER + SIZE_ID;
        if (arr_room_info[i].status != ROOM_INFO_STATUS_NORMAL)
            continue;
        // room number
        memcpy(buffer_send + size_send_packet, &arr_room_info[i].room_number, SIZE_ROOM_NUMBER);
        size_send_packet += SIZE_ROOM_NUMBER;

        // room status
        memcpy(buffer_send + size_send_packet, &(arr_room_info[i].status), SIZE_ROOM_STATUS);
        size_send_packet += SIZE_ROOM_STATUS;

        // room subject
        memcpy(buffer_send + size_send_packet, arr_room_info[i].subject, SIZE_ROOM_SUBJECT);
        size_send_packet += SIZE_ROOM_SUBJECT;

        // count member
        memcpy(buffer_send + size_send_packet, &arr_room_info[i].count_member, sizeof(arr_room_info[i].count_member));
        size_send_packet += sizeof(arr_room_info[i].count_member);

        // length
        memcpy(buffer_send + SIZE_CMD, &size_send_packet, SIZE_PACKET_LENGTH);

        // send packet
        if (sendPacket(p_socket_info, buffer_send, size_send_packet) < 0)
        {
            printLog("sendPacket()");
            return -1;
        }
    }

    return 0;
}

int cmdMyRoomList(socket_info *p_socket_info)
{
    char *buffer = p_socket_info->buffer;
    char *id = buffer + SIZE_HEADER;
    char buffer_send[SIZE_BUFFER];
    int size_send_packet = 0;

    TYPE_ROOM_NUMBER i;

    room_info arr_room_info[MAX_ROOM];
    int count_roominfo;
    char plain[SIZE_BUFFER];

    int retval = 0;

    if ((count_roominfo = dbSelectRoomOfUser(p_socket_info->id, arr_room_info, MAX_ROOM)) < 0)
    {
        printLog("dbSelectRoomOfUser()");
        return -2;
    }

    // make packet
    // cmd, id
    memcpy(buffer_send, buffer, SIZE_HEADER + SIZE_ID);

    // roomlist
    for (i = 0; i < count_roominfo; i++)
    {
        size_send_packet = SIZE_HEADER + SIZE_ID;
        // room number
        memcpy(buffer_send + size_send_packet, &arr_room_info[i].room_number, SIZE_ROOM_NUMBER);
        size_send_packet += SIZE_ROOM_NUMBER;

        // status
        memcpy(buffer_send + size_send_packet, &(arr_room_info[i].status), SIZE_ROOM_STATUS);
        size_send_packet += SIZE_ROOM_STATUS;

        // subject
        memcpy(buffer_send + size_send_packet, arr_room_info[i].subject, SIZE_ROOM_SUBJECT);
        size_send_packet += SIZE_ROOM_SUBJECT;

        // secret key
        memcpy(buffer_send + size_send_packet, arr_room_info[i].key, SIZE_SECRET_KEY);
        size_send_packet += SIZE_SECRET_KEY;

        // count member
        memcpy(buffer_send + size_send_packet, &arr_room_info[i].count_member, sizeof(arr_room_info[i].count_member));
        size_send_packet += sizeof(arr_room_info[i].count_member);

        // chatting log
        if ((retval = readChattingLog(&arr_room_info[i], id, buffer_send + size_send_packet, (size_t)(SIZE_BUFFER - size_send_packet))) < 0)
        {
            printLog("readChattingLog()");
            retval = 0;
        }
        if(arr_room_info[i].status == ROOM_INFO_STATUS_SECRET)
        {
            retval = decryptBuffer(arr_room_info[i].key, buffer_send + size_send_packet, retval, plain);
            retval = removeBeforeJoin(plain, retval, id);
            retval = encryptBuffer(arr_room_info[i].key, plain, retval, buffer_send + size_send_packet);
        }
        else
        {
            retval = removeBeforeJoin(buffer_send + size_send_packet, retval, id);
        }
        size_send_packet += retval;

        // length
        memcpy(buffer_send + SIZE_CMD, &size_send_packet, SIZE_PACKET_LENGTH);

        // send packet
        if (sendPacket(p_socket_info, buffer_send, size_send_packet) < 0)
        {
            printLog("sendPacket()");
            return -1;
        }

    }

    return 0;
}

int cmdChattingMessage(socket_info *p_socket_info)
{
    char *buffer = p_socket_info->buffer;
    TYPE_PACKET_LENGTH size_packet = *(TYPE_PACKET_LENGTH *)(buffer + SIZE_CMD);
    TYPE_ROOM_NUMBER room_number = *(TYPE_ROOM_NUMBER *)(buffer + SIZE_HEADER + SIZE_ID);
    char *id = buffer + SIZE_HEADER;
    char *message = buffer + (SIZE_HEADER + SIZE_ID + SIZE_ROOM_NUMBER);
    int size_message = size_packet - (SIZE_HEADER + SIZE_ID + SIZE_ROOM_NUMBER);

    char buffer_plain[SIZE_BUFFER];
    char *message_send = message;
    int size_message_send = size_message;

    room_info room;
    if(dbSelectRoomOfRoomNumber(room_number, &room) < 0)
    {
        printLog("dbSelectRoomOfRoomNumber()");
        return -2;
    }

    if (room.status == ROOM_INFO_STATUS_SECRET)
    {
        size_message_send = decryptBuffer(room.key, message, size_message, buffer_plain);
        message_send = buffer_plain;
    }

    // send message packet
    if (sprideChattingMessage(
        // ptr_room_info
        &room,
        // id
        id,
        // message
        message_send,
        // size_message
        size_message_send) < 0)
    {
        printLog("sprideChattingMessage()");
        return -1;
    }

    return 0;
}

int cmdInvite(socket_info *p_socket_info)
{
    char *buffer = p_socket_info->buffer;
    char *yourid = buffer + SIZE_HEADER + SIZE_ID;
    socket_info *p_socket_info_ret;

    char buffer_send[SIZE_BUFFER];
    TYPE_PACKET_LENGTH size_send_packet;

    if(findSocketInfoArr(yourid, &p_socket_info_ret) < 0)
    {
        printLog("findSocketInfoArr()");
        return -1;
    }

    size_send_packet = 0;
    //cmd, myid
    memcpy(buffer_send + size_send_packet, buffer, SIZE_HEADER + SIZE_ID);
    size_send_packet += SIZE_HEADER + SIZE_ID;

    //length
    memcpy(buffer_send + SIZE_CMD, &size_send_packet, SIZE_PACKET_LENGTH);

    // send packet
    if (sendPacket(p_socket_info, buffer_send, size_send_packet) < 0)
    {
        printLog("sendPacket()");
        return -1;
    }

    size_send_packet = 0;
    //cmd, yourid
    memcpy(buffer_send + size_send_packet, buffer, SIZE_HEADER + SIZE_ID);
    size_send_packet += SIZE_HEADER + SIZE_ID;

    //room_number
    memcpy(buffer_send + size_send_packet, buffer + SIZE_HEADER + SIZE_ID + SIZE_ID, SIZE_ROOM_NUMBER);
    size_send_packet += SIZE_ROOM_NUMBER;

    //length
    memcpy(buffer_send + SIZE_CMD, &size_send_packet, SIZE_PACKET_LENGTH);

    // send packet
    if (sendPacket(p_socket_info_ret, buffer_send, size_send_packet) < 0)
    {
        printLog("sendPacket()");
        return -1;
    }
    return 0;
}
/********************* end cmd Function ***************************/

// daemon function
int initDaemon()
{
    pid_t pid;

    umask(0);

    if (daemon(1, 1) == -1) {
        printLog("daemon()");
        return -1;
    }

    if ((pid = fork()) == -1) {
        printLog("fork()");
        return -1;
    }
    else if (pid > 0) {           /* parent */
        _exit(0);
    }
    /* child */

    stdin = freopen("/dev/null", "r", stdin);
    stdout = freopen("/dev/null", "w", stdout);
    stderr = freopen("/dev/null", "w", stderr);

    return 0;
}
/*************** END OF FILE **********************************************/





