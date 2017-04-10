#include <stdio.h>
#include <string.h>

int main()
{
	char buf[4];

	printf("ret = %d\n", sprintf(buf, "%s", "1234567890"));

	printf("len = %d\n", strlen(buf));
	printf("buf = %s\n", buf);
	return 0;
}
