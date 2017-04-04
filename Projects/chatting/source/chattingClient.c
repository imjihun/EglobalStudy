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
int PORT = 8000;

/*************** Prototypes ***********************************************/
void nonblock(int sockfd);

void printMessage(const char *Format, ...);

int initClient(int *sockfd);
int transferMessage(int srcfd, int dstfd, char *buffer);
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
		if(transferMessage(0, sockfd, buffer) != 0)
			break;
		if(transferMessage(sockfd, 1, buffer) != 0)
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

int transferMessage(int srcfd, int dstfd, char *buffer)
{
	int retval;

	if((retval = read(srcfd, buffer, BUF_SIZE - 1)) > 1)
	{
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













