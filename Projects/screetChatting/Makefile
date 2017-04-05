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
LOG_DIR = log

OBJS	=		

OUT		=	
SUB_MAKE_DIR =	clientDir serverDir

.SUFFIXES:
.SUFFIXES: .o .c


all: $(SUB_MAKE_DIR) $(OBJ_DIR) $(OUT) $(LOG_DIR)
	cp ./server/serverMain ./
	cp ./client/testEchoClient ./

proc: $(OUT)
	cd client; $(MAKE) exec
thr:
	cd server; $(MAKE) exec

$(LOG_DIR):
	mkdir $@

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

clean:
	$(RM) $(OBJ_DIR) $(OUT) serverMain testEchoClient
	cd client; $(MAKE) clean
	cd server; $(MAKE) clean
