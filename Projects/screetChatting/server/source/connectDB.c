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


/*************** New Data Types *******************************************/

/*************** Definitions / Macros  ************************************/

/*************** Global Variables *****************************************/

MYSQL g_conn;
MYSQL *g_connection = NULL;
/*************** Prototypes ***********************************************/

/*************** Function *************************************************/
int _dbViewAllTable()
{
    char query[SIZE_QUERY];
    int query_stat;
    MYSQL_RES *sql_result;
    MYSQL_ROW sql_row;

    sprintf(query, "select * from %s", "userinfo");
    query_stat = mysql_query(g_connection, query);
    if (query_stat != 0)
    {
        printLog("Mysql query error : %s", mysql_error(&g_conn));
        return -2;
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
        printLog("Mysql query error : %s", mysql_error(&g_conn));
        return -2;
    }
    
    sql_result = mysql_store_result(g_connection);
    
    printf("| %-16s | %-16s | %-20s | %-32s | %-20s |\n", "roominfo_number", "roominfo_status", "roominfo_subject", "roominfo_key", "roominfo_countmember");
    while ( (sql_row = mysql_fetch_row(sql_result)) != NULL )
    {
        printf("| %-16d | ", atoi(sql_row[0]));
        printf("%-16s | ", sql_row[1]);
        printf("%-20s | ", sql_row[2]);
        if(sql_row[2] == NULL)
        {
            printf("%-32s | ", "null");
        }
        else
        {
            printf("%-32s | ", sql_row[3]);
        }
        printf("%-20d |\n", atoi(sql_row[4]));
    }
    printf("\n");

    mysql_free_result(sql_result);

//////////////////////////////////////////////////////////////////

    sprintf(query, "select * from %s", "roomuser");
    query_stat = mysql_query(g_connection, query);
    if (query_stat != 0)
    {
        printLog("Mysql query error : %s", mysql_error(&g_conn));
        return -2;
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
int dbViewAllTable()
{
#ifdef VIEWTABLE
    _dbViewAllTable();
#endif // VIEWTABLE
    return 0;
}
int dbInsertUserinfo(char* id)
{
    char query[SIZE_QUERY];
    int query_stat;

    // printLog("\t\tcalled dbInsertUserinfo()\n");

    sprintf(query, "insert into userinfo (userinfo_id) value('%s')", id);
    query_stat = mysql_query(g_connection, query);
    if (query_stat != 0)
    {
        printLog("Mysql query error : %s\n", mysql_error(&g_conn));
        // return -2;
    }
    return dbViewAllTable();
}

int dbInsertRoominfo(room_info *p_room_info)
{
    char query[SIZE_QUERY];
    int query_stat;

    char string_key[SIZE_SECRET_KEY * 2 + 1];
    int idx_string_key;
    int idx_key;

    // printLog("\t\tcalled dbInsertRoominfo()\n");

    idx_string_key = 0;
    for(idx_key = 0; idx_key < SIZE_SECRET_KEY; idx_key++)
        idx_string_key += sprintf(string_key + idx_string_key, "%02x", *(unsigned char *)(p_room_info->key + idx_key));
    string_key[idx_string_key] = '\0';

    // sprintf(query, "insert into roominfo (roominfo_number, roominfo_status, roominfo_subject, roominfo_key) value(%d, '%c', '%s', '%s')",
    //                     p_room_info->room_number, p_room_info->status, p_room_info->subject, string_key);
    sprintf(query, "insert into roominfo (roominfo_status, roominfo_subject, roominfo_key) value('%c', '%s', '%s')",
                        p_room_info->status, p_room_info->subject, string_key);
    query_stat = mysql_query(g_connection, query);
    if (query_stat != 0)
    {
        printLog("Mysql query error : %s\n", mysql_error(&g_conn));
        return -2;
    }

    // room_number auto_increment load and save
    p_room_info->room_number = mysql_insert_id(g_connection);

    dbViewAllTable();
    return 0;
}
int dbDeleteRoominfo(TYPE_ROOM_NUMBER room_number)
{
    char query[SIZE_QUERY];
    int query_stat;

    // printLog("\t\tcalled dbDeleteRoominfo(%d)\n", room_number);

    sprintf(query, "delete from roominfo where roominfo_number=%d", room_number);
    query_stat = mysql_query(g_connection, query);
    if (query_stat != 0)
    {
        printLog("Mysql query error : %s\n", mysql_error(&g_conn));
        return -2;
    }
    return dbViewAllTable();
}

int dbInsertRoomUser(TYPE_ROOM_NUMBER room_number, char*id)
{
    char query[SIZE_QUERY];
    int query_stat;

    // printLog("\t\tcalled dbInsertRoomUser()\n");

    sprintf(query, "insert into roomuser (roominfo_number, userinfo_id) value(%d, '%s')", room_number, id);
    query_stat = mysql_query(g_connection, query);
    if (query_stat != 0)
    {
        printLog("Mysql query error : %s\n", mysql_error(&g_conn));
        return -2;
    }
    return dbViewAllTable();
}
int dbDeleteRoomUser(TYPE_ROOM_NUMBER room_number, char*id)
{
    char query[SIZE_QUERY];
    int query_stat;

    // printLog("\t\tcalled dbDeleteRoomUser()\n");

    sprintf(query, "delete from roomuser where roominfo_number=%d and userinfo_id='%s'", room_number, id);
    query_stat = mysql_query(g_connection, query);
    if (query_stat != 0)
    {
        printLog("Mysql query error : %s\n", mysql_error(&g_conn));
        return -2;
    }
    return dbViewAllTable();
}


int dbSelectKeyOfRoom(TYPE_ROOM_NUMBER room_number, TYPE_SECRET_KEY *key)
{
    char query[SIZE_QUERY];
    int query_stat;
    MYSQL_RES *sql_result;
    MYSQL_ROW sql_row;

    char key_elem[3];
    int i;

    // printLog("\t\tcalled dbSelectKeyOfRoom()\n");

    sprintf(query, "select roominfo_key from roominfo where roominfo_number=%d", room_number);
    query_stat = mysql_query(g_connection, query);
    if (query_stat != 0)
    {
        printLog("Mysql query error : %s", mysql_error(&g_conn));
        return -2;
    }
    
    sql_result = mysql_store_result(g_connection);
    while ( (sql_row = mysql_fetch_row(sql_result)) != NULL)
    {
        // room key (string -> hex)
        for(i = 0; i < SIZE_SECRET_KEY; i++)
        {
            key_elem[0] = sql_row[0][i*2 + 0];
            key_elem[1] = sql_row[0][i*2 + 1];
            key_elem[2] = '\0';

            key[i] = (TYPE_SECRET_KEY)strtol(key_elem, NULL, 16);
        }
    }

    return 0;
}
int dbSelectAllRoom(room_info *arr_room_info_ret, int size_arr)
{
    char query[SIZE_QUERY];
    int query_stat;
    MYSQL_RES *sql_result;
    MYSQL_ROW sql_row;

    int count_roominfo;
    int room_number;
    int size_subject;
    char key_elem[3];
    int i;

    // printLog("\t\tcalled dbSelectAllRoom()\n");

    sprintf(query, "select * from %s where roominfo_status='N'", "roominfo");
    query_stat = mysql_query(g_connection, query);
    if (query_stat != 0)
    {
        printLog("Mysql query error : %s", mysql_error(&g_conn));
        return -2;
    }
    
    sql_result = mysql_store_result(g_connection);
    count_roominfo = 0;
    while ( (sql_row = mysql_fetch_row(sql_result)) != NULL && count_roominfo < size_arr )
    {
        // room number
        room_number = atoi(sql_row[0]);
        arr_room_info_ret[count_roominfo].room_number = room_number;

        // room status
        arr_room_info_ret[count_roominfo].status = sql_row[1][0];

        // room subject
        size_subject = strlen(sql_row[2]);
        memcpy(arr_room_info_ret[count_roominfo].subject, sql_row[2], size_subject);
        memset(arr_room_info_ret[count_roominfo].subject + size_subject, 0, SIZE_ROOM_SUBJECT - size_subject);

        // room key (string -> hex)
        for(i = 0; i < SIZE_SECRET_KEY; i++)
        {
            key_elem[0] = sql_row[3][i*2 + 0];
            key_elem[1] = sql_row[3][i*2 + 1];
            key_elem[2] = '\0';

            arr_room_info_ret[count_roominfo].key[i] = (TYPE_SECRET_KEY)strtol(key_elem, NULL, 16);
        }

        arr_room_info_ret[count_roominfo].count_member = atoi(sql_row[4]);

        count_roominfo++;
    }

    mysql_free_result(sql_result);

    return count_roominfo;
}
int dbSelectUserInRoom(int room_number, char (*arr_id_ret)[SIZE_ID], int size_arr_id)
{
    char query[SIZE_QUERY];
    int query_stat;
    MYSQL_RES *sql_result;
    MYSQL_ROW sql_row;

    int count_id;
    int size_id;

    // printLog("\t\tcalled dbSelectUserInRoom()\n");

    sprintf(query, "select * from %s where roominfo_number=%d", "roomuser", room_number);
    query_stat = mysql_query(g_connection, query);
    if (query_stat != 0)
    {
        printLog("Mysql query error : %s", mysql_error(&g_conn));
        return -2;
    }
    
    sql_result = mysql_store_result(g_connection);
    count_id = 0;
    while ( (sql_row = mysql_fetch_row(sql_result)) != NULL && count_id < size_arr_id )
    {
        // id
        size_id = strlen(sql_row[1]);
        memcpy(arr_id_ret[count_id], sql_row[1], size_id);
        memset(arr_id_ret[count_id] + size_id, 0, SIZE_ID - size_id);

        count_id++;
    }

    mysql_free_result(sql_result);

    return count_id;
}
int dbSelectRoomOfUser(char *id, room_info *arr_room_info_ret, int size_arr)
{
    char query[SIZE_QUERY];
    int query_stat;
    MYSQL_RES *sql_result;
    MYSQL_ROW sql_row;

    int count_roominfo;
    int room_number;
    int size_subject;
    char key_elem[3];
    int i;

    // printLog("\t\tcalled dbSelectRoomOfUser()\n");

    sprintf(query, "select * from roominfo where roominfo_number=any(select roominfo_number from roomuser where userinfo_id='%s')", id);
    query_stat = mysql_query(g_connection, query);
    if (query_stat != 0)
    {
        printLog("Mysql query error : %s", mysql_error(&g_conn));
        return -2;
    }
    
    sql_result = mysql_store_result(g_connection);
    count_roominfo = 0;
    while ( (sql_row = mysql_fetch_row(sql_result)) != NULL && count_roominfo < size_arr )
    {
        // room number
        room_number = atoi(sql_row[0]);
        if(room_number >= MAX_ROOM || room_number < 0)
            continue;
        arr_room_info_ret[count_roominfo].room_number = room_number;

        // room status
        arr_room_info_ret[count_roominfo].status = sql_row[1][0];

        // room subject
        size_subject = strlen(sql_row[2]);
        memcpy(arr_room_info_ret[count_roominfo].subject, sql_row[2], size_subject);
        memset(arr_room_info_ret[count_roominfo].subject + size_subject, 0, SIZE_ROOM_SUBJECT - size_subject);

        // room key (string -> hex)
        for(i = 0; i < SIZE_SECRET_KEY; i++)
        {
            key_elem[0] = sql_row[3][i*2 + 0];
            key_elem[1] = sql_row[3][i*2 + 1];
            key_elem[2] = '\0';

            arr_room_info_ret[count_roominfo].key[i] = (TYPE_SECRET_KEY)strtol(key_elem, NULL, 16);
        }

        // count member
        arr_room_info_ret[count_roominfo].count_member = atoi(sql_row[4]);

        count_roominfo++;
    }

    mysql_free_result(sql_result);

    return count_roominfo;
}
int dbSelectRoomOfRoomNumber(TYPE_ROOM_NUMBER room_number, room_info *p_room_info_ret)
{
    char query[SIZE_QUERY];
    int query_stat;
    MYSQL_RES *sql_result;
    MYSQL_ROW sql_row;

    int size_subject;
    char key_elem[3];
    int i;

    // printLog("\t\tcalled dbSelectRoomOfRoomNumber()\n");

    sprintf(query, "select * from roominfo where roominfo_number=%d", room_number);
    query_stat = mysql_query(g_connection, query);
    if (query_stat != 0)
    {
        printLog("Mysql query error : %s", mysql_error(&g_conn));
        return -2;
    }
    
    sql_result = mysql_store_result(g_connection);

    if ( (sql_row = mysql_fetch_row(sql_result)) == NULL)
    {
        mysql_free_result(sql_result);
        return -2;
    }

    // room number
    p_room_info_ret->room_number = room_number;

    // room status
    p_room_info_ret->status = sql_row[1][0];

    // room subject
    size_subject = strlen(sql_row[2]);
    memcpy(p_room_info_ret->subject, sql_row[2], size_subject);
    memset(p_room_info_ret->subject + size_subject, 0, SIZE_ROOM_SUBJECT - size_subject);

    // room key (string -> hex)
    for(i = 0; i < SIZE_SECRET_KEY; i++)
    {
        key_elem[0] = sql_row[3][i*2 + 0];
        key_elem[1] = sql_row[3][i*2 + 1];
        key_elem[2] = '\0';

        p_room_info_ret->key[i] = (TYPE_SECRET_KEY)strtol(key_elem, NULL, 16);
    }

    // count member
    p_room_info_ret->count_member = atoi(sql_row[4]);

    mysql_free_result(sql_result);

    return 0;
}
int dbCountUserInRoom(TYPE_ROOM_NUMBER room_number)
{
    char query[SIZE_QUERY];
    int query_stat;
    MYSQL_RES *sql_result;
    MYSQL_ROW sql_row;

    int retval = -2;

    // printLog("\t\tcalled dbCountUserInRoom()\n");

    sprintf(query, "select roominfo_countmember from roominfo where roominfo_number=%d", room_number);
    query_stat = mysql_query(g_connection, query);
    if (query_stat != 0)
    {
        printLog("Mysql query error : %s", mysql_error(&g_conn));
        return -2;
    }

    sql_result = mysql_store_result(g_connection);
    if(sql_result)
    {
        sql_row = mysql_fetch_row(sql_result);
        if(sql_row)
            retval = atoi(sql_row[0]);
        else
            retval = -2;
    }

    mysql_free_result(sql_result);
    return retval;
}

int dbCheckRoomUser(TYPE_ROOM_NUMBER room_number, char *id)
{
    char query[SIZE_QUERY];
    int query_stat;
    MYSQL_RES *sql_result;
    MYSQL_ROW sql_row;

    int count = 0;

    sprintf(query, "select count(*) from roomuser where roominfo_number=%d and userinfo_id='%s'"
                    , room_number, id);
    query_stat = mysql_query(g_connection, query);
    if (query_stat != 0)
    {
        printLog("Mysql query error : %s", mysql_error(&g_conn));
        return -2;
    }

    sql_result = mysql_store_result(g_connection);
    if(sql_result)
    {
        sql_row = mysql_fetch_row(sql_result);
        count = atoi(sql_row[0]);
    }
    mysql_free_result(sql_result);

    if(count == 0)
        return -1;
    else
        return 0;
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
        printLog("Mysql g_connection error : %s", mysql_error(&g_conn));
        return -2;
    }
    return 0;
}
int dbClose()
{
    mysql_close(g_connection);
    return 0;
}


int dbCreateAllTableIfNotExists()
{
    char query[SIZE_QUERY];
    int query_stat;

    sprintf(query, 
        "create table if not exists userinfo (  userinfo_id varchar(%d) NOT NULL, \
                                                primary key (userinfo_id) \
                                            ) engine=InnoDB DEFAULT CHARSET=utf8",
        SIZE_ID);
    query_stat = mysql_query(g_connection, query);
    if (query_stat != 0)
    {
        printLog("Mysql query error : %s\n", mysql_error(&g_conn));
        return -2;
    }


    sprintf(query, 
        "create table if not exists roominfo (  roominfo_number bigint(%d) unsigned NOT NULL auto_increment, \
                                                roominfo_status varchar(1) NOT NULL, \
                                                roominfo_subject varchar(%d) NOT NULL, \
                                                roominfo_key varchar(%d) NOT NULL, \
                                                roominfo_countmember bigint(10) NOT NULL default 0, \
                                                primary key (roominfo_number) \
                                            ) engine=InnoDB DEFAULT CHARSET=utf8",
        SIZE_ROOM_NUMBER, SIZE_ROOM_SUBJECT, SIZE_SECRET_KEY * 2);
    query_stat = mysql_query(g_connection, query);
    if (query_stat != 0)
    {
        printLog("Mysql query error : %s\n", mysql_error(&g_conn));
        return -2;
    }

    sprintf(query, 
        "create table if not exists roomuser (  roominfo_number bigint(%d) unsigned NOT NULL, \
                                                userinfo_id varchar(%d) NOT NULL, \
                                                foreign key (roominfo_number) \
                                                    references roominfo (roominfo_number) \
                                                    on delete cascade, \
                                                foreign key (userinfo_id) \
                                                    references userinfo (userinfo_id) \
                                                    on delete cascade, \
                                                primary key (roominfo_number, userinfo_id) \
                                            ) engine=InnoDB DEFAULT CHARSET=utf8",
        SIZE_ROOM_NUMBER, SIZE_ID);
    query_stat = mysql_query(g_connection, query);
    if (query_stat != 0)
    {
        printLog("Mysql query error : %s\n", mysql_error(&g_conn));
        return -2;
    }

/////////////////////////trigger///////////////////////////
    sprintf(query, "drop trigger if exists increase_member");
    query_stat = mysql_query(g_connection, query);
    if (query_stat != 0)
    {
        printLog("Mysql query error : %s\n", mysql_error(&g_conn));
        return -2;
    }
    sprintf(query,
        "create trigger increase_member after insert \
            on roomuser \
            for each row \
            begin \
                update roominfo set roominfo_countmember=roominfo_countmember+1 where roominfo_number=new.roominfo_number; \
            end");
    query_stat = mysql_query(g_connection, query);
    if (query_stat != 0)
    {
        printLog("Mysql query error : %s\n", mysql_error(&g_conn));
        return -2;
    }

    sprintf(query, "drop trigger if exists decrease_member");
    query_stat = mysql_query(g_connection, query);
    if (query_stat != 0)
    {
        printLog("Mysql query error : %s\n", mysql_error(&g_conn));
        return -2;
    }
    sprintf(query,
        "create trigger decrease_member after delete \
            on roomuser \
            for each row \
            begin \
                update roominfo set roominfo_countmember=roominfo_countmember-1 where roominfo_number=old.roominfo_number; \
                delete from roominfo where roominfo_number=old.roominfo_number and roominfo_countmember<1; \
            end");
    query_stat = mysql_query(g_connection, query);
    if (query_stat != 0)
    {
        printLog("Mysql query error : %s\n", mysql_error(&g_conn));
        return -2;
    }

    return dbViewAllTable();
}

int dbDropAllTable()
{
    int query_stat;

    query_stat = mysql_query(g_connection, 
        "drop table roomuser");
    if (query_stat != 0)
    {
        printLog("Mysql query error : %s\n", mysql_error(&g_conn));
        //return -2;
    }
    query_stat = mysql_query(g_connection, 
        "drop table userinfo");
    if (query_stat != 0)
    {
        printLog("Mysql query error : %s\n", mysql_error(&g_conn));
        //return -2;
    }
    query_stat = mysql_query(g_connection, 
        "drop table roominfo");
    if (query_stat != 0)
    {
        printLog("Mysql query error : %s\n", mysql_error(&g_conn));
        //return -2;
    }

    return 0;
}

/*************** END OF FILE **********************************************/

