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

/*************** New Data Types *******************************************/

/*************** Definitions / Macros  ************************************/
#define BUF_SIZE		1024

/*************** Global Variables *****************************************/
char IP[20] = "127.0.0.1";
int PORT = 9000;
int g_cnt_recv = 0;

/*************** Prototypes ***********************************************/
void nonblock(int sockfd);

void printMessage(const char *Format, ...);

int initClient(int *sockfd);
int sendMessage(int srcfd, int dstfd, char *buffer);
int recvMessage(int srcfd, int dstfd, char *buffer);
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
	char buffer[BUF_SIZE];

	if(initClient(&sockfd) != 0)
		return -1;

	memset(buffer, 0, BUF_SIZE);

	while(1)
	{
		if(sendMessage(0, sockfd, buffer) != 0)
			break;
		if(recvMessage(sockfd, 1, buffer) != 0)
			break;
	}

	close(sockfd);

	return 0;
}

int initClient(int *sockfd)
{
	struct sockaddr_in server_address;
	int retval;

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

int sendMessage(int srcfd, int dstfd, char *buffer)
{
	int retval;
	unsigned short cmd = 80;

	if((retval = read(srcfd, buffer + 6, BUF_SIZE - g_cnt_recv - 1)) > 1)
	{
		memcpy(buffer, &cmd, 2);
		retval += 6;
		memcpy(buffer + 2, &retval, 4);
		buffer[retval] = 0;
		write(dstfd, buffer, retval);
		fflush(stdin);
	}
	
	else if(retval <= 0 && errno != EAGAIN)
	{
		return -1;
	}
	return 0;
}
int recvMessage(int srcfd, int dstfd, char *buffer)
{
	int retval;
	int size_packet;

	if((retval = read(srcfd, buffer + g_cnt_recv, BUF_SIZE - g_cnt_recv - 1)) > 1)
	{
		g_cnt_recv += retval;
		if(g_cnt_recv < 6)
			return 0;
		size_packet = *(unsigned int *)(buffer + 2);
		//memcpy(&size_packet, buffer + 2, 4);
		if(g_cnt_recv < size_packet)
			return 0;

		buffer[g_cnt_recv] = 0;

		memcpy(buffer, "cli : ", 6);
		write(dstfd, buffer + 6, g_cnt_recv - 6);
		g_cnt_recv = 0;
		fflush(stdin);
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













