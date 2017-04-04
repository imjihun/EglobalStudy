CC = gcc
LN = ln -f -s
RM = rm -rf
AR = ar

SYS_LIB =

INCLUDE = ../lib
LIBS = $(SYS_LIB)

LIB_NAME = aes
LIB_DIR = ~/lib
LIB_LINKFILENAME = lib$(LIB_NAME).so
LIB_FULLNAMES = $(LIB_LINKFILENAME).1.0.1
LFLAGS = -fPIC

CFLAGS = -c -Wall -O2 -g
OFLAGS = 
THREAD = -pthread
OBJ_DIR = .

all: $(LIB_LINKFILENAME)

.SUFFIXES:
.SUFFIXES: .o .c

$(OBJ_DIR)/%.o : %.c
	$(CC) $(CFLAGS) $(LFLAGS) $(INCLUDE) -o $@ $<

$(LIB_LINKFILENAME) : $(LIB_FULLNAMES)
	$(LN) $(LIB_DIR)/$(LIB_FULLNAMES) $(LIB_DIR)/$(LIB_LINKFILENAME)

$(LIB_FULLNAMES) : $(LIB_NAME).o
	$(CC) -shared -o $(LIB_FULLNAMES) $(LIB_NAME).o
	cp $(LIB_FULLNAMES) $(LIB_DIR)/

clean:
	$(RM) $(OBJ_DIR)/*.o $(LIB_FULLNAMES) $(LIB_LINKFILENAME)
