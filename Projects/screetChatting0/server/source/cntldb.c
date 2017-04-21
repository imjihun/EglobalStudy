#include "connectDB.h"
#include "chattingLog.h"
#include <stdio.h>

int main(int argc, char **argv)
{
    if(argc < 2 || 
        (argv[1][0] != '-' || strlen(argv[1]) != 2))
    {
        printf("usage %s <-option>\n", argv[0]);
        printf("\t -v : view all tables\n");
        printf("\t -c : create all tables\n");
        printf("\t -d : drop all tables\n");
        printf("\n");
        return -1;
    }

    dbOpen();
    switch(argv[1][1])
    {
    case 'v':
        _dbViewAllTable();
        break;
    case 'c':
        dbCreateAllTableIfNotExists();
        break;
    case 'd':
        dbDropAllTable();
        break;
    default:
        break;
    }
    dbClose();
    return 0;
}
