CC = gcc
LN = ln -f -s
RM = rm -rf
AR = ar

SYS_LIB =

INCLUDE = -I../lib/
LIBS = $(SYS_LIB)
MYLIB_NAMES = aes
MYLIB_DIRS = -L../lib
MYLIB_OPTION = $(MYLIB_DIRS)/ -l$(MYLIB_NAMES)
LFLAGS = -fPIC

CFLAGS = -c -Wall -O2 -g
OFLAGS = 
THREAD = -pthread
OBJ_DIR = .

OBJS 	=	$(OBJ_DIR)/cmpfile.o\
			$(OBJ_DIR)/fileDecryption.o\
			$(OBJ_DIR)/fileEncryption.o

all: enc dec cmpfile

.SUFFIXES:
.SUFFIXES: .o .c

$(OBJ_DIR)/%.o : %.c
	$(CC) $(CFLAGS) $(INCLUDE) -o $@ $<

enc : fileEncryption.o
	$(CC) $(OFLAGS) $(THREAD) -o $@ $< $(LIBS) $(MYLIB_OPTION)
dec : fileDecryption.o
	$(CC) $(OFLAGS) $(THREAD) -o $@ $< $(LIBS) $(MYLIB_OPTION)
cmpfile : cmpfile.o
	$(CC) $(OFLAGS) $(THREAD) -o $@ $<

clean:
	$(RM) $(OBJ_DIR)/*.o enc dec cmpfile
