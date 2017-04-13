/* 2017-04-04 : created by JHLim */
/***************************************************************************
* Copyright (c) 2004-2016, eGlobal Systems, Co., Ltd.
* Seoul, Republic of Korea
* All Rights Reserved.
*
* Description : prototype.c
*       xxxx
*
***************************************************************************/

/*************** Header files *********************************************/
#include "connectDB.h"

#include <stdlib.h>

/*************** New Data Types *******************************************/

/*************** Definitions / Macros  ************************************/

/*************** Global Variables *****************************************/

MYSQL g_conn;
MYSQL *g_connection = NULL;
/*************** Prototypes ***********************************************/

/*************** Function *************************************************/

int viewAllTable()
{
    char query[SIZE_QUERY];
    int query_stat;
    MYSQL_RES *sql_result;
    MYSQL_ROW sql_row;

    sprintf(query, "select * from %s", "userinfo");
    query_stat = mysql_query(g_connection, query);
    if (query_stat != 0)
    {
        fprintf(stderr, "Mysql query error : %s", mysql_error(&g_conn));
        return -3;
    }
    
    sql_result = mysql_store_result(g_connection);
    
    printf("| %-16s |\n", "userinfo_id");
    while ( (sql_row = mysql_fetch_row(sql_result)) != NULL )
    {
        printf("| %-16s |\n", sql_row[0]);
    }
    printf("\n");

    mysql_free_result(sql_result);

//////////////////////////////////////////////////////////////////

    sprintf(query, "select * from %s", "roominfo");
    query_stat = mysql_query(g_connection, query);
    if (query_stat != 0)
    {
        fprintf(stderr, "Mysql query error : %s", mysql_error(&g_conn));
        return -3;
    }
    
    sql_result = mysql_store_result(g_connection);
    
    printf("| %-16s | %-20s | %-16s |\n", "roominfo_number", "roominfo_subject", "roominfo_key");
    while ( (sql_row = mysql_fetch_row(sql_result)) != NULL )
    {
        printf("| %-16d | ", atoi(sql_row[0]));
        printf("%-20s | ", sql_row[1]);
        if(sql_row[2] == NULL)
            printf("%-16s |\n", "null");
        else
            printf("%-16s |\n", "exist");
    }
    printf("\n");

    mysql_free_result(sql_result);

//////////////////////////////////////////////////////////////////

    sprintf(query, "select * from %s", "roomuser");
    query_stat = mysql_query(g_connection, query);
    if (query_stat != 0)
    {
        fprintf(stderr, "Mysql query error : %s", mysql_error(&g_conn));
        return -3;
    }
    
    sql_result = mysql_store_result(g_connection);
    
    printf("| %-16s | %-16s |\n", "roominfo_number", "userinfo_id");
    while ( (sql_row = mysql_fetch_row(sql_result)) != NULL )
    {
        printf("| %-16d | ", atoi(sql_row[0]));
        printf("%-16s |\n", sql_row[1]);
    }
    printf("---------------------------------------------------------------\n");

    mysql_free_result(sql_result);

    return 0;
}
int dbInsertUserinfo(char* id)
{
    char query[SIZE_QUERY];
    int query_stat;

    printf("\t\tcalled dbInsertUserinfo()\n");

    sprintf(query, "insert into userinfo (userinfo_id) value('%s')", id);
    query_stat = mysql_query(g_connection, query);
    if (query_stat != 0)
    {
        fprintf(stderr, "Mysql query error : %s\n", mysql_error(&g_conn));
        //return -2;
    }
    return viewAllTable();
}

int dbInsertRoominfo(TYPE_ROOM_NUMBER room_number, char *room_subject, uint8_t *key)
{
    char query[SIZE_QUERY];
    int query_stat;

    printf("\t\tcalled dbInsertRoominfo()\n");

    sprintf(query, "insert into roominfo (roominfo_number, roominfo_subject) value(%d, '%s')",
                        room_number, room_subject);
    if(key == NULL)
    {
        ;
    }
    else
    {
        ;
    }

    query_stat = mysql_query(g_connection, query);
    if (query_stat != 0)
    {
        fprintf(stderr, "Mysql query error : %s\n", mysql_error(&g_conn));
        //return -2;
    }
    return viewAllTable();
}
int dbDeleteRoominfo(TYPE_ROOM_NUMBER room_number)
{
    char query[SIZE_QUERY];
    int query_stat;

    printf("\t\tcalled dbDeleteRoominfo(%d)\n", room_number);

    sprintf(query, "delete from roominfo where roominfo_number=%d", room_number);
    query_stat = mysql_query(g_connection, query);
    if (query_stat != 0)
    {
        fprintf(stderr, "Mysql query error : %s\n", mysql_error(&g_conn));
        //return -2;
    }
    return viewAllTable();
}

