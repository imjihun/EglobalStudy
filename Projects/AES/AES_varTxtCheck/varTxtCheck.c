/***************************************************************************
* Copyright (c) 2004-2016, eGlobal Systems, Co., Ltd.
* Seoul, Republic of Korea
* All Rights Reserved.
*
* Description : test.c
*   xxxx
*
***************************************************************************/

/*************** Header files *********************************************/
#include <stdio.h>
#include <string.h>
#include <stdint.h>
#include <stdlib.h>
#include "aes.h"
/*************** New Data Types *******************************************/

/*************** Definitions / Macros  ************************************/

/*************** Global Variables *****************************************/

/*************** Prototypes ***********************************************/
static void phex(uint8_t* str);
static void phex_n(uint8_t* str, int n);
static int VarTxt_Known_Answer_Test_Values(int keySize);
static int cmpVal(const char* str, const uint8_t* val, int size);

/*************** Function *************************************************/
int main(int argc, char** argv)
{
    int keySize = 128;
    int retval = 0;

    if(( retval = VarTxt_Known_Answer_Test_Values(keySize)) != 0)
      printf("[KeySize = %d] varTxt Error [%d]\n\n", keySize, retval);
    else
      printf("[KeySize = %d] varTxt Success\n\n", keySize);

    keySize = 192;
    if(( retval = VarTxt_Known_Answer_Test_Values(keySize)) != 0)
      printf("[KeySize = %d] varTxt Error [%d]\n\n", keySize, retval);
    else
      printf("[KeySize = %d] varTxt Success\n\n", keySize);

    keySize = 256;
    if(( retval = VarTxt_Known_Answer_Test_Values(keySize)) != 0)
      printf("[KeySize = %d] varTxt Error [%d]\n\n", keySize, retval);
    else
      printf("[KeySize = %d] varTxt Success\n\n", keySize);

    return 0;
}
static int cmpVal(const char* str, const uint8_t* val, int size)
{
  int idx_str = 0, idx_val = 0;
  int tmp = 0;
  for(; idx_val < size; idx_str+=2, idx_val++)
  {
    if(str[idx_str] >= '0' && str[idx_str] <= '9')
      tmp = str[idx_str] - '0';
    else if(str[idx_str] >= 'a' && str[idx_str] <= 'z')
      tmp = str[idx_str] - 'a' + 10;

    tmp = tmp << 4;

    if(str[idx_str + 1] >= '0' && str[idx_str + 1] <= '9')
      tmp += str[idx_str + 1] - '0';
    else if(str[idx_str + 1] >= 'a' && str[idx_str + 1] <= 'z')
      tmp += str[idx_str + 1] - 'a' + 10;

    if(tmp != val[idx_val])
      return -1;
  }
  return 0;
}
static int VarTxt_Known_Answer_Test_Values(int keySize)
{    
  FILE* fp;
  char fileName[256], fBuf[1024];
  // Example of more verbose verification
  uint8_t i, j, buf[16];

  // 192bit key
  uint8_t key[32] =        { (uint8_t) 0x0, (uint8_t) 0x0, (uint8_t) 0x0, (uint8_t) 0x0, (uint8_t) 0x0, (uint8_t) 0x0, (uint8_t) 0x0, (uint8_t) 0x0
                            , (uint8_t) 0x0, (uint8_t) 0x0, (uint8_t) 0x0, (uint8_t) 0x0, (uint8_t) 0x0, (uint8_t) 0x0, (uint8_t) 0x0, (uint8_t) 0x0
                            , (uint8_t) 0x0, (uint8_t) 0x0, (uint8_t) 0x0, (uint8_t) 0x0, (uint8_t) 0x0, (uint8_t) 0x0, (uint8_t) 0x0, (uint8_t) 0x0
                            , (uint8_t) 0x0, (uint8_t) 0x0, (uint8_t) 0x0, (uint8_t) 0x0, (uint8_t) 0x0, (uint8_t) 0x0, (uint8_t) 0x0, (uint8_t) 0x0  };
  // 128bit text
  uint8_t plain_text[16] = { (uint8_t) 0x80, (uint8_t) 0x0, (uint8_t) 0x0, (uint8_t) 0x0, (uint8_t) 0x0, (uint8_t) 0x0, (uint8_t) 0x0, (uint8_t) 0x0, (uint8_t) 0x0, (uint8_t) 0x0, (uint8_t) 0x0, (uint8_t) 0x0, (uint8_t) 0x0, (uint8_t) 0x0, (uint8_t) 0x0, (uint8_t) 0x0 };

  uint8_t testVar[8] = { (uint8_t) 0x80, (uint8_t) 0xc0, (uint8_t) 0xe0, (uint8_t) 0xf0, (uint8_t) 0xf8, (uint8_t) 0xfc, (uint8_t) 0xfe, (uint8_t) 0xff };


  printf("ECB encrypt verbose:\n");
  printf("KeySize = %d\n", keySize);
  printf("key: ");
  phex_n(key, keySize / 8);

  sprintf(fileName, "varTxt_%d.txt", keySize);
  fp = fopen(fileName, "r");
  if(fp == NULL)
    return -3;

  for(i=0; i<16; i++)
  {
    for(j=0; j<8; j++)
    {
      plain_text[i] = testVar[j];
      memset(buf, 0, 16);

      AES_ECB_encrypt(plain_text, key, buf, keySize);

      if(fgets(fBuf, 1024, fp) == NULL)
        return -2;
      if(cmpVal(fBuf + 32 + 1, buf, 16) != 0)
      {
        phex(plain_text);
        phex(buf);
        printf("%s", fBuf + 32 + 1);
        return -1;
      }

    }
  }

  fclose(fp);
  return 0;
}

// prints string as hex
static void phex(uint8_t* str)
{
    unsigned char i;
    for(i = 0; i < 16; ++i)
        printf("%.2x", str[i]);
    printf("\n");
}
static void phex_n(uint8_t* str, int n)
{
    unsigned char i;
    for(i = 0; i < n; ++i)
        printf("%.2x", str[i]);
    printf("\n");
}

/*************** END OF FILE **********************************************/