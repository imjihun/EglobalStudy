#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>
#include <arpa/inet.h>
#include <sys/socket.h>
#include <stdarg.h>
#include <sys/time.h>
#include <time.h>
#include <errno.h>


#define BUF_SIZE		4096
#define SEND_CNT		1000
#define SEND_SIZE 		4095
#define SOCKET_MAX		1024
#define TEST_CNT		10

//#define DEBUG
#define LOG


char IP[20] = "127.0.0.1";
int PORT = 9000;

int g_arr_socket[SOCKET_MAX];
int g_connected_cnt = 0;

char g_testname[256];

void viewPrint(const char *str)
{
	puts(str);
}
void filePrint(const char *str)
{
	FILE *fp, *fp_date;
	char filename[256];
	time_t timer;
	struct tm *t;

	timer = time(NULL);
	t = localtime(&timer);
	sprintf(filename, "log/%d_%d_%d_%d_%s.txt", t->tm_year + 1900, t->tm_mon, t->tm_mday, t->tm_hour, g_testname);
	fp_date = fopen(filename, "a");

	sprintf(filename, "log/%s_log.txt", g_testname);
	fp = fopen(filename, "a");

	fprintf(fp, "%s\n", str);
	fprintf(fp_date, "%s\n", str);

	fclose(fp);
	fclose(fp_date);
}
void printLog(const char *Format, ...)
{
	char buf[512] = {0,}; 
	va_list ap; 
	
	strcpy (buf, "[Client] : "); 
	
	va_start(ap, Format); 
	vsprintf(buf + strlen(buf), Format, ap); 
	va_end(ap); 

#ifdef DEBUG
	viewPrint(buf);
#endif

#ifdef LOG
	filePrint(buf);
#endif
}

double transferTest(int cur_socket, int send_cnt, int send_size)
{
	int i;
	struct timeval start_point, end_point;
	double operating_time;
	char buffer[BUF_SIZE];
	int retval;

	memset(buffer, '1', BUF_SIZE);

	gettimeofday(&start_point, NULL);
	for(i = 0; i < send_cnt; i++)
	{
		if(send_size > 0)
		{
			send(cur_socket, buffer, send_size, 0);
			retval = recv(cur_socket, buffer, sizeof(buffer), 0);
			if(send_size != retval)
			{
				perror("Not Equal!");
				return -1;
			}
			buffer[retval] = '\0';
		}
	}
	gettimeofday(&end_point, NULL);

	operating_time = (double)(end_point.tv_sec)+(double)(end_point.tv_usec)/1000000.0-(double)(start_point.tv_sec)-(double)(start_point.tv_usec)/1000000.0;

	return operating_time;
}

double connectingTest(int connect_cnt)
{	
	int i, connected_cnt;
	struct timeval start_point, end_point;
	double operating_time;
	struct sockaddr_in connect_adress;
	int retval;

	memset(&connect_adress, 0, sizeof(connect_adress));
	connect_adress.sin_family = AF_INET;
	connect_adress.sin_addr.s_addr = inet_addr(IP);
	connect_adress.sin_port = htons(PORT);

	connected_cnt = g_connected_cnt;
	gettimeofday(&start_point, NULL);
	for(i = connected_cnt; i < connect_cnt + connected_cnt && i < SOCKET_MAX; i++)	
	{
		g_arr_socket[i] = socket(AF_INET, SOCK_STREAM, 0);
		printLog("connect request [connect cnt = %d]", g_connected_cnt);
		retval = connect(g_arr_socket[i], 
					(struct sockaddr*)&connect_adress,
					sizeof(connect_adress));
		if(retval == -1)
		{
			printLog("connect error [%d / %d : %d]\n", i, connect_cnt, g_connected_cnt);
			return -1;
		}
		g_connected_cnt++;
		printLog("connect success [connect cnt = %d]\n", g_connected_cnt);
	}
	gettimeofday(&end_point, NULL);

	operating_time = (double)(end_point.tv_sec)+(double)(end_point.tv_usec)/1000000.0-(double)(start_point.tv_sec)-(double)(start_point.tv_usec)/1000000.0;

	return operating_time;
}

