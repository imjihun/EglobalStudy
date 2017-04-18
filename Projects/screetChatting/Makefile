CC = gcc
LN = ln -f -s
RM = rm -rf
AR = ar
RANLIB = ranlib

SYS_LIB =

INCLUDE =
LIBS = $(SYS_LIB)

CFLAGS = -c -Wall -O2 -g
OFLAGS = 
THREAD = -pthread
SRC_DIR = source
OBJ_DIR = obj

OBJS	=		

OUT		=	
SUB_MAKE_DIR =	aesDir clientDir serverDir

.SUFFIXES:
.SUFFIXES: .o .c


all: $(SUB_MAKE_DIR) $(OBJ_DIR) $(OUT) serverLogs
	cp ./server/mainServer ./
	cp ./server/log ./
	cp ./client/testClient ./
	cp ./server/cntldb ./

serverLogs:
	mkdir serverLogs

proc: $(OUT)
	cd client; $(MAKE) exec
thr:
	cd server; $(MAKE) exec

$(OBJ_DIR):
	mkdir $@

$(OBJ_DIR)/%.o : $(SRC_DIR)/%.c
	$(CC) $(CFLAGS) $(INCLUDE) -o $@ $<

% : $(OBJ_DIR)/%.o
	$(CC) $(OFLAGS) $(THREAD) -o $@ $< $(LIBS)

clientDir:
	cd client; $(MAKE)
serverDir:
	cd server; $(MAKE)
aesDir:
	cd aes; $(MAKE)


clean:
	$(RM) $(OBJ_DIR) $(OUT) mainServer testClient
	cd client; $(MAKE) clean
	cd server; $(MAKE) clean
	cd aes; $(MAKE) clean
