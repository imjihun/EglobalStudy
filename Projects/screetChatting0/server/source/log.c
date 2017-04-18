#include "chattingLog.h"
#include <stdio.h>

int main(int argc, char **argv)
{
	if(argc < 2 || 
		(argv[1][0] != '-' || strlen(argv[1]) != 2) ||
		(argv[1][1] == 'v' && argv[2] == NULL))
	{
		printf("usage %s <-option>\n", argv[0]);
		printf("\t -l : list file\n");
		printf("\t -v (filename) : view file\n");
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
		viewFile(argv[2]);
		break;
	default:
		printf("usage %s <option>\n", argv[0]);
		break;
	}
	return 0;
}
