CC = gcc
LN = ln -f -s
RM = rm -rf
AR = ar
RANLIB = ranlib

SYS_LIB =

INCLUDE = -I/usr/include/mysql
LIB_MYSQL = -rdynamic -L/usr/lib64/mysql -lmysqlclient -lz -lcrypt -lnsl -lm -L/usr/lib64 -lssl -lcrypto
LIBS = $(SYS_LIB) $(LIB_MYSQL)

CFLAGS = -c -Wall -O2 -g
OFLAGS = 
THREAD = -pthread
SRC_DIR = source
OBJ_DIR = obj
LOG_DIR = log

OBJS	=	
OUT		=	test

.SUFFIXES:
.SUFFIXES: .o .c


all: $(OBJ_DIR) $(OBJS) $(OUT) $(LOG_DIR)

$(OBJ_DIR):
	mkdir $@
$(LOG_DIR):
	mkdir $@

$(OBJ_DIR)/%.o : $(SRC_DIR)/%.c
	$(CC) $(CFLAGS) $(INCLUDE) -o $@ $<

% : $(OBJ_DIR)/%.o
	$(CC) $(OFLAGS) $(THREAD) -o $@ $< $(LIBS) $(OBJS)

clean:
	$(RM) $(OBJ_DIR) $(OUT) $(LOG_DIR)
