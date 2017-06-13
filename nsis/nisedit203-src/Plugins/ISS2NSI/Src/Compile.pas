unit Compile;

{
  Inno Setup
  Copyright (C) 1998-2002 Jordan Russell
  Portions by Martijn Laan
  For conditions of distribution and use, see LICENSE.TXT.


  This version is modified by Héctor Mauricio Rodríguez Segura <ranametal@blistering.net>
  For the original version go to www.innosetup.com

  This version of the Inno Setup compiler don't create a Inno Setup executable. Only
  parses the script to convert it in a NSIS script.

  History
    1.0a2 (xx/xx/2003)
     * InfoBeforeFile, InfoAfterFile support.
     * MUI 1.66 support.

    1.0a1 (09/11/2003)
     * MUI 1.65 support.

    1.0a0 (05/20/2003)
     * First release.

}

{$I VERSION.INC}

interface

uses
  Windows, SysUtils, CompInt, Classes;

const
  Iss2Nsi_Version = '1.0a2'; // Version number of the modified compiler

procedure ISConvertFileToFile(const NSISScript, InnoSetupScript: String);
procedure ISConvertFileToLines(const InnoSetupScript: String; Lines: TStrings);

type
  EISCompileError = class(Exception);

implementation

uses
  Commctrl, Consts, TypInfo, Struct, CompMsgs;

type
  TBreakStringRec = record
    ParamName: String;
    ParamData: String;
  end;

  PBreakStringArray = ^TBreakStringArray;
  TBreakStringArray = array[0..15] of TBreakStringRec;

  TParamInfo = record
    Name: PChar;
    Flags: set of (piNoEmpty, piNoQuotes);
  end;

  TEnumIniSectionProc = procedure(const Line: PChar; const Ext: Integer) of object;

  TSetupSectionDirectives = (
    ssAdminPrivilegesRequired,
    ssAllowNoIcons,
    ssAllowRootDirectory,
    ssAllowUNCPath,
    ssAlwaysRestart,
    ssAlwaysShowComponentsList,
    ssAlwaysShowDirOnReadyPage,
    ssAlwaysShowGroupOnReadyPage,
    ssAlwaysUsePersonalGroup,
    ssAppCopyright,
    ssAppId,
    ssAppMutex,
    ssAppName,
    ssAppPublisher,
    ssAppPublisherURL,
    ssAppSupportURL,
    ssAppUpdatesURL,
    ssAppVerName,
    ssAppVersion,
    ssBackColor,
    ssBackColor2,
    ssBackColorDirection,
    ssBackSolid,
    ssBits,
    ssChangesAssociations,
    ssCompression,
    ssCompressLevel,
    ssCreateAppDir,
    ssCreateUninstallRegKey,
    ssDefaultDirName,
    ssDefaultGroupName,
    ssDefaultUserInfoName,
    ssDefaultUserInfoOrg,
    ssDirExistsWarning,
    ssDisableAppendDir,
    ssDisableDirExistsWarning,
    ssDisableDirPage,
    ssDisableFinishedPage,
    ssDisableProgramGroupPage,
    ssDisableReadyMemo,
    ssDisableReadyPage,
    ssDisableStartupPrompt,
    ssDiskClusterSize,
    ssDiskSize,
    ssDiskSpanning,
    ssDontMergeDuplicateFiles,
    ssEnableDirDoesntExistWarning,
    ssExtraDiskSpaceRequired,
    ssInfoAfterFile,
    ssInfoBeforeFile,
    ssInternalCompressLevel,
    ssInstallMode,
    ssFlatComponentsList,
    ssLicenseFile,
    ssMessagesFile,
    ssMinVersion,
    ssOnlyBelowVersion,
    ssOutputBaseFilename,
    ssOutputDir,
    ssOverwriteUninstRegEntries,
    ssPassword,
    ssPrivilegesRequired,
    ssReserveBytes,
    ssRestartIfNeededByRun,
    ssShowComponentSizes,
    ssSourceDir,
    ssUpdateUninstallLogAppName,
    ssUninstallable,
    ssUninstallDisplayIcon,
    ssUninstallDisplayName,
    ssUninstallFilesDir,
    ssUninstallLogMode,
    ssUninstallRestartComputer,
    ssUninstallStyle,
    ssUsePreviousAppDir,
    ssUsePreviousGroup,
    ssUsePreviousSetupType,
    ssUsePreviousTasks,
    ssUsePreviousUserInfo,
    ssUseSetupLdr,
    ssUserInfoPage,
    ssWindowResizable,
    ssWindowShowCaption,
    ssWindowStartMaximized,
    ssWindowVisible,
    ssWizardImageBackColor,
    ssWizardImageFile,
    ssWizardSmallImageBackColor,
    ssWizardSmallImageFile,
    ssWizardStyle);

  TAllowedConst = (acOldData, acBreak);
  TAllowedConsts = set of TAllowedConst;

  TSetupCompiler = class
  private
    ScriptFiles: TStringList;

    TypeEntries,
    ComponentEntries,
    TaskEntries,
    DirEntries,
    FileEntries,
    FileLocationEntries,
    IconEntries,
    IniEntries,
    RegistryEntries,
    InstallDeleteEntries,
    UninstallDeleteEntries,
    RunEntries,
    UninstallRunEntries: TList;

    FileLocationEntryFilenames, WarningsList: TStringList;

    OutputDir, OutputBaseFilename: String;
    CompressWithBzip: Boolean;
    InternalCompressLevel, CompressLevel: Integer;
    SourceDirSet, DontMergeDuplicateFiles: Boolean;

    SetupHeader: TSetupHeader;

    ReadmeFile, UninstallerExe, ISSSourceFile: String;
    NSISLines: TStrings;

    SetupDirectiveLines: array[TSetupSectionDirectives] of Integer;
    UseSetupLdr, DiskSpanning, {HasRegSvr,} HasRestart, BackSolid: Boolean;
    DiskTotalBytes, DiskClusterSize, ReserveBytes: Longint;
     LicenseFile, InfoBeforeFile, InfoAfterFile, WizardImageFile: String;
    WizardSmallImageFile: String;

    LineNumber: Integer;
    ParseFilenameStack: TStringList;

    procedure AddStatus (const S: String);
    procedure AbortCompile (const Msg: String);
    procedure AbortCompileFmt (const Msg: String; const Args: array of const);
    procedure AbortCompileOnLine (const Msg: String);
    procedure AbortCompileOnLineFmt (const Msg: String;
      const Args: array of const);
    procedure AbortCompileParamError (const Msg, ParamName: String);
    function PrependDirName (const Filename, Dir: String): String;
    function PrependSourceDirName (const Filename: String): String;
    procedure BreakString (S: PChar; var Output: TBreakStringArray);
    procedure DoCallback (const Code: Integer; var Data: TCompilerCallbackData);
    procedure EnumIniSection (const EnumProc: TEnumIniSectionProc;
      const SectionName: String; const Ext: Integer; const Verbose: Boolean;
      const Filename: String);
    function CompareParamName (const S: TBreakStringRec;
      const ParamInfo: array of TParamInfo;
      var ParamNamesFound: array of Boolean): Integer;
    procedure EnumTypes (const Line: PChar; const Ext: Integer);
    procedure EnumComponents (const Line: PChar; const Ext: Integer);
    procedure EnumTasks (const Line: PChar; const Ext: Integer);
    procedure EnumDirs (const Line: PChar; const Ext: Integer);
    procedure EnumIcons (const Line: PChar; const Ext: Integer);
    procedure EnumINI (const Line: PChar; const Ext: Integer);
    procedure EnumRegistry (const Line: PChar; const Ext: Integer);
    procedure EnumDelete (const Line: PChar; const Ext: Integer);
    procedure EnumFiles (const Line: PChar; const Ext: Integer);
    procedure EnumRun (const Line: PChar; const Ext: Integer);
    procedure EnumSetup (const Line: PChar; const Ext: Integer);
    function ReadScriptFile(const Filename: String): TStringList;
    procedure SeparateDirective (const Line: PChar; var Key, Value: String);
    procedure GenerateNSISScript;
  public
    AppData: Longint;
    CallbackProc: TCompilerCallbackProc;
    SourceDir, OriginalSourceDir: String;
    constructor Create (AOwner: TComponent);
    destructor Destroy; override;
    procedure Compile;
  end;

const
  ParamCommonFlags = 'Flags';
  ParamCommonComponents = 'Components';
  ParamCommonTasks = 'Tasks';
  ParamCommonMinVersion = 'MinVersion';
  ParamCommonOnlyBelowVersion = 'OnlyBelowVersion';

  DefaultTypeEntryNames: array[0..2] of PChar = ('full', 'compact', 'custom');

  MaxDiskSize = 2100000000;

type
  TColor = $7FFFFFFF-1..$7FFFFFFF;

const
  clScrollBar = TColor(COLOR_SCROLLBAR or $80000000);
  clBackground = TColor(COLOR_BACKGROUND or $80000000);
  clActiveCaption = TColor(COLOR_ACTIVECAPTION or $80000000);
  clInactiveCaption = TColor(COLOR_INACTIVECAPTION or $80000000);
  clMenu = TColor(COLOR_MENU or $80000000);
  clWindow = TColor(COLOR_WINDOW or $80000000);
  clWindowFrame = TColor(COLOR_WINDOWFRAME or $80000000);
  clMenuText = TColor(COLOR_MENUTEXT or $80000000);
  clWindowText = TColor(COLOR_WINDOWTEXT or $80000000);
  clCaptionText = TColor(COLOR_CAPTIONTEXT or $80000000);
  clActiveBorder = TColor(COLOR_ACTIVEBORDER or $80000000);
  clInactiveBorder = TColor(COLOR_INACTIVEBORDER or $80000000);
  clAppWorkSpace = TColor(COLOR_APPWORKSPACE or $80000000);
  clHighlight = TColor(COLOR_HIGHLIGHT or $80000000);
  clHighlightText = TColor(COLOR_HIGHLIGHTTEXT or $80000000);
  clBtnFace = TColor(COLOR_BTNFACE or $80000000);
  clBtnShadow = TColor(COLOR_BTNSHADOW or $80000000);
  clGrayText = TColor(COLOR_GRAYTEXT or $80000000);
  clBtnText = TColor(COLOR_BTNTEXT or $80000000);
  clInactiveCaptionText = TColor(COLOR_INACTIVECAPTIONTEXT or $80000000);
  clBtnHighlight = TColor(COLOR_BTNHIGHLIGHT or $80000000);
  cl3DDkShadow = TColor(COLOR_3DDKSHADOW or $80000000);
  cl3DLight = TColor(COLOR_3DLIGHT or $80000000);
  clInfoText = TColor(COLOR_INFOTEXT or $80000000);
  clInfoBk = TColor(COLOR_INFOBK or $80000000);

  clBlack = TColor($000000);
  clMaroon = TColor($000080);
  clGreen = TColor($008000);
  clOlive = TColor($008080);
  clNavy = TColor($800000);
  clPurple = TColor($800080);
  clTeal = TColor($808000);
  clGray = TColor($808080);
  clSilver = TColor($C0C0C0);
  clRed = TColor($0000FF);
  clLime = TColor($00FF00);
  clYellow = TColor($00FFFF);
  clBlue = TColor($FF0000);
  clFuchsia = TColor($FF00FF);
  clAqua = TColor($FFFF00);
  clLtGray = TColor($C0C0C0);
  clDkGray = TColor($808080);
  clWhite = TColor($FFFFFF);
  clNone = TColor($1FFFFFFF);
  clDefault = TColor($20000000);

type
  TColorEntry = record
    Value: TColor;
    Name: string;
  end;

const
  Colors: array[0..41] of TColorEntry = (
    (Value: clBlack; Name: 'clBlack'),
    (Value: clMaroon; Name: 'clMaroon'),
    (Value: clGreen; Name: 'clGreen'),
    (Value: clOlive; Name: 'clOlive'),
    (Value: clNavy; Name: 'clNavy'),
    (Value: clPurple; Name: 'clPurple'),
    (Value: clTeal; Name: 'clTeal'),
    (Value: clGray; Name: 'clGray'),
    (Value: clSilver; Name: 'clSilver'),
    (Value: clRed; Name: 'clRed'),
    (Value: clLime; Name: 'clLime'),
    (Value: clYellow; Name: 'clYellow'),
    (Value: clBlue; Name: 'clBlue'),
    (Value: clFuchsia; Name: 'clFuchsia'),
    (Value: clAqua; Name: 'clAqua'),
    (Value: clWhite; Name: 'clWhite'),
    (Value: clScrollBar; Name: 'clScrollBar'),
    (Value: clBackground; Name: 'clBackground'),
    (Value: clActiveCaption; Name: 'clActiveCaption'),
    (Value: clInactiveCaption; Name: 'clInactiveCaption'),
    (Value: clMenu; Name: 'clMenu'),
    (Value: clWindow; Name: 'clWindow'),
    (Value: clWindowFrame; Name: 'clWindowFrame'),
    (Value: clMenuText; Name: 'clMenuText'),
    (Value: clWindowText; Name: 'clWindowText'),
    (Value: clCaptionText; Name: 'clCaptionText'),
    (Value: clActiveBorder; Name: 'clActiveBorder'),
    (Value: clInactiveBorder; Name: 'clInactiveBorder'),
    (Value: clAppWorkSpace; Name: 'clAppWorkSpace'),
    (Value: clHighlight; Name: 'clHighlight'),
    (Value: clHighlightText; Name: 'clHighlightText'),
    (Value: clBtnFace; Name: 'clBtnFace'),
    (Value: clBtnShadow; Name: 'clBtnShadow'),
    (Value: clGrayText; Name: 'clGrayText'),
    (Value: clBtnText; Name: 'clBtnText'),
    (Value: clInactiveCaptionText; Name: 'clInactiveCaptionText'),
    (Value: clBtnHighlight; Name: 'clBtnHighlight'),
    (Value: cl3DDkShadow; Name: 'cl3DDkShadow'),
    (Value: cl3DLight; Name: 'cl3DLight'),
    (Value: clInfoText; Name: 'clInfoText'),
    (Value: clInfoBk; Name: 'clInfoBk'),
    (Value: clNone; Name: 'clNone'));


////////////////////////////////////////////////////////////////////////////////
//  NEW Structs: redefine some strucs in the Struc.pas unit
////////////////////////////////////////////////////////////////////////////////

const
  SetupRegistryEntryStrings = 6;
type
  PSetupRegistryEntry = ^TSetupRegistryEntry;
  TSetupRegistryEntry = packed record
    Subkey, ValueName, ValueData: String;
    Components, Tasks, RootKey: String; { RootKey is String and not HKEY}
    MinVersion, OnlyBelowVersion: TSetupVersionData;
    Typ: (rtNone, rtString, rtExpandString, rtDWord, rtBinary, rtMultiString);
    Options: set of (roCreateValueIfDoesntExist, roUninsDeleteValue,
      roUninsClearValue, roUninsDeleteEntireKey, roUninsDeleteEntireKeyIfEmpty,
      roPreserveStringType, roDeleteKey, roDeleteValue, roNoError,
      roDontCreateKey);
  end;

const
  SetupRunEntryStrings = 9;
type
  PSetupRunEntry = ^TSetupRunEntry;
  TSetupRunEntry = packed record
    Name, Parameters, WorkingDir, RunOnceId, StatusMsg: String;
    Description, Components, Tasks, ShowCmd: String; { ShowCmd is String and not Integer }
    MinVersion, OnlyBelowVersion: TSetupVersionData;
    Wait: (rwWaitUntilTerminated, rwNoWait, rwWaitUntilIdle);
    Options: set of (roShellExec, roSkipIfDoesntExist,
      roPostInstall, roUnchecked, roSkipIfSilent, roSkipIfNotSilent,
      roHideWizard);
  end;

const
  SetupDirEntryStrings = 4;
type
  PSetupDirEntry = ^TSetupDirEntry;
  TSetupDirEntry = packed record
    DirName, Attribs: String; { Attribs is String and not Integer }
    Components, Tasks: String;
    MinVersion, OnlyBelowVersion: TSetupVersionData;
    Options: set of (doUninsNeverUninstall, doDeleteAfterInstall,
      doUninsAlwaysUninstall);
  end;

const
  SetupFileEntryStrings = 6;
type
  PSetupFileEntry = ^TSetupFileEntry;
  TSetupFileEntry = packed record { Attribs is String and not Integer }
    SourceFilename, DestName, InstallFontName, Attribs: String;
    Components, Tasks: String;
    ExternalFile: Boolean; { <- Added this }
    MinVersion, OnlyBelowVersion: TSetupVersionData;
    LocationEntry: Integer;
    ExternalSize: Longint;
    Options: set of (foConfirmOverwrite, foUninsNeverUninstall, foRestartReplace,
      foDeleteAfterInstall, foRegisterServer, foRegisterTypeLib, foSharedFile,
      foCompareTimeStamp, foFontIsntTrueType,
      foSkipIfSourceDoesntExist, foOverwriteReadOnly, foOverwriteSameVersion,
      foCustomDestName, foOnlyIfDestFileExists, foNoRegError,
      foUninsRestartDelete, foOnlyIfDoesntExist, foIgnoreVersion,
      foPromptIfOlder);
    FileType: (ftUserFile, ftUninstExe, ftRegSvrExe);
  end;

const
  SetupIconEntryStrings = 11;
type
  PSetupIconEntry = ^TSetupIconEntry;
  TSetupIconEntry = packed record
    IconName, Filename, Parameters, WorkingDir, IconFilename, Comment,
    HotKey, ShowCmd, IconIndex: String; { HotKey, ShowCmd and IconIndex is String and not Integer }
    Components, Tasks: String;
    MinVersion, OnlyBelowVersion: TSetupVersionData;
    CloseOnExit: TSetupIconCloseOnExit;
    Options: set of (ioUninsNeverUninstall, ioCreateOnlyIfFileExists,
      ioUseAppPaths);
  end;


