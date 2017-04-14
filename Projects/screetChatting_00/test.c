#include <stdio.h>

void main()
{
	char arr[5] = "1b99";
	char tmp[3];
	int i;

	for(i = 0; i < 2; i++)
	{
		tmp[0] = arr[2*i + 0];
		tmp[1] = arr[2*i + 1];
		tmp[2] = '\0';

		printf("%x\n", (unsigned char)strtol(tmp, NULL, 16));
	}

}
