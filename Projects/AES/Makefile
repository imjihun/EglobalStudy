CC = gcc
LN = ln -f -s
RM = rm -rf
AR = ar

SYS_LIB =

INCLUDE = ../lib
LIBS = $(SYS_LIB)
MYLIB_NAMES = aes
MYDLL_LINKFILENAME = lib$(MYLIB_NAMES).so
MYDLL_FULLNAMES = $(MYDLL_LINKFILENAME).1.0.1
LFLAGS = -fPIC

CFLAGS = -c -Wall -O2 -g
OFLAGS = 
THREAD = -pthread
OBJ_DIR = .

MAKE = make

all: lib_ AES_varTxtCheck_ AES_File_Encrypt_Decrypt_

.SUFFIXES:
.SUFFIXES: .o .c


lib_:
	cd lib; $(MAKE)
AES_varTxtCheck_:
	cd AES_varTxtCheck; $(MAKE)
AES_File_Encrypt_Decrypt_:
	cd AES_File_Encrypt_Decrypt; $(MAKE)

clean:
	cd lib; $(MAKE) clean
	cd AES_varTxtCheck; $(MAKE) clean
	cd AES_File_Encrypt_Decrypt; $(MAKE) clean