// Moved here from SetupEnt.pas
procedure SEFreeRec (const P: Pointer; const NumStrings: Integer);
begin
  if P = nil then Exit;
  if NumStrings > 0 then  { Finalize in Delphi versions < 5 can't be called with zero count }
    Finalize (String(P^), NumStrings);
  FreeMem (P);
end;


// Moved here from CmnFunc2.pas
function RemoveQuotes (const S: String): String;
{ Opposite of AddQuotes; removes any quotes around the string. }
begin
  Result := S;
  while (Result <> '') and (Result[1] = '"') do
    Delete (Result, 1, 1);
  while (Result <> '') and (AnsiLastChar(Result)^ = '"') do
    SetLength (Result, Length(Result)-1);
end;

function AddBackslash (const S: String): String;
{ Adds a trailing backslash to the string, if one wasn't there already.
  But if S is an empty string, the function returns an empty string. }
begin
  Result := S;
  if (Result <> '') and (AnsiLastChar(Result)^ <> '\') then
    Result := Result + '\';
end;

procedure StringChange (var S: String; const FromStr, ToStr: String);
{ Change all occurrences in S of FromStr to ToStr }
var
  StartPos, I: Integer;
label 1;
begin
  if FromStr = '' then Exit;
  StartPos := 1;
1:for I := StartPos to Length(S)-Length(FromStr)+1 do begin
    if Copy(S, I, Length(FromStr)) = FromStr then begin
      Delete (S, I, Length(FromStr));
      Insert (ToStr, S, I);
      StartPos := I + Length(ToStr);
      goto 1;
    end;
  end;
end;

function RemoveBackslash (const S: String): String;
{ Removes the trailing backslash from the string, if one exists }
begin
  Result := S;
  if (Result <> '') and (AnsiLastChar(Result)^ = '\') then
    SetLength (Result, Length(Result)-1);
end;


function IdentToColor(const Ident: string; var Color: Longint): Boolean;
var
  I: Integer;
begin
  for I := Low(Colors) to High(Colors) do
    if CompareText(Colors[I].Name, Ident) = 0 then
    begin
      Result := True;
      Color := Longint(Colors[I].Value);
      Exit;
    end;
  Result := False;
end;

function StringToColor(const S: string): TColor;
begin
  if not IdentToColor(S, Longint(Result)) then
    Result := TColor(StrToInt(S));
end;

function AddPeriod (const S: String): String;
begin
  Result := S;
  if (Result <> '') and (AnsiLastChar(Result)^ > '.') then
    Result := Result + '.';
end;

function IsRelativePath (const Filename: String): Boolean;
var
  L: Integer;
begin
  Result := True;
  L := Length(Filename);
  if ((L >= 1) and (Filename[1] = '\')) or
     ((L >= 2) and (Filename[1] in ['A'..'Z', 'a'..'z']) and (Filename[2] = ':')) then
    Result := False;
end;


{ TSetupCompiler }

constructor TSetupCompiler.Create (AOwner: TComponent);
begin
  inherited Create;
  ScriptFiles := TStringList.Create;
  ParseFilenameStack := TStringList.Create;
  TypeEntries := TList.Create;
  ComponentEntries := TList.Create;
  TaskEntries := TList.Create;
  DirEntries := TList.Create;
  FileEntries := TList.Create;
  FileLocationEntries := TList.Create;
  IconEntries := TList.Create;
  IniEntries := TList.Create;
  RegistryEntries := TList.Create;
  InstallDeleteEntries := TList.Create;
  UninstallDeleteEntries := TList.Create;
  RunEntries := TList.Create;
  UninstallRunEntries := TList.Create;
  FileLocationEntryFilenames := TStringList.Create;
  WarningsList := TStringList.Create;
end;

destructor TSetupCompiler.Destroy;
begin
  WarningsList.Free;
  FileLocationEntryFilenames.Free;
  UninstallRunEntries.Free;
  RunEntries.Free;
  UninstallDeleteEntries.Free;
  InstallDeleteEntries.Free;
  RegistryEntries.Free;
  IniEntries.Free;
  IconEntries.Free;
  FileLocationEntries.Free;
  FileEntries.Free;
  DirEntries.Free;
  TaskEntries.Free;
  ComponentEntries.Free;
  TypeEntries.Free;
  ParseFilenameStack.Free;
  ScriptFiles.Free;
  inherited Destroy;
end;


procedure TSetupCompiler.DoCallback (const Code: Integer;
  var Data: TCompilerCallbackData);
begin
  if CallbackProc(Code, Data, AppData) = iscrRequestAbort then
    Abort;
end;

function TSetupCompiler.ReadScriptFile(const Filename: String): TStringList;
var
  I: Integer;
  F: TextFile;
  Lines: TStringList;
  Reset: Boolean;
  Data: TCompilerCallbackData;
  L: String;
begin
  for I := 0 to ScriptFiles.Count-1 do
    if AnsiCompareFileName(ScriptFiles[I], Filename) = 0 then begin
      Result := TStringList(ScriptFiles.Objects[I]);
      Exit;
    end;

  Lines := TStringList.Create;
  try
    if Filename = '' then begin
      Reset := True;
      while True do begin
        Data.Reset := Reset;
        Data.LineRead := nil;
        DoCallback(iscbReadScript, Data);
        if Data.LineRead = nil then
          Break;
        Lines.Add(Data.LineRead);
        Reset := False;
      end;
    end
    else begin
      AssignFile(F, Filename);
      FileMode := fmOpenRead or fmShareDenyWrite;
      try
        System.Reset(F);
      except
        on E: Exception do
          AbortCompileOnLineFmt(SCompilerErrorOpeningIncludeFile,
            [Filename, E.Message]);
      end;
      try
        while not Eof(F) do begin
          Readln(F, L);
          Lines.Add(L);
        end;
      finally
        CloseFile(F);
      end;
    end;
  except
    Lines.Free;
    raise;
  end;

  ScriptFiles.AddObject(Filename, Lines);
  Result := Lines;
end;

procedure TSetupCompiler.EnumIniSection (const EnumProc: TEnumIniSectionProc;
  const SectionName: String; const Ext: Integer; const Verbose: Boolean;
  const Filename: String);
var
  FoundSection: Boolean;
  LastSection: String;

  procedure DoFile(Filename: String); forward;

  procedure Directive(D: String);
  var
    Dir: String;
  begin
    if Copy(D, 1, Length('include')) = 'include' then begin
      Delete(D, 1, Length('include'));
      if (D = '') or (D[1] > ' ') then
        AbortCompileOnLine(SCompilerInvalidDirective);
      D := TrimLeft(D);
      if (Length(D) < 3) or (D[1] <> '"') or (AnsiLastChar(D)^ <> '"') then
        AbortCompileOnLine(SCompilerInvalidDirective);
      if Filename = '' then
        Dir := OriginalSourceDir
      else
        Dir := ExtractFilePath(Filename);
      DoFile(PrependDirName(RemoveQuotes(D), Dir));
    end
    else
      AbortCompileOnLine(SCompilerInvalidDirective);
  end;

  procedure DoFile(Filename: String);
  var
    Lines: TStringList;
    B, L: String;
    SaveLineNumber, I: Integer;
  begin
    if Filename <> '' then
      Filename := ExpandFileName(PrependSourceDirName(Filename));

    { Check if it's a recursive include }
    for I := 0 to ParseFilenameStack.Count-1 do
      if AnsiCompareFileName(ParseFilenameStack[I], Filename) = 0 then
        AbortCompileOnLineFmt(SCompilerRecursiveInclude, [Filename]);

    Lines := ReadScriptFile(Filename);
    SaveLineNumber := LineNumber;
    ParseFilenameStack.Add(Filename);

    LineNumber := 0;
    while LineNumber < Lines.Count do begin
      B := Lines[LineNumber];
      Inc (LineNumber);
      L := Trim(B);
      { Check for blank lines or comments }
      if (L = '') or (L[1] = ';') then Continue;
      if L[1] = '#' then begin
        { Compiler directive }
        Directive(Copy(L, 2, Maxint));
      end
      else if L[1] = '[' then begin
        { Section tag }
        I := Pos(']', L);
        if I < 3 then
          AbortCompileOnLine (SCompilerSectionTagInvalid);
        L := Copy(L, 2, I-2);
        if L[1] = '/' then begin
          L := Copy(L, 2, Maxint);
          if (LastSection = '') or (CompareText(L, LastSection) <> 0) then
            AbortCompileOnLineFmt (SCompilerSectionBadEndTag, [L]);
          FoundSection := False;
          LastSection := '';
        end
        else begin
          FoundSection := (CompareText(L, SectionName) = 0);
          LastSection := L;
        end;
      end
      else begin
        if not FoundSection then begin
          if LastSection = '' then
            AbortCompileOnLine (SCompilerTextNotInSection);
          Continue;  { not on the right section }
        end;
        if Verbose then
          AddStatus (Format(SCompilerStatusReadSectionLine, [SectionName, LineNumber]));
        EnumProc (PChar(B), Ext);
      end;
    end;

    LineNumber := SaveLineNumber;
    ParseFilenameStack.Delete(ParseFilenameStack.Count-1);
  end;

begin
  FoundSection := False;
  LastSection := '';
  DoFile(Filename);
end;

procedure TSetupCompiler.BreakString (S: PChar; var Output: TBreakStringArray);
var
  ColonPos, SemicolonPos, QuotePos, P, P2: PChar;
  ParamName, Data: String;
  QuoteFound, FirstQuoteFound, LastQuoteFound, AddChar, FirstNonspaceFound: Boolean;
  CurParm, Len, I: Integer;
begin
  CurParm := 0;
  while (S <> nil) and (CurParm <= High(TBreakStringArray)) do begin
    ColonPos := StrScan(S, ':');
    if ColonPos = nil then
      ParamName := StrPas(S)
    else
      SetString (ParamName, S, ColonPos-S);
    ParamName := Trim(ParamName);
    if ParamName = '' then Break;
    if ColonPos = nil then
      AbortCompileOnLineFmt (SCompilerParamHasNoValue, [ParamName]);
    S := ColonPos + 1;
    SemicolonPos := StrScan(S, ';');
    QuotePos := StrScan(S, '"');
    QuoteFound := QuotePos <> nil;
    if QuoteFound and (SemicolonPos <> nil) and (QuotePos > SemicolonPos) then
      QuoteFound := False;
    if not QuoteFound then begin
      Data := '';
      P := S;
      if SemicolonPos <> nil then
        P2 := SemicolonPos
      else
        P2 := StrEnd(S);
      FirstNonspaceFound := False;
      Len := 0;
      I := 0;
      while P < P2 do begin
        if (P^ <> ' ') or FirstNonspaceFound then begin
          FirstNonspaceFound := True;
          Data := Data + P^;
          Inc (I);
          if P^ <> ' ' then Len := I;
        end;
        Inc (P);
      end;
      SetLength (Data, Len);
    end
    else begin
      Data := '';
      SemicolonPos := nil;
      P := S;
      FirstQuoteFound := False;
      LastQuoteFound := False;
      while P^ <> #0 do begin
        AddChar := False;
        case P^ of
          ' ': AddChar := FirstQuoteFound;
          '"': if not FirstQuoteFound then
                 FirstQuoteFound := True
               else begin
                 Inc (P);
                 if P^ = '"' then
                   AddChar := True
                 else begin
                   LastQuoteFound := True;
                   while P^ <> #0 do begin
                     case P^ of
                       ' ': ;
                       ';': begin
                              SemicolonPos := P;
                              Break;
                            end;
                     else
                       AbortCompileOnLineFmt (SCompilerParamQuoteError, [ParamName]);
                     end;
                     Inc (P);
                   end;
                   Break;
                 end;
               end;
        else
          if not FirstQuoteFound then
            AbortCompileOnLineFmt (SCompilerParamQuoteError, [ParamName]);
          AddChar := True;
        end;
        if AddChar then
          Data := Data + P^;
        Inc (P);
      end;
      if not LastQuoteFound then
        AbortCompileOnLineFmt (SCompilerParamMissingClosingQuote, [ParamName]);
    end;
    S := SemicolonPos;
    if S <> nil then Inc (S);
    Output[CurParm].ParamName := ParamName;
    Output[CurParm].ParamData := Data;
    Inc (CurParm);
  end;
end;

procedure TSetupCompiler.AddStatus (const S: String);
var
  Data: TCompilerCallbackData;
begin
  Data.StatusMsg := PChar(S);
  CallbackProc (iscbNotifyStatus, Data, AppData);
end;

procedure TSetupCompiler.AbortCompile (const Msg: String);
begin
  raise EISCompileError.Create(Msg);
end;

procedure TSetupCompiler.AbortCompileFmt (const Msg: String; const Args: array of const);
begin
  AbortCompile (Format(Msg, Args));
end;

procedure TSetupCompiler.AbortCompileOnLine (const Msg: String);
{ AbortCompileOnLine is now equivalent to AbortCompile }
begin
  AbortCompile (Msg);
end;

procedure TSetupCompiler.AbortCompileOnLineFmt (const Msg: String;
  const Args: array of const);
begin
  AbortCompileOnLine (Format(Msg, Args));
end;

procedure TSetupCompiler.AbortCompileParamError (const Msg, ParamName: String);
begin
  AbortCompileOnLineFmt (Msg, [ParamName]);
end;

function TSetupCompiler.PrependDirName (const Filename, Dir: String): String;
begin
  if CompareText(Copy(Filename, 1, 9), 'compiler:') = 0 then
    Result := '${NSISDIR}' + Copy(Filename, 10, Maxint)
  else begin
    if (Filename = '') or not IsRelativePath(Filename) then
      Result := Filename
    else
      Result := Dir + Filename;
  end;
end;

function TSetupCompiler.PrependSourceDirName (const Filename: String): String;
begin
  Result := PrependDirName(Filename, SourceDir);
end;


function TSetupCompiler.CompareParamName (const S: TBreakStringRec;
  const ParamInfo: array of TParamInfo;
  var ParamNamesFound: array of Boolean): Integer;
var
  I: Integer;
begin
  Result := -1;
  for I := 0 to High(ParamInfo) do
    if StrIComp(ParamInfo[I].Name, PChar(S.ParamName)) = 0 then begin
      Result := I;
      if ParamNamesFound[I] then
        AbortCompileParamError (SCompilerParamDuplicated, StrPas(ParamInfo[I].Name));
      ParamNamesFound[I] := True;
      if (piNoEmpty in ParamInfo[I].Flags) and (S.ParamData = '') then
        AbortCompileParamError (SCompilerParamEmpty2, StrPas(ParamInfo[I].Name));
      if (piNoQuotes in ParamInfo[I].Flags) and (Pos('"', S.ParamData) <> 0) then
        AbortCompileParamError (SCompilerParamNoQuotes2, StrPas(ParamInfo[I].Name));
      Exit;
    end;
  { Unknown parameter }
  AbortCompileOnLineFmt (SCompilerParamUnknownParam, [S.ParamName]);
end;

function ExtractStr (var S: String): String;
var
  I: Integer;
begin
  I := Pos(' ', S);
  if I = 0 then I := Length(S)+1;
  Result := Trim(Copy(S, 1, I-1));
  S := Trim(Copy(S, I+1, Maxint));
end;

function ExtractFlag (var S: String; const FlagStrs: array of PChar): Integer;
var
  I: Integer;
  F: String;
begin
  F := ExtractStr(S);
  if F = '' then begin
    Result := -2;
    Exit;
  end;

  Result := -1;
  for I := 0 to High(FlagStrs) do
    if StrIComp(FlagStrs[I], PChar(F)) = 0 then begin
      Result := I;
      Break;
    end;
end;

function ExtractType(var S: String; const TypeEntries: TList): Integer;
var
  I: Integer;
  F: String;
begin
  F := ExtractStr(S);
  if F = '' then begin
    Result := -2;
    Exit;
  end;

  Result := -1;
  if TypeEntries.Count <> 0 then begin
    for I := 0 to TypeEntries.Count-1 do
      if CompareText(PSetupTypeEntry(TypeEntries[I]).Name, F) = 0 then begin
        Result := I;
        Break;
      end;
  end else begin
    for I := 0 to High(DefaultTypeEntryNames) do
      if StrIComp(DefaultTypeEntryNames[I], PChar(F)) = 0 then begin
        Result := I;
        Break;
      end;
  end;
end;

function ExtractComponent(var S: String; const ComponentEntries: TList): Integer;
var
  I: Integer;
  F: String;
begin
  F := ExtractStr(S);
  if F = '' then begin
    Result := -2;
    Exit;
  end;

  Result := -1;
  for I := 0 to ComponentEntries.Count-1 do
    if CompareText(PSetupComponentEntry(ComponentEntries[I]).Name, F) = 0 then begin
      Result := I;
      Break;
    end;
end;

function ExtractTask(var S: String; const TaskEntries: TList): Integer;
var
  I: Integer;
  F: String;
begin
  F := ExtractStr(S);
  if F = '' then begin
    Result := -2;
    Exit;
  end;

  Result := -1;
  for I := 0 to TaskEntries.Count-1 do
    if CompareText(PSetupTaskEntry(TaskEntries[I]).Name, F) = 0 then begin
      Result := I;
      Break;
    end;
end;

procedure AddToCommaText(var CommaText: String; const S: String);
begin
  if CommaText <> '' then
    CommaText := CommaText + ',';
  CommaText := CommaText + S;
end;

function StrToVersionNumbers (const S: String; var VerData: TSetupVersionData): Boolean;
  procedure Split (const Str: String; var Ver: TSetupVersionDataVersion;
    var ServicePack: Word);
  var
    I, J: Integer;
    Z, B: String;
    HasBuild: Boolean;
  begin
    Cardinal(Ver) := 0;
    ServicePack := 0;
    Z := Lowercase(Str);
    I := Pos('sp', Z);
    if I <> 0 then begin
      J := StrToInt(Copy(Z, I+2, Maxint));
      if (J < Low(Byte)) or (J > High(Byte)) then
        Abort;
      ServicePack := J shl 8;
      { ^ Shift left 8 bits because we're setting the "major" service pack
        version number. This parser doesn't currently accept "minor" service
        pack version numbers. }
      SetLength (Z, I-1);
    end;
    I := Pos('.', Z);
    if I = Length(Z) then Abort;
    if I <> 0 then begin
      J := StrToInt(Copy(Z, 1, I-1));
      if (J < Low(Ver.Major)) or (J > High(Ver.Major)) then
        Abort;
      Ver.Major := J;
      Z := Copy(Z, I+1, Maxint);
      I := Pos('.', Z);
      HasBuild := I <> 0;
      if not HasBuild then
        I := Length(Z)+1;
      B := Copy(Z, I+1, Maxint);
      Z := Copy(Z, 1, I-1);
      J := StrToInt(Z);
      if (J < 0) or (J > 99) then Abort;
      if (J < 10) and (Z[1] <> '0') then J := J * 10;
      Ver.Minor := J;
      if HasBuild then begin
        J := StrToInt(B);
        if (J < Low(Ver.Build)) or (J > High(Ver.Build)) then
          Abort;
        Ver.Build := J;
      end;
    end
    else begin  { no minor version specified }
      J := StrToInt(Str);
      if (J < Low(Ver.Major)) or (J > High(Ver.Major)) then
        Abort;
      Ver.Major := J;
    end;
  end;
var
  I: Integer;
  SP: Word;
begin
  try
    I := Pos(',', S);
    if I = 0 then Abort;
    Split (Trim(Copy(S, 1, I-1)),
      TSetupVersionDataVersion(VerData.WinVersion), SP);
    if SP <> 0 then Abort;  { only NT has service packs }
    Split (Trim(Copy(S, I+1, Maxint)),
      TSetupVersionDataVersion(VerData.NTVersion), VerData.NTServicePack);
    Result := True;
  except
    Result := False;
  end;
end;

procedure TSetupCompiler.SeparateDirective (const Line: PChar;
  var Key, Value: String);
var
  P, P2: PChar;
  L: Cardinal;
begin
  Key := '';
  Value := '';
  P := Line;
  while (P^ <> #0) and (P^ <= ' ') do
    Inc (P);
  if P^ = #0 then
    Exit;
  P2 := P;
  while (P2^ <> #0) and (P2^ <> '=') do
    Inc (P2);
  L := P2 - P;
  SetLength (Key, L);
  Move (P^, Key[1], Length(Key));
  Key := TrimRight(Key);
  if P2^ = #0 then
    Exit;
  P := P2 + 1;
  while (P^ <> #0) and (P^ <= ' ') do
    Inc (P);
  if P^ = #0 then
    Exit;
  Value := TrimRight(StrPas(P));
end;

procedure TSetupCompiler.EnumSetup (const Line: PChar; const Ext: Integer);
var
  KeyName, Value: String;
  I: Integer;
  Directive: TSetupSectionDirectives;

  procedure Invalid;
  begin
    AbortCompileOnLineFmt (SCompilerEntryInvalid2, ['Setup', KeyName]);
  end;

  function StrToBool (S: String): Boolean;
  begin
    Result := False;
    S := Lowercase(S);
    if (S = '0') or (S = 'no') or (S = 'false') then
      { Result already False }
    else if (S = '1') or (S = 'yes') or (S = 'true') then
      Result := True
    else
      Invalid;
  end;

  procedure SetSetupHeaderOption (const Option: TSetupHeaderOption);
  begin
    if not StrToBool(Value) then
      Exclude (SetupHeader.Options, Option)
    else
      Include (SetupHeader.Options, Option);
  end;

begin
  SeparateDirective (Line, KeyName, Value);

  if KeyName = '' then
    Exit;
  I := GetEnumValue(TypeInfo(TSetupSectionDirectives), 'ss' + KeyName);
  if I = -1 then Exit;
  Directive := TSetupSectionDirectives(I);
  if SetupDirectiveLines[Directive] <> 0 then
    AbortCompileOnLineFmt (SCompilerEntryAlreadySpecified, ['Setup', KeyName]);
  SetupDirectiveLines[Directive] := LineNumber;
  case Directive of
    ssAdminPrivilegesRequired: begin
        if StrToBool(Value) then
          SetupHeader.PrivilegesRequired := prAdmin;
        WarningsList.Add (Format(SCompilerEntrySuperseded2, ['Setup', KeyName,
           'PrivilegesRequired']));
      end;
    ssAllowNoIcons: begin
        SetSetupHeaderOption (shAllowNoIcons);
      end;
    ssAllowRootDirectory: begin
        SetSetupHeaderOption (shAllowRootDirectory);
      end;
    ssAllowUNCPath: begin
        SetSetupHeaderOption (shAllowUNCPath);
      end;
    ssAlwaysRestart: begin
        SetSetupHeaderOption (shAlwaysRestart);
      end;
    ssAlwaysUsePersonalGroup: begin
        SetSetupHeaderOption (shAlwaysUsePersonalGroup);
      end;
    ssAlwaysShowComponentsList: begin
        SetSetupHeaderOption (shAlwaysShowComponentsList);
      end;
    ssAlwaysShowDirOnReadyPage: begin
        SetSetupHeaderOption (shAlwaysShowDirOnReadyPage);
      end;
    ssAlwaysShowGroupOnReadyPage: begin
        SetSetupHeaderOption (shAlwaysShowGroupOnReadyPage);
      end;
    ssAppCopyright: begin
        SetupHeader.AppCopyright := Value;
      end;
    ssAppId: begin
        if Length(Value) > 127 then
          AbortCompileOnLineFmt (SCompilerEntryTooLong, ['Setup', KeyName, 127]);
        SetupHeader.AppId := Value;
      end;
    ssAppMutex: begin
        SetupHeader.AppMutex := Trim(Value);
      end;
    ssAppName: begin
        if Value = '' then
          Invalid;
        if Length(Value) > 127 then
          AbortCompileOnLineFmt (SCompilerEntryTooLong, ['Setup', KeyName, 127]);
        SetupHeader.AppName := Value;
      end;
    ssAppPublisher: begin
        if Value = '' then
          Invalid;
        SetupHeader.AppPublisher := Value;
      end;
    ssAppPublisherURL: begin
        if Value = '' then
          Invalid;
        SetupHeader.AppPublisherURL := Value;
      end;
    ssAppSupportURL: begin
        if Value = '' then
          Invalid;
        SetupHeader.AppSupportURL := Value;
      end;
    ssAppUpdatesURL: begin
        if Value = '' then
          Invalid;
        SetupHeader.AppUpdatesURL := Value;
      end;
    ssAppVerName: begin
        if Value = '' then
          Invalid;
        SetupHeader.AppVerName := Value;
      end;
    ssAppVersion: begin
        if Value = '' then
          Invalid;
        SetupHeader.AppVersion := Value;
      end;
    ssBackColor: begin
        try
          SetupHeader.BackColor := StringToColor(Value);
        except
          Invalid;
        end;
      end;
    ssBackColor2: begin
        try
          SetupHeader.BackColor2 := StringToColor(Value);
        except
          Invalid;
        end;
      end;
    ssBackColorDirection: begin
        if CompareText(Value, 'toptobottom') = 0 then
          Exclude (SetupHeader.Options, shBackColorHorizontal)
        else if CompareText(Value, 'lefttoright') = 0 then
          Include (SetupHeader.Options, shBackColorHorizontal)
        else
          Invalid;
      end;
    ssBackSolid: begin
        BackSolid := StrToBool(Value);
      end;
    ssBits: begin  { obsolete }
        I := StrToIntDef(Value, -1);
        if (I <> 16) and (I <> 32) then
          AbortCompileOnLine (SCompilerBitsNot16or32_2);
        if I = 16 then
          AbortCompileOnLine (SCompilerNeedToUse16);
        WarningsList.Add (Format(SCompilerEntryObsolete, ['Setup', KeyName]));
      end;
    ssChangesAssociations: begin
        SetSetupHeaderOption (shChangesAssociations)
      end;
    ssCompression: begin
        Value := Lowercase(Trim(Value));
        if Value = 'none' then
          CompressLevel := 0
        else if Value = 'zip' then
          { do nothing; it's the default }
        else if Value = 'bzip' then begin
          CompressWithBzip := True;
          CompressLevel := 9;
        end
        else if Copy(Value, 1, 4) = 'zip/' then begin
          I := StrToIntDef(Copy(Value, 5, Maxint), -1);
          if (I < 1) or (I > 9) then
            Invalid;
          CompressLevel := I;
        end
        else if Copy(Value, 1, 5) = 'bzip/' then begin
          I := StrToIntDef(Copy(Value, 6, Maxint), -1);
          if (I < 1) or (I > 9) then
            Invalid;
          CompressWithBzip := True;
          CompressLevel := I;
        end
        else
          Invalid;
      end;
    ssCompressLevel: begin
        if not CompressWithBzip then begin
          I := StrToIntDef(Value, -1);
          if (I < 0) or (I > 9) then
            Invalid;
          CompressLevel := I;
        end;
        WarningsList.Add (Format(SCompilerEntrySuperseded2, ['Setup', KeyName,
           'Compression']));
      end;
    ssCreateAppDir: begin
        SetSetupHeaderOption (shCreateAppDir);
      end;
    ssCreateUninstallRegKey: begin
        SetSetupHeaderOption (shCreateUninstallRegKey);
      end;
    ssDefaultDirName: begin
        SetupHeader.DefaultDirName := Value;
      end;
    ssDefaultGroupName: begin
        SetupHeader.DefaultGroupName := Value;
      end;
    ssDefaultUserInfoName: begin
        SetupHeader.DefaultUserInfoName := Value;
      end;
    ssDefaultUserInfoOrg: begin
        SetupHeader.DefaultUserInfoOrg := Value;
      end;
    ssDirExistsWarning: begin
        if CompareText(Value, 'auto') = 0 then
          SetupHeader.DirExistsWarning := ddAuto
        else if StrToBool(Value) then
          { ^ exception will be raised if Value is invalid }
          SetupHeader.DirExistsWarning := ddYes
        else
          SetupHeader.DirExistsWarning := ddNo;
      end;
    ssDisableAppendDir: begin
        SetSetupHeaderOption (shDisableAppendDir);
      end;
    ssDisableDirExistsWarning: begin  { obsolete; superceded by "DirExistsWarning" }
        if SetupDirectiveLines[ssDirExistsWarning] = 0 then begin
          if StrToBool(Value) then
            SetupHeader.DirExistsWarning := ddNo
          else
            SetupHeader.DirExistsWarning := ddAuto;
        end;
        WarningsList.Add (Format(SCompilerEntrySuperseded2, ['Setup', KeyName,
           'DirExistsWarning']));
      end;
    ssDisableDirPage: begin
        SetSetupHeaderOption (shDisableDirPage);
      end;
    ssDisableFinishedPage: begin
        SetSetupHeaderOption (shDisableFinishedPage);
      end;
    ssDisableProgramGroupPage: begin
        SetSetupHeaderOption (shDisableProgramGroupPage);
      end;
    ssDisableReadyMemo: begin
        SetSetupHeaderOption (shDisableReadyMemo);
      end;
    ssDisableReadyPage: begin
        SetSetupHeaderOption (shDisableReadyPage);
      end;
    ssDisableStartupPrompt: begin
        SetSetupHeaderOption (shDisableStartupPrompt);
      end;
    ssDiskClusterSize: begin
        Val (Value, DiskClusterSize, I);
        if I <> 0 then
          Invalid;
        if (DiskClusterSize < 1) or (DiskClusterSize > 32768) then
          AbortCompileOnLine (SCompilerDiskClusterSizeInvalid);
      end;
    ssDiskSize: begin
        Val (Value, DiskTotalBytes, I);
        if I <> 0 then
          Invalid;
        if (DiskTotalBytes < 262144) or (DiskTotalBytes > MaxDiskSize) then
          AbortCompileFmt (SCompilerDiskSizeInvalid, [262144, MaxDiskSize]);
      end;
    ssDiskSpanning: begin
        DiskSpanning := StrToBool(Value);
      end;
    ssDontMergeDuplicateFiles: begin
        DontMergeDuplicateFiles := StrToBool(Value);
      end;
    ssEnableDirDoesntExistWarning: begin
        SetSetupHeaderOption (shEnableDirDoesntExistWarning);
      end;
    ssExtraDiskSpaceRequired: begin
        Val (Value, SetupHeader.ExtraDiskSpaceRequired, I);
        if (I <> 0) or (SetupHeader.ExtraDiskSpaceRequired < 0) then
          Invalid;
      end;
    ssFlatComponentsList: begin
        SetSetupHeaderOption (shFlatComponentsList);
      end;
    ssInfoBeforeFile: begin
        InfoBeforeFile := RemoveQuotes(Value);
      end;
    ssInfoAfterFile: begin
        InfoAfterFile := RemoveQuotes(Value);
      end;
    ssInstallMode: begin
{
        if CompareText(Value, 'normal') = 0 then
          SetupHeader.InstallMode := imNormal
        else if CompareText(Value, 'silent') = 0 then
          SetupHeader.InstallMode := imSilent
        else if CompareText(Value, 'verysilent') = 0 then
          SetupHeader.InstallMode := imVerySilent
        else
          Invalid;
}
        WarningsList.Add (Format(SCompilerInstallModeObsolete, ['Setup', KeyName]));
      end;
    ssInternalCompressLevel: begin
        I := StrToIntDef(Value, -1);
        if (I < 0) or (I > 9) then
          Invalid;
        InternalCompressLevel := I;
      end;
    ssLicenseFile: begin
        LicenseFile := RemoveQuotes(Value);
      end;
    ssMinVersion: begin
        if not StrToVersionNumbers(Value, SetupHeader.MinVersion) then
          Invalid;
        if (SetupHeader.MinVersion.WinVersion <> 0) and
           (SetupHeader.MinVersion.WinVersion < $04000000{4.0}) then
          AbortCompileOnLineFmt (SCompilerMinVersionWinTooLow, ['4.0']);
        if (SetupHeader.MinVersion.NTVersion <> 0) and
           (SetupHeader.MinVersion.NTVersion < $04000000{4.0}) then
          AbortCompileOnLineFmt (SCompilerMinVersionNTTooLow, ['4.0']);
      end;
    ssOnlyBelowVersion: begin
        if not StrToVersionNumbers(Value, SetupHeader.OnlyBelowVersion) then
          Invalid;
      end;
    ssOutputBaseFilename: begin
        Value := RemoveQuotes(Value);
        if Value = '' then
          Invalid;
        OutputBaseFilename := Value;
      end;
    ssOutputDir: begin
        Value := RemoveQuotes(Value);
        if Value = '' then
          Invalid;
        OutputDir := Value;
      end;
    ssOverwriteUninstRegEntries: begin  { obsolete; ignored }
        { was: SetSetupHeaderOption (shOverwriteUninstRegEntries); }
        WarningsList.Add (Format(SCompilerEntryObsolete, ['Setup', KeyName]));
      end;
    {ssPassword: begin
        if Value <> '' then begin
          SetupHeader.Password := GetCRC32(Value[1], Length(Value));
          Include (SetupHeader.Options, shPassword);
        end;
      end;}
    ssPrivilegesRequired: begin
        if CompareText(Value, 'none') = 0 then
          SetupHeader.PrivilegesRequired := prNone
        else if CompareText(Value, 'poweruser') = 0 then
          SetupHeader.PrivilegesRequired := prPowerUser
        else if CompareText(Value, 'admin') = 0 then
          SetupHeader.PrivilegesRequired := prAdmin
        else
          Invalid;
      end;
    ssReserveBytes: begin
        Val (Value, ReserveBytes, I);
        if (I <> 0) or (ReserveBytes < 0) then
          Invalid;
      end;
    ssRestartIfNeededByRun: begin
        SetSetupHeaderOption (shRestartIfNeededByRun);
      end;
    ssShowComponentSizes: begin
        SetSetupHeaderOption (shShowComponentSizes);
      end;
    ssSourceDir: begin
        SourceDirSet := True;
        if IsRelativePath(Value) then
          SourceDir := AddBackslash(SourceDir) + Value
        else
          SourceDir := Value;
      end;
    ssUpdateUninstallLogAppName: begin
        SetSetupHeaderOption (shUpdateUninstallLogAppName);
      end;
    ssUninstallable: begin
        SetSetupHeaderOption (shUninstallable);
      end;
    ssUninstallDisplayIcon: begin
        SetupHeader.UninstallDisplayIcon := RemoveQuotes(Value);
      end;
    ssUninstallDisplayName: begin
        SetupHeader.UninstallDisplayName := Value;
      end;
    ssUninstallFilesDir: begin
        Value := RemoveQuotes(Value);
        if Value = '' then
          Invalid;
        SetupHeader.UninstallFilesDir := Value;
      end;
    ssUninstallLogMode: begin
        if CompareText(Value, 'append') = 0 then
          SetupHeader.UninstallLogMode := lmAppend
        else if CompareText(Value, 'new') = 0 then
          SetupHeader.UninstallLogMode := lmNew
        else if CompareText(Value, 'overwrite') = 0 then
          SetupHeader.UninstallLogMode := lmOverwrite
        else
          Invalid;
      end;
    ssUninstallRestartComputer: begin
        SetSetupHeaderOption (shUninstallRestartComputer);
      end;
    ssUninstallStyle: begin
        if CompareText(Value, 'classic') = 0 then
          SetupHeader.UninstallStyle := usClassic
        else if CompareText(Value, 'modern') = 0 then
          SetupHeader.UninstallStyle := usModern
        else
          Invalid;
      end;
    ssUsePreviousAppDir: begin
        SetSetupHeaderOption (shUsePreviousAppDir);
      end;
    ssUsePreviousGroup: begin
        SetSetupHeaderOption (shUsePreviousGroup);
      end;
    ssUsePreviousSetupType: begin
        SetSetupHeaderOption (shUsePreviousSetupType);
      end;
    ssUsePreviousTasks: begin
        SetSetupHeaderOption (shUsePreviousTasks);
      end;
    ssUsePreviousUserInfo: begin
        SetSetupHeaderOption (shUsePreviousUserInfo);
      end;
    ssUseSetupLdr: begin
        UseSetupLdr := StrToBool(Value);
      end;
    ssUserInfoPage: begin
        SetSetupHeaderOption (shUserInfoPage);
      end;
    ssWindowResizable: begin
        SetSetupHeaderOption (shWindowResizable);
      end;
    ssWindowShowCaption: begin
        SetSetupHeaderOption (shWindowShowCaption);
      end;
    ssWindowStartMaximized: begin
        SetSetupHeaderOption (shWindowStartMaximized);
      end;
    ssWindowVisible: begin
        SetSetupHeaderOption (shWindowVisible);
      end;
    ssWizardImageBackColor: begin
        try
          SetupHeader.WizardImageBackColor := StringToColor(Value);
        except
          Invalid;
        end;
      end;
    ssWizardSmallImageBackColor: begin
        try
          SetupHeader.WizardSmallImageBackColor := StringToColor(Value);
        except
          Invalid;
        end;
      end;
    ssWizardImageFile: begin
        Value := RemoveQuotes(Value);
        if Value = '' then
          Invalid;
        WizardImageFile := Value;
      end;
    ssWizardSmallImageFile: begin
        Value := RemoveQuotes(Value);
        if Value = '' then
          Invalid;
        WizardSmallImageFile := Value;
      end;
    ssWizardStyle: begin
        if CompareText(Value, 'modern') = 0 then begin
          { no-op }
        end else
          Invalid;
      end;
  end;
end;

procedure TSetupCompiler.EnumTypes (const Line: PChar; const Ext: Integer);
const
  ParamTypesName = 'Name';
  ParamTypesDescription = 'Description';
  ParamNames: array[0..4] of TParamInfo = (
    (Name: ParamCommonFlags; Flags: []),
    (Name: ParamTypesName; Flags: [piNoEmpty]),
    (Name: ParamTypesDescription; Flags: [piNoEmpty]),
    (Name: ParamCommonMinVersion; Flags: []),
    (Name: ParamCommonOnlyBelowVersion; Flags: []));
  Flags: array[0..0] of PChar = (
    'iscustom');
var
  Params: TBreakStringArray;
  ParamNameFound: array[Low(ParamNames)..High(ParamNames)] of Boolean;
  NewTypeEntry: PSetupTypeEntry;
  P: Integer;
begin
  FillChar (ParamNameFound, SizeOf(ParamNameFound), 0);
  BreakString (Line, Params);

  NewTypeEntry := AllocMem(SizeOf(TSetupTypeEntry));
  try
    with NewTypeEntry^ do begin
      MinVersion := SetupHeader.MinVersion;

      for P := Low(Params) to High(Params) do
        with Params[P] do begin
          if ParamName = '' then Break;
          case CompareParamName(Params[P], ParamNames, ParamNameFound) of
            0: while True do
                 case ExtractFlag(ParamData, Flags) of
                   -2: Break;
                   -1: AbortCompileParamError (SCompilerParamUnknownFlag2, ParamCommonFlags);
                   0: Include (Options, toIsCustom);
                 end;
            1: Name := LowerCase(ParamData);
            2: Description := ParamData;
            3: if not StrToVersionNumbers(ParamData, MinVersion) then
                 AbortCompileParamError (SCompilerParamInvalid2, ParamCommonMinVersion);
            4: if not StrToVersionNumbers(ParamData, OnlyBelowVersion) then
                 AbortCompileParamError (SCompilerParamInvalid2, ParamCommonOnlyBelowVersion);
          end;
        end;

      if not ParamNameFound[1] then
        AbortCompileParamError (SCompilerParamNotSpecified, ParamTypesName);
      if not ParamNameFound[2] then
        AbortCompileParamError (SCompilerParamNotSpecified, ParamTypesDescription);
    end;
  except
    SEFreeRec (NewTypeEntry, SetupTypeEntryStrings);
    raise;
  end;
  TypeEntries.Add (NewTypeEntry);
end;

procedure TSetupCompiler.EnumComponents (const Line: PChar; const Ext: Integer);
const
  ParamComponentsName = 'Name';
  ParamComponentsDescription = 'Description';
  ParamComponentsExtraDiskSpaceRequired = 'ExtraDiskSpaceRequired';
  ParamComponentsTypes = 'Types';
  ParamNames: array[0..6] of TParamInfo = (
    (Name: ParamCommonFlags; Flags: []),
    (Name: ParamComponentsName; Flags: [piNoEmpty]),
    (Name: ParamComponentsDescription; Flags: [piNoEmpty]),
    (Name: ParamComponentsExtraDiskSpaceRequired; Flags: []),
    (Name: ParamComponentsTypes; Flags: []),
    (Name: ParamCommonMinVersion; Flags: []),
    (Name: ParamCommonOnlyBelowVersion; Flags: []));
  Flags: array[0..2] of PChar = (
    'fixed', 'restart', 'disablenouninstallwarning');
var
  Params: TBreakStringArray;
  ParamNameFound: array[Low(ParamNames)..High(ParamNames)] of Boolean;
  NewComponentEntry: PSetupComponentEntry;
  P, I: Integer;
begin
  FillChar (ParamNameFound, SizeOf(ParamNameFound), 0);
  BreakString (Line, Params);

  NewComponentEntry := AllocMem(SizeOf(TSetupComponentEntry));
  try
    with NewComponentEntry^ do begin
      MinVersion := SetupHeader.MinVersion;

      for P := Low(Params) to High(Params) do
        with Params[P] do begin
          if ParamName = '' then Break;
          case CompareParamName(Params[P], ParamNames, ParamNameFound) of
            0: while True do
                 case ExtractFlag(ParamData, Flags) of
                   -2: Break;
                   -1: AbortCompileParamError (SCompilerParamUnknownFlag2, ParamCommonFlags);
                   0: Include (Options, coFixed);
                   1: Include (Options, coRestart);
                   2: Include (Options, coDisableNoUninstallWarning);
                 end;
            1: Name := LowerCase(ParamData);
            2: Description := ParamData;
            3: begin
               Val (ParamData, ExtraDiskSpaceRequired, I);
               if (I <> 0) or (ExtraDiskSpaceRequired < 0) then
                AbortCompileOnLine (SCompilerComponentsExtraDiskSpaceRequiredInvalid);
               end;
            4: while True do begin
                 I := ExtractType(ParamData, TypeEntries);
                 case I of
                   -2: Break;
                   -1: AbortCompileParamError (SCompilerParamUnknownType, ParamComponentsTypes);
                   else begin
                     if TypeEntries.Count <> 0 then
                       AddToCommaText(Types, PSetupTypeEntry(TypeEntries[I]).Name)
                     else
                       AddToCommaText(Types, DefaultTypeEntryNames[I]);
                   end;
                 end;
               end;
            5: if not StrToVersionNumbers(ParamData, MinVersion) then
                 AbortCompileParamError (SCompilerParamInvalid2, ParamCommonMinVersion);
            6: if not StrToVersionNumbers(ParamData, OnlyBelowVersion) then
                 AbortCompileParamError (SCompilerParamInvalid2, ParamCommonOnlyBelowVersion);
          end;
        end;

      if not ParamNameFound[1] then
        AbortCompileParamError (SCompilerParamNotSpecified, ParamComponentsName);
      if not ParamNameFound[2] then
        AbortCompileParamError (SCompilerParamNotSpecified, ParamComponentsDescription);

      if coRestart in Options then
        HasRestart := True;
    end;
  except
    SEFreeRec (NewComponentEntry, SetupComponentEntryStrings);
    raise;
  end;
  ComponentEntries.Add (NewComponentEntry);
end;

procedure TSetupCompiler.EnumTasks (const Line: PChar; const Ext: Integer);
const
  ParamTasksName = 'Name';
  ParamTasksDescription = 'Description';
  ParamTasksGroupDescription = 'GroupDescription';
  ParamNames: array[0..6] of TParamInfo = (
    (Name: ParamCommonFlags; Flags: []),
    (Name: ParamTasksName; Flags: [piNoEmpty, piNoQuotes]),
    (Name: ParamTasksDescription; Flags: [piNoEmpty]),
    (Name: ParamTasksGroupDescription; Flags: [piNoEmpty]),
    (Name: ParamCommonComponents; Flags: []),
    (Name: ParamCommonMinVersion; Flags: []),
    (Name: ParamCommonOnlyBelowVersion; Flags: []));
  Flags: array[0..3] of PChar = (
    'exclusive', 'unchecked', 'restart', 'checkedonce');
var
  Params: TBreakStringArray;
  ParamNameFound: array[Low(ParamNames)..High(ParamNames)] of Boolean;
  NewTaskEntry: PSetupTaskEntry;
  P, I: Integer;
begin
  FillChar (ParamNameFound, SizeOf(ParamNameFound), 0);
  BreakString (Line, Params);

  NewTaskEntry := AllocMem(SizeOf(TSetupTaskEntry));
  try
    with NewTaskEntry^ do begin
      MinVersion := SetupHeader.MinVersion;

      for P := Low(Params) to High(Params) do
        with Params[P] do begin
          if ParamName = '' then Break;
          case CompareParamName(Params[P], ParamNames, ParamNameFound) of
            0: while True do
                 case ExtractFlag(ParamData, Flags) of
                   -2: Break;
                   -1: AbortCompileParamError (SCompilerParamUnknownFlag2, ParamCommonFlags);
                   0: Include (Options, toExclusive);
                   1: Include (Options, toUnchecked);
                   2: Include (Options, toRestart);
                   3: Include (Options, toCheckedOnce);
                 end;
            1: Name := ParamData;
            2: Description := ParamData;
            3: GroupDescription := ParamData;
            4: while True do begin
                 I := ExtractComponent(ParamData, ComponentEntries);
                 case I of
                   -2: Break;
                   -1: AbortCompileParamError (SCompilerParamUnknownComponent, ParamCommonComponents);
                   else
                     AddToCommaText(Components, PSetupComponentEntry(ComponentEntries[I]).Name)
                 end;
               end;
            5: if not StrToVersionNumbers(ParamData, MinVersion) then
                 AbortCompileParamError (SCompilerParamInvalid2, ParamCommonMinVersion);
            6: if not StrToVersionNumbers(ParamData, OnlyBelowVersion) then
                 AbortCompileParamError (SCompilerParamInvalid2, ParamCommonOnlyBelowVersion);
          end;
        end;

      if not ParamNameFound[1] then
        AbortCompileParamError (SCompilerParamNotSpecified, ParamTasksName);
      if not ParamNameFound[2] then
        AbortCompileParamError (SCompilerParamNotSpecified, ParamTasksDescription);

      if (toUnchecked in Options) and (toCheckedOnce in Options) then
        AbortCompileOnLineFmt (SCompilerParamErrorBadCombo2,
          [ParamCommonFlags, 'unchecked', 'checkedonce']);

      if toRestart in Options then
        HasRestart := True;
    end;
  except
    SEFreeRec (NewTaskEntry, SetupTaskEntryStrings);
    raise;
  end;
  TaskEntries.Add (NewTaskEntry);
end;

procedure TSetupCompiler.EnumDirs (const Line: PChar; const Ext: Integer);
const
  ParamDirsName = 'Name';
  ParamDirsAttribs = 'Attribs';
  ParamNames: array[0..6] of TParamInfo = (
    (Name: ParamCommonFlags; Flags: []),
    (Name: ParamDirsName; Flags: [piNoEmpty, piNoQuotes]),
    (Name: ParamCommonComponents; Flags: []),
    (Name: ParamCommonTasks; Flags: []),
    (Name: ParamCommonMinVersion; Flags: []),
    (Name: ParamCommonOnlyBelowVersion; Flags: []),
    (Name: ParamDirsAttribs; Flags: []));
  Flags: array[0..2] of PChar = (
    'uninsneveruninstall', 'deleteafterinstall', 'uninsalwaysuninstall');
  AttribsFlags: array[0..2] of PChar = (
    'readonly', 'hidden', 'system');
var
  Params: TBreakStringArray;
  ParamNameFound: array[Low(ParamNames)..High(ParamNames)] of Boolean;
  NewDirEntry: PSetupDirEntry;
  P, I: Integer;
begin
  FillChar (ParamNameFound, SizeOf(ParamNameFound), 0);
  BreakString (Line, Params);

  NewDirEntry := AllocMem(SizeOf(TSetupDirEntry));
  try
    with NewDirEntry^ do begin
      MinVersion := SetupHeader.MinVersion;

      for P := Low(Params) to High(Params) do
        with Params[P] do begin
          if ParamName = '' then Break;
          case CompareParamName(Params[P], ParamNames, ParamNameFound) of
            0: while True do
                 case ExtractFlag(ParamData, Flags) of
                   -2: Break;
                   -1: AbortCompileParamError (SCompilerParamUnknownFlag2, ParamCommonFlags);
                   0: Include (Options, doUninsNeverUninstall);
                   1: Include (Options, doDeleteAfterInstall);
                   2: Include (Options, doUninsAlwaysUninstall);
                 end;
            1: DirName := ParamData;
            2: while True do begin
                 I := ExtractComponent(ParamData, ComponentEntries);
                 case I of
                   -2: Break;
                   -1: AbortCompileParamError (SCompilerParamUnknownComponent, ParamCommonComponents);
                   else
                     AddToCommaText(Components, PSetupComponentEntry(ComponentEntries[I]).Name)
                 end;
               end;
            3: while True do begin
                 I := ExtractTask(ParamData, TaskEntries);
                 case I of
                   -2: Break;
                   -1: AbortCompileParamError (SCompilerParamUnknownTask, ParamCommonTasks);
                   else
                     AddToCommaText(Tasks, PSetupTaskEntry(TaskEntries[I]).Name)
                 end;
               end;
            4: if not StrToVersionNumbers(ParamData, MinVersion) then
                 AbortCompileParamError (SCompilerParamInvalid2, ParamCommonMinVersion);
            5: if not StrToVersionNumbers(ParamData, OnlyBelowVersion) then
                 AbortCompileParamError (SCompilerParamInvalid2, ParamCommonOnlyBelowVersion);
            6: begin
                 while True do
                 case ExtractFlag(ParamData, AttribsFlags) of
                   -2: Break;
                   -1: AbortCompileParamError (SCompilerParamUnknownFlag2, ParamDirsAttribs);
                   0: Attribs := Attribs + ' READONLY ';
                   1: Attribs := Attribs + ' HIDDEN ';
                   2: Attribs := Attribs + ' SYSTEM ';
                 end;
                 Attribs := Trim(Attribs);
                 StringChange(Attribs, '  ', '|');
               end
          end;
        end;

      if not ParamNameFound[1] then
        AbortCompileParamError (SCompilerParamNotSpecified, ParamDirsName);

      if (doUninsNeverUninstall in Options) and
         (doUninsAlwaysUninstall in Options) then
        AbortCompileOnLineFmt (SCompilerParamErrorBadCombo2,
          [ParamCommonFlags, 'uninsneveruninstall', 'uninsalwaysuninstall']);
    end;
  except
    SEFreeRec (NewDirEntry, SetupDirEntryStrings);
    raise;
  end;
  DirEntries.Add (NewDirEntry);
end;

procedure TSetupCompiler.EnumIcons (const Line: PChar; const Ext: Integer);
const
  ParamIconsName = 'Name';
  ParamIconsFilename = 'Filename';
  ParamIconsParameters = 'Parameters';
  ParamIconsWorkingDir = 'WorkingDir';
  ParamIconsHotKey = 'HotKey';
  ParamIconsIconFilename = 'IconFilename';
  ParamIconsIconIndex = 'IconIndex';
  ParamIconsComment = 'Comment';
  ParamNames: array[0..12] of TParamInfo = (
    (Name: ParamCommonFlags; Flags: []),
    (Name: ParamIconsName; Flags: [piNoEmpty, piNoQuotes]),
    (Name: ParamIconsFilename; Flags: [piNoEmpty, piNoQuotes]),
    (Name: ParamIconsParameters; Flags: []),
    (Name: ParamIconsWorkingDir; Flags: [piNoQuotes]),
    (Name: ParamIconsHotKey; Flags: []),
    (Name: ParamIconsIconFilename; Flags: [piNoQuotes]),
    (Name: ParamIconsIconIndex; Flags: []),
    (Name: ParamCommonComponents; Flags: []),
    (Name: ParamCommonTasks; Flags: []),
    (Name: ParamCommonMinVersion; Flags: []),
    (Name: ParamCommonOnlyBelowVersion; Flags: []),
    (Name: ParamIconsComment; Flags: []));
  Flags: array[0..6] of PChar = (
    'uninsneveruninstall', 'runminimized', 'createonlyiffileexists',
    'useapppaths', 'closeonexit', 'dontcloseonexit', 'runmaximized');
var
  Params: TBreakStringArray;
  ParamNameFound: array[Low(ParamNames)..High(ParamNames)] of Boolean;
  NewIconEntry: PSetupIconEntry;
  P, I: Integer;
  S: String;
begin
  FillChar (ParamNameFound, SizeOf(ParamNameFound), 0);
  BreakString (Line, Params);

  NewIconEntry := AllocMem(SizeOf(TSetupIconEntry));
  try
    with NewIconEntry^ do begin
      MinVersion := SetupHeader.MinVersion;

      IconIndex := '""';
      ShowCmd := '""';
      HotKey := '""';

      for P := Low(Params) to High(Params) do
        with Params[P] do begin
          if ParamName = '' then Break;
          case CompareParamName(Params[P], ParamNames, ParamNameFound) of
            0: while True do
                 case ExtractFlag(ParamData, Flags) of
                   -2: Break;
                   -1: AbortCompileParamError (SCompilerParamUnknownFlag2, ParamCommonFlags);
                   0: Include (Options, ioUninsNeverUninstall);
                   //1: ShowCmd := SW_SHOWMINNOACTIVE;
                   2: Include (Options, ioCreateOnlyIfFileExists);
                   3: Include (Options, ioUseAppPaths);
                   4: CloseOnExit := icYes;
                   5: CloseOnExit := icNo;
                   6: ShowCmd := 'SW_SHOWMAXIMIZED';
                 end;
            1: IconName := ParamData;
            2: Filename := ParamData;
            3: Parameters := ParamData;
            4: WorkingDir := ParamData;
            5: begin
                 StringChange(ParamData, '+', '|');
                 HotKey :=  Trim(ParamData);
               end;
            6: IconFilename := ParamData;
            7: IconIndex := ParamData;
            8: while True do begin
                 I := ExtractComponent(ParamData, ComponentEntries);
                 case I of
                   -2: Break;
                   -1: AbortCompileParamError (SCompilerParamUnknownComponent, ParamCommonComponents);
                   else
                     AddToCommaText(Components, PSetupComponentEntry(ComponentEntries[I]).Name)
                 end;
               end;
            9: while True do begin
                 I := ExtractTask(ParamData, TaskEntries);
                 case I of
                   -2: Break;
                   -1: AbortCompileParamError (SCompilerParamUnknownTask, ParamCommonTasks);
                   else
                     AddToCommaText(Tasks, PSetupTaskEntry(TaskEntries[I]).Name)
                 end;
               end;
            10: if not StrToVersionNumbers(ParamData, MinVersion) then
                 AbortCompileParamError (SCompilerParamInvalid2, ParamCommonMinVersion);
            11: if not StrToVersionNumbers(ParamData, OnlyBelowVersion) then
                  AbortCompileParamError (SCompilerParamInvalid2, ParamCommonOnlyBelowVersion);
            12: Comment := ParamData;
          end;
        end;

      if not ParamNameFound[1] then
        AbortCompileParamError (SCompilerParamNotSpecified, ParamIconsName);
      if not ParamNameFound[2] then
        AbortCompileParamError (SCompilerParamNotSpecified, ParamIconsFilename);

      if Pos('"', IconName) <> 0 then
        AbortCompileParamError (SCompilerParamNoQuotes2, ParamIconsName);
      if AnsiPos('\', IconName) = 0 then
        AbortCompileOnLine (SCompilerIconsNamePathNotSpecified);

      if (IconIndex <> '""') and (IconFilename = '') then
        IconFilename := Filename;

      S := IconName;
      if Copy(S, 1, 8) = '{group}\' then
        Delete (S, 1, 8);
    end;
  except
    SEFreeRec (NewIconEntry, SetupIconEntryStrings);
    raise;
  end;
  IconEntries.Add (NewIconEntry);
end;

procedure TSetupCompiler.EnumINI (const Line: PChar; const Ext: Integer);
const
  ParamIniFilename = 'Filename';
  ParamIniSection = 'Section';
  ParamIniKey = 'Key';
  ParamIniString = 'String';
  ParamNames: array[0..8] of TParamInfo = (
    (Name: ParamCommonFlags; Flags: []),
    (Name: ParamIniFilename; Flags: [piNoQuotes]),
    (Name: ParamIniSection; Flags: [piNoEmpty]),
    (Name: ParamIniKey; Flags: [piNoEmpty]),
    (Name: ParamIniString; Flags: []),
    (Name: ParamCommonComponents; Flags: []),
    (Name: ParamCommonTasks; Flags: []),
    (Name: ParamCommonMinVersion; Flags: []),
    (Name: ParamCommonOnlyBelowVersion; Flags: []));
  Flags: array[0..3] of PChar = (
    'uninsdeleteentry', 'uninsdeletesection', 'createkeyifdoesntexist',
    'uninsdeletesectionifempty');
var
  Params: TBreakStringArray;
  ParamNameFound: array[Low(ParamNames)..High(ParamNames)] of Boolean;
  NewIniEntry: PSetupIniEntry;
  P, I: Integer;
begin
  FillChar (ParamNameFound, SizeOf(ParamNameFound), 0);
  BreakString (Line, Params);

  NewIniEntry := AllocMem(SizeOf(TSetupIniEntry));
  try
    with NewIniEntry^ do begin
      MinVersion := SetupHeader.MinVersion;

      for P := Low(Params) to High(Params) do
        with Params[P] do begin
          if ParamName = '' then Break;
          case CompareParamName(Params[P], ParamNames, ParamNameFound) of
            0: while True do
                 case ExtractFlag(ParamData, Flags) of
                   -2: Break;
                   -1: AbortCompileParamError (SCompilerParamUnknownFlag2, ParamCommonFlags);
                   0: Include (Options, ioUninsDeleteEntry);
                   1: Include (Options, ioUninsDeleteEntireSection);
                   2: Include (Options, ioCreateKeyIfDoesntExist);
                   3: Include (Options, ioUninsDeleteSectionIfEmpty);
                 end;
            1: Filename := ParamData;
            2: Section := ParamData;
            3: Entry := ParamData;
            4: begin
                 Value := ParamData;
                 Include (Options, ioHasValue);
               end;
            5: while True do begin
                 I := ExtractComponent(ParamData, ComponentEntries);
                 case I of
                   -2: Break;
                   -1: AbortCompileParamError (SCompilerParamUnknownComponent, ParamCommonComponents);
                   else
                     AddToCommaText(Components, PSetupComponentEntry(ComponentEntries[I]).Name)
                 end;
               end;
            6: while True do begin
                 I := ExtractTask(ParamData, TaskEntries);
                 case I of
                   -2: Break;
                   -1: AbortCompileParamError (SCompilerParamUnknownTask, ParamCommonTasks);
                   else
                     AddToCommaText(Tasks, PSetupTaskEntry(TaskEntries[I]).Name)
                 end;
               end;
            7: if not StrToVersionNumbers(ParamData, MinVersion) then
                 AbortCompileParamError (SCompilerParamInvalid2, ParamCommonMinVersion);
            8: if not StrToVersionNumbers(ParamData, OnlyBelowVersion) then
                 AbortCompileParamError (SCompilerParamInvalid2, ParamCommonOnlyBelowVersion);
          end;
        end;

      if not ParamNameFound[1] then
        AbortCompileParamError (SCompilerParamNotSpecified, ParamIniFilename);
      if not ParamNameFound[2] then
        AbortCompileParamError (SCompilerParamNotSpecified, ParamIniSection);

      if (ioUninsDeleteEntry in Options) and
         (ioUninsDeleteEntireSection in Options) then
        AbortCompileOnLineFmt (SCompilerParamErrorBadCombo2,
          [ParamCommonFlags, 'uninsdeleteentry', 'uninsdeletesection']);
      if (ioUninsDeleteEntireSection in Options) and
         (ioUninsDeleteSectionIfEmpty in Options) then
        AbortCompileOnLineFmt (SCompilerParamErrorBadCombo2,
          [ParamCommonFlags, 'uninsdeletesection', 'uninsdeletesectionifempty']);

    end;
  except
    SEFreeRec (NewIniEntry, SetupIniEntryStrings);
    raise;
  end;
  IniEntries.Add (NewIniEntry);
end;

procedure TSetupCompiler.EnumRegistry (const Line: PChar; const Ext: Integer);
const
  ParamRegistryRoot = 'Root';
  ParamRegistrySubkey = 'Subkey';
  ParamRegistryValueType = 'ValueType';
  ParamRegistryValueName = 'ValueName';
  ParamRegistryValueData = 'ValueData';
  ParamNames: array[0..9] of TParamInfo = (
    (Name: ParamCommonFlags; Flags: []),
    (Name: ParamRegistryRoot; Flags: []),
    (Name: ParamRegistrySubkey; Flags: []),
    (Name: ParamRegistryValueType; Flags: []),
    (Name: ParamRegistryValueName; Flags: []),
    (Name: ParamRegistryValueData; Flags: []),
    (Name: ParamCommonComponents; Flags: []),
    (Name: ParamCommonTasks; Flags: []),
    (Name: ParamCommonMinVersion; Flags: []),
    (Name: ParamCommonOnlyBelowVersion; Flags: []));
  Flags: array[0..9] of PChar = (
    'createvalueifdoesntexist', 'uninsdeletevalue', 'uninsdeletekey',
    'uninsdeletekeyifempty', 'uninsclearvalue', 'preservestringtype',
    'deletekey', 'deletevalue', 'noerror', 'dontcreatekey');

  function ConvertBinaryString (const S: String): String;
    procedure Invalid;
    begin
      AbortCompileParamError (SCompilerParamInvalid2, ParamRegistryValueData);
    end;
  var
    I: Integer;
    C: Char;
    B: Byte;
    N: Integer;
    procedure EndByte;
    begin
      case N of
        0: ;
        2: begin
             Result := Result + Chr(B);
             N := 0;
             B := 0;
           end;
      else
        Invalid;
      end;
    end;
  begin
    Result := '';
    N := 0;
    B := 0;
    for I := 1 to Length(S) do begin
      C := UpCase(S[I]);
      case C of
        ' ': EndByte;
        '0'..'9': begin
               Inc (N);
               if N > 2 then
                 Invalid;
               B := (B shl 4) or (Ord(C) - Ord('0'));
             end;
        'A'..'F': begin
               Inc (N);
               if N > 2 then
                 Invalid;
               B := (B shl 4) or (10 + Ord(C) - Ord('A'));
             end;
      else
        Invalid;
      end;
    end;
    EndByte;
  end;

  function ConvertDWordString (const S: String): String;
  var
    DW: Longint;
    E: Integer;
  begin
    Val (S, DW, E);
    if E <> 0 then
      AbortCompileParamError (SCompilerParamInvalid2, ParamRegistryValueData);
    SetLength (Result, SizeOf(Longint));
    Longint((@Result[1])^) := DW;
  end;

var
  Params: TBreakStringArray;
  ParamNameFound: array[Low(ParamNames)..High(ParamNames)] of Boolean;
  NewRegistryEntry: PSetupRegistryEntry;
  P, I: Integer;
  AData: String;
begin
  FillChar (ParamNameFound, SizeOf(ParamNameFound), 0);
  BreakString (Line, Params);

  NewRegistryEntry := AllocMem(SizeOf(TSetupRegistryEntry));
  try
    with NewRegistryEntry^ do begin
      MinVersion := SetupHeader.MinVersion;

      for P := Low(Params) to High(Params) do
        with Params[P] do begin
          if ParamName = '' then Break;
          case CompareParamName(Params[P], ParamNames, ParamNameFound) of
            0: while True do
                 case ExtractFlag(ParamData, Flags) of
                   -2: Break;
                   -1: AbortCompileParamError (SCompilerParamUnknownFlag2, ParamCommonFlags);
                   0: Include (Options, roCreateValueIfDoesntExist);
                   1: Include (Options, roUninsDeleteValue);
                   2: Include (Options, roUninsDeleteEntireKey);
                   3: Include (Options, roUninsDeleteEntireKeyIfEmpty);
                   4: Include (Options, roUninsClearValue);
                   5: Include (Options, roPreserveStringType);
                   6: Include (Options, roDeleteKey);
                   7: Include (Options, roDeleteValue);
                   8: Include (Options, roNoError);
                   9: Include (Options, roDontCreateKey);
                 end;
            1: RootKey := Uppercase(Trim(ParamData));
            2: begin
                 if (ParamData <> '') and (ParamData[1] = '\') then
                   AbortCompileParamError (SCompilerParamNoPrecedingBackslash, ParamRegistrySubkey);
                 Subkey := RemoveBackslash(ParamData);
               end;
            3: begin
                 ParamData := Uppercase(Trim(ParamData));
                 if ParamData = 'NONE' then
                   Typ := rtNone
                 else if ParamData = 'STRING' then
                   Typ := rtString
                 else if ParamData = 'EXPANDSZ' then
                   Typ := rtExpandString
                 else if ParamData = 'MULTISZ' then
                   Typ := rtMultiString
                 else if ParamData = 'DWORD' then
                   Typ := rtDWord
                 else if ParamData = 'BINARY' then
                   Typ := rtBinary
                 else
                   AbortCompileParamError (SCompilerParamInvalid2, ParamRegistryValueType);
               end;
            4: ValueName := ParamData;
            5: AData := ParamData;
            6: while True do begin
                 I := ExtractComponent(ParamData, ComponentEntries);
                 case I of
                   -2: Break;
                   -1: AbortCompileParamError (SCompilerParamUnknownComponent, ParamCommonComponents);
                   else
                     AddToCommaText(Components, PSetupComponentEntry(ComponentEntries[I]).Name)
                 end;
               end;
            7: while True do begin
                 I := ExtractTask(ParamData, TaskEntries);
                 case I of
                   -2: Break;
                   -1: AbortCompileParamError (SCompilerParamUnknownTask, ParamCommonTasks);
                   else
                     AddToCommaText(Tasks, PSetupTaskEntry(TaskEntries[I]).Name)
                 end;
               end;
            8: if not StrToVersionNumbers(ParamData, MinVersion) then
                 AbortCompileParamError (SCompilerParamInvalid2, ParamCommonMinVersion);
            9:if not StrToVersionNumbers(ParamData, OnlyBelowVersion) then
                 AbortCompileParamError (SCompilerParamInvalid2, ParamCommonOnlyBelowVersion);
          end;
        end;

      if not ParamNameFound[1] then
        AbortCompileParamError (SCompilerParamNotSpecified, ParamRegistryRoot);
      if not ParamNameFound[2] then
        AbortCompileParamError (SCompilerParamNotSpecified, ParamRegistrySubkey);

      if (roUninsDeleteEntireKey in Options) and
         (roUninsDeleteEntireKeyIfEmpty in Options) then
        AbortCompileOnLineFmt (SCompilerParamErrorBadCombo2,
          [ParamCommonFlags, 'uninsdeletekey', 'uninsdeletekeyifempty']);
      if (roUninsDeleteEntireKey in Options) and
         (roUninsClearValue in Options) then
        AbortCompileOnLineFmt (SCompilerParamErrorBadCombo2,
          [ParamCommonFlags, 'uninsclearvalue', 'uninsdeletekey']);
      if (roUninsDeleteValue in Options) and
         (roUninsDeleteEntireKey in Options) then
        AbortCompileOnLineFmt (SCompilerParamErrorBadCombo2,
          [ParamCommonFlags, 'uninsdeletevalue', 'uninsdeletekey']);
      if (roUninsDeleteValue in Options) and
         (roUninsClearValue in Options) then
        AbortCompileOnLineFmt (SCompilerParamErrorBadCombo2,
          [ParamCommonFlags, 'uninsdeletevalue', 'uninsclearvalue']);
      if ((roUninsDeleteEntireKey in Options) or (roDeleteKey in Options)) and
         (RootKey <> 'HKEY') and (AnsiPos('\', Subkey) = 0) then
        AbortCompileOnLine (SCompilerRegistryCantDeleteKey);

      case Typ of
        rtString, rtExpandString, rtMultiString:
          ValueData := AData;
        rtDWord:
          ValueData := ConvertDWordString(AData);
        rtBinary:
          ValueData := ConvertBinaryString(AData);
      end;
    end;
  except
    SEFreeRec (NewRegistryEntry, SetupRegistryEntryStrings);
    raise;
  end;
  RegistryEntries.Add (NewRegistryEntry);
end;

procedure TSetupCompiler.EnumDelete (const Line: PChar; const Ext: Integer);
const
  ParamDeleteType = 'Type';
  ParamDeleteName = 'Name';
  ParamNames: array[0..5] of TParamInfo = (
    (Name: ParamDeleteType; Flags: []),
    (Name: ParamDeleteName; Flags: [piNoEmpty]),
    (Name: ParamCommonComponents; Flags: []),
    (Name: ParamCommonTasks; Flags: []),
    (Name: ParamCommonMinVersion; Flags: []),
    (Name: ParamCommonOnlyBelowVersion; Flags: []));
  Types: array[TSetupDeleteType] of PChar = (
    'files', 'filesandordirs', 'dirifempty');
var
  Params: TBreakStringArray;
  ParamNameFound: array[Low(ParamNames)..High(ParamNames)] of Boolean;
  NewDeleteEntry: PSetupDeleteEntry;
  P, I: Integer;
  Valid: Boolean;
  J: TSetupDeleteType;
begin
  FillChar (ParamNameFound, SizeOf(ParamNameFound), 0);
  BreakString (Line, Params);

  NewDeleteEntry := AllocMem(SizeOf(TSetupDeleteEntry));
  try
    with NewDeleteEntry^ do begin
      MinVersion := SetupHeader.MinVersion;

      for P := Low(Params) to High(Params) do
        with Params[P] do begin
          if ParamName = '' then Break;
          case CompareParamName(Params[P], ParamNames, ParamNameFound) of
            0: begin
                 ParamData := Trim(ParamData);
                 Valid := False;
                 for J := Low(J) to High(J) do
                   if StrIComp(Types[J], PChar(ParamData)) = 0 then begin
                     DeleteType := J;
                     Valid := True;
                     Break;
                   end;
                 if not Valid then
                   AbortCompileParamError (SCompilerParamInvalid2, ParamDeleteType);
               end;
            1: Name := ParamData;
            2: while True do begin
                 I := ExtractComponent(ParamData, ComponentEntries);
                 case I of
                   -2: Break;
                   -1: AbortCompileParamError (SCompilerParamUnknownComponent, ParamCommonComponents);
                   else
                     AddToCommaText(Components, PSetupComponentEntry(ComponentEntries[I]).Name)
                 end;
               end;
            3: while True do begin
                 I := ExtractTask(ParamData, TaskEntries);
                 case I of
                   -2: Break;
                   -1: AbortCompileParamError (SCompilerParamUnknownTask, ParamCommonTasks);
                   else
                     AddToCommaText(Tasks, PSetupTaskEntry(TaskEntries[I]).Name)
                 end;
               end;
            4: if not StrToVersionNumbers(ParamData, MinVersion) then
                 AbortCompileParamError (SCompilerParamInvalid2, ParamCommonMinVersion);
            5: if not StrToVersionNumbers(ParamData, OnlyBelowVersion) then
                 AbortCompileParamError (SCompilerParamInvalid2, ParamCommonOnlyBelowVersion);
          end;
        end;

      if not ParamNameFound[0] then
        AbortCompileParamError (SCompilerParamNotSpecified, ParamDeleteType);
      if not ParamNameFound[1] then
        AbortCompileParamError (SCompilerParamNotSpecified, ParamDeleteName);
    end;
  except
    SEFreeRec (NewDeleteEntry, SetupDeleteEntryStrings);
    raise;
  end;
  if Ext = 0 then begin
    InstallDeleteEntries.Add (NewDeleteEntry);
  end
  else begin
    UninstallDeleteEntries.Add (NewDeleteEntry);
  end;
end;

procedure TSetupCompiler.EnumFiles (const Line: PChar; const Ext: Integer);

  function EscapeBraces (const S: String): String;
  { Changes all '{' to '{{' }
  var
    I: Integer;
  begin
    Result := S;
    I := 1;
    while I <= Length(Result) do begin
      if Result[I] = '{' then begin
        Insert ('{', Result, I);
        Inc (I);
      end
      else if Result[I] in LeadBytes then
        Inc (I);
      Inc (I);
    end;
  end;

const
  ParamFilesSource = 'Source';
  ParamFilesDestDir = 'DestDir';
  ParamFilesDestName = 'DestName';
  ParamFilesCopyMode = 'CopyMode';
  ParamFilesAttribs = 'Attribs';
  ParamFilesFontInstall = 'FontInstall';
  ParamNames: array[0..10] of TParamInfo = (
    (Name: ParamCommonFlags; Flags: []),
    (Name: ParamFilesSource; Flags: [piNoEmpty, piNoQuotes]),
    (Name: ParamFilesDestDir; Flags: [piNoEmpty, piNoQuotes]),
    (Name: ParamFilesDestName; Flags: [piNoEmpty, piNoQuotes]),
    (Name: ParamFilesCopyMode; Flags: []),
    (Name: ParamFilesAttribs; Flags: []),
    (Name: ParamFilesFontInstall; Flags: [piNoEmpty]),
    (Name: ParamCommonComponents; Flags: []),
    (Name: ParamCommonTasks; Flags: []),
    (Name: ParamCommonMinVersion; Flags: []),
    (Name: ParamCommonOnlyBelowVersion; Flags: []));
  Flags: array[0..20] of PChar = (
    'confirmoverwrite', 'uninsneveruninstall', 'isreadme', 'regserver',
    'sharedfile', 'restartreplace', 'deleteafterinstall',
    'comparetimestamp', 'fontisnttruetype', 'regtypelib', 'external',
    'skipifsourcedoesntexist', 'overwritereadonly', 'onlyifdestfileexists',
    'recursesubdirs', 'noregerror', 'allowunsafefiles', 'uninsrestartdelete',
    'onlyifdoesntexist', 'ignoreversion', 'promptifolder');
  AttribsFlags: array[0..2] of PChar = (
    'readonly', 'hidden', 'system');
var
  Params: TBreakStringArray;
  ParamNameFound: array[Low(ParamNames)..High(ParamNames)] of Boolean;
  NewFileEntry: PSetupFileEntry;
  P, I: Integer;
  ADestDir, ADestName, AInstallFontName: String;
  IsReadmeFile: Boolean;
begin
  FillChar (ParamNameFound, SizeOf(ParamNameFound), 0);
  if Ext = 0 then
    BreakString (Line, Params);

  NewFileEntry := nil;
  try
    NewFileEntry := AllocMem(SizeOf(TSetupFileEntry));
    with NewFileEntry^ do begin
      MinVersion := SetupHeader.MinVersion;

    ADestName := '';
    ADestDir := '';
    AInstallFontName := '';
    IsReadmeFile := False;

    Attribs := '';

    for P := Low(Params) to High(Params) do
       with Params[P] do begin
         if ParamName = '' then Break;
         case CompareParamName(Params[P], ParamNames, ParamNameFound) of
           0: while True do
                case ExtractFlag(ParamData, Flags) of
                  -2: Break;
                  -1: AbortCompileParamError (SCompilerParamUnknownFlag2, ParamCommonFlags);
                  0: Include (Options, foConfirmOverwrite);
                  1: Include (Options, foUninsNeverUninstall);
                  2: IsReadmeFile := True;
                  3: Include (Options, foRegisterServer);
                  4: Include (Options, foSharedFile);
                  5: Include (Options, foRestartReplace);
                  6: Include (Options, foDeleteAfterInstall);
                  7: Include (Options, foCompareTimeStamp);
                  8: Include (Options, foFontIsntTrueType);
                  9: Include (Options, foRegisterTypeLib);
                  10: ExternalFile := True;
                  11: Include (Options, foSkipIfSourceDoesntExist);
                  12: Include (Options, foOverwriteReadOnly);
                  13: Include (Options, foOnlyIfDestFileExists);
                  15: Include (Options, foNoRegError);
                  17: Include (Options, foUninsRestartDelete);
                  18: Include (Options, foOnlyIfDoesntExist);
                  19: Include (Options, foIgnoreVersion);
                  20: Include (Options, foPromptIfOlder);
                end;
           1: NewFileEntry.SourceFilename := ParamData;
           2: ADestDir := ParamData;
           3: ADestName := ParamData;
           4: begin
                ParamData := Trim(ParamData);
                if CompareText(ParamData, 'normal') = 0 then begin
                  Include(Options, foPromptIfOlder);
                end
                else if CompareText(ParamData, 'onlyifdoesntexist') = 0 then begin
                  Include(Options, foOnlyIfDoesntExist);
                end
                else if CompareText(ParamData, 'alwaysoverwrite') = 0 then begin
                  Include(Options, foIgnoreVersion);
                end
                else if CompareText(ParamData, 'alwaysskipifsameorolder') = 0 then begin
                end
                else
                  AbortCompileParamError (SCompilerParamInvalid2, ParamFilesCopyMode);
              end;
           5: begin
                while True do
                case ExtractFlag(ParamData, AttribsFlags) of
                  -2: Break;
                  -1: AbortCompileParamError (SCompilerParamUnknownFlag2, ParamFilesAttribs);
                  0: Attribs := Attribs + ' READONLY ';
                  1: Attribs := Attribs + ' HIDDEN ';
                  2: Attribs := Attribs + ' SYSTEM ';
                end;
                Attribs := Trim(Attribs);
                StringChange(Attribs, '  ', '|');
              end;
           6: AInstallFontName := ParamData;
           7: while True do begin
                I := ExtractComponent(ParamData, ComponentEntries);
                case I of
                  -2: Break;
                  -1: AbortCompileParamError (SCompilerParamUnknownComponent, ParamCommonComponents);
                  else
                   AddToCommaText(Components, PSetupComponentEntry(ComponentEntries[I]).Name)
                end;
              end;
           8: while True do begin
                I := ExtractTask(ParamData, TaskEntries);
                case I of
                  -2: Break;
                  -1: AbortCompileParamError (SCompilerParamUnknownTask, ParamCommonTasks);
                  else
                    AddToCommaText(Tasks, PSetupTaskEntry(TaskEntries[I]).Name)
                end;
              end;
         end;
       end;

      if Ext = 0 then begin
        if not ParamNameFound[1] then
          AbortCompileParamError (SCompilerParamNotSpecified, ParamFilesSource);
        if not ParamNameFound[2] then
          AbortCompileParamError (SCompilerParamNotSpecified, ParamFilesDestDir);
      end;

      if (ADestDir = '{tmp}') or (Copy(ADestDir, 1, 4) = '{tmp}\') then
        Include (Options, foDeleteAfterInstall);
      if foDeleteAfterInstall in Options then begin
        if foRestartReplace in Options then
          AbortCompileOnLineFmt (SCompilerFilesTmpBadFlag, ['restartreplace']);
        if foUninsNeverUninstall in Options then
          AbortCompileOnLineFmt (SCompilerFilesTmpBadFlag, ['uninsneveruninstall']);
        if foRegisterServer in Options then
          AbortCompileOnLineFmt (SCompilerFilesTmpBadFlag, ['regserver']);
        if foRegisterTypeLib in Options then
          AbortCompileOnLineFmt (SCompilerFilesTmpBadFlag, ['regtypelib']);
        if foSharedFile in Options then
          AbortCompileOnLineFmt (SCompilerFilesTmpBadFlag, ['sharedfile']);
        Include (Options, foUninsNeverUninstall);
      end;

      {if foRestartReplace in Options then
        HasRestart := True;
      if (foRegisterServer in Options) or (foRegisterTypeLib in Options) then
        HasRegSvr := True;}

      {if AInstallFontName <> '' then begin
        if not(foFontIsntTrueType in Options) then
          AInstallFontName := AInstallFontName + ' (TrueType)';
        InstallFontName := AInstallFontName;
      end;}
      ADestDir := AddBackslash(ADestDir);
    end;

    if ADestName = '' then ADestName := ExtractFileName(NewFileEntry.SourceFilename);
    NewFileEntry.DestName := AddBackslash(ADestDir) + ADestName;
    if IsReadmeFile then ReadmeFile := NewFileEntry.DestName;
    FileEntries.Add(NewFileEntry);
  except
    SEFreeRec (NewFileEntry, SetupFileEntryStrings);
    raise;
  end;
end;

procedure TSetupCompiler.EnumRun (const Line: PChar; const Ext: Integer);
const
  ParamRunFilename = 'Filename';
  ParamRunParameters = 'Parameters';
  ParamRunWorkingDir = 'WorkingDir';
  ParamRunRunOnceId = 'RunOnceId';
  ParamRunStatusMsg = 'StatusMsg';
  ParamDescription = 'Description';
  ParamNames: array[0..10] of TParamInfo = (
    (Name: ParamCommonFlags; Flags: []),
    (Name: ParamRunFilename; Flags: [piNoEmpty, piNoQuotes]),
    (Name: ParamRunParameters; Flags: []),
    (Name: ParamRunWorkingDir; Flags: []),
    (Name: ParamRunRunOnceId; Flags: []),
    (Name: ParamDescription; Flags: []),
    (Name: ParamCommonComponents; Flags: []),
    (Name: ParamCommonTasks; Flags: []),
    (Name: ParamCommonMinVersion; Flags: []),
    (Name: ParamCommonOnlyBelowVersion; Flags: []),
    (Name: ParamRunStatusMsg; Flags: []));
  Flags: array[0..11] of PChar = (
    'nowait', 'waituntilidle', 'shellexec', 'skipifdoesntexist',
    'runminimized', 'runmaximized', 'showcheckbox', 'postinstall',
    'unchecked', 'skipifsilent', 'skipifnotsilent', 'hidewizard');
var
  Params: TBreakStringArray;
  ParamNameFound: array[Low(ParamNames)..High(ParamNames)] of Boolean;
  NewRunEntry: PSetupRunEntry;
  P, I: Integer;
begin
  FillChar (ParamNameFound, SizeOf(ParamNameFound), 0);
  BreakString (Line, Params);

  NewRunEntry := AllocMem(SizeOf(TSetupRunEntry));
  try
    with NewRunEntry^ do begin
      MinVersion := SetupHeader.MinVersion;
      ShowCmd := '';

      for P := Low(Params) to High(Params) do
        with Params[P] do begin
          if ParamName = '' then Break;
          case CompareParamName(Params[P], ParamNames, ParamNameFound) of
            0: while True do
                 case ExtractFlag(ParamData, Flags) of
                   -2: Break;
                   -1: AbortCompileParamError (SCompilerParamUnknownFlag2, ParamCommonFlags);
                   0: Wait := rwNoWait;
                   1: Wait := rwWaitUntilIdle;
                   2: Include (Options, roShellExec);
                   3: Include (Options, roSkipIfDoesntExist);
                   //4: ShowCmd := SW_SHOWMINNOACTIVE;
                   5: ShowCmd := 'SW_SHOWMAXIMIZED';
                   6: begin
                        if (Ext = 1) then
                          AbortCompileParamError (SCompilerParamUnsupportedFlag, ParamCommonFlags);
                        WarningsList.Add (Format(SCompilerRunFlagObsolete, ['showcheckbox', 'postinstall']));
                        Include (Options, roPostInstall);
                      end;
                   7: begin
                        if (Ext = 1) then
                          AbortCompileParamError (SCompilerParamUnsupportedFlag, ParamCommonFlags);
                        Include (Options, roPostInstall);
                      end;
                   8: begin
                        if (Ext = 1) then
                          AbortCompileParamError (SCompilerParamUnsupportedFlag, ParamCommonFlags);
                        Include (Options, roUnchecked);
                      end;
                   9: begin
                        if (Ext = 1) then
                          AbortCompileParamError (SCompilerParamUnsupportedFlag, ParamCommonFlags);
                        Include (Options, roSkipIfSilent);
                      end;
                   10: begin
                        if (Ext = 1) then
                          AbortCompileParamError (SCompilerParamUnsupportedFlag, ParamCommonFlags);
                        Include (Options, roSkipIfNotSilent);
                      end;
                   11: Include (Options, roHideWizard);
                 end;
            1: Name := ParamData;
            2: Parameters := ParamData;
            3: WorkingDir := ParamData;
            4: begin
                 if (Ext = 0) and (ParamData <> '') then
                   AbortCompileOnLine (SCompilerRunCantUseRunOnceId);
                 RunOnceId := ParamData;
               end;
            5: begin
                 if (Ext = 1) and (ParamData <> '') then
                   AbortCompileOnLine (SCompilerUninstallRunCantUseDescription);
                 Description := ParamData;
               end;
            6: while True do begin
                 I := ExtractComponent(ParamData, ComponentEntries);
                 case I of
                   -2: Break;
                   -1: AbortCompileParamError (SCompilerParamUnknownComponent, ParamCommonComponents);
                   else
                     AddToCommaText(Components, PSetupComponentEntry(ComponentEntries[I]).Name)
                 end;
               end;
            7: while True do begin
                 I := ExtractTask(ParamData, TaskEntries);
                 case I of
                   -2: Break;
                   -1: AbortCompileParamError (SCompilerParamUnknownTask, ParamCommonTasks);
                   else
                     AddToCommaText(Tasks, PSetupTaskEntry(TaskEntries[I]).Name)
                 end;
               end;
            8: if not StrToVersionNumbers(ParamData, MinVersion) then
                 AbortCompileParamError (SCompilerParamInvalid2, ParamCommonMinVersion);
            9: if not StrToVersionNumbers(ParamData, OnlyBelowVersion) then
                 AbortCompileParamError (SCompilerParamInvalid2, ParamCommonOnlyBelowVersion);
            10: StatusMsg := ParamData;
          end;
        end;

      if not ParamNameFound[1] then
        AbortCompileParamError (SCompilerParamNotSpecified, ParamRunFilename);

    end;
  except
    SEFreeRec (NewRunEntry, SetupRunEntryStrings);
    raise;
  end;
  if Ext = 0 then begin
    RunEntries.Add(NewRunEntry)
  end
  else begin
    UninstallRunEntries.Add(NewRunEntry);
  end;
end;

procedure TSetupCompiler.Compile;

  procedure FreeListItems (const List: TList; const NumStrings: Integer);
  var
    I: Integer;
  begin
    for I := List.Count-1 downto 0 do begin
      SEFreeRec (List[I], NumStrings);
      List.Delete (I);
    end;
  end;

  procedure FreeScriptFiles;
  var
    I: Integer;
    SL: TStringList;
  begin
    for I := ScriptFiles.Count-1 downto 0 do begin
      SL := TStringList(ScriptFiles.Objects[I]);
      ScriptFiles.Delete(I);
      SL.Free;
    end;
  end;

{var
  LicenseText, InfoBeforeText, InfoAfterText: String;}

begin
  LineNumber := 0;
  ParseFilenameStack.Clear;

  try
    Finalize (SetupHeader);
    FillChar (SetupHeader, SizeOf(SetupHeader), 0);

    { Initialize defaults }
    OriginalSourceDir := AddBackslash(ExpandFileName(SourceDir));
    OutputDir := 'Output';
    OutputBaseFilename := 'setup';
    CompressLevel := 7;

    SetupHeader.Options := [shCreateAppDir, shUninstallable,
      shWindowStartMaximized, shWindowShowCaption, shWindowResizable,
      shCreateUninstallRegKey, shUsePreviousAppDir, shUsePreviousGroup,
      shUsePreviousSetupType, shAlwaysShowComponentsList, shFlatComponentsList,
      shShowComponentSizes, shUsePreviousTasks, shUpdateUninstallLogAppName,
      shAllowUNCPath, shUsePreviousUserInfo, shRestartIfNeededByRun];

    SetupHeader.UninstallFilesDir := '{app}';
    SetupHeader.DefaultUserInfoName := '{sysuserinfoname}';
    SetupHeader.DefaultUserInfoOrg := '{sysuserinfoorg}';
    SetupHeader.BackColor := clBlue;
    SetupHeader.BackColor2 := clBlack;

    BackSolid := False;

    SetupHeader.WizardImageBackColor := $400000;
    SetupHeader.WizardSmallImageBackColor := clWhite;
    SetupHeader.UninstallStyle := usModern;

    WizardImageFile := '';
    WizardSmallImageFile := '';

    { Read [Setup] section }
    EnumIniSection (EnumSetup, 'Setup', 0, True, '');

    { Verify settings set in [Setup] section }
    SetupHeader.BaseFilename := OutputBaseFilename;
    if SetupDirectiveLines[ssAppName] = 0 then
      AbortCompileFmt (SCompilerEntryMissing2, ['Setup', 'AppName']);
    if SetupDirectiveLines[ssAppVerName] = 0 then
      AbortCompileFmt (SCompilerEntryMissing2, ['Setup', 'AppVerName']);
    if SetupHeader.AppId = '' then
      SetupHeader.AppId := SetupHeader.AppName;
    if SetupHeader.UninstallDisplayName = '' then
      SetupHeader.UninstallDisplayName := SetupHeader.AppId;
    LineNumber := SetupDirectiveLines[ssDefaultDirName];

    UninstallerExe := AddBackSlash(SetupHeader.UninstallFilesDir) + 'uninstall.exe';

    if SetupHeader.DefaultDirName = '' then begin
      if shCreateAppDir in SetupHeader.Options then
        AbortCompileFmt (SCompilerEntryMissing2, ['Setup', 'DefaultDirName'])
      else
        SetupHeader.DefaultDirName := '?ERROR?';
    end;
    LineNumber := SetupDirectiveLines[ssDefaultGroupName];

    if SetupHeader.DefaultGroupName = '' then
      SetupHeader.DefaultGroupName := '(Default)';

    LineNumber := SetupDirectiveLines[ssUninstallDisplayIcon];
    LineNumber := SetupDirectiveLines[ssUninstallFilesDir];
    LineNumber := SetupDirectiveLines[ssDefaultUserInfoName];
    LineNumber := SetupDirectiveLines[ssDefaultUserInfoOrg];

    if BackSolid then
      SetupHeader.BackColor2 := SetupHeader.BackColor;
    LineNumber := 0;

    SourceDir := AddBackslash(ExpandFileName(SourceDir));
    if IsRelativePath(OutputDir) then
      OutputDir := SourceDir + OutputDir;
    //OutputDir := ExpandFileName(OutputDir);
    OutputDir := AddBackslash(OutputDir);

    EnumIniSection (EnumTypes, 'Types', 0, True, '');
    { Read [Components] section }
    EnumIniSection (EnumComponents, 'Components', 0, True, '');
    { Read [Tasks] section }
    EnumIniSection (EnumTasks, 'Tasks', 0, True, '');
    { Read [Dirs] section }
    EnumIniSection (EnumDirs, 'Dirs', 0, True, '');
    { Read [Icons] section }
    EnumIniSection (EnumIcons, 'Icons', 0, True, '');
    { Read [INI] section }
    EnumIniSection (EnumINI, 'INI', 0, True, '');
    { Read [Registry] section }
    EnumIniSection (EnumRegistry, 'Registry', 0, True, '');
    { Read [InstallDelete] section }
    EnumIniSection (EnumDelete, 'InstallDelete', 0, True, '');
    { Read [UninstallDelete] section }
    EnumIniSection (EnumDelete, 'UninstallDelete', 1, True, '');
    { Read [Run] section }
    EnumIniSection (EnumRun, 'Run', 0, True, '');
    { Read [UninstallRun] section }
    EnumIniSection (EnumRun, 'UninstallRun', 1, True, '');
    { Read [Files] section }
    EnumIniSection (EnumFiles, 'Files', 0, True, '');
    { Generate the NSIS script }
    GenerateNSISScript;

    asm jmp @1; db 0,'Inno Setup Compiler, Copyright (C) 1998-2002 Jordan Russell',0; @1: end;
    { Note: Removing or modifying the copyright text is a violation of the
      Inno Setup license agreement; see LICENSE.TXT. }
  finally
    WarningsList.Clear;
    { Free all the data }
    FreeListItems (TypeEntries, SetupTypeEntryStrings);
    FreeListItems (ComponentEntries, SetupComponentEntryStrings);
    FreeListItems (TaskEntries, SetupTaskEntryStrings);
    FreeListItems (DirEntries, SetupDirEntryStrings);
    FreeListItems (FileEntries, SetupFileEntryStrings);
    FreeListItems (FileLocationEntries, SetupFileLocationEntryStrings);
    FreeListItems (IconEntries, SetupIconEntryStrings);
    FreeListItems (IniEntries, SetupIniEntryStrings);
    FreeListItems (RegistryEntries, SetupRegistryEntryStrings);
    FreeListItems (InstallDeleteEntries, SetupDeleteEntryStrings);
    FreeListItems (UninstallDeleteEntries, SetupDeleteEntryStrings);
    FreeListItems (RunEntries, SetupRunEntryStrings);
    FreeListItems (UninstallRunEntries, SetupRunEntryStrings);
    FileLocationEntryFilenames.Clear;
    FreeScriptFiles;
  end;
end;

procedure TSetupCompiler.GenerateNSISScript;
var
  UninstallDirs, InstDelDirs, UninstallFiles,
  InstDelFiles, UninstIni, UninstReg: TStrings;
  S2, S, OutPath: String;
  I: Integer;
  HaveRegOldValue: Boolean;

  function IssConst2NsiVar(const IssConst: String): String;
  type
    TConstVarRec = record
      Iss, Nsi: String;
    end;
  const
    Consts: array[0..38] of TConstVarRec = ((Iss: '{src}'; Nsi: '$EXEDIR';),
    (Iss: '{srcexe}'; Nsi: '{srcexe}'), (Iss: '{tmp}'; Nsi: '$TEMP'),
    (Iss: '{app}'; Nsi: '$INSTDIR'), (Iss: '{win}'; Nsi: '$WINDIR'),
    (Iss: '{sys}'; Nsi: '$SYSDIR'), (Iss: '{sd}'; Nsi: '{sd}'),
    (Iss: '{groupname}'; Nsi: ''), (Iss: '{fonts}'; Nsi: '{fonts}'),
    (Iss: '{hwnd}'; Nsi: '$HWNDPARENT'), (Iss: '{pf}'; Nsi: '$PROGRAMFILES'),
    (Iss: '{cf}'; Nsi: '$PROGRAMFILES\Common files'), (Iss: '{computername}'; Nsi: '{computername}'),
    (Iss: '{dao}'; Nsi: '{dao}'), (Iss: '{cmd}'; Nsi: '{cmd}'), (Iss: '{username}'; Nsi: '{username}'),
    (Iss: '{wizardhwnd}'; Nsi: '$HWNDPARENT'), (Iss: '{sysuserinfoname}'; Nsi: '{sysuserinfoname}'),
    (Iss:'{sysuserinfoorg}'; Nsi: 'sysuserinfoorg'), (Iss: '{userinfoname}'; Nsi: 'userinfoname'),
    (Iss: '{userinfoorg}'; Nsi: 'userinfoorg'), (Iss: '{uninstallexe}'; Nsi: ''),
    (Iss: '{group}'; Nsi: ''),
    (Iss: '{userdesktop}'; Nsi: '$DESKTOP'), (Iss: '{userstartmenu}'; Nsi: '$STARTMENU'),
    (Iss: '{userprograms}'; Nsi: '$SMPROGRAMS'), (Iss: '{userstartup}'; Nsi: '$SMSTARTUP'),
    (Iss: '{commondesktop}'; Nsi: '$DESKTOP'), (Iss: '{commonstartmenu}'; Nsi: '$STARTMENU'),
    (Iss: '{commonprograms}'; Nsi: '$SMPROGRAMS'), (Iss: '{commonstartup}'; Nsi: '$SMSTARTUP'),
    (Iss: '{sendto}'; Nsi: '{sendto}'), (Iss: '{userappdata}'; Nsi: '{userappdata}'),
    (Iss: '{userdocs}'; Nsi: '{userdocs}'), (Iss: '{commonappdata}'; Nsi: '{commonappdata}'),
    (Iss: '{commondocs}'; Nsi: '{commondocs}'), (Iss: '{usertemplates}'; Nsi: '{usertemplates}'),
    (Iss: '{commontemplates}'; Nsi: '{commontemplates}'), (Iss: '{localappdata}'; Nsi: '{localappdata}'));
  var
    C: Integer;
  begin
    Result := IssConst;
    for C := 0 to 38 do
    with Consts[C] do
    begin
     if Iss = '{uninstallexe}' then
       Nsi := UninstallerExe
     else if (Iss = '{group}') or (Iss = '{groupname}') then
     begin
       if shDisableProgramGroupPage in SetupHeader.Options then
         Nsi := SetupHeader.DefaultGroupName
       else
         Nsi := '${MUI_STARTMENUPAGE_VARIABLE}';
       if Iss = '{group}' then
         Nsi := '$SMPROGRAMS\' + Nsi;
     end;
     StringChange(Result, Iss, Nsi);
    end;
  end;

  function ColorToHTML(const Color: TColor): String;
  begin
    Result := Format('%.2x%.2x%.2x', [GetRValue(Color),
       GetGValue(Color), GetBValue(Color)]);
  end;

  procedure OpenSection(const SectionName: String);
  begin
    NSISLines.Add('Section ' + SectionName);
  end;

  procedure CloseSection;
  begin
    NSISLines.Add('SectionEnd');
    NSISLines.Add('');
  end;

  procedure AddFormat(const S: String; Params: array of const);
  begin
    NSISLines.Add(Format(S, Params));
  end;

  function CoteStr(const S: String): String;
  var
    Cote: Char;
    HaveSQ, HaveDQ: Boolean;
  begin
    Cote := '"';
    HaveDQ := AnsiPos('"', S) > 0;
    HaveSQ := AnsiPos(#39, S) > 0;
    if HaveDQ then
    begin
      if not HaveSQ then
        Cote := #39
      else
        Cote := '`';
    end else
    if HaveSQ then
    begin
      if not HaveDQ then
        Cote := '"'
      else
        Cote := '`';
    end;
    Result := AnsiQuotedStr(S, Cote);
  end;


  procedure Define(const Simbol, Value: String);
  begin
    if Value <> '' then
      AddFormat('!define %s %s', [Simbol, CoteStr(Value)])
    else
      AddFormat('!define %s', [Simbol]);
  end;

  procedure AddNSIDeleteEntry(const FileToDelete: String);
  begin
    AddFormat('  Delete %s', [CoteStr(FileToDelete)]);
    if CompareText(ExtractFileExt(FileToDelete), '.HLP') = 0 then
    begin
      AddFormat('  Delete %s', [CoteStr(ChangeFileExt(FileToDelete, '.GID'))]);
      AddFormat('  Delete %s', [CoteStr(ChangeFileExt(FileToDelete, '.FTS'))]);
    end;
  end;

  procedure AddDeleteEntry(Entry: Pointer);
  begin
    with TSetupDeleteEntry(Entry^) do
    begin
      Name := IssConst2NsiVar(Name);
      case DeleteType of
        dfFiles: AddNSIDeleteEntry(Name);
        dfFilesAndOrSubdirs: AddFormat('  RMDir /r %s', [CoteStr(Name)]);
        dfDirIfEmpty: AddFormat('  RMDir %s', [CoteStr(Name)]);
      end;
    end;
  end;

  procedure AddRunEntry(Entry: Pointer);
  var
    CmdLine: String;
  begin
    with TSetupRunEntry(Entry^) do
    begin
      Name := IssConst2NsiVar(Name);
      Parameters := IssConst2NsiVar(Parameters);
      if Parameters <> '' then
        CmdLine := '"' + Name + '" ' + Parameters
      else
        CmdLine := Name;
      CmdLine := CoteStr(CmdLine);
      if Wait = rwWaitUntilTerminated then
      begin
        if StatusMsg <> '' then
          AddFormat('  DetailPrint %s', [CoteStr(StatusMsg)]);
        AddFormat('  ExecWait %s', [CmdLine])
      end else
      begin
        if roShellExec in Options then
          AddFormat('  ShellExec %s %s %s', [CoteStr(Name), CoteStr(Parameters), ShowCmd])
        else
          AddFormat('  Exec %s', [CmdLine]);
      end;
    end;
  end;

  // Add S to the top of List if S does not exists
  procedure AddStr(Lines: TStrings; const S: String);
  begin
    if Lines.IndexOf(S) < 0 then
      Lines.Insert(0, S);
  end;

  procedure AddStrFormat(Lines: TStrings; const S: String; Args: array of const);
  begin
    AddStr(Lines, Format(S, Args));
  end;

  procedure AddInfoPage(const S: String);
  begin
    NSISLines.Add('!define MUI_PAGE_HEADER_TEXT "Information"');
    NSISLines.Add('!define MUI_PAGE_HEADER_SUBTEXT "Please read the following important information before continuing."');
    AddFormat('!define MUI_LICENSEPAGE_TEXT_TOP %s', [CoteStr('When you are ready to continue with Setup, click Next.')]);
    NSISLines.Add('!define MUI_LICENSEPAGE_TEXT "'' '' ''&Next''"');
    AddFormat('!insertmacro MUI_PAGE_LICENSE %s', [CoteStr(S)]);
  end;

begin
  OutPath := '';
  UninstallerExe := IssConst2NsiVar(UninstallerExe);

  NSISLines.BeginUpdate;
  try
    UninstallDirs := TStringList.Create;
    InstDelDirs := TStringList.Create;
    UninstallFiles := TStringList.Create;
    InstDelFiles := TStringList.Create;
    UninstIni := TStringList.Create;
    UninstReg := TStringList.Create;
    try
      if IsLibrary then
        NSISLines.Add('; Script generated by the ISS2NSI pluging.')
      else
        NSISLines.Add('; Script generated by the ISS2NSI program.');
      NSISLines.Add('; Generated from: ' + ISSSourceFile);

      { This neet to be fixed }
      if TypeEntries.Count > 0 then
        NSISLines.Add('; WARNING! Type entries currently not supported.');
      if ComponentEntries.Count > 0 then
        NSISLines.Add('; WARNING! Component entries currently not supported.');
      if TaskEntries.Count > 0 then
        NSISLines.Add('; WARNING! Task entries currently not supported.');

      NSISLines.Add('');
      if SourceDirSet then
      begin
        AddFormat('!cd "%s"', [SourceDir]);
        NSISLines.Add('');
      end;

      NSISLines.Add('; MUI 1.66 compatible');      
      NSISLines.Add('!include "MUI.nsh"');
      NSISLines.Add('');

      if CompressWithBzip then
      begin
       NSISLines.Add('SetCompressor bzip2');
       NSISLines.Add('');
      end;

      NSISLines.Add('!define MUI_ABORTWARNING');
      if WizardImageFile <> '' then
        NSISLines.Add(Format('!define MUI_SPECIALBITMAP %s', [CoteStr(WizardImageFile)]));

      NSISLines.Add('!insertmacro MUI_PAGE_WELCOME');
      if LicenseFile <> '' then
      begin
        NSISLines.Add('!define MUI_LICENSEPAGE_RADIOBUTTONS');
        Addformat('!insertmacro MUI_PAGE_LICENSE %s', [CoteStr(LicenseFile)]);
      end;

      if InfoBeforeFile <> '' then
        AddInfoPage(InfoBeforeFile);

      if (not (shDisableProgramGroupPage in SetupHeader.Options)) and
        (IconEntries.Count > 0) then
      begin
        AddFormat('!define MUI_STARTMENUPAGE_DEFAULTFOLDER %s',
          [CoteStr(SetupHeader.DefaultGroupName)]);
        NSISLines.Add('!define MUI_STARTMENUPAGE_REGISTRY_ROOT "HKLM"');
        NSISLines.Add('!define MUI_STARTMENUPAGE_REGISTRY_KEY ' +
          CoteStr('Software\Microsoft\Windows\CurrentVersion\Uninstall\%s'+ SetupHeader.AppId));
        NSISLines.Add('!define MUI_STARTMENUPAGE_REGISTRY_VALUENAME "NSIS:StartMenuDir"');
        AddStr(UninstReg, '  DeleteRegValue ${MUI_STARTMENUPAGE_REGISTRY_ROOT} "${MUI_STARTMENUPAGE_REGISTRY_KEY}" "${MUI_STARTMENUPAGE_REGISTRY_VALUENAME}"');
        if not (shAllowNoIcons in SetupHeader.Options) then
          NSISLines.Add('!define MUI_STARTMENUPAGE_NODISABLE');
        NSISLines.Add('!insertmacro MUI_PAGE_STARTMENU');
      end;

      if not (shDisableDirPage in SetupHeader.Options) then
        NSISLines.Add('!insertmacro MUI_PAGE_DIRECTORY');

      NSISLines.Add('!insertmacro MUI_PAGE_INSTFILES');

      if InfoAfterFile <> '' then
        AddInfoPage(InfoAfterFile);

      if not (shDisableFinishedPage in SetupHeader.Options) then
      begin
        if ReadmeFile <> '' then
          AddFormat('!define MUI_FINISHPAGE_SHOWREADME %s', [CoteStr(IssConst2NsiVar(ReadmeFile))]);
        NSISLines.Add('!insertmacro MUI_PAGE_FINISH');
      end;

      if shUninstallable in SetupHeader.Options then
        NSISLines.Add('!insertmacro MUI_UNPAGE_INSTFILES');

      NSISLines.Add('!insertmacro MUI_LANGUAGE "English"');
      NSISLines.Add('');

      AddFormat('Name %s', [CoteStr(Trim(SetupHeader.AppName + ' ' + SetupHeader.AppVersion))]);
      AddFormat('BrandingText %s', [CoteStr(SetupHeader.AppCopyright)]);
      AddFormat('OutFile %s', [CoteStr(OutputDir + OutputBaseFilename + '.exe')]);
      AddFormat('InstallDir %s', [CoteStr(IssConst2NsiVar(SetupHeader.DefaultDirName))]);
      if shAllowRootDirectory in Setupheader.Options then
        NSISLines.Add('AllowRootDirInstall true');

      if shWindowVisible in SetupHeader.Options then
        AddFormat('BGGradient %s %s', [ColorToHTML(SetupHeader.BackColor),
           ColorToHTML(SetupHeader.BackColor2)]);

      NSISLines.Add('ShowInstDetails show');
      if shUninstallable in SetupHeader.Options then
        NSISLines.Add('ShowUnInstDetails show');
      if CompressLevel = 0 then
        NSISLines.Add('SetCompress off');
      if DontMergeDuplicateFiles then
        NSISLines.Add('SetDatablockOptimize off');
      NSISLines.Add('');

      if not (shDisableStartupPrompt in SetupHeader.Options) then
      begin
        NSISLines.Add('Function .onInit');
        NSISLines.Add('  MessageBox MB_YESNO|MB_ICONQUESTION "This will install $(^Name). Do you wish to continue?" IDYES +2');
        NSISLines.Add('  Abort');
        NSISLines.Add('FunctionEnd');
        NSISLines.Add('');
      end;

      { [InstallDelete] is processed. }
      if InstallDeleteEntries.Count > 0 then
      begin
        OpenSection('-InstallDelete');
        for I := 0 to InstallDeleteEntries.Count - 1 do
          AddDeleteEntry(InstallDeleteEntries[I]);
        CloseSection;
      end;

      { The entries in [UninstallDelete] are stored in the uninstall log. }

      { The application directory is created, if necessary. }

      { [Dirs] is processed. }
      if DirEntries.Count > 0 then
      begin
        OpenSection('-Dirs');
        for I := 0 to DirEntries.Count - 1 do
        with TSetupDirEntry(DirEntries[I]^) do
        begin
          DirName := CoteStr(IssConst2NsiVar(DirName));
          AddFormat('  CreateDirectory %s', [DirName]);
          if Attribs <> '' then
            AddFormat('  SetFileAttributes %s %s', [DirName, Attribs]);

          if not (doUninsNeverUninstall in Options) then
            AddStrFormat(UninstallDirs, '  RMDir %s', [DirName]);
          if doDeleteAfterInstall in Options then
             AddStrFormat(InstDelDirs, '  RMDir %s', [DirName])
        end;
        CloseSection;
      end;

      { A filename for the uninstall log is reserved, if necessary. }

      { [Files] is processed. }
      OpenSection('-Files');
      for I := 0 to FileEntries.Count - 1 do
      with TSetupFileEntry(FileEntries[I]^) do
      begin
        DestName := IssConst2NsiVar(DestName);
        if not ExternalFile then
        begin
          if OutPath <> ExtractFileDir(DestName) then
          begin
            OutPath := ExtractFileDir(DestName);
            AddStrFormat(UninstallDirs, '  RMDir %s', [CoteStr(OutPath)]);
            AddFormat('  SetOutPath %s', [CoteStr(OutPath)]);
          end;

          if CompareText(ExtractFileName(DestName), ExtractFileName(SourceFileName)) = 0 then
            AddFormat('  File %s', [CoteStr(SourceFilename)])
          else begin
            S := '/oname=' + ExtractFileName(DestName);
            if AnsiPos(' ', S) > 0 then
              S := '"' + S + '"';
            AddFormat('  File %s %s', [S, CoteStr(SourceFilename)]);
          end;
          if Attribs <> '' then
            AddFormat('  SetFileAttributes %s %s', [CoteStr(DestName), Attribs]);
        end else
          AddFormat('  CopyFiles /silent %s %s', [CoteStr(IssConst2NsiVar(SourceFileName)),
             CoteStr(DestName)]);

        if foDeleteAfterInstall in Options then
          AddStrFormat(InstDelFiles, '  Delete %s', [CoteStr(DestName)])
        else begin
          if foRegisterServer in Options then
            AddFormat('  RegDLL %s', [CoteStr(DestName)]);
          if not (foUninsNeverUninstall in Options) then
          begin
            if foUninsRestartDelete in Options then
              AddStrFormat(UninstallFiles, '  Delete /REBOOTOK %s', [CoteStr(DestName)])
            else
              AddStrFormat(UninstallFiles, '  Delete %s', [CoteStr(DestName)]);
            if foRegisterServer in Options then
              AddStrFormat(UninstallFiles, '  UnRegDLL %s ; <- Please check if you really want this here',
                [CoteStr(DestName)]);
          end;
        end;
      end;
      CloseSection;

      { [Icons] is processed. }
      if IconEntries.Count > 0 then
      begin
        OpenSection('-Icons');
        if not (shDisableProgramGroupPage in SetupHeader.Options) then
          NSISLines.Add('!insertmacro MUI_STARTMENU_WRITE_BEGIN');
        for I := 0 to IconEntries.Count - 1 do
        with TSetupIconEntry(IconEntries[I]^) do
        begin
          if shDisableProgramGroupPage in SetupHeader.Options then
             StringChange(IconName, '{group}', '$SMPROGRAMS\' + SetupHeader.DefaultGroupName);
          IconName := IssConst2NsiVar(IconName) + '.lnk';

          S := CoteStr(ExtractFileDir(IconName));
          S2 := Format('  RMDir %s', [S]);
          if UninstallDirs.IndexOf(S2) < 0 then
          begin
            AddFormat('  CreateDirectory %s', [S]);
            AddStr(UninstallDirs, S2);
          end;

          S := '';
          if (Parameters <> '') or (IconFileName <> '') or (Comment <> '') or
           (ShowCmd <> '""') or (HotKey <> '""') then
             S := CoteStr(IssConst2NsiVar(Parameters)) + ' ' +
             CoteStr(IssConst2NsiVar(IconFileName)) + ' ' + IconIndex +
             ' ' + ShowCmd + ' ' + HotKey + ' ' + CoteStr(Comment);

          AddFormat('  CreateShortCut %s %s %s', [CoteStr(IconName),
            CoteStr(IssConst2NsiVar(Filename)), S]);
          AddStrFormat(UninstallFiles, '  Delete %s', [CoteStr(IconName)]);
        end;
        if not (shDisableProgramGroupPage in SetupHeader.Options) then
          NSISLines.Add('!insertmacro MUI_STARTMENU_WRITE_END');
        CloseSection;
      end;

      { [INI] is processed. }
      if IniEntries.Count > 0 then
      begin
        OpenSection('-INI');
        for I := 0 to IniEntries.Count - 1 do
        with TSetupIniEntry(IniEntries[I]^) do
        begin
          FileName := CoteStr(IssConst2NsiVar(FileName));
          Section := CoteStr(IssConst2NsiVar(Section));
          Entry := CoteStr(IssConst2NsiVar(Entry));
          Value := CoteStr(IssConst2NsiVar(Value));
          AddFormat('  WriteIniStr %s %s %s %s', [Filename, Section,
            Entry, Value]);
          if ioUninsDeleteEntry in Options then
            AddStrFormat(UninstIni, '  DeleteINIStr %s %s %s',
              [FileName, Section, Entry]);
          if ioUninsDeleteEntireSection in Options then
            AddStrFormat(UninstIni, '  DeleteINISec %s %s', [FileName, Section]);
        end;
        CloseSection;
      end;

      { [Registry] is processed. }
      if RegistryEntries.Count > 0 then
      begin
        OpenSection('-Registry');
        HaveRegOldValue := False;
        for I := 0  to RegistryEntries.Count - 1 do
        with TSetupRegistryEntry(RegistryEntries[I]^) do
        if Pos('{olddata}', ValueData) > 0 then
        begin
          HaveRegOldValue := True;
          Break;
        end;
        if HaveRegOldValue then NSISLines.Add('  Push $R0');
        for I := 0 to RegistryEntries.Count - 1 do
        with TSetupRegistryEntry(RegistryEntries[I]^) do
        begin
          SubKey := CoteStr(IssConst2NsiVar(SubKey));
          ValueName := CoteStr(IssConst2NsiVar(ValueName));
          ValueData := CoteStr(IssConst2NsiVar(ValueData));
          if Pos('{olddata}', ValueData) > 0 then
          begin
            case Typ of
              rtString, rtExpandString: AddFormat('  ReadRegStr $R0 %s %s %s', [RootKey,
                Subkey, ValueName]);
              rtDWord: AddFormat('  ReadRegDWord $R0 %s %s %s', [RootKey,
                Subkey, ValueName]);
            end;
            StringChange(ValueData, '{olddata}', '$R0');
          end;

          if roDeleteValue in Options then
            AddFormat('  DeleteRegValue %s %s %s', [RootKey, Subkey, ValueName]);
          if roDeleteKey in Options then
            AddFormat('  DeleteRegKey %s %s',[RootKey,Subkey]);

          case Typ of
            rtNone: AddFormat('  WriteRegStr %s %s "" ""', [RootKey,Subkey]);
            rtString: AddFormat('  WriteRegStr %s %s %s %s', [RootKey,
              Subkey, ValueName, ValueData]);
            rtExpandString: AddFormat('  WriteRegExpandStr %s %s %s %s', [RootKey,
              Subkey, ValueName, ValueData]);
            rtDWord: AddFormat('  WriteRegDWord %s %s %s %s', [RootKey,
              Subkey, ValueName, ValueData]);
            rtBinary: AddFormat('  WriteRegBin %s %s %s %s', [RootKey,
              Subkey, ValueName, ValueData]);
            rtMultiString: AddFormat('  WriteRegStr %s %s %s %s', [RootKey,
              Subkey, ValueName, ValueData]);
            (* TODO: Convert {break} to Null character *)
            end;

            if roUninsDeleteEntireKey in Options then
              AddStrFormat(UninstReg, '  DeleteRegKey %s %s',[RootKey,Subkey]) else
            if roUninsDeleteEntireKeyIfEmpty in Options then
              AddStrFormat(UninstReg, '  DeleteRegKey /ifempty %s %s',[RootKey,Subkey]) else
            if roUninsDeleteValue in Options then
              AddStrFormat(UninstReg, '  DeleteRegValue %s %s %s',[RootKey,Subkey,ValueName]);
        end;
        if HaveRegOldValue then NSISLines.Add('  Pop $R0');
        CloseSection;
      end;

      { Files that needed to be registered are now registered. }

      { The Add/Remove Programs entry for the program is created}

      { The entries in [UninstallRun] are stored in the uninstall log. }

      { The uninstaller EXE and log are finalized and saved to disk. }

      { If ChangesAssociations was set to yes, file associations are refreshed now. }

      if (shChangesAssociations in SetupHeader.Options) or
        (shCreateUninstallRegKey in SetupHeader.Options) or (shUninstallable in SetupHeader.Options) then
      begin
        OpenSection('-PostInstall');
        if (shCreateUninstallRegKey in SetupHeader.Options) or (shUninstallable in SetupHeader.Options) then
        begin
          S := 'HKLM ' + CoteStr('Software\Microsoft\Windows\CurrentVersion\Uninstall\'+ SetupHeader.AppId);
          AddFormat('  WriteRegStr %s "DisplayName" %s', [S, CoteStr(SetupHeader.UninstallDisplayName)]);
          AddFormat('  WriteRegStr %s "UninstallString" %s', [S, CoteStr(UninstallerExe)]);
          if SetupHeader.UninstallDisplayIcon <> '' then
            AddFormat('  WriteRegStr %s "DisplayIcon" %s', [S, CoteStr(SetupHeader.UninstallDisplayIcon)]);
          if SetupHeader.AppPublisher <> '' then
            AddFormat('  WriteRegStr %s "Publisher" %s', [S, CoteStr(SetupHeader.AppPublisher)]);
          if SetupHeader.AppPublisherURL <> '' then
            AddFormat('  WriteRegStr %s "URLInfoAbout" %s', [S, CoteStr(SetupHeader.AppPublisherURL)]);
          if SetupHeader.AppSupportURL <> '' then
            AddFormat('  WriteRegStr %s "HelpLink" %s', [S, CoteStr(SetupHeader.AppSupportURL)]);
          if SetupHeader.AppUpdatesURL <> '' then
            AddFormat('  WriteRegStr %s "URLUpdateInfo" %s', [S, CoteStr(SetupHeader.AppUpdatesURL)]);
          if SetupHeader.AppVersion <> '' then
            AddFormat('  WriteRegStr %s "DisplayVersion" %s', [S, CoteStr(SetupHeader.AppVersion)]);
          AddStr(UninstReg, '  DeleteRegKey ' + S);
        end;
        if (shUninstallable in SetupHeader.Options) then
        begin
          AddFormat('  WriteUninstaller %s', [CoteStr(UninstallerExe)]);
          AddStrFormat(UninstallDirs, '  RMDir %s', [CoteStr(ExtractFileDir(UninstallerExe))]);
          AddStrFormat(UninstallFiles, '  Delete %s', [CoteStr(UninstallerExe)]);
        end;
        if (shChangesAssociations in SetupHeader.Options) then
          NSISLines.Add('  System::Call "shell32.dll::SHChangeNotify(l, l, i, i) v (0x08000000, 0, 0, 0)"');
        CloseSection;
      end;

      { [Run] is processed. }
      if RunEntries.Count > 0 then
      begin
        OpenSection('-Run');
        for I := 0 to RunEntries.Count - 1 do
          AddRunEntry(RunEntries[I]);
        CloseSection;
      end;

      if (InstDelDirs.Count > 0) or (InstDelFiles.Count > 0) then
      begin
         OpenSection('-InstallDeleteTemp');
         NSISLines.AddStrings(InstDelFiles);
         if InstDelDirs.Count > 0 then NSISLines.Add('');
         NSISLines.AddStrings(InstDelDirs);
         CloseSection;
      end;

      if shUninstallable in SetupHeader.Options then
      begin
        NSISLines.Add('');
        NSISLines.Add('#### Uninstaller code ####');
        NSISLines.add('');

        NSISLines.Add('Function un.onInit');
        NSISLines.Add('  MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 "Are you sure you want to completely remove $(^Name) and all of its components?" IDYES +2');
        NSISLines.Add('  Abort');
        NSISLines.Add('FunctionEnd');
        NSISLines.Add('');

        NSISLines.Add('Function un.onUninstSuccess');
        NSISLines.Add('  HideWindow');
        NSISLines.Add('  MessageBox MB_ICONINFORMATION|MB_OK "$(^Name) was successfully removed from your computer."');
        NSISLines.Add('FunctionEnd');
        NSISLines.Add('');

        OpenSection('Uninstall');
        if not (shDisableProgramGroupPage in SetupHeader.Options) then
        begin
          NSISLines.Add('  ReadRegStr ${MUI_STARTMENUPAGE_VARIABLE} ${MUI_STARTMENUPAGE_REGISTRY_ROOT} "${MUI_STARTMENUPAGE_REGISTRY_KEY}" "${MUI_STARTMENUPAGE_REGISTRY_VALUENAME}"');
          NSISLines.Add('');
        end;

        for I := 0 to UninstallRunEntries.Count - 1 do
          AddRunEntry(UninstallRunEntries[I]);
        if UninstallRunEntries.Count > 0 then NSISLines.Add('');

        for I := 0 to UninstallDeleteEntries.Count - 1 do
          AddDeleteEntry(UninstallDeleteEntries[I]);
        if UninstallDeleteEntries.Count > 0 then NSISLines.Add('');

        NSISLines.AddStrings(UninstReg);
        if UnInstReg.Count > 0 then NSISLines.Add('');

        NSISLines.AddStrings(UnInstIni);
        if UnInstIni.Count > 0 then NSISLines.Add('');

        NSISLines.AddStrings(UninstallFiles);
        if UninstallFiles.Count > 0 then NSISLines.Add('');

        NSISLines.AddStrings(UninstallDirs);
        if UninstallDirs.Count > 0 then NSISLines.Add('');

        NSISLines.Add('  SetAutoClose true');
        CloseSection;
      end;
    finally
      InstDelDirs.Free;
      UninstallDirs.Free;
      UninstallFiles.Free;
      InstDelFiles.Free;
      UnInstIni.Free;
      UninstReg.Free;
    end;
  finally
    NSISLines.EndUpdate;
  end;
end;


{ Interface functions }

var
  ISSLines: TStrings;
  CurLineNumber: Integer;

function CompilerCallbackProc (Code: Integer; var Data: TCompilerCallbackData;
  AppData: Longint): Integer; stdcall;
var
  CurLine: String;
begin
  Result := iscrSuccess;
  case Code of
    iscbReadScript: begin
        if Data.Reset then CurLineNumber := 0;
        if CurLineNumber < ISSLines.Count then
        begin
          CurLine := ISSLines[CurLineNumber];
          Data.LineRead := PChar(CurLine);
          Inc (CurLineNumber);
        end;
     end;
  end;
end;

procedure ISCompileScript (const ASourceFile: String; ANSISLines: TStrings);
var
  SetupCompiler: TSetupCompiler;
begin
  SetupCompiler := TSetupCompiler.Create(nil);
  try
    SetupCompiler.AppData := 0;

    SetupCompiler.CallbackProc := CompilerCallbackProc;
    SetupCompiler.SourceDir := ExtractFileDir(ASourceFile);

    SetupCompiler.NSISLines := ANSISLines;
    SetupCompiler.ISSSourceFile := ASourceFile;

    SetupCompiler.Compile;
  finally
    SetupCompiler.Free;
  end;
end;


procedure ISConvertFileToFile(const NSISScript, InnoSetupScript: String);
var
 Lines: TStrings;
begin
  Lines := TStringList.Create;
  try
    ISConvertFileToLines(InnoSetupScript, Lines);
    Lines.SaveToFile(NSISScript);
  finally
    Lines.Free;
  end;
end;

procedure ISConvertFileToLines(const InnoSetupScript: String; Lines: TStrings);
begin
  ISSLines := TStringList.Create;
  try
    ISSLines.LoadFromFile(InnoSetupScript);
    ISCompileScript(InnoSetupScript, Lines);
  finally
    ISSLines.Free;
  end;
end;

end.
