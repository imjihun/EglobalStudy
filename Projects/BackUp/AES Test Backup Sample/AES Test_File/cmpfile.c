#include <stdio.h>

#define TYPEBUF char
#define MAXBUF 4096
int main(int argc, char **argv)
{
	int retval = 0;
	size_t retval1, retval2;
	TYPEBUF buf1[MAXBUF], buf2[MAXBUF];

	FILE *fp1, *fp2;

	if(argc != 3)
		return -1;

	fp1 = fopen(argv[1], "rb");
	fp2 = fopen(argv[2], "rb");

	while((retval1 = fread(buf1, sizeof(TYPEBUF), MAXBUF, fp1)) > 0
		&& (retval2 = fread(buf2, sizeof(TYPEBUF), MAXBUF, fp2)) > 0)
	{
		if(retval1 != retval2 || (retval = memcmp(buf1, buf2, retval1)) != 0)
		{
			printf("Not Equal[%d]\n", retval);
			fclose(fp1);
			fclose(fp2);
			return 0;
		}
	}

	printf("Equal\n");
	fclose(fp1);
	fclose(fp2);
	return 0;
}
