{
  HM NIS Edit (c) 2003-2005 Héctor Mauricio Rodríguez Segura <ranametal@users.sourceforge.net>
  For conditions of distribution and use, see license.txt

  Editor frame

}
unit UEdit;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  ExtCtrls, StdCtrls, SynEditExport, SynEdit, ComCtrls, Menus, UCustomMDIChild,
  SynEditKeyCmds, SynEditTypes, SynMemo, UCompilerProfiles;

const
  HM_LOG_COMPILER_OUTPUT_LINE = WM_USER + $1006;

type
  TEditFrm = class;

  TCompilerThread = class(TThread)
  private
    FEditorHandle: THandle;
    FEditor: TEditFrm;
    FRun: Boolean;
    procedure ThreadTerminate(Sender: TObject);
    procedure WriteLog(const S: String);
  protected
    procedure Execute; override;
  public
    ProcessHandle, ReadHandle: THandle;
    CommandLine: String;
    constructor Create(Editor: TEditFrm; Run: Boolean);
    destructor Destroy; override;
    procedure TerminateNow;
  end;

  TEditFrm = class(TCustomMDIChild)
    Edit: TSynMemo;
    Splitter1: TSplitter;
    LogBox: TSynEdit;
    procedure EditChange(Sender: TObject);
    procedure FormCreate(Sender: TObject);
    procedure FormClose(Sender: TObject; var Action: TCloseAction);
    procedure FormCloseQuery(Sender: TObject; var CanClose: Boolean);
    procedure FormActivate(Sender: TObject);
    procedure EditStatusChange(Sender: TObject;
      Changes: TSynStatusChanges);
    procedure EditSpecialLineColors(Sender: TObject; Line: Integer;
      var Special: Boolean; var FG, BG: TColor);
    procedure EditMouseDown(Sender: TObject; Button: TMouseButton;
      Shift: TShiftState; X, Y: Integer);
    procedure EditKeyDown(Sender: TObject; var Key: Word;
      Shift: TShiftState);
    procedure EditContextHelp(Sender: TObject; word: String);
    procedure EditKeyPress(Sender: TObject; var Key: Char);
    procedure EditMouseMove(Sender: TObject; Shift: TShiftState; X,
      Y: Integer);
    procedure EditKeyUp(Sender: TObject; var Key: Word;
      Shift: TShiftState);
    procedure LogBoxKeyDown(Sender: TObject; var Key: Word;
      Shift: TShiftState);
    procedure EditMouseCursor(Sender: TObject;
      const aLineCharPos: TBufferCoord; var aCursor: TCursor);
    procedure EditClick(Sender: TObject);
    procedure LogBoxClick(Sender: TObject);
    procedure LogBoxMouseCursor(Sender: TObject;
      const aLineCharPos: TBufferCoord; var aCursor: TCursor);
    procedure LogBoxSpecialLineColors(Sender: TObject; Line: Integer;
      var Special: Boolean; var FG, BG: TColor);
  private
    FHintWord: String;
    FHintLine: Integer;
    FErrorLine: Integer;
    FControlKeyCount: Integer;
    SaveLogBoxHeight: Integer;
    FIsCompiling: Boolean;
    SaveReadOnly: Boolean;
    CompThread: TCompilerThread;
    FCmdParam, FCmd: string;
    procedure SetIsCompiling(Value: Boolean);
    procedure SetErrorLine(ALine: Integer);
    procedure SetLogBoxVisible(const Value: Boolean);
    function GetLogBoxVisible: Boolean;
    procedure WMMouseLeave(var Msg: TMessage); message CM_MOUSELEAVE;
    procedure WMCopyData(var Msg: TWMCopyData); message WM_COPYDATA;
    procedure WMTimer(var Msg: TMessage); message WM_TIMER;
    procedure HMLogCompilerOutputLine(var Msg: TMessage); message HM_LOG_COMPILER_OUTPUT_LINE;
    procedure ShowScriptErrorLine(ErrorFileName: String;
      ErrorLineNumber: Integer);
    function HintWindowReleaseHandle: Boolean;
    function InternalFindFunctionLines(Lines: TStrings; const CommandName,
      FunctionMacroName: string; SearchInNSISCONFIG: Boolean = True): Integer;
    function InternalFindFunction(const CommandName, FunctionMacroName: string): Integer;
  protected
    function GetSynEdit: TSynEdit; override;
  public
    SuccessCompile: Boolean;
    IsHeader, IsText: Boolean;
    CompilerProfileIndex: Integer;

    property LogBoxVisible: Boolean read GetLogBoxVisible write SetLogBoxVisible;
    property ErrorLine: Integer read FErrorLine write SetErrorLine;
    property IsCompiling: Boolean read FIsCompiling write SetIsCompiling;

    function GetCompilerProfile: TCompilerProfile;
    procedure UpdateEditor; override;
    procedure GetErrorLine;
    function ExeName: String;
    procedure Compilar(Run: Boolean = False);
    procedure RunSetup;
    function AllowCompile: Boolean;

    procedure PauseCompile;
    procedure ResumeCompile;
    procedure StopCompile;

    procedure SaveFile(AFileName: String); override;
    procedure LoadFile(AFileName: String); override;

    procedure ShowEditCaret;

    function FindFunction(const Cmd, Param: string): Integer;
    procedure FindLabel(const LabelName: string);

    procedure OpenIncFile(const IncFile: string);
  end;


