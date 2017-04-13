#include <unistd.h>
#include <sys/types.h>
#include <sys/stat.h>
#include <stdio.h>

#define PATH "log"
void print_id(const char *comment);
int main(void)
{
	char resolved_path[255] = "log";

	if(realpath(PATH, resolved_path) == NULL)
		return -1;

	printf("%s\n", resolved_path);
	return 0;
}
int initDaemon(void)
{
	pid_t pid;
	FILE *fp;
	char resolved_path[255];

	umask(0);

	if (daemon(1, 1) == -1) {
	/* handle error */
		return 1;
	}

	if ((pid = fork()) == -1) {
		return 1;
	}
	else if (pid > 0) {           /* parent */
		_exit(0);
	}
	/* child */
	
	stdin = freopen("/dev/null", "r", stdin);
	stdout = freopen("/dev/null", "w", stdout);
	stderr = freopen("/dev/null", "w", stderr);

	realpath("log/log.txt", resolved_path);

	return 0;
}

void print_id(const char *comment)
{
	fprintf(stderr, "sid: %5d, pgid: %5d, pid: %5d, ppid: %5d   # %s\n", 
		(int)getsid(0), (int)getpgid(0), (int)getpid(), (int)getppid(), comment);
}

