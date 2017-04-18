/* 2017-04-04 : created by JHLim */
/***************************************************************************
* Copyright (c) 2004-2016, eGlobal Systems, Co., Ltd.
* Seoul, Republic of Korea
* All Rights Reserved.
*
* Description : chttingClient.c
*		xxxx
*
***************************************************************************/

/*************** Header files *********************************************/
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>
#include <arpa/inet.h>
#include <sys/socket.h>
#include <stdarg.h>
#include <sys/time.h>
#include <fcntl.h>
#include <asm-generic/errno-base.h>
#include <errno.h>

#include "../../macroFile.h"
#include "../../aes/source/aes.h"

/*************** New Data Types *******************************************/

/*************** Definitions / Macros  ************************************/
#define BUF_SIZE		1024

/*************** Global Variables *****************************************/
char IP[20] = "127.0.0.1";
int PORT = 9000;
char buffer[BUF_SIZE];
int g_cnt_recv = 0;

/*************** Prototypes ***********************************************/
void nonblock(int sockfd);

void printMessage(const char *Format, ...);

int initClient(int *sockfd);
int sendMessage(int srcfd, int dstfd);
int recvMessage(int srcfd, int dstfd);
int main_chattingClient();

/*************** Function *************************************************/
int main(int argc, char** argv)
{
	if(argc == 3)
	{
		strcpy(IP, argv[1]);
		PORT = atoi(argv[2]);
	}

	return main_chattingClient();
}

int main_chattingClient()
{
	int sockfd;

	if(initClient(&sockfd) != 0)
		return -1;

	while(1)
	{
		if(sendMessage(0, sockfd) != 0)
			break;
		if(recvMessage(sockfd, 1) != 0)
			break;
	}

	close(sockfd);

	return 0;
}

int initClient(int *sockfd)
{
	struct sockaddr_in server_address;
	int retval;
//	char *buf[255];
//	TYPE_CMD cmd;
//	TYPE_PACKET_LENGTH size;

	memset(&server_address, 0, sizeof(server_address));
	server_address.sin_family = AF_INET;
	server_address.sin_addr.s_addr = inet_addr(IP);
	server_address.sin_port = htons(PORT);

	(*sockfd) = socket(AF_INET, SOCK_STREAM, 0);
	retval = connect((*sockfd), 
				(struct sockaddr*)&server_address,
				sizeof(server_address));
	if(retval == -1)
	{
		printMessage("connect error\n");
		return -1;
	}
/*
	cmd = CMD_CREATE_ID;
	memcpy(buf, &cmd, SIZE_CMD);

	size = SIZE_HEADER
	printf("input your id [max length = 15] > ");
	do
	{
		gets(buf + size);
	}
	while(strlen(buf) > 15);

	size += strlen(buf);
	memcpy(buf + SIZE_CMD, &size, SIZE_PACKET_LENGTH);

	send((*sockfd), buf, size, 0);
*/
	nonblock(0);
	nonblock((*sockfd));

	return 0;
}

void nonblock(int sockfd)
{
    int opts;
    opts = fcntl(sockfd, F_GETFL);
    if(opts < 0)
    {
        perror("fcntl(F_GETFL)\n");
        exit(1);
    }
    opts = (opts | O_NONBLOCK);
    if(fcntl(sockfd, F_SETFL, opts) < 0)
    {
        perror("fcntl(F_SETFL)\n");
        exit(1);
    }
}
int setChar(char *src, int elem_size, char *dst)
{
	int idx_src = 0;
	int idx_dst = 0;
	int cnt = 0;

	for(idx_src = 0; idx_src < elem_size && src[idx_src] != ' ';)
	{
		dst[idx_dst++] = src[idx_src++];
		cnt++;
	}
	if(cnt < elem_size)
	{
		memset(dst + idx_dst, 0, elem_size - cnt);
	}
	return idx_src + 1;
}
int setInt(char *src, char *dst)
{
	int i;
	int tmp;
	for(i = 0; src[i] != ' '; i++)
		;
	src[i] = '\0';

	tmp = atoi(src);

	memcpy(dst, &tmp, 4);

	return i + 1;
}

