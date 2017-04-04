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

OBJS 	=	$(OBJ_DIR)/varTxtCheck.o

all: varTxtCheck

.SUFFIXES:
.SUFFIXES: .o .c

$(OBJ_DIR)/%.o : %.c
	$(CC) $(CFLAGS) $(INCLUDE) -o $@ $<

varTxtCheck : $(OBJS)
	$(CC) $(OFLAGS) $(THREAD) -o $@ $(OBJS) $(LIBS) $(MYLIB_OPTION)

clean:
	$(RM) $(OBJ_DIR)/*.o varTxtCheck $(MYDLL_FULLNAMES) $(MYLIB_FULLNAMES) $(MYDLL_LINKFILENAME)
