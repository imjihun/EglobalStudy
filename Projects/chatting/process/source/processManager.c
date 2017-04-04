#include <sys/time.h>
#include <sys/types.h>
#include <unistd.h>

#include <sys/socket.h>
#include <sys/types.h>
#include <sys/stat.h> 
#include <stdlib.h> 
#include <sys/poll.h> 

#include <netinet/in.h>
#include <arpa/inet.h>

#include <stdio.h>
#include <stdarg.h>
#include <string.h>

#include "myProcess.h"

int PORT = PROCESS_MANAGER_PORT;
char MAIN_PROC_PATH[256] = "./mainProc";

int writeAll(struct pollfd *client, char *buf);
int startMainProc();
int initServer(int *listenfd, int port);
void printLog(const char *Format, ...);

int main(int argc, char **argv)
{
    int listenfd, client_fd;
    socklen_t addrlen;
    struct sockaddr_in client_addr;

    int nread;
    int maxi;
    int sockfd;
    int readn;
    int i= 0;
    char buf[BUF_SIZE];
    struct pollfd client[MAX_CLIENT];

    int cnt_connect = 0;

    if(initServer(&listenfd, PORT) == -1)
        return -1;
    
    client[0].fd = listenfd;
    client[0].events = POLLIN;
    for (i = 1; i < MAX_CLIENT; i++)
    {
        client[i].fd = -1;
    }

    if(startMainProc() == -1)
        return -1;

    maxi = 0;
    while(1)
    {
        nread = poll(client, maxi + 1, -1);

        if(nread != 1)
            printLog("nread = %d\n", nread);

        if(nread == 0)
            continue;
        else if(nread < 0)
        {
            perror("poll error");
            return -1;
        }

        if (client[0].revents & POLLIN)
        {
            addrlen = sizeof(client_addr);
            client_fd = accept(listenfd,
                    (struct sockaddr *)&client_addr, &addrlen);
            for (i = 1; i < MAX_CLIENT; i++)
            {
                if (client[i].fd < 0)
                {
                    client[i].fd = client_fd;
                    break;
                }
            }

            if (i == MAX_CLIENT)
            {
                perror("too many clients : ");
                exit(0);
            }

            client[i].events = POLLIN;
            if (i > maxi)
            {
                maxi = i;
            }

            printLog("Accept OK [%d / connect cnt = %d]\n", client_fd, ++cnt_connect);
            if (--nread <= 0)
                continue;
        }

        for (i = 1; i <= maxi; i++)
        {
            if ((sockfd = client[i].fd) < 0)
                continue;
            if (client[i].revents & (POLLIN | POLLERR))
            {
                if( (readn = read(sockfd, buf, BUF_SIZE-1)) <= 0)
                {
                    printLog("close [%d / connect cnt = %d]\n", sockfd, --cnt_connect);
                    close(sockfd);
                    client[i].fd = -1;
                }
                else
                {
                    buf[readn] = '\0';
                    writeAll(client, buf);
                }
            }
        }
    }
}

void printLog(const char *Format, ...)
{
#ifdef DEBUG
    char buf[512] = {0,}; 
    va_list ap; 
    
    strcpy (buf, "[processManager] : "); 
    
    va_start(ap, Format); 
    vsprintf(buf + strlen(buf), Format, ap); 
    va_end(ap); 
    
    puts(buf);
#endif
}

int initServer(int *listenfd, int port)
{
    struct sockaddr_in server_addr;

    if(((*listenfd) = socket(AF_INET, SOCK_STREAM, 0)) == -1)
    {
        perror("socket error");
        return -1;
    }   
    memset((void *)&server_addr, 0x00, sizeof(server_addr));
    server_addr.sin_family = AF_INET;
    server_addr.sin_addr.s_addr = htonl(INADDR_ANY);
    server_addr.sin_port = htons(port);
    
    if(bind((*listenfd), (struct sockaddr *)&server_addr, sizeof(server_addr)) == -1)
    {
        perror("bind error");
        return -1;
    }   
    if(listen((*listenfd), 5) == -1)
    {
        perror("listen error");
        return -1;
    }
    return 0;
}
int startMainProc()
{
    int pid;

    // child
    if ((pid = fork()) == 0) 
    {
        execl(MAIN_PROC_PATH, "");
    }
    // parent
    else if(pid > 0)
    {
        return 0;
    }
    else
    {
        perror("fork() error");
        return -1;
    }
    return 0;
}


int writeAll(struct pollfd *client, char *buf)
{
    int i;
    for(i = 0; i < MAX_CLIENT; i++)
    {
        if(client[i].fd != -1)
            write(client[i].fd, buf, strlen(buf));
    }
    return 0;
}
