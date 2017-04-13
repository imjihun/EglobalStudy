#include <stdio.h>


int main()
{
	FILE *fp = fopen("t.txt", "r");

	fseek(fp, -1000, SEEK_END);
	printf("ftell = %d\n", ftell(fp));
	perror("error");
	perror("error");

	return 0;
}