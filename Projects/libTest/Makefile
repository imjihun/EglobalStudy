CC = gcc
LN = ln -f -s
RM = rm -rf
AR = ar
RANLIB = ranlib

SYS_LIB =

INCLUDE =
LIBS = $(SYS_LIB)
MYLIB_SRCS = printhello.c
MYLIB_OBJS = $(MYLIB_SRCS:%.c=%.o)
MYLIB_NAMES = printhello
MYDLL_LINKFILENAME = $(MYLIB_NAMES).so
MYDLL_FULLNAMES = $(MYLIB_NAMES).so.1.0.1
MYLIB_FULLNAMES = lib$(MYLIB_NAMES).a
MYLIB_DIRS = -L.
MYLIB_OPTION = $(MYLIB_DIRS)/ -l$(MYLIB_NAMES)
LFLAGS = -fPIC

CFLAGS = -c -Wall -O2 -g
OFLAGS = 
THREAD = -pthread
OBJ_DIR = .

OBJS 	=	$(OBJ_DIR)/main.o

all: $(MYDLL_LINKFILENAME) $(MYLIB_FULLNAMES) main

.SUFFIXES:
.SUFFIXES: .o .c

$(OBJ_DIR)/%.o : %.c
	$(CC) $(CFLAGS) $(INCLUDE) -o $@ $<

main : $(OBJS)
	$(CC) $(OFLAGS) $(THREAD) -o $@ $(OBJS) $(LIBS) $(MYLIB_OPTION)

$(MYLIB_FULLNAMES) : $(MYLIB_OBJS)
	$(AR) rcv $@ $(MYLIB_OBJS)
	$(RANLIB) $@

$(MYDLL_LINKFILENAME) : $(MYDLL_FULLNAMES)
	$(LN) $(MYDLL_FULLNAMES) $(MYDLL_LINKFILENAME)

$(MYDLL_FULLNAMES) : $(MYLIB_OBJS)
	$(CC) -shared -o $(MYDLL_FULLNAMES) $(MYLIB_OBJS)

$(MYLIB_OBJS) : $(MYLIB_SRCS)
	$(CC) $(LFLAGS) -c $(MYLIB_SRCS)

clean:
	$(RM) $(OBJ_DIR)/*.o main $(MYDLL_FULLNAMES) $(MYLIB_FULLNAMES) $(MYDLL_LINKFILENAME)
