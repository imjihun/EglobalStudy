{
  HM NIS Edit (c) 2003-2004 Héctor Mauricio Rodríguez Segura <ranametal@users.sourceforge.net>
  For conditions of distribution and use, see license.txt

  Compiler settings frame

}
unit UCompilerConfig;

interface

uses 
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  ComCtrls, StdCtrls, NewGroupBox, UCompilerProfiles;

type
  TCompilerConfigFrm = class(TFrame)
    GroupBox8: TNewGroupBox;
    Label14: TStaticText;
    Label15: TStaticText;
    CopilEdt: TEdit;
    ExaminarExeBtn: TButton;
    HTMLEdt: TEdit;
    ExaminarHtmlBtn: TButton;
    UseIntegratedBrowserChk: TCheckBox;
    GroupBox7: TNewGroupBox;
    NoConfigChk: TCheckBox;
    NoCDChk: TCheckBox;
    GroupBox9: TNewGroupBox;
    SimbLst: TListView;
    AgregarBtn: TButton;
    EditarBtn: TButton;
    EliminarBtn: TButton;
    AbrirDlg: TOpenDialog;
    SaveScriptsBeforeCompileChk: TCheckBox;
    procedure AgregarBtnClick(Sender: TObject);
    procedure EditarBtnClick(Sender: TObject);
    procedure EliminarBtnClick(Sender: TObject);
    procedure SimbLstDblClick(Sender: TObject);
    procedure SimbLstKeyDown(Sender: TObject; var Key: Word;
      Shift: TShiftState);
    procedure ExaminarExeBtnClick(Sender: TObject);
    procedure ExaminarHtmlBtnClick(Sender: TObject);
    procedure AbrirDlgCanClose(Sender: TObject; var CanClose: Boolean);
  private
    { Private declarations }
  public
    AddingSymbol: Boolean;
    UpdateUsageList: Boolean;
    procedure InitValues(Profile: TCompilerProfile);
    procedure SaveValues(Profile: TCompilerProfile);
    constructor Create(AOwner: TComponent); override;
  end;

implementation

uses UEditSimbol, Utils, UMain;

{$R *.DFM}

{ TCompilerConfigFrm }

constructor TCompilerConfigFrm.Create(AOwner: TComponent);
begin
  inherited Create(AOwner);
  AgregarBtn.Caption := LangStr('AddSimbol');
  EditarBtn.Caption := LangStr('EditSimbol');
  EliminarBtn.Caption := LangStr('DeleteSimbol');
  Label14.Caption := LangStr('Compiler');
  Label15.Caption := LangStr('Help');
  NoConfigChk.Caption := LangStr('NoConfigFile');
  NOCDChk.Caption := LangStr('NOCD');
  GroupBox8.Caption := LangStr('Paths');
  GroupBox7.Caption := LangStr('Options');
  GroupBox9.Caption := LangStr('Simbols');
  SimbLst.Columns[0].Caption := LangStr('SimbolColumn');
  SimbLst.Columns[1].Caption := LangStr('ValueColumn');
  UseIntegratedBrowserChk.Caption := LangStr('UseIntegratedBrowser4Help');
  ExaminarExeBtn.Hint := LangStr('Browse');
  ExaminarHtmlBtn.Hint := LangStr('Browse');
  SaveScriptsBeforeCompileChk.Caption := LangStr('SaveScriptsBeforeCompile');  
end;

procedure TCompilerConfigFrm.AgregarBtnClick(Sender: TObject);
var
  S, V: String;
begin
  S := '';
  V := '';
  AddingSymbol := True;
  if EditSimbol(S, V, Self) and (S <> '') then
  with SimbLst.Items.Add do
  begin
    Caption := S;
    SubItems.Add(V);
  end;
end;

procedure TCompilerConfigFrm.EditarBtnClick(Sender: TObject);
var
  S, V: String;
begin
  AddingSymbol := False;
  if SimbLst.Selected <> nil then
  with SimbLst.Selected do
  begin
    S := Caption;
    V := SubItems[0];
    if EditSimbol(S, V, Self) then
    begin
      Caption := S;
      SubItems[0] := V;
    end;
  end;
end;

