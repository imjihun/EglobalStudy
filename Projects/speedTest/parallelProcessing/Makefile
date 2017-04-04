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

OUT		=	threadServer processServer

.SUFFIXES:
.SUFFIXES: .o .c

	
all: $(OBJ_DIR) $(OUT)

exec: $(OUT)
	./processServer&
	./threadServer&


$(OBJ_DIR) :
	mkdir $@

$(OBJ_DIR)/%.o : $(SRC_DIR)/%.c
	$(CC) $(CFLAGS) $(INCLUDE) -o $@ $<

% : $(OBJ_DIR)/%.o
	$(CC) $(OFLAGS) $(THREAD) -o $@ $< $(LIBS)

clean:
	$(RM) $(OBJ_DIR) $(OUT)
