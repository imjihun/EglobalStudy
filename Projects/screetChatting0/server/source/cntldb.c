#include "connectDB.h"
#include <stdio.h>

int main(int argc, char **argv)
{
    if(argc < 2)
        return -1;

    dbOpen();
    switch(argv[1][0])
    {
    case 'v':
        viewAllTable();
        break;
    case 'c':
        dbCreateAllTable();
        break;
    case 'd':
        dbDropAllTable();
        system("rm -rf /home/user1/chattingLog");
        break;
    default:
        dbDropAllTable();
        dbCreateAllTable();
        break;
    }
    dbClose();
    return 0;
}
