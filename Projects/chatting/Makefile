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

OUT		=	chattingClient
SUB_MAKE_DIR =	processDir threadDir

.SUFFIXES:
.SUFFIXES: .o .c


all: $(SUB_MAKE_DIR) $(OBJ_DIR) $(OUT) $(LOG_DIR)

proc: $(OUT)
	cd process; $(MAKE) exec
thr:
	cd thread; $(MAKE) exec

$(LOG_DIR):
	mkdir $@

$(OBJ_DIR):
	mkdir $@

$(OBJ_DIR)/%.o : $(SRC_DIR)/%.c
	$(CC) $(CFLAGS) $(INCLUDE) -o $@ $<

% : $(OBJ_DIR)/%.o
	$(CC) $(OFLAGS) $(THREAD) -o $@ $< $(LIBS)

processDir:
	cd process; $(MAKE)
threadDir:
	cd thread; $(MAKE)

clean:
	$(RM) $(OBJ_DIR) $(OUT)
	cd process; $(MAKE) clean
	cd thread; $(MAKE) clean
