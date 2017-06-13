{=============================================================================================
The contents of this file are subject to the Mozilla Public License
Version 1.1 (the "License"); you may not use this file except in compliance
with the License. You may obtain a copy of the License at
http://www.mozilla.org/MPL/

Software distributed under the License is distributed on an "AS IS" basis,
WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License for
the specific language governing rights and limitations under the License.

The Original Code is SynHighlighterNSIS.pas, released on 2003-03-10

The Initial Developer of the Original Code is Mariusz 'kg' Jakubowski <mjakubowski@go2.pl>.
Portions created by Mariusz Jakubowski are Copyright (C) 2003 Mariusz Jakubowski.
All Rights Reserved.

Contributor(s):
   H�ctor Mauricio Rodr�guez Segura (HMRS)  <ranametal@users.sourceforge.net>

Last Modified: 2003-11-13

Known Issues:
-------------------------------------------------------------------------------}

unit SynHighlighterNSIS;

{$I SynEdit.inc}

interface

uses
  SysUtils, Classes,
  {$IFDEF SYN_CLX}
  Qt, QControls, QGraphics,
  {$ELSE}
  Windows, Messages, Controls, Graphics, Registry,
  {$ENDIF}
  SynEditTypes, SynEditHighlighter, SynHighlighterHashEntries;