int dbInsertRoomUser(TYPE_ROOM_NUMBER room_number, char*id)
{
    char query[SIZE_QUERY];
    int query_stat;

    printf("\t\tcalled dbInsertRoomUser()\n");

    sprintf(query, "insert into roomuser (roominfo_number, userinfo_id) value(%d, '%s')", room_number, id);
    query_stat = mysql_query(g_connection, query);
    if (query_stat != 0)
    {
        fprintf(stderr, "Mysql query error : %s\n", mysql_error(&g_conn));
        //return -2;
    }
    return viewAllTable();
}
int dbDeleteRoomUser(TYPE_ROOM_NUMBER room_number, char*id)
{
    char query[SIZE_QUERY];
    int query_stat;

    printf("\t\tcalled dbDeleteRoomUser()\n");

    sprintf(query, "delete from roomuser where roominfo_number=%d and userinfo_id='%s'", room_number, id);
    query_stat = mysql_query(g_connection, query);
    if (query_stat != 0)
    {
        fprintf(stderr, "Mysql query error : %s\n", mysql_error(&g_conn));
        //return -2;
    }
    return viewAllTable();
}

int dbSelectAllRoom(room_info *arr_room_info_ret, int size_arr)
{
    char query[SIZE_QUERY];
    int query_stat;
    MYSQL_RES *sql_result;
    MYSQL_ROW sql_row;
    int i;

    sprintf(query, "select * from %s", "roominfo");
    query_stat = mysql_query(g_connection, query);
    if (query_stat != 0)
    {
        fprintf(stderr, "Mysql query error : %s", mysql_error(&g_conn));
        return -3;
    }
    
    sql_result = mysql_store_result(g_connection);
    i = 0;
    while ( (sql_row = mysql_fetch_row(sql_result)) != NULL && i < size_arr )
    {
        // struct 에 룸넘버가 없는데 추가해야 함 여기서부터 시작. startstart
        arr_room_info_ret[i].room_number = atoi(aql_row[0]);
        printf("| %-16s |\n", sql_row[0]);
        i++;
    }

    mysql_free_result(sql_result);

    return viewAllTable();
}

int dbOpen()
{
    mysql_init(&g_conn);

    g_connection = mysql_real_connect(&g_conn, DB_HOST,
                                    DB_USER, DB_PASS,
                                    DB_NAME, 3306,
                                    (char *)NULL, 0);

    if (g_connection == NULL)
    {
        fprintf(stderr, "Mysql g_connection error : %s", mysql_error(&g_conn));
        return -2;
    }
    return 0;
}

int dbClose()
{
    mysql_close(g_connection);
    return 0;
}
int dbCreateAllTable()
{
    char query[SIZE_QUERY];
    int query_stat;

    sprintf(query, 
        "create table userinfo (    userinfo_id varchar(%d) NOT NULL, \
                                    primary key (userinfo_id) \
                                ) engine=InnoDB DEFAULT CHARSET=utf8",
        SIZE_ID);
    query_stat = mysql_query(g_connection, query);
    if (query_stat != 0)
    {
        fprintf(stderr, "Mysql query error : %s\n", mysql_error(&g_conn));
        return -2;
    }


    sprintf(query, 
        "create table roominfo (    roominfo_number bigint(%d) unsigned NOT NULL, \
                                    roominfo_subject varchar(%d), \
                                    roominfo_key blob(%d), \
                                    primary key (roominfo_number) \
                                ) engine=InnoDB DEFAULT CHARSET=utf8",
        SIZE_ROOM_NUMBER, SIZE_ROOM_SUBJECT, SIZE_SECRET_KEY);
    query_stat = mysql_query(g_connection, query);
    if (query_stat != 0)
    {
        fprintf(stderr, "Mysql query error : %s\n", mysql_error(&g_conn));
        return -2;
    }

    sprintf(query, 
        "create table roomuser (    roominfo_number bigint(%d) unsigned NOT NULL, \
                                    userinfo_id varchar(%d) NOT NULL, \
                                    foreign key (roominfo_number) \
                                        references roominfo (roominfo_number) \
                                        on delete cascade, \
                                    foreign key (userinfo_id) \
                                        references userinfo (userinfo_id) \
                                        on delete cascade \
                                ) engine=InnoDB DEFAULT CHARSET=utf8",
        SIZE_ROOM_NUMBER, SIZE_ID);
    query_stat = mysql_query(g_connection, query);
    if (query_stat != 0)
    {
        fprintf(stderr, "Mysql query error : %s\n", mysql_error(&g_conn));
        return -2;
    }

    return viewAllTable();
}

int dbDropAllTable()
{
    int query_stat;

    query_stat = mysql_query(g_connection, 
        "drop table roomuser");
    if (query_stat != 0)
    {
        fprintf(stderr, "Mysql query error : %s\n", mysql_error(&g_conn));
        //return -2;
    }
    query_stat = mysql_query(g_connection, 
        "drop table userinfo");
    if (query_stat != 0)
    {
        fprintf(stderr, "Mysql query error : %s\n", mysql_error(&g_conn));
        //return -2;
    }
    query_stat = mysql_query(g_connection, 
        "drop table roominfo");
    if (query_stat != 0)
    {
        fprintf(stderr, "Mysql query error : %s\n", mysql_error(&g_conn));
        //return -2;
    }

    return 0;
}

/*************** END OF FILE **********************************************/

