
#include <mysql.h>
#include <string.h>
#include <stdio.h>

#define DB_HOST "127.0.0.1"
#define DB_USER "root"
#define DB_PASS "root00"
#define DB_NAME "test"

#define SIZE_QUERY  1024


int dbOpen();
int dbClose();
int createAllTable();
int deleteAllTable();
