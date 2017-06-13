{
  HM NIS Edit (c) 2003-2004 Héctor Mauricio Rodríguez Segura <ranametal@users.sourceforge.net>
  For conditions of distribution and use, see license.txt

  Splash screen form

}
unit UEditDirectory;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  StdCtrls, NewGroupBox;

type
  TEditDirectoryFrm = class(TForm)
    DirEdt: TEdit;
    Button1: TButton;
    StaticText2: TStaticText;
    DestinoEdt: TComboBox;
    StaticText1: TStaticText;
    AceptarBtn: TButton;
    CancelarBtn: TButton;
    NewGroupBox2: TNewGroupBox;
    OverwriteChk: TComboBox;
    procedure Button1Click(Sender: TObject);
    procedure FormCreate(Sender: TObject);
  private
    { Private declarations }
  public
    { Public declarations }
  end;

var
  EditDirectoryFrm: TEditDirectoryFrm;

implementation
uses ShellAPI, Utils, UWizard, ScriptGen;

{$R *.DFM}

var
  FLastUsedDir: string;
  
procedure TEditDirectoryFrm.Button1Click(Sender: TObject);
var
  Dir: String;
begin
  Dir := FLastUsedDir;
  if SelectDirectory(LangStr('SelectDirectory'), Dir) then
  begin
    DirEdt.Text := Dir;
    FLastUsedDir := Dir;
  end;
end;

procedure TEditDirectoryFrm.FormCreate(Sender: TObject);
begin
  InitFont(Font);
  if WizardFrm <> nil then
    WizardFrm.GetDirList(DestinoEdt.Items);
  Caption := LangStr('EditDirectory');
  StaticText1.Caption := LangStr('SelectDirecory');
  StaticText2.Caption := LangStr('DirDest');
  AceptarBtn.Caption := LangStr('OK');
  CancelarBtn.Caption := LangStr('Cancel');
  NewGroupBox2.Caption := LangStr('IfFileExists');
  OverwriteChk.Items.Add(LangStr('OverwriteOn'));
  OverwriteChk.Items.Add(LangStr('OverwriteOff'));
  OverwriteChk.Items.Add(LangStr('OverwriteTry'));
  OverwriteChk.Items.Add(LangStr('OverwriteIfNewer'));
  OverwriteChk.ItemIndex := OverwriteChk.Items.IndexOf(LangStr('OverwriteIfNewer'));
end;

end.
