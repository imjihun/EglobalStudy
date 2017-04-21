/* 2017-04-04 : created by JHLim */
/***************************************************************************
* Copyright (c) 2004-2016, eGlobal Systems, Co., Ltd.
* Seoul, Republic of Korea
* All Rights Reserved.
*
* Description : prototype.c
*		xxxx
*
***************************************************************************/

/*************** Header files *********************************************/
#include "encrypt.h"

/*************** New Data Types *******************************************/

/*************** Definitions / Macros  ************************************/

/*************** Global Variables *****************************************/
TYPE_SECRET_KEY g_nonce[8] = { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07 };
TYPE_SECRET_KEY g_iv[] = { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f };

/*************** Prototypes ***********************************************/

/*************** Function *************************************************/
/*
void view(char *buffer, int size)
{
    int i;

    for(i=0; i<size; i++)
    {
        printf("%c", *(buffer + i));
    }
    printf("\n");
}
*/
void makeKey(TYPE_SECRET_KEY *key)
{
    int i;

    srand(time(NULL));
    for (i = 0; i < SIZE_SECRET_KEY; i++)
    {
        key[i] = rand() % MAX_TYPE_SECRET_KEY;
    }
}

int encryptBuffer(TYPE_SECRET_KEY *key, char *buffer, int length_buffer, char *buffer_cipher)
{
    int retval = 0;
    retval = AES_CBC_encrypt_buffer(
        // output
        (TYPE_SECRET_KEY *)buffer_cipher,
        // input
        (TYPE_SECRET_KEY *)buffer,
        // length_buffer
        length_buffer,
        // key
        key,
        // iv
        g_iv, 128);
    /*
    aesCtrEncryptBuffer(
    // output
    (TYPE_SECRET_KEY *)buffer_cipher,
    // input
    (TYPE_SECRET_KEY *)buffer,
    // length_buffer
    length_buffer,
    // key
    g_arr_room_info[room_number].key,
    // nonce
    g_nonce);
    retval = length_buffer;
    */
    return retval;
}

int decryptBuffer(TYPE_SECRET_KEY *key, char *buffer_cipher, int length_buffer_cipher, char *buffer_ret)
{
    int retval = 0;

    retval = AES_CBC_decrypt_buffer(
        // output
        (TYPE_SECRET_KEY *)buffer_ret,
        // input
        (TYPE_SECRET_KEY *)buffer_cipher,
        // length_buffer
        length_buffer_cipher,
        // key
        key,
        // iv, keysize
        g_iv, 128);
    /*
    aesCtrEncryptBuffer(
    // output
    (TYPE_SECRET_KEY *)buffer_ret,
    // input
    (TYPE_SECRET_KEY *)buffer_cipher,
    // length_buffer
    length_buffer_cipher,
    // key
    g_arr_room_info[room_number].key,
    // nonce
    g_nonce);
    retval = length_buffer_cipher;
    */

    return retval;
}
/*************** END OF FILE **********************************************/



