#include <sys/time.h>
#include <sys/types.h>
#include <unistd.h>

#include <sys/socket.h>
#include <sys/types.h>

#include <netinet/in.h>
#include <arpa/inet.h>

#include <stdio.h>
#include <stdarg.h>
#include <string.h>

#define BUF_SIZE 4096 
#define SOCK_SETSIZE 1021
//#define PRINTLOG

int PORT = 9000;

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
    if(listen(listen_fd, 5) == -1)
    {
        perror("listen error");
        return -1;
    }
    return listen_fd;
}
int main(int argc, char **argv)
{
    int listen_fd, client_fd;
    int client_optvalue=1;
    socklen_t addrlen;
    struct sockaddr_in client_addr;
    int fd_num;
    int maxfd = 0;
    int sockfd;
    int readn;
    int i= 0;
    char buf[BUF_SIZE];
    fd_set readfds, allfds;
    struct timeval timeout;

    int cnt_connect = 0;


    if((listen_fd = createServerSocket()) < 0)
        return -1;
    
    FD_ZERO(&readfds);
    FD_SET(listen_fd, &readfds);

    maxfd = listen_fd;
    timeout.tv_sec = 100;
    timeout.tv_usec = 0;
    while(1)
    {
        allfds = readfds;
        fd_num = select(1024 , &allfds, (fd_set *)0,
                      (fd_set *)0, NULL);

        if(fd_num == 0)
            continue;
        else if(fd_num < 0)
        {
            perror("select error");
            return -1;
        }

        if (FD_ISSET(listen_fd, &allfds))
        {
            addrlen = sizeof(client_addr);
            client_fd = accept(listen_fd,
                    (struct sockaddr *)&client_addr, &addrlen);
            if(client_fd <= 0)
            {
                perror("accept error");
                return -1;
            }
            FD_SET(client_fd,&readfds);
            setsockopt(client_fd,SOL_SOCKET,SO_REUSEADDR,&client_optvalue,sizeof(client_optvalue));
            if (client_fd > maxfd)
                maxfd = client_fd;
            printLog("Accept OK [%d / %d]", client_fd, ++cnt_connect);
            continue;
        }

        for (i = 0; i <= maxfd; i++)
        {
            sockfd = i;
            if (FD_ISSET(sockfd, &allfds))
            {
                if( (readn = read(sockfd, buf, BUF_SIZE-1)) == 0)
                {
                    close(sockfd);
                    FD_CLR(sockfd, &readfds);
                    printLog("close [%d / %d]", sockfd, --cnt_connect);
                }
                else
                {
                    buf[readn] = '\0';
                    write(sockfd, buf, strlen(buf));
                }
                if (--fd_num <= 0)
                    break;
            }
        }
    }
}
