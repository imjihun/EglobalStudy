#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>
#include <arpa/inet.h>
#include <sys/socket.h>
#include <stdarg.h>
#include <sys/time.h>


#define BUF_SIZE		4096

char IP[20] = "127.0.0.1";
int PORT = 9000;

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
int main(int argc, char** argv)
{
	int clientSocket;
	struct sockaddr_in 	clientAddress;
	char buffer[BUF_SIZE];
	int	retval = 0, send_size;

	if(argc == 3)
	{
		strcpy(IP, argv[1]);
		PORT = atoi(argv[2]);
	}


	memset(&clientAddress, 0, sizeof(clientAddress));
	clientAddress.sin_family = AF_INET;
	clientAddress.sin_addr.s_addr = inet_addr(IP);
	clientAddress.sin_port = htons(PORT);

	clientSocket = socket(AF_INET, SOCK_STREAM, 0);
	retval = connect(clientSocket, 
				(struct sockaddr*)&clientAddress,
				sizeof(clientAddress));
	if(retval == -1)
	{
		printMessage("connect error\n");
		return -1;
	}


	for(;;)
	{
		fflush(stdin);
		fgets(buffer, BUF_SIZE, stdin);
		send_size = strlen(buffer) - 1;
		if(send_size > 0)
		{
			send(clientSocket, buffer, send_size, 0);
			retval = recv(clientSocket, buffer, sizeof(buffer), 0);
			buffer[retval] = 0;
			printMessage("recv[%d] => %s\n", retval, buffer);
		}
	}

	close(clientSocket);

	return 0;
}