type
  TtkTokenKind = (tkComment, tkIdentifier, tkKey, tkNull, tkNumber, tkParameter,
    tkParameterWithSlash, tkSpace, tkString, tkSymbol, {tkUnknown, }tkFunction,
    tkDirective, tkVariable, tkPluginCall, tkCallbackFunction);

  TRangeState = (rsUnknown, rsString, rsComment, rsCStyleComment, rsSection);

  TProcTableProc = procedure of object;

  TSynNSISSyn = class(TSynCustomHighlighter)
  private
    fLine: PChar;
    fLineNumber: integer;
    fProcTable: array[#0..#255] of TProcTableProc;

    Run: longint;
    fStringLen: integer;
    fToIdent: PChar;
    fTokenPos: integer;
    fTokenID: TtkTokenKind;
    fRange: TRangeState;

    // Added by HMRS 05/15/2003
    FHighlightVarsInsideStrings: Boolean;
    // HMRS 05/15/2003 END

    VarInString: Boolean;
    StringOpenChar: char;

    fCommentAttri: TSynHighlighterAttributes;
    fFunctionAttri: TSynHighlighterAttributes;
    fDirectiveAttri: TSynHighlighterAttributes;
    fVariableAttri: TSynHighlighterAttributes;
    fIdentifierAttri: TSynHighlighterAttributes;
    fKeyAttri: TSynHighlighterAttributes;
    fNumberAttri: TSynHighlighterAttributes;
    fSpaceAttri: TSynHighlighterAttributes;
    fStringAttri: TSynHighlighterAttributes;
    fParamAttri: TSynHighlighterAttributes;

    // Added by HMRS 05/12/2003
    fPluginCallAttri: TSynHighlighterAttributes;
    fCallbackFunctionAttri: TSynHighlighterAttributes;
    // HMRS 05/12/2003 END

    fKeywords: TSynHashEntryList;

    function KeyHash(ToHash: PChar): integer;
    function KeyComp(const aKey: string): boolean;
    procedure NullProc;
    procedure SpaceProc;
    procedure IdentProc;
    procedure NumberProc;
    procedure CommentOpenProc;
    procedure CommentProc;
    procedure StringOpenProc;
    procedure StringProc;
    procedure UnknownProc;
    procedure VariableProc;
    // Added by HMRS 06/04/2003 BEGIN
    procedure CStyleCommentOpenProc;
    procedure CStyleCommentProc;
    // Added by HMRS 06/04/2003 END

    procedure DoAddKeyword(AKeyword: string; AKind: integer);
    procedure MakeMethodTables;

    // Added HMRS 05/15/2003 BEGIN
    procedure SetHighlightVarsInsideStrings(Value: Boolean);
    // HMRS 05/15/2003 END
  protected
    function GetIdentChars: TSynIdentChars; override;
  public
    {$IFNDEF SYN_CPPB_1}
    class {$ENDIF}
      function GetLanguageName: string; override;
  public
    constructor Create(AOwner: TComponent); override;
    destructor Destroy; override;
    function IdentKind(MayBe: PChar): TtkTokenKind;
    function GetDefaultAttribute(Index: integer): TSynHighlighterAttributes;
      override;
    function GetRange: Pointer; override;
    procedure ResetRange; override;
    procedure SetRange(Value: Pointer); override;
    function GetEol: boolean; override;
    function GetToken: string; override;
    function GetTokenAttribute: TSynHighlighterAttributes; override;
    function GetTokenID: TtkTokenKind;
    function GetTokenKind: integer; override;
    function GetTokenPos: integer; override;
    procedure Next; override;
    procedure SetLine(NewValue: string; LineNumber: integer); override;

    // Added HMRS 05/15/2003 BEGIN
    procedure Assign(Source: TPersistent); override;
    // HMRS 05/15/2003 END
  published
    property SpaceAttri: TSynHighlighterAttributes read fSpaceAttri write fSpaceAttri;
    property IdentifierAttri: TSynHighlighterAttributes
      read fIdentifierAttri write fIdentifierAttri;
    property KeyAttri: TSynHighlighterAttributes read fKeyAttri write fKeyAttri;
    property NumberAttri: TSynHighlighterAttributes read fNumberAttri write fNumberAttri;
    property StringAttri: TSynHighlighterAttributes read fStringAttri write fStringAttri;

    property CommentAttri: TSynHighlighterAttributes read fCommentAttri write fCommentAttri;
    property FunctionAttri: TSynHighlighterAttributes
      read fFunctionAttri write fFunctionAttri;
    property DirectiveAttri: TSynHighlighterAttributes
      read fDirectiveAttri write fDirectiveAttri;
    property VariableAttri: TSynHighlighterAttributes
      read fVariableAttri write fVariableAttri;

    // Added by HMRS 05/12/2003
    property PluginCallAttri: TSynHighlighterAttributes read FPluginCallAttri
      write FPluginCallAttri;
    property CallbackFunctionAttri: TSynHighlighterAttributes read FCallbackFunctionAttri
      write FCallbackFunctionAttri;
    // HMRS 05/12/2003 END

    property ParameterAttri: TSynHighlighterAttributes read fParamAttri write fParamAttri;
    property HighlightVarsInsideStrings: Boolean read FHighlightVarsInsideStrings
      write SetHighlightVarsInsideStrings;
  end;

procedure Register;

// Added by HMRS 27/03/2003 BEGIN
var
  TokensFileName: String;
// HMRS 27/03/2003 END

implementation

uses
  SynEditStrConst,
  // Added by HMRS 27/03/2003 BEGIN
  IniFiles;
  // HMRS 27/03/2003 END


{$IFDEF SYN_COMPILER_3_UP}
resourcestring
  {$ELSE}
const
  {$ENDIF}
  SYNS_FilterNSIS = 'NSIS Script Files (*.nsi)|*.nsi';
  SYNS_AttrParameter = 'Parameter';
  // HMRSAdded by HMRD 01/05/2003 BEGIN
  SYNS_AttrParameterWithSlash = 'ParameterWSlash';
  SYNS_AttrPluginCall = 'Plugin call';
  SYNS_AttrCallbackFunction = 'Callback function';
  // HMRS 27/03/2003 END
  SYNS_LangNSIS = 'NSIS Script';

procedure Register;
begin
  RegisterComponents(SYNS_HighlightersPage, [TSynNSISSyn]);
end;

// Added by HMRS 08/07/2003 BEGIN
const
  UserVarValidChars = ['a'..'z', 'A'..'Z', '_'];
// HMRS 08/07/2003 END

var
  Identifiers: array[#0..#255] of bytebool;
  mHashTable: array[#0..#255] of integer;

const
  Keywords: string =
    'Function,FunctionEnd,Section,SectionEnd,SectionIn,SubSection,SubSectionEnd,' +
    'Page,UninstPage,AddBrandingImage,AllowRootDirInstall,AutoCloseWindow,BGGradient,BrandingText,' +
    'ChangeUI,CheckBitmap,CompletedText,ComponentText,CRCCheck,DetailsButtonText,DirShow,' +
    'DirText,FileErrorText,InstallButtonText,InstallColors,InstallDir,InstallDirRegKey,InstProgressFlags,' +
    'InstType,LicenseBkColor,LicenseData,LicenseText,MiscButtonText,SetFont,ShowInstDetails,' +
    'ShowUninstDetails,SilentInstall,SilentUnInstall,SpaceTexts,SubCaption,UninstallButtonText,UninstallCaption,' +
    'UninstallIcon,UninstallSubCaption,UninstallText,WindowIcon,XPStyle,SetCompress,SetCompressor,' +
    'SetDatablockOptimize,SetDateSave,SetOverwrite,SetPluginUnload,AddSize,OutFile,Name,' +
    'Caption,Icon,LicenseForceSelection,Var,VIAddVersionKey,VIProductVersion,PageEx,' +
    'PageExEnd,PageCallbacks,DirVar,FileBufSize,LicenseLangString,LoadLanguageFile,LangString,' +
    'LangStringUP,AllowSkipFiles';

  Functions: string =
    'File,Exec,ExecWait,ExecShell,Rename,Delete,RMDir,' +
    'WriteRegStr,WriteRegExpandStr,WriteRegDWORD,WriteRegBin,WriteINIStr,ReadRegStr,ReadRegDWORD,' +
    'ReadEnvStr,ExpandEnvStrings,DeleteRegValue,DeleteRegKey,EnumRegKey,EnumRegValue,DeleteINISec,' +
    'DeleteINIStr,CreateDirectory,CopyFiles,SetFileAttributes,CreateShortCut,GetFullPathName,SearchPath,' +
    'GetTempFileName,CallInstDLL,RegDLL,UnRegDLL,GetDLLVersion,GetDLLVersionLocal,GetFileTime,' +
    'GetFileTimeLocal,Goto,Call,Return,IfErrors,ClearErrors,SetErrors,' +
    'FindWindow,SendMessage,IsWindow,IfFileExists,MessageBox,StrCmp,IntCmp,' +
    'IntCmpU,Abort,Quit,GetFunctionAddress,GetLabelAddress,GetCurrentAddress,FindFirst,' +
    'FindNext,FindClose,FileOpen,FileClose,FileRead,FileWrite,FileReadByte,' +
    'FileWriteByte,FileSeek,SetDetailsView,SetDetailsPrint,SetAutoClose,DetailPrint,Sleep,' +
    'BringToFront,HideWindow,SetShellVarContext,StrCpy,StrLen,Push,Pop,' +
    'Exch,IntOp,IntFmt,Reboot,IfRebootFlag,SetRebootFlag,WriteUninstaller,' +
    'LogSet,LogText,SectionSetFlags,SectionGetFlags,SectionSetText,SectionGetText,GetDlgItem,' +
    'SetBrandingImage,CreateFont,ReserveFile,GetWindowText,ShowWindow,FlushINI,SectionSetInstTypes,' +
    'SectionGetInstTypes,InitPluginsDir,ReadIniStr,IfAbort,SectionSetSize,SectionGetSize,SetCurInstType,' +
    'GetCurInstType,InstTypeSetText,InstTypeGetText,Nop,EnableWindow,SetCtlColors,IfSilent,' +
    'SetSilent,SetOutPath,LockWindow';

  Parameters: string =
    'custom,license,components,directory,instfiles,uninstConfirm,true,' +
    'false,on,off,force,show,hide,nevershow,' +
    'normal,silent,silentlog,auto,zlib,bzip2,try,' +
    'ifnewer,manual,alwaysoff,RO,SW_SHOWNORMAL,SW_SHOWMAXIMIZED,SW_SHOWMINIMIZED,' +
    'HKCR,HKEY_CLASSES_ROOT,HKLM,HKEY_LOCAL_MACHINE,HKCU,HKEY_CURRENT_USER,HKU,' +
    'HKEY_USERS,HKCC,HKEY_CURRENT_CONFIG,HKDD,HKEY_DYN_DATA,HKPD,HKEY_PERFORMANCE_DATA,' +
    'FILE_ATTRIBUTE_NORMAL,ARCHIVE,FILE_ATTRIBUTE_ARCHIVE,HIDDEN,FILE_ATTRIBUTE_HIDDEN,OFFLINE,FILE_ATTRIBUTE_OFFLINE,' +
    'READONLY,FILE_ATTRIBUTE_READONLY,SYSTEM,FILE_ATTRIBUTE_SYSTEM,TEMPORARY,FILE_ATTRIBUTE_TEMPORARY,MB_OK,MB_OKCANCEL,' +
    'MB_ABORTRETRYIGNORE,MB_RETRYCANCEL,MB_YESNO,MB_YESNOCANCEL,MB_ICONEXCLAMATION,MB_ICONINFORMATION,MB_ICONQUESTION,' +
    'MB_ICONSTOP,MB_TOPMOST,MB_SETFOREGROUND,MB_RIGHT,MB_DEFBUTTON1,MB_DEFBUTTON2,MB_DEFBUTTON3,' +
    'MB_DEFBUTTON4,IDABORT,IDCANCEL,IDIGNORE,IDNO,IDOK,IDRETRY,' +
    'IDYES,current,all,none,listonly,textonly,both,' +
    'lastused,checkbox,radiobuttons';

  ParametersWithSlash: string =
    '/lang,/TRIMLEFT,/TRIMRIGHT,/TRIMCENTER,/windows,/NOCUSTOM,/CUSTOMSTRING,' +
    '/COMPONENTSONLYONCUSTOM,/e,/REBOOTOK,/nonfatal,/a,/r,/oname,' +
    '/ifempty,/NOUNLOAD,/FILESONLY,/SHORT,/ITALIC,/UNDERLINE,/STRIKE,' +
    '/TIMEOUT,/IMGID,/RESIZETOFIT,/components,/silent';

  Directives: string =
    '!system,!include,!cd,!packhdr,!define,!undef,!ifdef,' +
    '!ifndef,!endif,!else,!macro,!macroend,!insertmacro,!verbose,' +
    '!warning,!error,!echo,!addIncludeDir,!addplugindir';

  Variables: String =
    '$0,$1,$2,$3,$4,$5,$6,$7,$8,$9,$R0,$R1,$R2,$R3,$R4,$R5,$R6,$R7,$R8,$R9,$INSTDIR,' +
    '$OUTDIR,$CMDLINE,$PROGRAMFILES,$DESKTOP,$EXEDIR,$WINDIR,$SYSDIR,$TEMP,' +
    '$STARTMENU,$SMPROGRAMS,$SMSTARTUP,$QUICKLAUNCH,$HWNDPARENT,$LANGUAGE,$PLUGINSDIR';

  CallbackFunctions: string =
    '.onGUIInit,.onInit,.onGUIEnd,.onInstFailed,.onInstSuccess,.onMouseOverSection,.onSelChange,' +
    '.onUserAbort,.onVerifyInstDir,un.onGUIInit,un.onGUIEnd,un.onInit,un.onUninstFailed,un.onUninstSuccess,' +
    'un.onUserAbort';

procedure MakeIdentTable;
var
  c: char;
begin
  FillChar(Identifiers, SizeOf(Identifiers), 0);
  for c := 'a' to 'z' do
    Identifiers[c] := True;
  for c := 'A' to 'Z' do
    Identifiers[c] := True;
  for c := '0' to '9' do
    Identifiers[c] := True;

  Identifiers['_'] := True;
  // Added HMRS 04/22/2003
  Identifiers['!'] := True;
  Identifiers['/'] := True;
  Identifiers['$'] := True;
  Identifiers['.'] := True;
  Identifiers[':'] := True;
  // HMRS 04/22/2003 end

  FillChar(mHashTable, SizeOf(mHashTable), 0);
  mHashTable['_'] := 1;
  mHashTable['.'] := 1;
  mHashTable['!'] := 1;
  for c := 'a' to 'z' do
    mHashTable[c] := 2 + Ord(c) - Ord('a');
  for c := 'A' to 'Z' do
    mHashTable[c] := 2 + Ord(c) - Ord('A');
end;

function TSynNSISSyn.KeyHash(ToHash: PChar): integer;
begin
  Result := 0;
  while Identifiers[ToHash^] do
  begin
    {$IFOPT Q-}
    Result := 7 * Result + mHashTable[ToHash^];
    {$ELSE}
    Result := (7 * Result + mHashTable[ToHash^]) and $FFFFFF;
    {$ENDIF}
    inc(ToHash);
  end;
  Result := Result and $1FF;
  fStringLen := ToHash - fToIdent;
end;

function TSynNSISSyn.KeyComp(const aKey: string): boolean;
var
  i: integer;
  pKey1, pKey2: PChar;
begin
  pKey1 := fToIdent;
  pKey2 := pointer(aKey);
  for i := 1 to fStringLen do
  begin
    if mHashTable[pKey1^] <> mHashTable[pKey2^] then
    begin
      Result := False;
      exit;
    end;
    Inc(pKey1);
    Inc(pKey2);
  end;
  Result := True;
end;

function TSynNSISSyn.IdentKind(MayBe: PChar): TtkTokenKind;
var
  Entry: TSynHashEntry;
begin
  fToIdent := MayBe;
  Entry := fKeywords[KeyHash(MayBe)];
  while Assigned(Entry) do
  begin
    if Entry.KeywordLen > fStringLen then
      break
    else if Entry.KeywordLen = fStringLen then
      if KeyComp(Entry.Keyword) then
      begin
        Result := TtkTokenKind(Entry.Kind);
        exit;
      end;
    Entry := Entry.Next;
  end;
  Result := tkIdentifier;
end;

procedure TSynNSISSyn.MakeMethodTables;
var
  I: char;
begin
  for I := #0 to #255 do
    case I of
      #0: fProcTable[I] := NullProc;
      #1..#32: fProcTable[I] := SpaceProc;
      '0'..'9': fProcTable[I] := NumberProc;
      'A'..'Z', 'a'..'z',
      '_', '!', '.': fProcTable[I] := IdentProc;
      '/': fProcTable[I] :=  CStyleCommentOpenProc;
      ';', '#': fProcTable[I] := CommentOpenProc;
      '"', '''', '`': fProcTable[I] := StringOpenProc;
      '$': fProcTable[I] := VariableProc;
      //'/': fProcTable[I] := SlashProc;
      else
        fProcTable[I] := UnknownProc;
    end;
end;

constructor TSynNSISSyn.Create(AOwner: TComponent);

    // Added by  HMRS 27/03/2003 BEGIN
    function LoadTokensFromFile(const Section, Default: String): String;
    { Load keywords from a file. For no recompile the Highlighter when new versions of
      NSIS released. }
    var
      Lst: TStringList;
      C: Integer;
    begin
      Result := '';
      with TIniFile.Create(TokensFileName) do
      try
        Lst := TStringList.Create;
        try
          ReadSection(Section, Lst);
          for C := 0 to Lst.Count - 1 do
            Result := Result + ReadString(Section, Lst[C], '') + ',';
        finally
          Lst.Free;
        end;
      finally
        Free;
      end;
      if Result = '' then
        Result := Default;
    end;
    // HMRS 27/03/2003 END

begin
  inherited Create(AOwner);
  fKeywords := TSynHashEntryList.Create;

  fCommentAttri := TSynHighlighterAttributes.Create(SYNS_AttrComment);
  fCommentAttri.Style := [fsItalic];
  fCommentAttri.Foreground := clGray;
  AddAttribute(fCommentAttri);

  fFunctionAttri := TSynHighlighterAttributes.Create(SYNS_AttrFunction);
  fFunctionAttri.Foreground := clNavy;
  AddAttribute(fFunctionAttri);

  fDirectiveAttri := TSynHighlighterAttributes.Create(SYNS_AttrDirective);
  fDirectiveAttri.Foreground := clRed;
  AddAttribute(fDirectiveAttri);

  fVariableAttri := TSynHighlighterAttributes.Create(SYNS_AttrVariable);
  fVariableAttri.Foreground := clPurple;
  AddAttribute(fVariableAttri);

  fIdentifierAttri := TSynHighlighterAttributes.Create(SYNS_AttrIdentifier);
  fIdentifierAttri.Foreground := clWindowText;
  AddAttribute(fIdentifierAttri);

  fKeyAttri := TSynHighlighterAttributes.Create(SYNS_AttrReservedWord);
  fIdentifierAttri.Foreground := clWindowText;
  fKeyAttri.Style := [fsBold];
  AddAttribute(fKeyAttri);

  fNumberAttri := TSynHighlighterAttributes.Create(SYNS_AttrNumber);
  fNumberAttri.Foreground := clGreen;
  AddAttribute(fNumberAttri);

  fSpaceAttri := TSynHighlighterAttributes.Create(SYNS_AttrSpace);
  AddAttribute(fSpaceAttri);

  fStringAttri := TSynHighlighterAttributes.Create(SYNS_AttrString);
  fStringAttri.Foreground := clOlive;
  AddAttribute(fStringAttri);

  fParamAttri := TSynHighlighterAttributes.Create(SYNS_AttrParameter);
  fParamAttri.Foreground := clTeal;
  AddAttribute(fParamAttri);

  // Added by HMRS 05/12/2003 BEGIN
  fPluginCallAttri := TSynHighlighterAttributes.Create(SYNS_AttrPluginCall);
  fPluginCallAttri.Foreground := clWindowText;
  AddAttribute(fPluginCallAttri);

  fCallbackFunctionAttri := TSynHighlighterAttributes.Create(SYNS_AttrCallbackFunction);
  fCallbackFunctionAttri.Foreground := clWindowText;
  AddAttribute(fCallbackFunctionAttri);
  // HMRS 05/12/2003 END

  SetAttributesOnChange(DefHighlightChange);

  // Modified by HMRS 27/03/2003 BEGIN
  EnumerateKeywords(Ord(tkKey), LoadTokensFromFile(SYNS_AttrReservedWord, KeyWords),
     IdentChars, DoAddKeyword);
  EnumerateKeywords(Ord(tkFunction), LoadTokensFromFile(SYNS_AttrFunction, Functions),
     IdentChars, DoAddKeyword);
  EnumerateKeywords(Ord(tkParameter), LoadTokensFromFile(SYNS_AttrParameter, Parameters),
     IdentChars, DoAddKeyword);
  EnumerateKeywords(Ord(tkParameterWithSlash), LoadTokensFromFile(SYNS_AttrParameterWithSlash, ParametersWithSlash),
    IdentChars, DoAddKeyword);
  EnumerateKeywords(Ord(tkDirective), LoadTokensFromFile(SYNS_AttrDirective, Directives),
    IdentChars, DoAddKeyword);
  EnumerateKeywords(Ord(tkVariable), LoadTokensFromFile(SYNS_AttrVariable, Variables),
    IdentChars, DoAddKeyWord);
  EnumerateKeywords(Ord(tkCallbackFunction), LoadTokensFromFile(SYNS_AttrCallbackFunction,
     CallbackFunctions), IdentChars, DoAddKeyWord);
  // HMRS 27/03/2003 END

  FHighlightVarsInsideStrings := True;
  {IsKFD := False;}

  MakeMethodTables;
  fDefaultFilter := SYNS_FilterNSIS;
  WordBreakChars := WordBreakChars - ['!', '.', '/'];
end;

destructor TSynNSISSyn.Destroy;
begin
  fKeywords.Free;
  inherited Destroy;
end;

procedure TSynNSISSyn.SetLine(NewValue: string; LineNumber: integer);
begin
  fLine := PChar(NewValue);
  Run := 0;
  fLineNumber := LineNumber;
  Next;
end;

procedure TSynNSISSyn.IdentProc;
var
  P: PChar;
  I: Integer;
begin
  // Added by HMRS 05/14/2003 BEGIN
  { This check for vars inside strings without quotes. This is necessary since
   the '$' character is an identifier char. }
  P := fLine + Run;
  I := 0;
  while Identifiers[P^] do
  begin
    if P^ = '$' then
    begin
      fTokenID := tkIdentifier;
      Inc(Run, I);
      Exit;
    end;
    Inc(I);
    Inc(P);
  end;
  // HMRS 05/14/2003 END

  // Modified by HMRS 22/04/2003 BEGIN
  fTokenID := IdentKind(fLine + Run);
  case fTokenID of
    tkParameter:
      { For the MB_ICONSTOP|MB_YESNO params of Message box }
      if not (fLine[Run - 1] in [' ', '|']) then fTokenID := tkIdentifier;
    tkKey, tkFunction: // HMRS 19/10/2003
      // This avoid a problem with macro parameters that have the same name of a
      // key word or function(thanks Vince). For example:
      // !macro MYMACRO NAME
      // will color NAME in blue, even though here NAME is purely a macro parameter,
      // not the Name command.
      begin
        P := FLine;
        while P < FLine + Run do
        begin
          if not (P^ in [' ', #9]) then
          begin
            fTokenID := tkIdentifier;
            Break;
          end;
          Inc(P);
        end;
      end;
      // HMRS END
  end;
  // HMRS 22/04/2003 END

  // Added by HMRS 05/12/2003 BEGIN
  // Check for plugin call
  if fTokenID = tkIdentifier then
  begin
    P := fLine + Run;
    while Identifiers[P^] do
    begin
      if P = fLine + Run + fStringLen then Break;
      if (P^ = ':') then
      begin
        Inc(P);
        if P^ = ':' then
        begin
            fTokenID := tkPluginCall;
            Break;
        end;
      end;
      Inc(P);
    end;
  end;
  // HMRS 05/12/2003 END
  Inc(Run, fStringLen);
end;

procedure TSynNSISSyn.UnknownProc;
begin
  inc(Run);
  fTokenID := tkIdentifier;
end;

procedure TSynNSISSyn.NullProc;
begin
  fTokenID := tkNull;
end;

procedure TSynNSISSyn.SpaceProc;
begin
  fTokenID := tkSpace;
  repeat
    Inc(Run);
  until (fLine[Run] > #32) or (fLine[Run] = #0);
end;

// added by HMRS 07/17/2003 BEGIN
procedure TSynNSISSyn.CommentOpenProc;
begin
  if (Run = 0) or (FLine[Run - 1] in [' ', #9]) then
  begin
    fRange := rsComment;
    CommentProc;
    fTokenID := tkComment;
  end else
    UnknownProc
end;
// HMRS 07/17/2003 END

procedure TSynNSISSyn.CommentProc;
var
  BackSlashFound: Boolean;
begin
  // Modified by HMRS 07/17/2003 BEGIN
  //BackSlashFound :=  False;
  BackSlashFound := ((FLine[Run] in [#0..#32]) and (Trim(FLine) <> ''));
  case fLine[Run] of
     #0: NullProc;
     #1..#32: SpaceProc;
  else
    begin
      fTokenID := tkComment;
      repeat
        if FLine[Run] = '\' then BackSlashFound := True;
        Inc(Run);
        if BackSlashFound and not (FLine[Run] in [#0..#32]) then
          BackSlashFound := False;
      until (fLine[Run] in [#0, #10, #13]);
    end;
  end;
  if not BackSlashFound then
    fRange := rsUnKnown;
  // HMRS 07/17/2003 END
end;


// Added by HMRS 06/04/2003 BEGIN
// thanks to Ramon18
// I get this methods from the SynHighlighterSample.pas in the HighlighterDemo
procedure TSynNSISSyn.CStyleCommentOpenProc;
begin
  if (fLine[Run + 1] = '*') then
  begin
    Inc(Run);
    fRange := rsCStyleComment;
    CStyleCommentProc;
    fTokenID := tkComment;
  end
  else
    IdentProc;
end;

procedure TSynNSISSyn.CStyleCommentProc;
begin
  case fLine[Run] of
     #0: NullProc;
     #1..#32: SpaceProc;
    //#10: LFProc;
    //#13: CRProc;
  else
    begin
      fTokenID := tkComment;
      repeat
        if (fLine[Run] = '*') and
           (fLine[Run + 1] = '/') then
        begin
          Inc(Run, 2);
          fRange := rsUnKnown;
          Break;
        end;
        if not (fLine[Run] in [#0, #10, #13]) then
          Inc(Run);
      until fLine[Run] in [#0, #10, #13];
    end;
  end;
end;
// Added by HMRS 06/04/2003 END

procedure TSynNSISSyn.NumberProc;
var
  charset: set of char;
begin
  fTokenID := tkNumber;
  if (Length(fLine) > Run + 2) and (fLine[Run] = '0') and (fLine[Run + 1] = 'x') then
  begin
    Inc(Run);
    charset := ['0'..'9', 'A'..'F', 'a'..'f'];//hex
  end
  else
    charset := ['0'..'9'];//dec or octal
  repeat
    Inc(Run);
  until not (fLine[Run] in charset);
end;

procedure TSynNSISSyn.StringOpenProc;
begin
  fRange := rsString;
  VarInString := False;
  StringOpenChar := fLine[Run];
  StringProc;
end;

procedure TSynNSISSyn.StringProc;
var
  ToFix, BackSlashFound: Boolean;
begin
  BackSlashFound := ((FLine[Run] in [#0..#32]) and (Trim(FLine) <> ''));
  // this fix a bug :)
  ToFix := VarInString and (FLine[Run] = StringOpenChar);
  VarInString := False;
  case fLine[Run] of
     #0: NullProc;
//     #1..#32: SpaceProc;
  else begin
    fTokenID := tkString;
    if not ToFix then
    repeat
      if fLine[Run] = '$' then
      begin
         // find for scaped quotes
         if (FLine[Run + 1] = '\') and (FLine[Run + 2] in  ['"', '''', '`']) then
           Inc(Run, 2)
         else if FHighlightVarsInsideStrings then
         begin
           VarInString := True;
           Break;
         end;
      end;
      if FLine[Run] = '\' then BackSlashFound := True;
      Inc(Run);
      if BackSlashFound and not (FLine[Run] in [#0..#32]) then
        BackSlashFound := False;
    until fLine[Run] in [StringOpenChar, #0, #10, #13];
    if fLine[Run] = StringOpenChar then
    begin
      Inc(Run);
      fRange := rsUnKnown;
    end;
  end;
  end;
  if not BackSlashFound then
    fRange := rsUnKnown;
end;

procedure TSynNSISSyn.VariableProc;
// Added by HMRS 05/13/2003 BEGIN
var
  S: String;
  BraceCount: Integer;
label
  _end;
// HMRS 05/13/2003 END
begin
  // Modified by HMRS 05/13/2003 BEGIN
  inc(Run);
  if fLine[Run] = '$' then Inc(Run);

  if fLine[Run] = '{' then
  begin
    BraceCount := 1;
    fTokenID := tkVariable;
    repeat
      if fLine[Run] = '$' then
      begin
        if FLine[Run + 1] = '{' then Inc(BraceCount);
      end else
        if fLine[Run] = '}' then Dec(BraceCount);
      inc(Run)
    until (fLine[Run] in [#0, #13, #10]) or (BraceCount = 0);
    if Fline[Run] = '}' then Inc(Run);
  end else
  begin
    S := '$';
    fTokenID := tkVariable;
    while Identifiers[FLine[Run]] do
    begin
      S := S + FLine[Run];
      Inc(Run);
      if (IdentKind(PChar(S)) = tkVariable) and (UpperCase(S) = S) then
        goto _end;
    end;
    while FLine[Run] in UserVarValidChars do Inc(Run);
  end;

_end:
  if VarInString and not (FLine[Run] in [#0, #10, #13]) then
    fRange := rsString;
// HMRS 05/13/2003 END
end;

procedure TSynNSISSyn.Next;
begin
  fTokenPos := Run;
  case fRange of
    rsCStyleComment: CStyleCommentProc;
    rsComment: CommentProc;
    rsString: StringProc;
  else
    begin
      fRange := rsUnknown;
      fProcTable[fLine[Run]];
    end;
  end;
end;

function TSynNSISSyn.GetDefaultAttribute(Index: integer): TSynHighlighterAttributes;
begin
  case Index of
    SYN_ATTR_COMMENT: Result := fCommentAttri;
    SYN_ATTR_IDENTIFIER: Result := fIdentifierAttri;
    SYN_ATTR_KEYWORD: Result := fKeyAttri;
    SYN_ATTR_STRING: Result := fStringAttri;
    SYN_ATTR_WHITESPACE: Result := fSpaceAttri;
    else
      Result := nil;
  end;
end;

function TSynNSISSyn.GetEol: boolean;
begin
  Result := (fTokenId = tkNull);
end;

function TSynNSISSyn.GetToken: string;
var
  Len: longint;
begin
  Len := Run - fTokenPos;
  SetString(Result, (FLine + fTokenPos), Len);
end;

function TSynNSISSyn.GetTokenAttribute: TSynHighlighterAttributes;
begin
  case fTokenID of
    tkComment: Result := fCommentAttri;
    tkFunction: Result := fFunctionAttri;
    tkDirective: Result := fDirectiveAttri;
    tkVariable: Result := fVariableAttri;
    tkParameterWithSlash, tkParameter: Result := fParamAttri;
    tkIdentifier: Result := fIdentifierAttri;
    tkKey: Result := fKeyAttri;
    tkNumber: Result := fNumberAttri;
    tkSpace: Result := fSpaceAttri;
    tkString: Result := fStringAttri;
    // Added by HMRS 05/12/2003 BEGIN
    tkPluginCall: Result := fPluginCallAttri;
    tkCallbackFunction: Result := fCallbackFunctionAttri;
    // HMRS 05/12/2003 END
    else
      Result := nil;
  end;
end;

function TSynNSISSyn.GetTokenKind: integer;
begin
  Result := Ord(fTokenId);
end;

function TSynNSISSyn.GetTokenID: TtkTokenKind;
begin
  Result := fTokenId;
end;

function TSynNSISSyn.GetTokenPos: integer;
begin
  Result := fTokenPos;
end;

function TSynNSISSyn.GetIdentChars: TSynIdentChars;
begin
  Result :=  TSynValidStringChars +
     // Added by HMRS 28/03/2003 BEGIN
     ['!', '$', '.', '/', ':']; { <- for my Script editor help system }
     // HMRS 28/03/2003 END
end;

{$IFNDEF SYN_CPPB_1} class {$ENDIF}
function TSynNSISSyn.GetLanguageName: string;
begin
  Result := SYNS_LangNSIS;
end;

procedure TSynNSISSyn.DoAddKeyword(AKeyword: string; AKind: integer);
var
  HashValue: integer;
begin
  HashValue := KeyHash(PChar(AKeyword));
  fKeywords[HashValue] := TSynHashEntry.Create(AKeyword, AKind);
end;

// Added by HMRS 05/15/2003 BEGIN
procedure TSynNSISSyn.SetHighlightVarsInsideStrings(Value: Boolean);
begin
  if FHighlightVarsInsideStrings <> Value then
  begin
    FHighlightVarsInsideStrings := Value;
    DefHighlightChange(Self);
  end;
end;

procedure TSynNSISSyn.Assign(Source: TPersistent);
begin
   inherited Assign(Source);
   if Source is TSynNSISSyn then
     SetHighlightVarsInsideStrings(TSynNSISSyn(Source).FHighlightVarsInsideStrings);
end;
// HMRS 05/15/2003 END

procedure TSynNSISSyn.ResetRange;
begin
  fRange := rsUnknown;
end;

procedure TSynNSISSyn.SetRange(Value: Pointer);
begin
  fRange := TRangeState(Value);
end;

function TSynNSISSyn.GetRange: Pointer;
begin
  Result := Pointer(fRange);
end;

initialization
  MakeIdentTable;
  {$IFNDEF SYN_CPPB_1}
  RegisterPlaceableHighlighter(TSynNSISSyn);
  {$ENDIF}

  // Added by HMRS 27/03/2003 BEGIN
  TokensFileName := ExtractFilePath(ParamStr(0)) + 'NSIS.syn';
  // HMRS 27/03/2003 BEGIN
end.

