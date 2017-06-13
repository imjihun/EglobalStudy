{
  HM NIS Edit (c) 2003-2004 Héctor Mauricio Rodríguez Segura <ranametal@users.sourceforge.net>
  For conditions of distribution and use, see license.txt

  Compiler profiles form

}
unit UCompilerProfiles;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  StdCtrls, ComCtrls;

type
  TCompilerProfilesFrm = class(TForm)
    AgregarBtn: TButton;
    EditarBtn: TButton;
    EliminarBtn: TButton;
    CerrarBtn: TButton;
    ProfileList: TListView;
    RenameBtn: TButton;
    procedure ListBoxDblClick(Sender: TObject);
    procedure AgregarBtnClick(Sender: TObject);
    procedure EditarBtnClick(Sender: TObject);
    procedure EliminarBtnClick(Sender: TObject);
    procedure ProfileListEdited(Sender: TObject; Item: TListItem;
      var S: String);
    procedure RenameBtnClick(Sender: TObject);
    procedure ProfileListDblClick(Sender: TObject);
    procedure FormCreate(Sender: TObject);
    procedure ProfileListEditing(Sender: TObject; Item: TListItem;
      var AllowEdit: Boolean);
    procedure FormKeyDown(Sender: TObject; var Key: Word;
      Shift: TShiftState);
  private
  public
    procedure InitProfileList(List: TStrings);
    procedure AssignProfileList(List: TStrings);
  end;

  TCompilerProfile = class(TObject)
  private
    FCompilerVersion: string;
  public
    ProfileName: String;
    Compiler: String;
    HelpFile: String;
    UseIntegratedBrowser4Help: Boolean;
    NoConfig: Boolean;
    NoCD: Boolean;
    SaveScriptBeforeCompile: Boolean;
    Symbols: TStrings;
    constructor Create(const Name: String);
    destructor Destroy; override;
    function DisplayName: string;
    procedure Assign(Profile: TCompilerProfile);
    procedure Save;
    procedure Load;
    function GetCompilerVersion: string;
  end;

var
  CompilerProfilesFrm: TCompilerProfilesFrm;

implementation

uses UConfig, Utils,  UMain, UEditCompilerProfile,
  UEdit;

{$R *.DFM}

{ TCompilerProfile }

procedure TCompilerProfile.Assign(Profile: TCompilerProfile);
begin
  Compiler := Profile.Compiler;
  HelpFile := Profile.HelpFile;
  UseIntegratedBrowser4Help := Profile.UseIntegratedBrowser4Help;
  NoConfig := Profile.NoConfig;
  NoCD := Profile.NoCD;
  SaveScriptBeforeCompile := Profile.SaveScriptBeforeCompile;
  Symbols.Assign(Profile.Symbols);
end;

constructor TCompilerProfile.Create(const Name: String);
begin
  Symbols := TStringList.Create;
  ProfileName := Name;
end;

destructor TCompilerProfile.Destroy;
begin
  Symbols.Free;
  inherited Destroy;
end;

function TCompilerProfile.DisplayName: string;
begin
  Result := LangStr('CompProfile' + ProfileName, ProfileName);
end;

function TCompilerProfile.GetCompilerVersion: string;
var
  ProcHandle, ReadHandle: THandle;
  Buf: array[0..1024] of Char;
  BytesReaded: Cardinal;
  I: Integer;
