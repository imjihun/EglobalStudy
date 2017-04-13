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

#include "test.h"
/*************** New Data Types *******************************************/

/*************** Definitions / Macros  ************************************/

/*************** Global Variables *****************************************/

MYSQL conn;
MYSQL *connection = NULL;
/*************** Prototypes ***********************************************/



/*************** Function *************************************************/


int insertId(char* id)
{
    char query[SIZE_QUERY];
    int query_stat;

    sprintf(query, "insert into userinfo (userinfo_id) value('%s')", id);
    query_stat = mysql_query(connection, query);
    if (query_stat != 0)
    {
        fprintf(stderr, "Mysql query error : %s\n", mysql_error(&conn));
        return -2;
    }
}


int dbOpen()
{
    mysql_init(&conn);

    connection = mysql_real_connect(&conn, DB_HOST,
                                    DB_USER, DB_PASS,
                                    DB_NAME, 3306,
                                    (char *)NULL, 0);

    if (connection == NULL)
    {
        fprintf(stderr, "Mysql connection error : %s", mysql_error(&conn));
        return -2;
    }
    return 0;
}

int dbClose()
{
    mysql_close(connection);
    return 0;
}
int createAllTable()
{
    int query_stat;

    query_stat = mysql_query(connection, 
        "create table userinfo (    userinfo_id varchar(16) NOT NULL,\
                                    primary key (userinfo_id)\
                                ) engine=InnoDB DEFAULT CHARSET=utf8");
    if (query_stat != 0)
    {
        fprintf(stderr, "Mysql query error : %s\n", mysql_error(&conn));
        return -2;
    }

    query_stat = mysql_query(connection, 
        "create table roominfo (    roominfo_number bigint(20) unsigned NOT NULL,\
                                    roominfo_key varchar(16) ,\
                                    primary key (roominfo_number)\
                                ) engine=InnoDB DEFAULT CHARSET=utf8");
    if (query_stat != 0)
    {
        fprintf(stderr, "Mysql query error : %s\n", mysql_error(&conn));
        return -2;
    }

    query_stat = mysql_query(connection, 
        "create table roomuser (    userinfo_id varchar(16) NOT NULL,\
                                    roominfo_number bigint(20) unsigned NOT NULL,\
                                    foreign key (userinfo_id) \
                                        references userinfo (userinfo_id) \
                                        on delete cascade, \
                                    foreign key (roominfo_number) \
                                        references roominfo (roominfo_number) \
                                        on delete cascade \
                                ) engine=InnoDB DEFAULT CHARSET=utf8");
    if (query_stat != 0)
    {
        fprintf(stderr, "Mysql query error : %s\n", mysql_error(&conn));
        return -2;
    }

    return 0;
}

int deleteAllTable()
{
    int query_stat;

    query_stat = mysql_query(connection, 
        "drop table roomuser");
    if (query_stat != 0)
    {
        fprintf(stderr, "Mysql query error : %s\n", mysql_error(&conn));
        //return -2;
    }
    query_stat = mysql_query(connection, 
        "drop table userinfo");
    if (query_stat != 0)
    {
        fprintf(stderr, "Mysql query error : %s\n", mysql_error(&conn));
        //return -2;
    }
    query_stat = mysql_query(connection, 
        "drop table roominfo");
    if (query_stat != 0)
    {
        fprintf(stderr, "Mysql query error : %s\n", mysql_error(&conn));
        //return -2;
    }

    return 0;
}

/*************** END OF FILE **********************************************/

