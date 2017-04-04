CC = gcc
LN = ln -f -s
RM = rm -rf
AR = ar
RANLIB = ranlib

SYS_LIB = 

INCLUDE =
LIBS = $(SYS_LIB)

CFLAGS = -c -Wall -O2 -g -fPIC
OFLAGS = -shared
THREAD = -pthread
SRC_DIR = source
OBJ_DIR = obj
LIB_DIR_LOCAL = ./lib
LIB_DIR = ~/lib

LIB_NAME = printLog
OUT		=	$(LIB_DIR)/lib$(LIB_NAME).so.1.0.1\
			$(LIB_DIR)/lib$(LIB_NAME).so


.SUFFIXES: .so
.SUFFIXES: .o .c

all: $(LIB_DIR) $(OBJ_DIR) $(LIB_DIR_LOCAL) $(OUT)

$(OBJ_DIR):
	mkdir $@

$(LIB_DIR):
	mkdir $@

$(LIB_DIR_LOCAL):
	mkdir $@

$(OBJ_DIR)/$(LIB_NAME).o : $(SRC_DIR)/$(LIB_NAME).c
	$(CC) $(CFLAGS) $(INCLUDE) -o $@ $<

$(LIB_DIR_LOCAL)/lib$(LIB_NAME).so.1.0.1 : $(OBJ_DIR)/$(LIB_NAME).o
	$(CC) $(OFLAGS) $(THREAD) -o $@ $< $(LIBS)

$(LIB_DIR)/lib$(LIB_NAME).so.1.0.1 : $(LIB_DIR_LOCAL)/lib$(LIB_NAME).so.1.0.1 $(LIB_DIR)
	cp $< $@

$(LIB_DIR)/lib$(LIB_NAME).so : $(LIB_DIR)/lib$(LIB_NAME).so.1.0.1
	$(LN) $< $@





clean:
	$(RM) $(OBJ_DIR) $(LIB_DIR_LOCAL) $(OUT)
