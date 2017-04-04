#CC           = avr-gcc
#CFLAGS       = -Wall -mmcu=atmega16 -Os -Wl,-Map,fileEncryption.map
#OBJCOPY      = avr-objcopy
CC           = gcc
CFLAGS       = -Wall -Os
OBJCOPY      = objcopy

.SILENT:
.PHONY:  lint clean


all : enc dec

fileDecryption.o : fileDecryption.c
	# compiling fileDecryption.c
	$(CC) $(CFLAGS) -c fileDecryption.c -o fileDecryption.o

fileEncryption.o : fileEncryption.c
	# compiling fileEncryption.c
	$(CC) $(CFLAGS) -c fileEncryption.c -o fileEncryption.o

aes.o : aes.h aes.c
	# compiling aes.c
	$(CC) $(CFLAGS) -c aes.c -o aes.o

dec : aes.o fileDecryption.o
	# linking object code to binary
	$(CC) $(CFLAGS) aes.o fileDecryption.o -o dec

enc : aes.o fileEncryption.o
	# linking object code to binary
	$(CC) $(CFLAGS) aes.o fileEncryption.o -o enc

clean:
	rm -f *.OBJ *.LST *.o *.gch *.out *.hex *.map enc dec

