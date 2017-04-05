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

#define BUF_SIZE 4096
#define MAX_CLIENT    1024
//#define PRINTLOG

int PORT = 9100;

void printLog(const char *Format, ...)
{
#ifdef PRINTLOG
    char buf[512] = {0,}; 
    va_list ap; 
    
    strcpy (buf, "[Server] : "); 
    
    va_start(ap, Format); 
    vsprintf(buf + strlen(buf), Format, ap); 
    va_end(ap); 
    
    puts(buf);
#endif
}

int createServerSocket()
{
    int listen_fd;
    struct sockaddr_in server_addr;

    if((listen_fd = socket(AF_INET, SOCK_STREAM, 0)) == -1)
    {
        perror("socket error");
        return -1;
    }   
    memset((void *)&server_addr, 0x00, sizeof(server_addr));
    server_addr.sin_family = AF_INET;
    server_addr.sin_addr.s_addr = htonl(INADDR_ANY);
    server_addr.sin_port = htons(PORT);
    
    if(bind(listen_fd, (struct sockaddr *)&server_addr, sizeof(server_addr)) == -1)
    {
        perror("bind error");
        return -1;
    }   
    if(listen(listen_fd, SOMAXCONN) == -1)
    {
        perror("listen error");
        return -1;
    }
    return listen_fd;
}
int main(int argc, char **argv)
{
    int listen_fd, client_fd;
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

    listen_fd = createServerSocket();
    
    client[0].fd = listen_fd;
    client[0].events = POLLIN;
    for (i = 1; i < MAX_CLIENT; i++)
    {
        client[i].fd = -1;
    }

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
            client_fd = accept(listen_fd,
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

            printLog("Accept OK [%d -> %d / connect cnt = %d]\n", i, client_fd, ++cnt_connect);
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
                    printLog("close [%d -> %d / connect cnt = %d]\n", i, sockfd, --cnt_connect);
                    close(sockfd);
                    client[i].fd = -1;
                }
                else
                {
                    buf[readn] = '\0';
                    if(readn != write(sockfd, buf, strlen(buf)))
                    {
                        printf("write error\n");
                        return -1;
                    }
                    printf("[%d] read write\n", sockfd);
                }
            }
        }
    }
}