int makePacket(char *buffer, int cnt_read, char *packet)
{
	TYPE_CMD cmd;
	int idx_buffer, idx_packet;
	char chiper_text[SIZE_BUFFER];
	int size_chiper_text;

	uint8_t key[16] = { 0x2b, 0x7e, 0x15, 0x16, 0x28, 0xae, 0xd2, 0xa6, 0xab, 0xf7, 0x15, 0x88, 0x09, 0xcf, 0x4f, 0x3c };
	uint8_t nonce[8]  = { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07 };

	buffer[SIZE_CMD] = '\0';
	buffer[cnt_read] = ' ';

	cmd = atoi(buffer);
	memcpy(packet, &cmd, SIZE_CMD);

//		length = SIZE_PACKET_LENGTH + cnt_read - 1;
//		memcpy(packet + SIZE_CMD, &length, SIZE_PACKET_LENGTH);

	idx_buffer = SIZE_CMD + 1;
	idx_packet = SIZE_HEADER;

	// ID
	idx_buffer += setChar(buffer + idx_buffer, SIZE_ID, packet + idx_packet);
	idx_packet += SIZE_ID;

	switch(cmd)
	{
	case CMD_CREATE_ROOM:
		// IS_SECRET
		idx_buffer += setChar(buffer + idx_buffer, SIZE_ROOM_STATUS, packet + idx_packet);
		idx_packet += SIZE_ROOM_STATUS;
		// ROOM_SUBJECT
		idx_buffer += setChar(buffer + idx_buffer, SIZE_ROOM_SUBJECT, packet + idx_packet);
		idx_packet += SIZE_ROOM_SUBJECT;
		break;
	case CMD_ENTER_ROOM:
	case CMD_LEAVE_ROOM:
	case CMD_VIEW_ROOM:
		// ROOM_NUMBER
		idx_buffer += setInt(buffer + idx_buffer, packet + idx_packet);
		idx_packet += SIZE_ROOM_NUMBER;
		break;
	case CMD_CHATTING_MESSAGE:
		// ROOM_NUMBER
		idx_buffer += setInt(buffer + idx_buffer, packet + idx_packet);
		idx_packet += SIZE_ROOM_NUMBER;
		// MESSAGE
		size_chiper_text = cnt_read - idx_buffer;
		aesCtrEncryptBuffer((TYPE_SECRET_KEY *)chiper_text, (TYPE_SECRET_KEY *)(buffer + idx_buffer), size_chiper_text, key, nonce);
		setChar(chiper_text, size_chiper_text, packet + idx_packet);
		idx_packet += size_chiper_text;
		break;
	}


	memcpy(packet + SIZE_CMD, &idx_packet, SIZE_PACKET_LENGTH);

	return 0;
}

int sendMessage(int srcfd, int dstfd)
{
	int retval;
	TYPE_PACKET_LENGTH length;
	char buffer[BUF_SIZE];

	char buffer_send[SIZE_BUFFER];
	int cnt_send = 0;

	int i, j;

	if((retval = read(srcfd, buffer, BUF_SIZE)) > 1)
	{
		retval--;
		makePacket(buffer, retval, buffer_send);
		//printMessage("sock(%d) read = %d", srcfd, retval);

		length = *(TYPE_PACKET_LENGTH *)(buffer_send + SIZE_CMD);
		while(cnt_send < length)
		{
			retval = write(dstfd, buffer_send, length);
			if(retval < 0)
				return -1;
			cnt_send += retval;
		}

		sprintf(buffer, "[client] [send] cmd = %d, length = %d, data = ", *(TYPE_CMD *)buffer_send, length);
		for(i = SIZE_HEADER, j = strlen(buffer); i < length; i++, j+=retval)
		{
			retval = sprintf(buffer + j, "%2x ", buffer_send[i]);
		}
		sprintf(buffer + j, "\n");
		retval = write(1, buffer, strlen(buffer));

		fflush(stdin);
	}
	
	else if(retval <= 0 && errno != EAGAIN)
	{
		return -1;
	}

	return 0;
}
int recvMessage(int srcfd, int dstfd)
{
	int retval;
	int size_packet;

	char buffer_print[SIZE_BUFFER];
	int i, j;

	if((retval = read(srcfd, buffer + g_cnt_recv, BUF_SIZE - g_cnt_recv - 1)) > 1)
	{
		//printMessage("sock(%d) recv len = %d", srcfd, retval);
		//system("clear");
		g_cnt_recv += retval;
		while(1)
		{
			if(g_cnt_recv < 6)
				return 0;
			size_packet = *(TYPE_PACKET_LENGTH *)(buffer + 2);
			//memcpy(&size_packet, buffer + 2, 4);
			if(g_cnt_recv < size_packet)
				return 0;

			sprintf(buffer_print, "[client] [recv] cmd = %d, length = %d, data = ", *(TYPE_CMD *)buffer, size_packet);
			for(i = SIZE_HEADER, j = strlen(buffer_print); i < size_packet; i++, j+=retval)
			{
				retval = sprintf(buffer_print + j, "%2x ", buffer[i]);
			}
			sprintf(buffer_print + j, "\n");

			retval = write(dstfd, buffer_print, strlen(buffer_print));
			g_cnt_recv -= size_packet;
			//printMessage("sock(%d) write = %d", dstfd, retval);

			memcpy(buffer, buffer + size_packet, g_cnt_recv);
		}
		//printf("1) create room\n2) enter room\n3) view room\n4) send message\n > ");
	}
	
	else if(retval <= 0 && errno != EAGAIN)
	{
		return -1;
	}
	return 0;
}



void printMessage(const char *Format, ...)
{
	char buf[512] = {0,}; 
	va_list ap; 
	
	strcpy (buf, "[Client] : "); 
	
	va_start(ap, Format); 
	vsprintf(buf + strlen(buf), Format, ap); 
	va_end(ap); 
	
	puts(buf);
}


/*************** END OF FILE **********************************************/













