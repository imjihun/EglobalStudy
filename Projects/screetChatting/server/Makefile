CC = gcc
LN = ln -f -s
RM = rm -rf
AR = ar
RANLIB = ranlib

SYS_LIB =

LIB_MYSQL = -rdynamic -L/usr/lib64/mysql -lmysqlclient -lz -lcrypt -lnsl -lm -L/usr/lib64 -lssl -lcrypto
INCLUDE = -I/usr/include/mysql
LIBS = $(SYS_LIB) $(LIB_MYSQL) -L/home/user1/lib -laes

CFLAGS = -c -Wall -O2 -g
OFLAGS = 
THREAD = -pthread
SRC_DIR = source
OBJ_DIR = obj

OBJS	=	$(OBJ_DIR)/printLog.o \
			$(OBJ_DIR)/connectDB.o \
			$(OBJ_DIR)/chattingLog.o \
			$(OBJ_DIR)/encrypt.o

OUT		=	mainServer log

.SUFFIXES:
.SUFFIXES: .o .c


all: $(OBJ_DIR) $(OBJS) $(OUT) cntldb

cntldb: $(OBJ_DIR)/cntldb.o $(OBJS)
	$(CC) $(OFLAGS) -o $@ $< $(LIBS) $(OBJS)

$(OBJ_DIR):
	mkdir $@

$(OBJ_DIR)/printLog.o : ../printLog/printLog.c
	$(CC) $(CFLAGS) $(INCLUDE) -o $@ $<

$(OBJ_DIR)/%.o : $(SRC_DIR)/%.c
	$(CC) $(CFLAGS) $(INCLUDE) -o $@ $<

% : $(OBJ_DIR)/%.o
	$(CC) $(OFLAGS) $(THREAD) -o $@ $< $(LIBS) $(OBJS)

clean:
	$(RM) $(OBJ_DIR) $(OUT)
