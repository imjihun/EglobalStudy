CC = gcc
LN = ln -f -s
RM = rm -rf
AR = ar

SYS_LIB =

INCLUDE = ../lib
LIBS = $(SYS_LIB)
MYLIB_NAMES = aes
MYDLL_LINKFILENAME = lib$(MYLIB_NAMES).so
MYDLL_FULLNAMES = $(MYDLL_LINKFILENAME).1.0.1
LFLAGS = -fPIC

CFLAGS = -c -Wall -O2 -g
OFLAGS = 
THREAD = -pthread
OBJ_DIR = .

all: $(MYDLL_LINKFILENAME)
	pwd > /etc/ld.so.conf.d/aes.conf
	ldconfig

.SUFFIXES:
.SUFFIXES: .o .c

$(OBJ_DIR)/%.o : %.c
	$(CC) $(CFLAGS) $(LFLAGS) $(INCLUDE) -o $@ $<

$(MYDLL_LINKFILENAME) : $(MYDLL_FULLNAMES)
	$(LN) $(MYDLL_FULLNAMES) $(MYDLL_LINKFILENAME)

$(MYDLL_FULLNAMES) : $(MYLIB_NAMES).o
	$(CC) -shared -o $(MYDLL_FULLNAMES) $(MYLIB_NAMES).o

clean:
	$(RM) $(OBJ_DIR)/*.o $(MYDLL_FULLNAMES) $(MYDLL_LINKFILENAME)
