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

OUT		=	echoClient speedTestClient
SUB_MAKE_DIR =	parallelProcessing_ multiPlexing_

.SUFFIXES:
.SUFFIXES: .o .c


all: $(OBJ_DIR) $(LOG_DIR) $(OUT) parallelProcessingDir multiPlexingDir

test: $(OUT) $(LOG_DIR)
	cd parallelProcessing; $(MAKE) exec
	cd multiPlexing; $(MAKE) exec
	$(RM) $(LOG_DIR)/*log.txt
	./speedTestClient 127.0.0.1 9000 &
	./speedTestClient 127.0.0.1 9100 &
	./speedTestClient 127.0.0.1 9200 &
	./speedTestClient 127.0.0.1 9300 &
	./speedTestClient 127.0.0.1 9400 &

stop:
	pkill -9 Server

$(LOG_DIR):
	mkdir $@

$(OBJ_DIR):
	mkdir $@

$(OBJ_DIR)/%.o : $(SRC_DIR)/%.c
	$(CC) $(CFLAGS) $(INCLUDE) -o $@ $<

% : $(OBJ_DIR)/%.o
	$(CC) $(OFLAGS) $(THREAD) -o $@ $< $(LIBS)

parallelProcessingDir:
	cd parallelProcessing; $(MAKE)
multiPlexingDir:
	cd multiPlexing; $(MAKE)

clean:
	$(RM) $(OBJ_DIR) $(OUT)
	cd parallelProcessing; $(MAKE) clean
	cd multiPlexing; $(MAKE) clean