begin
  if FCompilerVersion = '' then
  begin
    if CreatePipeAndProcess('"' + Compiler + '" /VERSION', ProcHandle, ReadHandle) then
    begin
      if WaitForSingleObject(ProcHandle, 10000) = WAIT_OBJECT_0 then
      begin
        ReadFile(ReadHandle, Buf, SizeOf(Buf) - 1, BytesReaded, nil);
        CloseHandle(ProcHandle);
        CloseHandle(ReadHandle);

        Buf[BytesReaded] := #0;
        FCompilerVersion := Trim(Buf);

        { Check for old NSIS versions that not support the /VERSION command }
        I := AnsiPos(' - ', FCompilerVersion);
        if I = 0 then I := AnsiPos(#13, FCompilerVersion);
        if I > 0 then SetLength(FCompilerVersion, I-1);
      end;
    end;
  end;
  Result := FCompilerVersion;
end;

procedure TCompilerProfile.Load;
var
  S: String;
  C: Integer;
begin
  with OptionsIni do
  begin
    Self.Compiler := ReadString('Profiles\'+ProfileName, 'Compiler', '');
    Self.HelpFile := ReadString('Profiles\'+ProfileName, 'HelpFile', '');
    Self.UseIntegratedBrowser4Help := ReadBool('Profiles\'+ProfileName,
      'UseIntegratedBrowser4Help', False);
    Self.NoConfig := ReadBool('Profiles\'+ProfileName, 'NoConfig', False);
    Self.NoCD := ReadBool('Profiles\'+ProfileName, 'NoCD', False);
    Self.SaveScriptBeforeCompile := ReadBool('Profiles\'+ProfileName,
      'SaveScriptBeforeCompile', True);

    Symbols.Clear;
    ReadSection('Profiles\'+ProfileName+'\Symbols', Symbols);
    for C := 0 to Symbols.Count - 1 do
    begin
      S := Symbols[C];
      Symbols[C] := S + '=' + ReadString('Profiles\'+ProfileName+'\Symbols', S, '');
    end;
  end;
end;

procedure TCompilerProfile.Save;
var
  S: String;
  C: Integer;
begin
  with OptionsIni do
  begin
    WriteString('Profiles\'+ProfileName, 'Compiler', Compiler);
    WriteString('Profiles\'+ProfileName, 'HelpFile', HelpFile);
    WriteBool('Profiles\'+ProfileName, 'UseIntegratedBrowser4Help',
      UseIntegratedBrowser4Help);
    WriteBool('Profiles\'+ProfileName, 'NoConfig', NoConfig);
    WriteBool('Profiles\'+ProfileName, 'NoCD', NoCD);
    WriteBool('Profiles\'+ProfileName, 'SaveScriptBeforeCompile',
      SaveScriptBeforeCompile);


    EraseSection('Profiles\'+ProfileName+'\Symbols');
    for C := 0 to Symbols.Count - 1 do
    begin
      S := Symbols.Names[C];
      WriteString('Profiles\'+ProfileName+'\Symbols', S, Symbols.Values[S]);
    end;
  end;
end;


procedure TCompilerProfilesFrm.ListBoxDblClick(Sender: TObject);
begin
  EditarBtn.Click;
end;

procedure TCompilerProfilesFrm.AgregarBtnClick(Sender: TObject);
var
  NewProf: TCompilerProfile;
  ProfName: string;
  C: Integer;
  ProfileNoExists: Boolean;
begin
  ProfName := LangStr('NewProfile');
  repeat
    ProfileNoExists := True;
    if not InputQuery(LangStr('NewProfileCaption'), LangStr('NewProfilePrompt'), ProfName) then
     Exit;

    for C := 0 to ProfileList.Items.Count - 1 do
      if SameText(ProfName, ProfileList.Items[C].Caption) then
      begin
        WarningDlg(LangStrFormat('ProfileExists', [ProfName]));
        ProfileNoExists := False;
        Break;
      end;
  until ProfileNoExists;

  NewProf := TCompilerProfile.Create(ProfName);
  with ProfileList.Items.Add do
  begin
    NewProf.Assign(MainFrm.DefaultCompilerProfile);
    Caption := NewProf.ProfileName;
    Data := NewProf;
  end;
  EditCompilerProfile(NewProf);
end;

procedure TCompilerProfilesFrm.EditarBtnClick(Sender: TObject);
begin
  if ProfileList.Selected <> nil then
    EditCompilerProfile(TCompilerProfile(ProfileList.Selected.Data));
end;

procedure TCompilerProfilesFrm.EliminarBtnClick(Sender: TObject);
begin
{*}
  if (ProfileList.Selected <> nil) and (ProfileList.Selected.Index > 0) then
    ProfileList.Selected.Delete;
{*}
end;


procedure TCompilerProfilesFrm.AssignProfileList(List: TStrings);
var
  C: Integer;
begin
  List.BeginUpdate;
  try
    List.Clear;
    for C := 0 to ProfileList.Items.Count - 1 do
    with ProfileList.Items[C] do
      List.AddObject(Caption, TObject(Data));
  finally
    List.EndUpdate;
  end;
end;

procedure TCompilerProfilesFrm.InitProfileList(List: TStrings);
var
  C: Integer;
begin
  ProfileList.Items.BeginUpdate;
  try
    ProfileList.Items.Clear;
    for C := 0 to List.Count - 1 do
    with ProfileList.Items.Add do
    begin
      Caption := List[C];
      Data := TObject(List.Objects[C]);
    end;
  finally
    ProfileList.Items.EndUpdate;
  end;
end;

procedure TCompilerProfilesFrm.ProfileListEdited(Sender: TObject;
  Item: TListItem; var S: String);
var
  C: Integer;
begin
  for C := 0 to ProfileList.Items.Count - 1 do
  if (ProfileList.Items[C] <> Item) and SameText(S, ProfileList.Items[C].Caption) then
  begin
    WarningDlg(LangStrFormat('ProfileExists', [S]));
    S := Item.Caption;
    Exit;
  end;
  TCompilerProfile(Item.Data).ProfileName := S;
end;

procedure TCompilerProfilesFrm.RenameBtnClick(Sender: TObject);
begin
  if ProfileList.Selected <> nil then
    ProfileList.Selected.EditCaption;
end;

procedure TCompilerProfilesFrm.ProfileListDblClick(Sender: TObject);
begin
  EditarBtn.Click;
end;

procedure TCompilerProfilesFrm.FormCreate(Sender: TObject);
begin
  Caption := LangStr('CompilerProfilesCaption');
  AgregarBtn.Caption := LangStr('AddProfile');
  EditarBtn.Caption := LangStr('EditProfile');
  RenameBtn.Caption := LangStr('RenameProfile');
  EliminarBtn.Caption := LangStr('DeleteProfile');
  CerrarBtn.Caption := LangStr('Close');
end;

procedure TCompilerProfilesFrm.ProfileListEditing(Sender: TObject;
  Item: TListItem; var AllowEdit: Boolean);
begin
  AllowEdit := Item.Index <> 0;
end;

procedure TCompilerProfilesFrm.FormKeyDown(Sender: TObject; var Key: Word;
  Shift: TShiftState);
begin
  if Key = VK_F2 then
    RenameBtn.Click;  
end;


end.

