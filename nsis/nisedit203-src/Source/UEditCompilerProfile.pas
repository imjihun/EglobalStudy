{
  HM NIS Edit (c) 2003-2004 Héctor Mauricio Rodríguez Segura <ranametal@users.sourceforge.net>
  For conditions of distribution and use, see license.txt

  Compiler settings frame

}
unit UEditCompilerProfile;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  StdCtrls, UCompilerConfig, UCompilerProfiles, ExtCtrls;

type
  TEditCompilerProfileFrm = class(TForm)
    CancelBtn: TButton;
    OKBtn: TButton;
    Panel1: TPanel;
    CompilerConfigFrm: TCompilerConfigFrm;
    procedure FormCreate(Sender: TObject);
    procedure CompilerConfigFrmAbrirDlgCanClose(Sender: TObject;
      var CanClose: Boolean);
  private
    FCompilerProfile: TCompilerProfile;
  public
    { Public declarations }
  end;

var
  EditCompilerProfileFrm: TEditCompilerProfileFrm;

function EditCompilerProfile(Profile: TCompilerProfile): Boolean;
implementation

uses Utils;

{$R *.DFM}

function EditCompilerProfile(Profile: TCompilerProfile): Boolean;
begin
  with TEditCompilerProfileFrm.Create(Application) do
  try
    CompilerConfigFrm.InitValues(Profile);
    Caption := LangStrFormat('EditCompileProfileCaption', [Profile.DisplayName]);
    FCompilerProfile := Profile;
    Result := ShowModal = mrOK;
    if Result then
      CompilerConfigFrm.SaveValues(Profile);
  finally
    Free;
  end;
end;

procedure TEditCompilerProfileFrm.FormCreate(Sender: TObject);
begin
  OkBtn.Caption := LangStr('OK');
  CancelBtn.Caption := LangStr('Cancel');
end;

procedure TEditCompilerProfileFrm.CompilerConfigFrmAbrirDlgCanClose(
  Sender: TObject; var CanClose: Boolean);
begin
 CompilerConfigFrm.AbrirDlgCanClose(Sender, CanClose);
end;

end.
