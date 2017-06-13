unit CompInt;

{
  Inno Setup
  Copyright (C) 1998-2002 Jordan Russell
  Portions by Martijn Laan
  For conditions of distribution and use, see LICENSE.TXT.

  Compiler interface

  $jrsoftware: issrc/Projects/CompInt.pas,v 1.4 2002/11/09 19:45:41 jr Exp $
}

interface

uses
  Windows, SysUtils;

const
  { Constants passed in Code parameter of callback function }
  iscbReadScript = 1;      { Sent when compiler needs the next script line }
  iscbNotifyStatus = 2;    { Sent to notify the application of compiler status }
  iscbNotifyIdle = 3;      { Sent at various intervals during the compilation }
  iscbNotifySuccess = 4;   { Sent when compilation succeeds }
  iscbNotifyError = 5;     { Sent when compilation fails or is aborted by the
                             application }

  { Return values for callback function }
  iscrSuccess = 0;         { Return this for compiler to continue }
  iscrRequestAbort = 1;    { Return this to abort compilation }

  { Return values for ISDllCompileScript }
  isceNoError = 0;         { Successful }
  isceInvalidParam = 1;    { Bad parameters passed to function }
  isceCompileFailure = 2;  { There was an error compiling or it was aborted
                             by the application }

type
  { TCompilerCallbackData is a record passed to the callback function. The
    fields which you may access vary depending on what Code was passed to the
    callback function. }
  TCompilerCallbackData = record
    case Integer of
      iscbReadScript: (
        Reset: BOOL;          { [in] True if it needs the application to return
                                to the beginning of the script. In other words,
                                LineRead must be the first line of the script. }
        LineRead: PChar);     { [out] Application returns pointer to the next
                                line it reads, or a NULL pointer if the end of
                                file is reached. Application is responsible for
                                allocating a buffer to hold the line; LineRead
                                is initially NULL when the callback function
                                is called. }

      iscbNotifyStatus: (
        StatusMsg: PChar);    { [in] Contents of status message. }

      iscbNotifySuccess: (
        OutputExeFilename: PChar;  { [in] The name of the resulting setup.exe }
        DebugInfo: Pointer;        { [in] Debug info (new in 3.0.0.1) }
        DebugInfoSize: Cardinal);  { [in] Size of debug info (new in 3.0.0.1) }

      iscbNotifyError: (
        ErrorMsg: PChar;      { [in] The error message, or NULL if compilation
                                was aborted by the application. }
        ErrorFilename: PChar; { [in] Filename in which the error occured. This
                                is NULL if the file is the main script. }
        ErrorLine: Integer);  { [in] The line number the error occured on.
                                Zero if the error doesn't apply to any
                                particular line. }
  end;

  TCompilerCallbackProc = function(Code: Integer;
    var Data: TCompilerCallbackData; AppData: Longint): Integer; stdcall;

  TCompileScriptParams = record
    Size: Cardinal;       { [in] Set to SizeOf(TCompileScriptParams). }
    CompilerPath: PChar;  { [in] The "compiler:" directory. This is the
                            directory which contains the *.e32 files. If this
                            is set to NULL, the compiler will use the directory
                            containing the compiler DLL/EXE. }
    SourcePath: PChar;    { [in] The default source directory, and directory to
                            look in for #include files. Normally, this is
                            the directory containing the script file. This
                            cannot be NULL. }
    CallbackProc: TCompilerCallbackProc;
                          { [in] The callback procedure which the compiler calls
                            to read the script and for status notification. }
    AppData: Longint;     { [in] Application-defined. AppData is passed to the
                            callback function. }
  end;

  PCompilerVersionInfo = ^TCompilerVersionInfo;
  TCompilerVersionInfo = record
    Title: PChar;          { Name of compiler engine - 'Inno Setup' }
    Version: PChar;        { Version number text }
    BinVersion: Cardinal;  { Version number as an integer }
  end;

const
  ISCmplrDLL = 'ISCmplr.dll';

{ The ISDllCompileScript function begins compilation of a script. See the above
  description of the TCompileScriptParams record. Return value is one of the
  isce* constants. }
function ISDllCompileScript (const Params: TCompileScriptParams): Integer;
  stdcall; external ISCmplrDLL;

{ The ISDllGetVersion returns a pointer to a TCompilerVersionInfo record which
  contains information about the compiler version. }
function ISDllGetVersion: PCompilerVersionInfo; stdcall; external ISCmplrDLL;

implementation

end.
