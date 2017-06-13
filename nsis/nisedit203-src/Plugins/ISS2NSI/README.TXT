==================
ISS2NSI v1.0a2
==================
ISS2NSI is a tool for convert Inno Setup script files into NSIS script files.


Usage:
==================
Program:
At the command prompt write: iss2nsi issscript.iss [nsiscript.nsi]
if nsiscript.nsi is no specified then the converted script will be 
saved with the same name of the issscript.iss but with extension .nsi.

NIS Edit Plugin:
1- Close HM NIS Edit.
2- Put hmne_iss2nsi.dll in the plugins directory 
   (Default $PROGRAMFILES\HMSoft\NIS Edit\Plugins)
3- Start HM NIS Edit.
4- You will found a new item in the tools menu called "Convert InnoSetup script".

NOTE: Before compile the converted script please check it, because 
complete compatibility is NOT available. I design this tool to convert my
Inno Setup scripts and is not tested with more complex scripts.

The source code is included if you want to contribute to the project
for make the converted script 100% compatible with the original. 
Please send my your modifications to ranametal@blistering.net
Note  that this project is a modification to the Inno Setup 3.0.6 
compiler and modifications to the Inno Setup source files are under 
the Inno Setup license (see: src\is-src\is-license.txt). 

Note: the files in the src\is-src directory are unmodified Inno Setup 
source files needed by the Compile.pas unit located in the src directory