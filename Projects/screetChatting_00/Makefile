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
SUB_MAKE_DIR =	aesDir clientDir serverDir

.SUFFIXES:
.SUFFIXES: .o .c


all: $(SUB_MAKE_DIR) $(OBJ_DIR) $(OUT) $(LOG_DIR)
	cp ./server/mainServer ./
	cp ./client/testClient ./

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
aesDir:
	cd aes; $(MAKE)


clean:
	$(RM) $(OBJ_DIR) $(OUT) mainServer testClient $(LOG_DIR)
	cd client; $(MAKE) clean
	cd server; $(MAKE) clean
