/* 2016-11-10 : created by JMKim */
/***************************************************************************
* Copyright (c) 2004-2016, eGlobal Systems, Co., Ltd.
* Seoul, Republic of Korea
* All Rights Reserved.
*
* Description : co_base.c
*   xxxx
*
***************************************************************************/

/*************** Header files *********************************************/
#include <stdio.h>
#include <string.h>
#include <stdint.h>
#include "aes.h"
/*************** New Data Types *******************************************/

/*************** Definitions / Macros  ************************************/
#define MAXBUF 4096
/*************** Global Variables *****************************************/

/*************** Prototypes ***********************************************/
static int decrypt_cbc(const char* fileName);
static void phex_n(uint8_t* str, int n);
/*************** Function *************************************************/
int main(int argc, char **argv)
{
  char fileName[256];
  int idx = 0;

  if(argc < 2)
  {
    printf("FileName for Decryption : ");
    fflush(stdin);
    fscanf(stdin, "%s", fileName);
    argv[1] = fileName;
    argc++;
  }

  for(idx = 1; idx < argc; idx++)
  {
    if(!decrypt_cbc(argv[idx]))
      printf("%s Decrypt Success!!\n", argv[idx]);
    else
      printf("%s Decrypt Fail!!\n", argv[idx]);
  }
  
  return 0;
}

static int decrypt_cbc(const char* fileName)
{
  uint8_t key[] = { 0x2b, 0x7e, 0x15, 0x16, 0x28, 0xae, 0xd2, 0xa6, 0xab, 0xf7, 0x15, 0x88, 0x09, 0xcf, 0x4f, 0x3c };
  uint8_t iv[]  = { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f };
  uint8_t buf_in[MAXBUF], buf_out[MAXBUF];
  size_t retval = 0, elemSize = sizeof(uint8_t), fileSize = 0, readedSize = 0;

  FILE *fp_in, *fp_out;
  char outFileName[512];

  sprintf(outFileName, "%s.dec", fileName);

  fp_in = fopen(fileName, "rb");
  if(fp_in == NULL) return -1;
  fp_out = fopen(outFileName, "wb");
  if(fp_out == NULL) return -1;

  while((retval = fread(buf_in, elemSize, MAXBUF, fp_in)) != 0)
  {
    //phex_n(buf_in, retval);
    
    int outputSize = AES_CBC_decrypt_buffer(buf_out, buf_in, retval, key, iv, 128);

    fwrite(buf_out, elemSize, outputSize, fp_out);
    readedSize += retval;

    /*
    aesCtrEncryptBuffer(buf_out, buf_in, retval, key, iv);
    fwrite(buf_out, elemSize, retval, fp_out);
    readedSize += retval;
    */
    //phex_n(buf_out, retval);
  }
  fclose(fp_in);
  fclose(fp_out);
  return 0;
}
static void phex_n(uint8_t* str, int n)
{
    unsigned char i;
    for(i = 0; i < n; ++i)
        printf("%.2x", str[i]);
    printf("\n");
}

/*************** END OF FILE **********************************************/







