#include <pthread.h>
#include <stdio.h>
#include <sys/timeb.h>
#include <sys/socket.h>
#include <sys/types.h>
#include <sys/wait.h>
#include <sys/epoll.h>
#include <netinet/in.h>
#include <string.h>
#include <fcntl.h>
#include <signal.h>
#include <errno.h>

#include <stdlib.h>
#include <unistd.h>

#define MAX_CLIENT  1024 
#define BUF_SIZE    4096
//#define DEBUG 

int PORT = 9200;

// nonblock 소켓생성 
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

int reuse(int sockfd)
{
    int enable = 1;
    if (setsockopt(sockfd, SOL_SOCKET, SO_REUSEADDR, &enable, sizeof(int)) < 0)
    {
        perror("setsockopt(SO_REUSEADDR) failed");
        return -1;
    }
    return 0;
}

int main(int argc, char **argv)
{
    int listenfd;
    int epfd;
    struct epoll_event *events;
    struct epoll_event ev;

    struct sockaddr_in srv;
    int clifd;
    int i;
    int read_cnt, write_cnt;
    int res;
    char buffer[BUF_SIZE];

    if( (listenfd = socket(AF_INET, SOCK_STREAM, 0)) < 0)
    {
        perror("sockfd\n");
        exit(1);
    }

    reuse(listenfd);
    events = malloc(sizeof(struct epoll_event) * MAX_CLIENT);

    bzero(&srv, sizeof(srv));
    srv.sin_family = AF_INET;
    srv.sin_addr.s_addr = INADDR_ANY;
    srv.sin_port = htons(PORT);

    if( bind(listenfd, (struct sockaddr *) &srv, sizeof(srv)) < 0)
    {
        perror("bind\n");
        exit(1);
    }

    listen(listenfd, SOMAXCONN);

    epfd = epoll_create(MAX_CLIENT);
    if(!epfd)
    {
        perror("epoll_create\n");
        exit(1);
    }
    ev.events = EPOLLIN | EPOLLHUP;
    ev.data.fd = listenfd;
    if(epoll_ctl(epfd, EPOLL_CTL_ADD, listenfd, &ev) < 0)
    {
        perror("epoll_ctl, adding listenfd\n");
        exit(1);
    }
    for( ; ; )
    {
        res = epoll_wait(epfd, events, MAX_CLIENT, -1);
        for(i = 0; i < res; i++)
        {
            if(events[i].data.fd == listenfd)
            {
                clifd = accept(listenfd, NULL, NULL);
                if(clifd > 0)
                {
                    //nonblock(clifd);
                    ev.events = EPOLLIN;
                    ev.data.fd = clifd;
                    if(epoll_ctl(epfd, EPOLL_CTL_ADD, clifd, &ev) < 0)
                    {
                        perror("epoll_ctl ADD\n");
                        exit(1);
                    }
#ifdef DEBUG 
                    printf("%d connected\n", ++connect_cnt);
#endif
                }
            }
            else {
                //memset(buffer, 0x00, BUF_SIZE);
                read_cnt = recv(events[i].data.fd, buffer, BUF_SIZE-1, 0);
                if(read_cnt == 0)
                {
                    close(events[i].data.fd);
                    epoll_ctl(epfd, EPOLL_CTL_DEL, events[i].data.fd, NULL);
#ifdef DEBUG 
                    printf("%d closed connection\n", events[i].data.fd);
#endif
                }
                else if(read_cnt < 0)
                {
#ifdef DEBUG 
                    printf("%d error occured, errno: %d\n",
                            events[i].data.fd, errno);
#endif
                }
                else {
                    write_cnt = send(events[i].data.fd, buffer, read_cnt, 0);
                    if(read_cnt != write_cnt)
                    {
                        printf("[recv %d : send %d] write error\n", read_cnt, write_cnt);
                        return -1;
                    }
                    //printf("[%d] read write\n", events[i].data.fd);
#ifdef DEBUG 
                    bzero(&buffer, strlen(buffer));
                    printf("%d data received: %s\n",
                            events[i].data.fd, buffer);
#endif
                }
            }
        }
    }

    return 0;
}
