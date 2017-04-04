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

OBJS	=	$(OBJ_DIR)/printLog.o
OUT		=	processManager processMain


.SUFFIXES:
.SUFFIXES: .o .c


all: $(OBJ_DIR) $(OBJS) $(OUT)

exec: $(OUT)
	./processManager&

$(OBJ_DIR) :
	mkdir $@

$(OBJ_DIR)/%.o : $(SRC_DIR)/%.c
	$(CC) $(CFLAGS) $(INCLUDE) -o $@ $<

$(OBJS): ../printLog/printLog.c
	$(CC) $(CFLAGS) $(INCLUDE) -o $@ $<

% : $(OBJ_DIR)/%.o $(OBJS)
	$(CC) $(OFLAGS) $(THREAD) -o $@ $< $(LIBS) $(OBJS)

clean:
	$(RM) $(OBJ_DIR) $(OUT)
