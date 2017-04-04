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
static int encrypt_cbc(const char* fileName);
static void phex_n(uint8_t* str, int n);

/*************** Function *************************************************/
int main(int argc, char **argv)
{
  char fileName[256];
  int idx = 0;

  if(argc < 2)
  {
    printf("FileName for Encryption : ");
    fflush(stdin);
    fscanf(stdin, "%s", fileName);
    argv[1] = fileName;
    argc++;
  }

  for(idx = 1; idx < argc; idx++)
  {
    if(!encrypt_cbc(argv[idx]))
      printf("%s Encrypt Success!!\n", argv[idx]);
    else
      printf("%s Encrypt Fail!!\n", argv[idx]);
  }
  
  return 0;
}

static int encrypt_cbc(const char* fileName)
{
  uint8_t key[] = { 0x2b, 0x7e, 0x15, 0x16, 0x28, 0xae, 0xd2, 0xa6, 0xab, 0xf7, 0x15, 0x88, 0x09, 0xcf, 0x4f, 0x3c };
  uint8_t iv[]  = { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f };
  uint8_t buf_in[MAXBUF], buf_out[MAXBUF];
  size_t retval = 0, elemSize = sizeof(uint8_t), fileSize = 0;
  
  FILE *fp_in, *fp_out;
  char outFileName[512];

  sprintf(outFileName, "%s.enc", fileName);

  fp_in = fopen(fileName, "rb");
  if(fp_in == NULL) return -1;
  fp_out = fopen(outFileName, "wb");
  if(fp_out == NULL) return -1;

  fseek(fp_out, 16, SEEK_SET);
  while((retval = fread(buf_in, elemSize, MAXBUF, fp_in)) != 0)
  {
    //phex_n(buf_in, retval);

    AES_CBC_encrypt_buffer(buf_out, buf_in, retval, key, iv, 128);
    fwrite(buf_out, elemSize, retval % 16 != 0 ? (retval / 16 + 1) * 16 : retval, fp_out);

    fileSize += retval;
    //phex_n(buf_out, retval % 16 != 0 ? (retval / 16 + 1) * 16 : retval);
  }
  memcpy(buf_in, &fileSize, sizeof(fileSize));
  AES_CBC_encrypt_buffer(buf_out, buf_in, sizeof(fileSize), key, iv, 128);

  fseek(fp_out, 0, SEEK_SET);
  retval = fwrite(buf_out , elemSize, 16, fp_out);

  fclose(fp_out);
  fclose(fp_in);
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



// Enable both ECB and CBC mode. Note this can be done before including aes.h or at compile-time.
// E.g. with GCC by using the -D flag: gcc -c aes.c -DCBC=0 -DECB=1





