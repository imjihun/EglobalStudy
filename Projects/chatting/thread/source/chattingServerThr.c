#include <sys/socket.h>
#include <sys/stat.h>
#include <arpa/inet.h>
#include <sys/types.h>
#include <sys/shm.h>

#include <stdlib.h>
#include <stdio.h>
#include <unistd.h>
#include <string.h>
#include <pthread.h>

#define BUF_SIZE	1024
#define SOCKET_MAX	1024

int PORT = 8100;
int g_arr_sockfd[SOCKET_MAX];

void *recvSendFunc(void *arg);
int initServer();
int main_threadServer();

int initArr(int *arr_sockfd);
int insertSockfd(int sockfd, int *arr_sockfd);
int deleteSockfd(int sockfd, int *arr_sockfd);
int transferMessage(const char *msg, int len, int *arr_sockfd);

int main(int argc, char **argv)
{
	if(argc == 2)
		PORT = atoi(argv[1]);

	return main_threadServer();
}

int main_threadServer()
{
	int listenfd, sockfd;
	socklen_t socklen;
	struct sockaddr_in sockaddr;

	pthread_t thread_t;
	int th_id;
	
	if(initServer(&listenfd) != 0)
		return -1;

	while(1)
	{
		socklen = sizeof(sockaddr);
		sockfd = accept(listenfd, (struct sockaddr *)&sockaddr, &socklen);
		if(sockfd == -1)
		{
			perror("accept() error");
			return -1;
		}

		if(insertSockfd(sockfd, g_arr_sockfd) != 0)
			return -1;

		th_id = pthread_create(&thread_t, NULL, recvSendFunc, (void *)&sockfd);
		if(th_id != 0)
		{
			perror("pthread_create() Error");
			return -1;
		}
		if(pthread_detach(thread_t) != 0)
		{
			perror("pthread_detach() Error");
			return -1;
		}
	}
	return 0;
}

void *recvSendFunc(void *arg)
{
	int sockfd;
	int readn;
	char buf[BUF_SIZE];
	sockfd = *((int *)arg);

	while(1)
	{
		readn = read(sockfd, buf, BUF_SIZE);
		if(readn <= 0)
		{
			printf("close = %d\n", sockfd);
			break;
		}
		if(transferMessage(buf, readn, g_arr_sockfd) != 0)
			break;
	}
	deleteSockfd(sockfd, g_arr_sockfd);
	close(sockfd);
	return NULL;
}

int initArr(int *arr_sockfd)
{
	int i;
	for(i = 0; i < SOCKET_MAX; i++)
		arr_sockfd[i] = -1;

	return 0;
}
int insertSockfd(int sockfd, int *arr_sockfd)
{
	int i;
	for(i = 0; i < SOCKET_MAX; i++)
	{
		if(arr_sockfd[i] == -1)
			break;
	}
	if(i == SOCKET_MAX)
	{
		printf("socket full\n");
		return -1;
	}

	arr_sockfd[i] = sockfd;

	return 0;
}
int deleteSockfd(int sockfd, int *arr_sockfd)
{
	int i;
	for(i = 0; i < SOCKET_MAX; i++)
	{
		if(arr_sockfd[i] == sockfd)
		{
			arr_sockfd[i] = -1;
			return 0;
		}
	}
	printf("not found\n");
	return -1;
}
int transferMessage(const char *msg, int len, int *arr_sockfd)
{
	int i;
	int retval, prev_retval;
	for(i = 0; i < SOCKET_MAX; i++)
	{
		if(arr_sockfd[i] != -1)
		{
			prev_retval = retval = 0;

			while((retval += write(arr_sockfd[i], msg, len)) < len)
			{
				if(prev_retval == retval)
				{
					printf("write() error\n");
					return -1;
				}
				prev_retval = retval;
			}
		}
	}
	return 0;
}

int initServer(int *listenfd)
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
	sockaddr.sin_port = htons(PORT);

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

	initArr(g_arr_sockfd);
	return 0;
}



///////////////////////////////////////////////////////////////////////////////