double closingTest(int close_cnt)
{
	int i;
	struct timeval start_point, end_point;
	double operating_time;
	int retval;

	gettimeofday(&start_point, NULL);
	for(i = 0; i < close_cnt && i < SOCKET_MAX; i++)
	{
		printLog("close request [connect cnt = %d]", g_connected_cnt - i);
		if((retval = close(g_arr_socket[i])) == -1)
		{
			printLog("close error[errno = %d, fd = %d] [%d / %d : %d]\n", errno, g_arr_socket[i], i, close_cnt, g_connected_cnt);
			perror("");
			continue;
		}
		printLog("close success [connect cnt = %d]\n", g_connected_cnt - i - 1);
	}
	gettimeofday(&end_point, NULL);

	for( ; i < g_connected_cnt && i < SOCKET_MAX; i++)
	{
		g_arr_socket[i - close_cnt] = g_arr_socket[i];
	}
	g_connected_cnt -= close_cnt;

	operating_time = (double)(end_point.tv_sec)+(double)(end_point.tv_usec)/1000000.0-(double)(start_point.tv_sec)-(double)(start_point.tv_usec)/1000000.0;
	
	return operating_time;
}
int closeAllSocket()
{
	int i;
	for(i = 0; i < g_connected_cnt; i++)
	{
		if(close(g_arr_socket[i]) == -1)
		{
			printLog("close error [%d / %d]\n", i, g_connected_cnt);
			return -1;
		}
	}
	g_connected_cnt = 0;
	return 0;
}
double test(int connecting_cnt)
{
	int cur_socket;
	int	i;
	double average_time, time_connect, time_transfer, operating_time;
	int connect_test_cnt = 10;

	average_time = 0;
	srand(time(NULL));

	operating_time = connectingTest(connecting_cnt);
	if(operating_time < 0)
	{
		printLog("init error");
		return -1;
	}

	for(i = 0; i < TEST_CNT; i++)
	{
		time_connect = 0;
		operating_time = connectingTest(connect_test_cnt);
		if(operating_time == -1)
		{
			printLog("connect test error");
			return -1;
		}
		//printLog("%5d Test [Cur Socket = %5d, Send Count = %5d, Send Size = %5d] Time = %.3lf sec", i_test, cur_socket, SEND_CNT, SEND_SIZE, operating_time);
		time_connect += operating_time;

		cur_socket = g_arr_socket[rand() % g_connected_cnt];
		operating_time = transferTest(cur_socket, SEND_SIZE, SEND_CNT);
		if(operating_time == -1)
		{
			printLog("transfer test error");
			return -1;
		}
		//printLog("%5d Test [Cur Socket = %5d, Send Count = %5d, Send Size = %5d] Time = %.3lf sec", i, cur_socket, SEND_CNT, SEND_SIZE, operating_time);
		time_transfer = operating_time;
	
		operating_time = closingTest(connect_test_cnt);
		if(operating_time == -1)
		{
			printLog("close test error");
			return -1;
		}
		//printLog("%5d Test [Cur Socket = %5d, Send Count = %5d, Send Size = %5d] Time = %.3lf sec", i_test, cur_socket, SEND_CNT, SEND_SIZE, operating_time);	
		time_connect += operating_time;

		printLog("[%3d Test] Time Trasnfer = %.3lf ms / Time Connect = %.3lf ms / Total Time = %.3lf ms", i, time_transfer * 1000, time_connect * 1000, (time_transfer + time_connect) * 1000);
		average_time += time_transfer + time_connect;
	}

	printLog("[%-15s] total average time = %.3lf ms\n", g_testname, average_time / TEST_CNT * 1000);

	return average_time / TEST_CNT;
}

int main(int argc, char** argv)
{
	int connecting_cnt = 50;

	if(argc == 3)
	{
		strcpy(IP, argv[1]);
		PORT = atoi(argv[2]);
	}

	switch(PORT)
	{
	case 9000:
		strcpy(g_testname, "select");
		break;

	case 9100:
		strcpy(g_testname, "poll");
		break;

	case 9200:
		strcpy(g_testname, "epoll");
		break;

	case 9300:
		strcpy(g_testname, "multiProcess");
		break;

	case 9400:
		strcpy(g_testname, "multiThread");
		break;
	}

	printLog("%s : ", g_testname);
	if(test(connecting_cnt) == -1)
		printLog("error");

	closeAllSocket();

	printf("\n%s test finish\n", g_testname);
	return 0;
}












