#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <fcntl.h>
#include <signal.h>
#include <unistd.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#define BUF_SIZE 4096
 
 int PORT = 9300;

 void error_handler(const char *msg)
{
    fputs(msg, stderr);
    fputc('\n', stderr);
    exit(1);
}
 
int main(int argc, char **argv)
{
    int server_sock, client_sock;
    int message[BUF_SIZE];
    int message_len;
 
    struct sockaddr_in server_addr, client_addr;
    socklen_t client_addr_len;
 
    int ret, pid;
 
    if (argc == 2)
        PORT = atoi(argv[1]);
 
    /* Create Socket */
    server_sock = socket(PF_INET, SOCK_STREAM, 0);
    if (server_sock == -1)
        error_handler("socket() error");
 
    /* Address Setting */
    memset(&server_addr, 0, sizeof(server_addr));
    server_addr.sin_family      = AF_INET;
    server_addr.sin_addr.s_addr = htonl(INADDR_ANY);
    server_addr.sin_port        = htons(PORT);
 
    /* Bind Socket */
    ret = bind(server_sock,
               (struct sockaddr *)&server_addr,
               sizeof(server_addr));
    if (ret == -1)
        error_handler("bind() error");
 
    /* Listen (Wait Clients) */
    if (listen(server_sock, SOMAXCONN) == -1)
        error_handler("listen() error");
 
    /* Ignore the signal SIGCHLD        */
    /* so that NO zombie process occurs */
    signal(SIGCHLD, SIG_IGN);
 
    /* Loop Forever */
    while (1)
    {
        /* Connect with a client */
        client_addr_len = sizeof(client_addr_len);
        client_sock = accept(server_sock,
                             (struct sockaddr *)&client_addr,
                             &client_addr_len);
        if (client_sock == -1)
            error_handler("accept() error");
 
        /* Create Process */
        pid = fork();
 
        if (pid < 0)                        /* Error  */
        {
            error_handler("fork() error");
        }
 
        else if (pid > 0)                   /* Parent */
        {
            close(client_sock);
        }
 
        else                                /* Child  */
        {
            close(server_sock);
 
            /* Receive & Send */
            while ((message_len = read(client_sock, message, BUF_SIZE)) > 0)
            {
                if(message_len != write(client_sock, message, message_len))
                {
                    printf("write error\n");
                    return -1;
                }
                printf("[%d] read write\n", client_sock);
            }
 
            close(client_sock);
            return 0;
        }
    }
 
    close(server_sock);
 
    return 0;
}
