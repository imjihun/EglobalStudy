#include <sys/socket.h>
#include <sys/stat.h>
#include <arpa/inet.h>
#include <sys/types.h>

#include <stdlib.h>
#include <stdio.h>
#include <unistd.h>
#include <string.h>
#include <pthread.h>

#define BUF_SIZE	4096
#define SOCKET_MAX	1024

int PORT = 9400;

void *myFunc(void *arg);
int initServer();
int threadServer();

int main(int argc, char **argv)
{
	if(argc == 2)
		PORT = atoi(argv[1]);

	threadServer();

	return 0;
}

int initServer(int *listenfd)
{
	struct sockaddr_in sockaddr;

	if(((*listenfd) = socket(AF_INET, SOCK_STREAM, 0)) < 0)
	{
		perror("socket error");
		return -1;
	}

	memset((void *)&sockaddr,0x00,sizeof(sockaddr));
	sockaddr.sin_family = AF_INET;
	sockaddr.sin_addr.s_addr = htonl(INADDR_ANY);
	sockaddr.sin_port = htons(PORT);

	if( bind((*listenfd), (struct sockaddr *)&sockaddr, sizeof(sockaddr)) == -1)
	{
		perror("bind error");
		return -1;
	}

	if(listen((*listenfd), SOMAXCONN) == -1)
	{
		perror("listen error");
		return -1;
	}
	return 0;
}

int threadServer()
{
	struct sockaddr_in sockaddr;
	int listenfd, sockfd;
	socklen_t socklen;

	pthread_t thread_t;
	int th_id;
	
	if(initServer(&listenfd) != 0)
		return -1;

	while(1)
	{
		socklen = sizeof(sockaddr);
		sockfd = accept(listenfd, (struct sockaddr *)&sockaddr, &socklen);
		//printf("accept = %d\n", sockfd);
		if(sockfd == -1)
		{
			perror("accept error");
			return -1;
		}

		th_id = pthread_create(&thread_t, NULL, myFunc, (void *)&sockfd);
		if(th_id != 0)
		{
			perror("Thread Create Error");
			return -1;
		}
		pthread_detach(thread_t);
	}
}

void *myFunc(void *arg)
{
	int sockfd;
	int readn, writen;
	char buf[BUF_SIZE];
	sockfd = *((int *)arg);

	while(1)
	{
		readn = read(sockfd, buf, BUF_SIZE);
		if(readn <= 0)
		{
			//printf("close = %d\n", sockfd);
			break;
		}
		writen = write(sockfd, buf, readn);
		if(readn != writen)
		{
			printf("write error %d : %d\n", readn, writen);
			break;
		}
		printf("[%d] read write\n", sockfd);
	}
	close(sockfd);
	return NULL;
}
