/* 2017-04-04 : created by JHLim */
/***************************************************************************
* Copyright (c) 2004-2016, eGlobal Systems, Co., Ltd.
* Seoul, Republic of Korea
* All Rights Reserved.

* Description : prototype.h
*		head file for *.c : XXX
*
***************************************************************************/
#ifndef _PROTO_TYPE_H
#define _PROTO_TYPE_H

/*************** Header files *********************************************/
#include "../../aes/source/aes.h"
#include "../../macroFile.h"
#include <stdlib.h>
#include <stdio.h>
#include <time.h>

/*************** Assertions ***********************************************/

/*************** Macros ***************************************************/

/*************** Definitions / Macros *************************************/

/*************** New Data Types *******************************************/

/*************** Constant *************************************************/

/*************** typedef  *************************************************/

/*************** Prototypes ***********************************************/
void makeKey(TYPE_SECRET_KEY *key);
int decryptBuffer(TYPE_SECRET_KEY *key, char *buffer_cipher, int length_buffer_cipher, char *buffer_ret);
int encryptBuffer(TYPE_SECRET_KEY *key, char *buffer, int length_buffer, char *buffer_cipher);

#endif	/*_PROTO_TYPE_H */
/*************** END OF FILE **********************************************/


