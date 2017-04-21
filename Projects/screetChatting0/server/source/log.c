#include "chattingLog.h"
#include <stdio.h>

int main(int argc, char **argv)
{
	if(argc < 2 || 
		(argv[1][0] != '-' || strlen(argv[1]) != 2) ||
		(argv[1][1] == 'v' && (argv[2] == NULL || atoi(argv[2]) < 1 || atoi(argv[2]) > MAX_ROOM)))
	{
		printf("usage %s <-option>\n", argv[0]);
		printf("\t -l : list file\n");
		printf("\t -r : remove all files\n");
		printf("\t -v (file number) : view file\n");
        printf("\n");
		return -1;
	}

	if(initPath() != 0)
	{
		printf("path error\n");
		return -1;
	}

	switch(argv[1][1])
	{
	case 'l':
		viewList();
		break;
	case 'v':
		if(viewFile(atoi(argv[2])) != 0)
			printf("not exist file\n");
		break;
	case 'r':
		removeAllFiles();
		break;
	default:
		printf("usage %s <option>\n", argv[0]);
		break;
	}
	return 0;
}