var
  EditFrm: TEditFrm;
  CompilingCount: Integer = 0;
  UsageList: TStringList = nil;
  HintWindow: THintWindow = nil;

const
  StrDelim = [#39, '"', '`'];
  ComentChars = [';', '#'];
  sTempScriptExt = '.hmnetempscript';

implementation

uses ShellAPI, UMain, Utils, PluginsManger, PluginsInt;

{$R *.DFM}

var
  FIncludeFileNames: TStringList;

const
  SInclude = '!INCLUDE';
  sCall = 'CALL';
  sFunction = 'FUNCTION';
  sMacro = '!MACRO';
  sInsertMacro = '!INSERTMACRO';
  sGoto = 'GOTO';

procedure RemoveComment(var Line: string);
var
  C: Integer;
begin
  for C := 2 to Length(Line) do
    if ((Line[C] in ComentChars) or ((Line[C] = '/') and (C < Length(Line)) and
      (Line[C + 1] = '*'))) and (Line[C - 1] <= ' ') then
        Line := Trim(Copy(Line, 1, C - 1));
end;

function GetCommandParam(const Command, Line: string): string;
var
  CC: Char;
  I: Integer;
begin
  Result := Line;
  if Command <> '' then
    Delete(Result, 1, Length(Command));
  Result := Trim(Result);
  if Result <> '' then
  begin
    if Result[1] in Quotes then
      CC := Result[1]
    else
      CC := ' ';
    if CC in Quotes then  Delete(Result, 1, 1);
    I := AnsiPos(CC, Result) - 1;
    if I < 0 then I := MaxInt;
    Result := Copy(Result, 1, I);
  end;
end;

function ExpandIncludeFileName(const IncludeFileName: string): string;
begin
  Result := ExpandFileName(IncludeFileName);
  if not FileExists(Result) then
    Result := ExtractFilePath(MainFrm.CurCompilerProfile.Compiler) + 'include\' + IncludeFileName;
  if not FileExists(Result) then
    Result := StringReplace(IncludeFileName, '${NSISDIR}', ExtractFileDir(MainFrm.CurCompilerProfile.Compiler),
      [rfReplaceAll]);
end;

function GetWordUsage(const AWord: String): String;

  procedure GetFromCompiler;
  var
    ProcHandle, ReadHandle: THandle;
    Buf: array[0..1024] of Char;
    NumRead: Cardinal;
    I, C: Integer;
    S: String;
  begin
    if CreatePipeAndProcess('"' + MainFrm.CurCompilerProfile.Compiler +
          '" /V1 /CMDHELP ' + AWord, ProcHandle, ReadHandle) then
    try
      with TStringList.Create do
      try
        while ReadFile(ReadHandle, Buf, SizeOf(Buf) - 1, NumRead, nil) do
        begin
          Buf[NumRead] := #0;
          Append(Trim(Buf));
        end;
        for C := 0 to Count - 1 do
        if AnsiPos(LowerCase(AWord), LowerCase(Strings[C])) > 0 then
           Break;
        for I := C to Count - 1 do
        begin
          S := Strings[I];
          if I < Count - 1 then S := S + #13#10;
          Result := Result + S;
        end;
        Result := Trim(Result);
      finally
        Free;
      end;
    finally
      CloseHandle(ProcHandle);
      CloseHandle(ReadHandle);
    end;
  end;

  procedure AddWordUsage(const AWord, Usage: String);
  begin
    UsageList.Add(AWord+'='+Usage);
    UsageList.Sorted := True;
  end;

begin
  Result := '';
  if (AWord <> '') and MainFrm.WordHaveHintUsage(AWord) then
  begin
    if UsageList = nil then
      UsageList := TStringList.Create;

    Result := UsageList.Values[AWord];
    if Result = '' then
    begin
      Screen.Cursor := crHourGlass;
      try
        GetFromCompiler;
      finally
        Screen.Cursor := crDefault;
      end;
      if Result = '' then
      begin
        AddWordUsage(AWord, '*');
        Result :=  Format('Invalid command "%s"', [AWord]);
      end
      else
        AddWordUsage(AWord, Result);
    end else
      if Result = '*' then
        Result :=  Format('Invalid command "%s"', [AWord]);
  end;
end;


{ TCompilerThread }

constructor TCompilerThread.Create(Editor: TEditFrm; Run: Boolean);
begin
  Inc(CompilingCount);

  FEditor := Editor;
  FEditorHandle := FEditor.Handle;
  FRun := Run;

  FEditor.IsCompiling := True;
  FEditor.SuccessCompile := False;
  FEditor.LogBoxVisible := True;

  FEditor.ErrorLine := 0;
  FEditor.LogBox.Clear;

  inherited Create(True);
  FreeOnTerminate := True;
  OnTerminate := ThreadTerminate;

  Plugins.Notify(E_BEFORECOMPILE, Integer(PChar(FEditor.FileName)));
end;

destructor TCompilerThread.Destroy;
begin
  Dec(CompilingCount);
  inherited Destroy;
  FEditor.CompThread := nil;
  FEditor.IsCompiling := False;
end;

procedure TCompilerThread.WriteLog(const S: String);
begin
  SendMessage(FEditorHandle, HM_LOG_COMPILER_OUTPUT_LINE,
    Integer(PChar(S)), Length(S));
end;

procedure TCompilerThread.Execute;
var
  Buf: array[0..1024] of Char;
  NumRead: Cardinal;
begin
  try
    WriteLog('Command line: '#13#10 + CommandLine + #13#10#13#10);
    while (not Terminated) and ReadFile(ReadHandle, Buf, SizeOf(Buf) - 1, NumRead, nil) do
    begin
      Buf[NumRead] := #0;
      WriteLog(Buf);
    end;
    Sleep(1);
  except
    { NADA }
  end;
end;

procedure TCompilerThread.TerminateNow;
begin
   TerminateProcess(ProcessHandle, 0);
   Terminate;
end;

procedure TCompilerThread.ThreadTerminate(Sender: TObject);
var
  AExitCode: Cardinal;
begin
  try
    WaitForSingleObject(ProcessHandle, INFINITE);
    GetExitCodeProcess(ProcessHandle, AExitCode);

    CloseHandle(ProcessHandle);
    CloseHandle(ReadHandle);

    DeleteFile(FEditor.FileName + sTempScriptExt);
    
    if Terminated then
    begin
      WriteLog(#13#10 + LangStr('CompileCanceled') + #13#10);
      Plugins.Notify(E_COMPILECANCELED, Integer(PChar(FEditor.FileName)));
      Exit;
    end;

    FEditor.SuccessCompile := AExitCode = 0;

    if FEditor.SuccessCompile then
    begin
      Plugins.Notify(E_COMPILESUCCES, Integer(PChar(FEditor.FileName)));
      MessageBeep(MB_ICONINFORMATION);
      if FRun then FEditor.RunSetup;
    end else
    begin
      Plugins.Notify(E_COMPILEFAIL, Integer(PChar(FEditor.FileName)), AExitCode);
      MessageBeep(MB_ICONERROR);
      FEditor.GetErrorLine;
    end;
    if (not FEditor.SuccessCompile) or (not FRun) then
      FEditor.Show;
  except
    Application.HandleException(Self);
  end;
end;

{ TEditFrm }

procedure TEditFrm.SetLogBoxVisible(const Value: Boolean);
begin
  if Value <> GetLogBoxVisible then
  Begin
    if Value then
    begin
      LogBox.Height := SaveLogBoxHeight;
      if LogBox.Height <= 5 then
        LogBox.Height := 130
    end else
    begin
      SaveLogBoxHeight := LogBox.Height;
      LogBox.Height := 1;
    end;
  end;
end;

procedure TEditFrm.EditChange(Sender: TObject);
begin
  Modified := True;
  SuccessCompile := False;
end;

procedure TEditFrm.FormCreate(Sender: TObject);
begin
  LogBox.PopupMenu := MainFrm.LogBoxPopup;
  SetLogBoxVisible(False);
  FDefExt := '.nsi';
end;

procedure TEditFrm.FormClose(Sender: TObject; var Action: TCloseAction);
begin
  MainFrm.FirstEdit := nil;
end;

procedure TEditFrm.FormCloseQuery(Sender: TObject; var CanClose: Boolean);
begin
  CanClose := not IsCompiling;
  if not CanClose then
  begin
    PauseCompile;
    if QuestionDlg(LangStr('NoCloseCompiling'), mbQuestionDefBtn2) = IDYES then
      StopCompile;
    ResumeCompile;
  end;
end;

procedure TEditFrm.FormActivate(Sender: TObject);
begin
  MainFrm.IOPanel.Visible := False;
  MainFrm.EnableIOControls(False);
  MainFrm.CompProfilesComboBox.ItemIndex := MainFrm.CompProfilesComboBox.Strings.IndexOf(GetCompilerProfile.DisplayName);
end;

procedure TEditFrm.EditStatusChange(Sender: TObject;
  Changes: TSynStatusChanges);
begin
  if scModified in Changes then
    Self.Modified := Edit.Modified;
  MainFrm.UpdateStatus;
  if (not (scCaretX in Changes)) and (HintWindow <> nil) then
    HintWindowReleaseHandle;
  ShowEditCaret;
end;

procedure TEditFrm.EditSpecialLineColors(Sender: TObject; Line: Integer;
  var Special: Boolean; var FG, BG: TColor);
begin
  if (FErrorLine > 0) and (FErrorLine = Line) then
  begin
    Special := True;
    FG := clWhite;
    BG := clMaroon;
  end;
end;

procedure TEditFrm.ShowScriptErrorLine(ErrorFileName: String;
  ErrorLineNumber: Integer);
begin
  if ExtractFileExt(ErrorFileName) = sTempScriptExt then
    ErrorFileName := ChangeFileExt(ErrorFileName, '');
  if AnsiCompareFileName(FileName, ErrorFileName) = 0 then
    SetErrorLine(ErrorLineNumber) else
  TEditFrm(MainFrm.OpenFile(ErrorFileName)).ErrorLine := ErrorLineNumber;
end;

procedure TEditFrm.GetErrorLine;
var
  C: Integer;
  S: String;
begin
  for C := LogBox.Lines.Count - 1 downto 0 do
  begin
    S := LogBox.Lines[C];
    if AnsiPos(SError, Trim(S)) = 1 then
    begin
      ShowScriptErrorLine(GetErrorFileName(S), GetErrorLineNumber(S));
      Exit;
    end;
  end;
end;

procedure TEditFrm.SetErrorLine(ALine: Integer);
begin
    if FErrorLine > 0 then
      Edit.InvalidateLine (FErrorLine);
    FErrorLine := ALine;
    if FErrorLine > 0 then
    Begin
      Edit.CaretY := FErrorLine;
      Edit.CaretX := 0;
      Edit.EnsureCursorPosVisibleEx(True);
      Edit.InvalidateLine (FErrorLine);
    end;
end;

function TEditFrm.ExeName: String;
var
  C: Integer;
begin
  for C := LogBox.Lines.Count - 1 downto 0 do
  begin
    Result := Trim(LogBox.Lines[C]);
    if SameText(Copy(Result, 1, Length(SOutput)), SOutput) then
    begin
      Delete(Result, 1, Length(SOutput));
      SetLength(Result, Length(Result) - 1);
      Exit;
    end;
  end;
  Result := '';
end;

procedure TEditFrm.EditMouseDown(Sender: TObject; Button: TMouseButton;
  Shift: TShiftState; X, Y: Integer);
begin
  SetErrorLine(0);
end;

procedure TEditFrm.EditKeyDown(Sender: TObject; var Key: Word;
  Shift: TShiftState);

  procedure OpenIncludeFile;
  var
    S: String;
    I: Integer;

    function FindCmd(const Cmd, CmdParam: string): Boolean;
    begin
      I := AnsiPos(Cmd, S);
      Result := I = 1;
      if Result then
        FindFunction(CmdParam, GetCommandParam(Cmd, S));
    end;

  begin
    S := UpperCase(Trim(Edit.LineText));

    I := AnsiPos(SInclude, S);
    if I = 1 then
    begin
      OpenIncFile(GetCommandParam(sInclude, Edit.LineText));
      Exit;
    end;

    if FindCmd(sCall, sFunction) or FindCmd(sInsertMacro, sMacro) then
      Exit;
  end;

var
  P: TPoint;
begin
  SetErrorLine(0);

  if (Key = VK_RETURN) and (Shift = [ssCtrl]) then
  begin
    Screen.Cursor := crHourGlass;
    try
      OpenIncludeFile;
    finally
      Screen.Cursor := crDefault;
    end;
  end else
  if Key = VK_CONTROL then
  begin
    Inc(FControlKeyCount);
    if FControlKeyCount = 1 then
    begin
      P := Edit.ScreenToClient(Mouse.CursorPos);
      EditMouseMove(Edit, [ssCtrl], P.X, P.Y);
    end;
  end;
  { Para evitar asignar el MainFrm.SynAutoComplete.Editor cada vez que una
    nueva ventana de edición se activa, porque aperentemente en siertas
    sircunstancia causa un AV. }
  MainFrm.SynAutoComplete.EditorKeyDown(Sender, Key, Shift);
end;

procedure TEditFrm.EditContextHelp(Sender: TObject; word: String);
begin
  MainFrm.ShowHelp(Word);
end;

procedure TEditFrm.SetIsCompiling(Value: Boolean);
begin
  if Value <> FIsCompiling then
  begin
    FIsCompiling := Value;
    if FIsCompiling then
    begin
      SaveReadOnly := Edit.ReadOnly;
      Edit.ReadOnly := True;
    end
      else Edit.ReadOnly := SaveReadOnly;
  end;
end;

procedure TEditFrm.Compilar(Run: Boolean = False);

  function GetCommandLine: string;
  var
    CompilerProfile: TCompilerProfile;

    function GetParams: String;

      function GetDefines: String;
      var
        CurName: String;
        C: Integer;

        function GetValue: String;
        begin
          Result := CompilerProfile.Symbols.Values[CurName];
          if (Result <> '') and (AnsiPos(' ', Result) > 0) then
            Result := AnsiQuotedStr(Result, '"');
          if Result <> '' then
            Result := '=' + Result + ' '
          else
            Result := ' ';
        end;

      begin
        Result := '';
        for C := 0 to CompilerProfile.Symbols.Count - 1 do
        begin
          CurName := CompilerProfile.Symbols.Names[C];
          Result := Result + '/D' + CurName + GetValue;
        end;
      end;

    begin
      Result := OptionsIni.ReadString('Compiler', 'KK', '') + ' ';

      if OptionsIni.ReadBool('Compiler', 'UseNotifyWindow', False) then
        Result := Result + Format('/NOTIFYHWND %d ', [Handle]);

      if CompilerProfile.NoConfig then
        Result := Result + '/NOCONFIG ';

      if CompilerProfile.NOCD then
        Result := Result + '/NOCD ';

      Result := Result + GetDefines;
    end;

    function GetFileName: string;
    begin
      Result := FileName;
      if Modified and (not CompilerProfile.SaveScriptBeforeCompile) then
      begin
        Result := FileName + sTempScriptExt;
        Edit.Lines.SaveToFile(Result);
      end;
    end;

  begin
     CompilerProfile := GetCompilerProfile;
     Result := Format('"%s" %s "%s"', [CompilerProfile.Compiler, GetParams, GetFileName]);
  end;

  procedure RaiseLastError;
  begin
    raise Exception.Create('Error executing compiler.'#13#10 +
      SysErrorMessage(GetLastError));
  end;

begin
  CompThread := TCompilerThread.Create(Self, Run);
  try
    CompThread.CommandLine := GetCommandLine;
    if not CreatePipeAndProcess(CompThread.CommandLine, CompThread.ProcessHandle,
       CompThread.ReadHandle) then RaiseLastError;
    CompThread.Resume;
  except
    FreeAndNil(CompThread);
    raise;
  end;
end;

procedure TEditFrm.RunSetup;
var
  StartupInfo: TStartupInfo;
  ProcessInfo: TProcessInformation;
  SExeName: String;
begin
  SExeName := ExeName;
  Plugins.Notify(E_RUNEXECUTABLE, Integer(PChar(SExeName)));
  InitStartupInfo(StartupInfo);
  Win32Check(CreateProcess(nil, PChar('"' + SExeName + '"'), nil, nil,
    False, 0, nil, nil, StartupInfo, ProcessInfo));
  CloseHandle(ProcessInfo.hThread);
  CloseHandle(ProcessInfo.hProcess);
end;

procedure TEditFrm.EditKeyPress(Sender: TObject; var Key: Char);
begin
  MainFrm.SynAutoComplete.EditorKeyPress(Sender, Key);
end;

procedure TEditFrm.WMMouseLeave(var Msg: TMessage);
begin
  HintWindowReleaseHandle;
  FHintWord := '';
  FHintLine := -1;
  inherited;
end;

procedure TEditFrm.EditMouseMove(Sender: TObject; Shift: TShiftState; X,
  Y: Integer);
var
  S: String;
  Rc: TRect;
  Pt: TPoint;
  RowCol: TDisplayCoord;
begin
  if IsText or (CompilingCount > 0) or ((not MainFrm.Options.ShowCmdHint)
   and (not (ssCtrl in Shift))) then Exit;

  RowCol := Edit.PixelsToRowColumn(X, Y);
  S := Edit.GetWordAtRowCol(Edit.DisplayToBufferPos(RowCol));
  if (S <> FHintWord) or (FHintLine <> RowCol.Row) then
  begin
    HintWindowReleaseHandle;

    if HintWindow = nil then
      HintWindow := TShadowedHintWindow.Create(MainFrm);

    Pt := Mouse.CursorPos;
    FHintWord := S;
    FHintLine := RowCol.Row;

    S := GetWordUsage(FHintWord);

    if S <> '' then
    begin
      Rc := HintWindow.CalcHintRect(Screen.Width, S, nil);
      OffsetRect(Rc, Pt.X, Pt.Y);
      HintWindow.Color := Application.HintColor;
      HintWindow.Canvas.Font.Assign(Screen.HintFont);
      HintWindow.ActivateHint(Rc, S);
      SetTimer(Handle, 1, 100, nil);
    end;
  end;
end;

procedure TEditFrm.UpdateEditor;
var
  Ext: String;
begin
  Ext := ExtractFileExt(FileName);
  IsText := SameText(Ext, '.txt');
  IsHeader := SameText(Ext, '.nsh');

  if IsText or IsHeader then
  begin
    Splitter1.Visible := False;
    LogBox.Visible := False;
  end;

  if UseHighLighter then
  begin
    if not IsText then
      Edit.Highlighter := MainFrm.SynNSIS
    else
      Edit.Highlighter := nil;
  end else
    Edit.Highlighter := nil;
  Edit.Assign(MainFrm.EditorOptions);
  if IsText then Edit.Gutter.Visible := False;
  inherited UpdateEditor;
end;

type
  TSynEditAccess = class(TSynEdit);
procedure TEditFrm.ShowEditCaret;
begin
  if not ShowCaret(Edit.Handle) then
    TSynEditAccess(Edit).InitializeCaret;
end;

procedure TEditFrm.EditKeyUp(Sender: TObject; var Key: Word;
  Shift: TShiftState);
begin
  if (Key = VK_CONTROL) then
  begin
    if HintWindowReleaseHandle then
    begin
      FHintWord := '';
      FControlKeyCount := 0;
    end;
  end;
end;

function TEditFrm.GetLogBoxVisible: Boolean;
begin
   Result := LogBox.Height > 5;
end;

procedure TEditFrm.WMCopyData(var Msg: TWMCopyData);
const
  MAKENSIS_NOTIFY_SCRIPT = 0;
  MAKENSIS_NOTIFY_WARNING = 1;
  MAKENSIS_NOTIFY_ERROR = 2;
  MAKENSIS_NOTIFY_OUTPUT = 3;
begin
  case Msg.CopyDataStruct.dwData of
    MAKENSIS_NOTIFY_SCRIPT:;
    MAKENSIS_NOTIFY_WARNING:;
    MAKENSIS_NOTIFY_ERROR:;
    MAKENSIS_NOTIFY_OUTPUT:;
  end;
end;

procedure TEditFrm.LogBoxKeyDown(Sender: TObject; var Key: Word;
  Shift: TShiftState);
begin
  if Key = VK_RETURN then
     LogBoxClick(LogBox);
end;

function TEditFrm.HintWindowReleaseHandle: Boolean;
begin
  Result := HintWindow <> nil;
  if Result then
  begin
    HintWindow.ReleaseHandle;
    KillTimer(Handle, 1);
  end;
end;

procedure TEditFrm.SaveFile(AFileName: String);
begin
  if AFileName <> '' then
  begin
    FileName := AFileName;
    MainFrm.AddToRecent(FileName);
  end;
  Edit.Lines.SaveToFile(FileName);
  Modified := False;
  InitValues;
end;

function TEditFrm.GetSynEdit: TSynEdit;
begin
  Result := Edit;
end;

procedure TEditFrm.LoadFile(AFileName: String);
begin
  Edit.Lines.LoadFromFile(FileName);
  SuccessCompile := False;
  SetLogBoxVisible(False);
  Edit.Modified := False;
end;

function TEditFrm.AllowCompile: Boolean;
begin
  Result := (not IsText) and (not IsHeader) and (not IsCompiling);
end;

procedure TEditFrm.WMTimer(var Msg: TMessage);
begin
  { This should fix bug #858757}
  if WindowFromPoint(Mouse.CursorPos) <> Edit.Handle then
    HintWindowReleaseHandle;
end;


procedure TEditFrm.PauseCompile;
begin
  if CompThread <> nil then
    CompThread.Suspend;
end;

procedure TEditFrm.ResumeCompile;
begin
  if CompThread <> nil then
    CompThread.Resume;
end;

procedure TEditFrm.StopCompile;
begin
  if CompThread <> nil then
    CompThread.TerminateNow;
end;

function TEditFrm.GetCompilerProfile: TCompilerProfile;
begin
  if (CompilerProfileIndex >= 0) and (CompilerProfileIndex < MainFrm.CompProfilesComboBox.Strings.Count)then
    Result := TCompilerProfile(MainFrm.CompProfilesComboBox.Strings.Objects[CompilerProfileIndex])
  else
    Result := MainFrm.DefaultCompilerProfile;
end;

function TEditFrm.InternalFindFunctionLines(Lines: TStrings; const CommandName,
  FunctionMacroName: string; SearchInNSISCONFIG: Boolean = True): Integer;
{  Finds a function or macro }

  function FindFunctionInFile(const FileName: string;
    SearchInNSISCONFIG: Boolean = True): Integer;
  var
    aLines: TStrings;
    Form: TCustomForm;
  begin
    Result := 0;
    // First see if the file is already opened
    Form := MainFrm.FindMDIChild(FileName);
    if Form is TEditFrm then
      Result := TEditFrm(Form).InternalFindFunction(CommandName, FunctionMacroName)
    else if FileExists(FileName) then
    try
      aLines := TStringList.Create;
      try
        aLines.LoadFromFile(FileName);
        Result := InternalFindFunctionLines(aLines, CommandName, FunctionMacroName,
          SearchInNSISCONFIG);
      finally
        aLines.Free;
      end;
      if Result > 0 then
      begin
        Form := MainFrm.OpenFile(FileName);
        if Form is TEditFrm then
          TEditFrm(Form).Edit.GotoLineAndCenter(Result);
        Form.Show;
      end;
    except
      { nada }
    end;
  end;

var
  I, C: Integer;
  S: string;
  IncludeLinesNumbers: array of Integer;
begin
  IncludeLinesNumbers := nil;
  Result := 0;

  for C := 0 to Lines.Count - 1 do
  begin
    S := UpperCase(Trim(Lines[C]));
    if AnsiPos(SInclude, S) = 1 then
    begin
      SetLength(IncludeLinesNumbers, Length(IncludeLinesNumbers) + 1);
      IncludeLinesNumbers[Length(IncludeLinesNumbers) - 1] := C;
    end;
    if AnsiPos(CommandName, S) = 1 then
    begin
      S := GetCommandParam(CommandName, Lines[C]);
      if AnsiSameText(S, FunctionMacroName) then
      begin
        Result := C + 1;
        Exit;
      end;
    end;
  end;

  // For avoid infinite loop on circular includes
  if FIncludeFileNames = nil then
  begin
    FIncludeFileNames := TStringList.Create;
    FIncludeFileNames.Sorted := True;
  end;

  // Find in include files
  for C := 0 to Length(IncludeLinesNumbers) - 1 do
  begin
    S := Trim(Lines[IncludeLinesNumbers[C]]);
    S := ExpandIncludeFileName(GetCommandParam(sInclude, S));

    if FIncludeFileNames.IndexOf(S) < 0 then
    begin
      FIncludeFileNames.Add(S);
      I := FindFunctionInFile(S);
      if I > 0 then
      begin
        Result := -I;
        Exit;
      end;
    end;
  end;

  // Last find in the NSIS configuration file
  if SearchInNSISCONFIG then // << avoid infinite recursive call
    with MainFrm.CurCompilerProfile do
    if (not NoConfig) then
    begin
      I := FindFunctionInFile(ExtractFilePath(Compiler) +  'nsisconf.nsh', False);
      if I > 0 then
      begin
        Result := -I;
        Exit;
      end;
    end;
end;

function TEditFrm.InternalFindFunction(const CommandName, FunctionMacroName: string): Integer;
begin
  Result := InternalFindFunctionLines(Edit.Lines, CommandName, FunctionMacroName);
  if Result > 0 then
  begin
    Edit.GotoLineAndCenter(Result);
    Show;
  end;
end;

procedure TEditFrm.FindLabel(const LabelName: string);
begin
     { TODO: }
end;

procedure TEditFrm.EditMouseCursor(Sender: TObject;
  const aLineCharPos: TBufferCoord; var aCursor: TCursor);
var
  S: string;

  function IsHandeableCmd(const Cmd: string): Boolean;
  var
    BufferCoord: TBufferCoord;
  begin
    Result := AnsiPos(Cmd, UpperCase(S)) = 1;
    if Result then
    begin
      FCmdParam := GetCommandParam(Cmd, S);
      FCmd := Cmd;
      BufferCoord := aLineCharPos;
      S := Edit.GetWordAtRowCol(aLineCharPos);
      Result := AnsiPos(UpperCase(S), UpperCase(FCmdParam)) > 0;
    end;
  end;

begin
  if (GetKeyState(VK_CONTROL) < 0) then
  begin
    S := Trim(Edit.Lines[aLineCharPos.Line - 1]);
    if IsHandeableCmd(sInclude) or IsHandeableCmd(sCall) or
      IsHandeableCmd(sInsertMacro) then
        aCursor := crHandPoint;
  end else
    FCmd := '';
end;

function TEditFrm.FindFunction(const Cmd, Param: string): Integer;
begin
  Screen.Cursor := crHourGlass;
  try
    FIncludeFileNames := nil;
    try
      Result := InternalFindFunction(Cmd, Param);
    finally
      FreeAndNil(FIncludeFileNames);
    end;
  finally
    Screen.Cursor := crDefault;
  end;
end;

procedure TEditFrm.OpenIncFile(const IncFile: string);
var
  aIncFile: string;
begin
  aIncFile := ExpandIncludeFileName(IncFile);
  if FileExists(aIncFile) then
    MainFrm.OpenFile(aIncFile);
end;

procedure TEditFrm.EditClick(Sender: TObject);
begin
  if FCmd = sInclude then
    OpenIncFile(FCmdParam)
  else if (FCmd = sCall) then
    FindFunction(sFunction, FCmdParam)
  else if (FCmd = sInsertMacro) then
    FindFunction(sMacro, FCmdParam);
end;

const
  IDC_LINK = MakeIntResource(32649);

var
  CursorHandle: THandle;

procedure TEditFrm.HMLogCompilerOutputLine(var Msg: TMessage);
var
  TextLog: string;
begin
  LogBox.ExecuteCommand(ecEditorBottom, #0, nil);
  SetString(TextLog, PChar(Msg.WParam), Msg.LParam);
  LogBox.SelText := TextLog;
  LogBox.EnsureCursorPosVisible;
end;

procedure TEditFrm.LogBoxClick(Sender: TObject);
const
  sMacrodd = 'macro:';

  function GetConfigFileName: String;
  begin
    Result := ExtractFilePath(MainFrm.CurCompilerProfile.Compiler) + 'nsisconf.nsh';
    if not FileExists(Result) then
    begin
      Result := ChangeFileExt(Result, '.nsi');
      if not FileExists(Result) then Abort;
    end;
  end;

  function GetMacroName(const Line: string): string;
  begin
    Result := Trim(Line);
    Delete(Result, 1, Length(SErrorInMacro));
    Result := Trim(Result);
    Result := Trim(Copy(Result, 1, AnsiPos(SOnMacroLine, Result) - 1));
  end;

  function GetMacroLineNumber(Line: string): Integer;
  begin
    Delete(Line, 1, AnsiPos(SOnMacroLine, Line) + Length(SOnMacroLine));
    Result := StrToIntDef(Trim(Line), 0);
  end;

  function GetWarningMacroName(const Line: string): string;
  begin
    Result := Line;
    Delete(Result, 1, AnsiPos(sMacrodd, Line) + Length(sMacrodd) - 1);
    Delete(Result, AnsiPos(':', Result), MaxInt);
  end;

var
  Wfn, S: String;
  Wln: Integer;
  Form: TEditFrm;
begin
  //S := LogBox.Lines[LogBox.CaretPs.Y];
  S := LogBox.LineText;
  if AnsiPos(SErrorInMacro, S) > 0 then
  begin
    FindFunction(sMacro, GetMacroName(S));
    Form := MainFrm.CurEditFrm;
    if Form <> nil then
      ShowScriptErrorLine(Form.FileName, Form.Edit.CaretY + GetMacroLineNumber(S));
  end else
  if AnsiPos(SIncError, S) > 0 then
    ShowScriptErrorLine(GetIncErrorFileName(S), GetIncErrorLineNumber(S)) else
  if AnsiPos(SError, S) > 0 then
    ShowScriptErrorLine(GetErrorFileName(S), GetErrorLineNumber(S)) else
  if AnsiPos(SConfigError, S) > 0 then
    ShowScriptErrorLine(GetConfigFileName, GetConfigErrorLine(S)) else
  begin
    Wfn := GetWarningFileName(S);
    if (Wfn <> '') then
    begin
      if ExtractFileExt(Wfn) = sTempScriptExt then
        Wfn := ChangeFileExt(Wfn, '');
      if FileExists(Wfn) then
      begin
        Wln := GetWarningErrorLineNumber(S);
        if Wln > 0 then
          ShowScriptErrorLine(Wfn, Wln);
      end else
      if AnsiPos(SMacrodd, Wfn) = 1 then
      begin
        FindFunction(sMacro, GetWarningMacroName(S));
        Form := MainFrm.CurEditFrm;
        if Form <> nil then
          ShowScriptErrorLine(Form.FileName, Form.Edit.CaretY + GetWarningErrorLineNumber(S));
      end;
    end;
  end;
end;

procedure TEditFrm.LogBoxMouseCursor(Sender: TObject;
  const aLineCharPos: TBufferCoord; var aCursor: TCursor);
var
  Line: string;
begin
  Line := LogBox.Lines[aLineCharPos.Line - 1];
  if (AnsiPos(SIncError, Line) > 0)  or (AnsiPos(SError, Line) > 0)  or
  (AnsiPos(SConfigError, Line) > 0)  or (AnsiPos(SWarning, Trim(Line)) = 1) or
    (AnsiPos(SErrorInMacro, Line) > 0) then
    aCursor := crHandPoint;
end;

procedure TEditFrm.LogBoxSpecialLineColors(Sender: TObject; Line: Integer;
  var Special: Boolean; var FG, BG: TColor);
var
  aLine: string;
begin
  aLine := LogBox.Lines[Line - 1];
  Special := (AnsiPos(SIncError, aLine) > 0)  or (AnsiPos(SError, aLine) > 0)  or
    (AnsiPos(SConfigError, aLine) > 0)  or (AnsiPos(SWarning, Trim(aLine)) = 1) or
      (AnsiPos(SErrorInMacro, aLine) > 0);
  if Special then
    FG := clRed;
end;

initialization
  CursorHandle := LoadCursor(0, IDC_LINK);
  if CursorHandle <> 0 then
    Screen.Cursors[crHandPoint] := CursorHandle;
finalization
  UsageList.Free;
end.
