/* 2017-04-04 : created by JHLim */
/***************************************************************************
* Copyright (c) 2004-2016, eGlobal Systems, Co., Ltd.
* Seoul, Republic of Korea
* All Rights Reserved.
*
* Description : processMain.c
*		xxxx
*
***************************************************************************/

/*************** Header files *********************************************/
#include <sys/socket.h>
#include <sys/stat.h>
#include <arpa/inet.h>
#include <sys/types.h>
#include <sys/wait.h>

#include <stdlib.h>
#include <stdio.h>
#include <unistd.h>
#include <fcntl.h>

#include "myProcess.h"
/*************** New Data Types *******************************************/

/*************** Definitions / Macros  ************************************/

/*************** Global Variables *****************************************/

int PORT = MAIN_PROCESS_PORT;
char MANAGER_IP[20] = "127.0.0.1";
int MANAGER_PORT = PROCESS_MANAGER_PORT;

/*************** Prototypes ***********************************************/
int mainListenProc();
int initServer(int *listenfd, int port);
void childHandler(int signum);

int readWriteProcFunc(int sockfd);
int initClient(int *sockfd, int port);
int transferMessage(int srcfd, int dstfd, char *buffer);
void nonblock(int sockfd);
int reuse(int sockfd);

/*************** Function *************************************************/
int main(int argc, char **argv)
{
	if(argc == 2)
		PORT = atoi(argv[1]);

	return mainListenProc();
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

void childHandler(int signum)
{
	pid_t childpid;
    int childstatus;

    while ((childpid = waitpid( -1, &childstatus, WNOHANG)) > 0)
    {
#ifdef DEBUG
        if (WIFEXITED(childstatus))
        {
            printLog("PID %d exited normally.  Exit number:  %d\n", childpid, WEXITSTATUS(childstatus));
        }
        else
        {
            if (WIFSTOPPED(childstatus))
            {
                printLog("PID %d was stopped by %d\n", childpid, WSTOPSIG(childstatus));
            }
            else
            {
                if (WIFSIGNALED(childstatus))
                {
                    printLog("PID %d exited due to signal %d\n.", childpid, WTERMSIG(childstatus));
                }
                else
                {
                    printLog("waitpid");
                }
            }
        }
#endif
        ;
    }
}
int mainListenProc()
{
	int listenfd, sockfd;
	socklen_t socklen;
	struct sockaddr_in sockaddr;
	int i;

	int pid;
	
	if(initServer(&listenfd, PORT) != 0)
		return -1;

	signal(SIGCHLD, childHandler);
	for(i = 0; ; i++)
	{
		socklen = sizeof(sockaddr);
		sockfd = accept(listenfd, (struct sockaddr *)&sockaddr, &socklen);
		printLog("processMain accept()\n");
		if(sockfd > 0)
		{
			// child
	        if ((pid = fork()) == 0) 
	        {
	        	close(listenfd);

	        	return readWriteProcFunc(sockfd);
	        }

	        // parent
	        else if(pid > 0)
	        {
	        	close(sockfd);
	        	continue;
	        }
	        else
	        {
	        	printLog("fork() error");
	        	return -1;
	        }
		}
		else if(sockfd == -1 && errno != EAGAIN)
		{
			printLog("accept() error");
			return -1;
		}
	}
	return 0;
}

int transferMessage(int srcfd, int dstfd, char *buffer)
{
	int retval, recv_cnt, send_cnt = 0;

	if((recv_cnt = read(srcfd, buffer, BUF_SIZE - 1)) > 1)
	{
		buffer[recv_cnt] = '\0';
//		printLog("[%d]recv %s", srcfd, buffer);
		while(send_cnt < recv_cnt)
		{
			retval = write(dstfd, buffer + send_cnt, recv_cnt - send_cnt);
			if(retval > 0)
				send_cnt += retval;
		}
//		printLog("send_cnt = %d, recv_cnt = %d", send_cnt, recv_cnt);
//		printLog("[%d]send %s", dstfd, buffer);
	}
	else if(recv_cnt == 0)
		return -1;
	else if(recv_cnt < 0)
	{
		if(errno != EAGAIN)
			return -1;
	}
	return 0;
}

int readWriteProcFunc(int sockfd)
{
	int proc_manager_fd;
	char buffer[BUF_SIZE];

	nonblock(sockfd);

	if(initClient(&proc_manager_fd, MANAGER_PORT) == -1)
		printLog("initClient error");
	nonblock(proc_manager_fd);

	printLog("readwriteProcFunc while() start");
	while(1)
	{
		if(transferMessage(proc_manager_fd, sockfd, buffer) != 0)
			break;
		if(transferMessage(sockfd, proc_manager_fd, buffer) != 0)
			break;
	}

	close(sockfd);
	close(proc_manager_fd);

	return 0;
}

int initClient(int *sockfd, int port)
{
	struct sockaddr_in server_address;
	int retval;

	memset(&server_address, 0, sizeof(server_address));
	server_address.sin_family = AF_INET;
	server_address.sin_addr.s_addr = inet_addr(MANAGER_IP);
	server_address.sin_port = htons(port);

	(*sockfd) = socket(AF_INET, SOCK_STREAM, 0);
	retval = connect((*sockfd), 
				(struct sockaddr*)&server_address,
				sizeof(server_address));
	if(retval == -1)
	{
		printLog("connect error\n");
		return -1;
	}

	return 0;
}
int initServer(int *listenfd, int port)
{
	struct sockaddr_in sockaddr;

	if(((*listenfd) = socket(AF_INET, SOCK_STREAM, 0)) < 0)
	{
		printLog("socket() error");
		return -1;
	}

	if(reuse((*listenfd)) != 0)
		return -1;

	memset((void *)&sockaddr,0x00,sizeof(sockaddr));
	sockaddr.sin_family = AF_INET;
	sockaddr.sin_addr.s_addr = htonl(INADDR_ANY);
	sockaddr.sin_port = htons(port);

	if( bind((*listenfd), (struct sockaddr *)&sockaddr, sizeof(sockaddr)) == -1)
	{
		printLog("bind() error");
		return -1;
	}

	if(listen((*listenfd), 5) == -1)
	{
		printLog("listen() error");
		return -1;
	}

	printLog("[Server] port = %d Start", PORT);
	return 0;
}


void nonblock(int sockfd)
{
    int opts;
    opts = fcntl(sockfd, F_GETFL);
    if(opts < 0)
    {
        printLog("fcntl(F_GETFL)\n");
        exit(1);
    }
    opts = (opts | O_NONBLOCK);
    if(fcntl(sockfd, F_SETFL, opts) < 0)
    {
        printLog("fcntl(F_SETFL)\n");
        exit(1);
    }
}


/*************** END OF FILE **********************************************/