procedure TCompilerConfigFrm.EliminarBtnClick(Sender: TObject);
begin
  if SimbLst.Selected <> nil then
    SimbLst.Selected.Delete;
end;

procedure TCompilerConfigFrm.SimbLstDblClick(Sender: TObject);
begin
  EditarBtn.Click;
end;

procedure TCompilerConfigFrm.SimbLstKeyDown(Sender: TObject; var Key: Word;
  Shift: TShiftState);
begin
  case Key of
    VK_INSERT: AgregarBtn.Click;
    VK_RETURN: EditarBtn.Click;
    VK_DELETE: EliminarBtn.Click;
  end;
end;

procedure TCompilerConfigFrm.ExaminarExeBtnClick(Sender: TObject);
begin
  AbrirDlg.Filter := GetLangFileFilter(['ExeFileFilter']);
  AbrirDlg.FileName := ExtractFileName(CopilEdt.Text);
  AbrirDlg.InitialDir := ExtractFileDir(CopilEdt.Text);
  AbrirDlg.OnCanClose := AbrirDlgCanClose;
{$IFNDEF NO_DIALOGS_MODIFIED}
  AbrirDlg.UseDialogHook := True;
{$ENDIF}
  if AbrirDlg.Execute then
  begin
    UpdateUsageList := True;
    MainFrm.NSISVersion := '';
    CopilEdt.Text := AbrirDlg.FileName;
  end;
{$IFNDEF NO_DIALOGS_MODIFIED}
  AbrirDlg.UseDialogHook := False;
{$ENDIF}
end;

procedure TCompilerConfigFrm.ExaminarHtmlBtnClick(Sender: TObject);
begin
  AbrirDlg.Filter := GetLangFileFilter(['HelpFileFilter', 'CHMFileFilter', 'HTMLFileFilter']);
  AbrirDlg.FileName := ExtractFileName(HTMLEdt.Text);
  AbrirDlg.InitialDir := ExtractFileDir(HTMLEdt.Text);
  AbrirDlg.OnCanClose := nil;
  if AbrirDlg.Execute then
    HTMLEdt.Text := AbrirDlg.FileName;
end;

procedure TCompilerConfigFrm.AbrirDlgCanClose(Sender: TObject;
  var CanClose: Boolean);
begin
  CanClose := True;
  if not IsConsole(AbrirDlg.FileName) then
  begin
    ErrorDlg(LangStrFormat('NoConsole', [Abrirdlg.FileName]));
    CanClose := False;
  end;
end;

procedure TCompilerConfigFrm.InitValues(Profile: TCompilerProfile);
var
  C: Integer;
begin
  UseIntegratedBrowserChk.Checked := Profile.UseIntegratedBrowser4Help;
  NoConfigChk.Checked := Profile.NoConfig;
  NoCDChk.Checked := Profile.NOCD;
  CopilEdt.Text := Profile.Compiler;
  HTMLEdt.Text := Profile.HelpFile;
  SaveScriptsBeforeCompileChk.Checked := Profile.SaveScriptBeforeCompile;
  SimbLst.Items.Clear;
  for C := 0 to Profile.Symbols.Count - 1 do
  with SimbLst.Items.Add do
  begin
    Caption := Profile.Symbols.Names[C];
    SubItems.Add(Profile.Symbols.Values[Caption]);
  end;
end;

procedure TCompilerConfigFrm.SaveValues(Profile: TCompilerProfile);
var
  C: Integer;
begin
  Profile.UseIntegratedBrowser4Help := UseIntegratedBrowserChk.Checked;
  Profile.Compiler := CopilEdt.Text;
  Profile.HelpFile := HTMLEdt.Text;
  Profile.NoConfig := NoConfigChk.Checked;
  Profile.NOCD := NOCDChk.Checked;
  Profile.SaveScriptBeforeCompile := SaveScriptsBeforeCompileChk.Checked;
{  if UpdateUsageList then
    FreeAndNil(UsageList);}

  Profile.Symbols.Clear;
  for C := 0 to SimbLst.Items.Count - 1 do
    with SimbLst.Items[C] do
      Profile.Symbols.Add(Caption + '=' + SubItems[0]);
end;

end.
