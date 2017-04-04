#include <sys/socket.h>
#include <sys/stat.h>
#include <arpa/inet.h>
#include <sys/types.h>
#include <sys/wait.h>

#include <stdlib.h>
#include <stdio.h>
#include <unistd.h>
#include <string.h>
#include <errno.h>
#include <fcntl.h>

#include "myProcess.h"

char IP[20] = "127.0.0.1";
int PORT = MAIN_PROCESS_PORT;
int MANAGER_PORT = PROCESS_MANAGER_PORT;

int mainListenProc();
int initServer(int *listenfd, int port);
void childHandler(int signum);

int readWriteProcFunc(int sockfd);
int initClient(int *sockfd, int port);
int transferMessage(int srcfd, int dstfd, char *buffer);
void nonblock(int sockfd);

int main(int argc, char **argv)
{
	if(argc == 2)
		PORT = atoi(argv[1]);

	return mainListenProc();
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
            printf("PID %d exited normally.  Exit number:  %d\n", childpid, WEXITSTATUS(childstatus));
        }
        else
        {
            if (WIFSTOPPED(childstatus))
            {
                printf("PID %d was stopped by %d\n", childpid, WSTOPSIG(childstatus));
            }
            else
            {
                if (WIFSIGNALED(childstatus))
                {
                    printf("PID %d exited due to signal %d\n.", childpid, WTERMSIG(childstatus));
                }
                else
                {
                    perror("waitpid");
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

	int pid;
	
	if(initServer(&listenfd, PORT) != 0)
		return -1;

	signal(SIGCHLD, childHandler);
	while(1)
	{
		socklen = sizeof(sockaddr);
		sockfd = accept(listenfd, (struct sockaddr *)&sockaddr, &socklen);
		if(sockfd > 0)
		{
			// child
	        if ((pid = fork()) == 0) 
	        {
	        	close(listenfd);

	        	readWriteProcFunc(sockfd);
	        }

	        // parent
	        else if(pid > 0)
	        {
	        	close(sockfd);
	        	continue;
	        }
	        else
	        {
	        	perror("fork() error");
	        	return -1;
	        }
		}
		else if(sockfd == -1 && errno != EAGAIN)
		{
			perror("accept() error");
			return -1;
		}
	}
	return 0;
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

int readWriteProcFunc(int sockfd)
{
	int proc_manager_fd;
	char buffer[BUF_SIZE];

	nonblock(sockfd);

	if(initClient(&proc_manager_fd, MANAGER_PORT) == -1)
		perror("initClient error");
	nonblock(proc_manager_fd);

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
	server_address.sin_addr.s_addr = inet_addr(IP);
	server_address.sin_port = htons(port);

	(*sockfd) = socket(AF_INET, SOCK_STREAM, 0);
	retval = connect((*sockfd), 
				(struct sockaddr*)&server_address,
				sizeof(server_address));
	if(retval == -1)
	{
		perror("connect error\n");
		return -1;
	}

	return 0;
}
int initServer(int *listenfd, int port)
{
	struct sockaddr_in sockaddr;

	if(((*listenfd) = socket(AF_INET, SOCK_STREAM, 0)) < 0)
	{
		perror("socket() error");
		return -1;
	}

	memset((void *)&sockaddr,0x00,sizeof(sockaddr));
	sockaddr.sin_family = AF_INET;
	sockaddr.sin_addr.s_addr = htonl(INADDR_ANY);
	sockaddr.sin_port = htons(port);

	if( bind((*listenfd), (struct sockaddr *)&sockaddr, sizeof(sockaddr)) == -1)
	{
		perror("bind() error");
		return -1;
	}

	if(listen((*listenfd), 5) == -1)
	{
		perror("listen() error");
		return -1;
	}

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
