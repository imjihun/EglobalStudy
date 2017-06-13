{
  HM NIS Edit (c) 2003 Héctor Mauricio Rodríguez Segura <ranametal@users.sourceforge.net>
  For conditions of distribution and use, see license.txt

  Config edit simbol form
}
unit UEditSimbol;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  StdCtrls, ExtCtrls, UIStateForm, UCompilerConfig;

type
  TEditSimbolFrm = class(TUIStateForm)
    Label1: TStaticText;
    SimbEdt: TEdit;
    Label2: TStaticText;
    ValEdt: TEdit;
    Button1: TButton;
    Button2: TButton;
    Bevel1: TBevel;
    procedure FormCreate(Sender: TObject);
    procedure FormCloseQuery(Sender: TObject; var CanClose: Boolean);
  private
    FConfigFrame: TCompilerConfigFrm;
  public
    procedure LoadStrings(const ACaption, ALabel1, ALabel2: String);
  end;

{var
  EditSimbolFrm: TEditSimbolFrm;}

function EditSimbol(var Simbol, Val: String; AConfigFrame: TCompilerConfigFrm): Boolean;
implementation

uses Utils, UConfig;

{$R *.DFM}

function EditSimbol(var Simbol, Val: String; AConfigFrame: TCompilerConfigFrm): Boolean;
begin
  with TEditSimbolFrm.Create(Application) do
  try
    LoadStrings('EditSimbolCaption', 'Simbol', 'Value');
    SimbEdt.Text := Simbol;
    ValEdt.Text := Val;
    FConfigFrame :=  AConfigFrame;
    Result := ShowModal = mrOK;
    if Result then
    begin
      Simbol := SimbEdt.Text;
      Val := ValEdt.Text;
    end;
  finally
    Free;
  end;
end;

procedure TEditSimbolFrm.FormCreate(Sender: TObject);
begin
   InitFont(Font);
   Button2.Caption := LangStr('OK');
   Button1.Caption := LangStr('Cancel');
end;

procedure TEditSimbolFrm.FormCloseQuery(Sender: TObject;
  var CanClose: Boolean);

   function YaExiste: Boolean;
   var
     C: Integer;
   begin
     Result := True;
     for C := 0 to FConfigFrame.SimbLst.Items.Count - 1 do
       if AnsiSameText(FConfigFrame.SimbLst.Items[C].Caption, SimbEdt.Text) and
       (FConfigFrame.AddingSymbol or (FConfigFrame.SimbLst.Items[C] <> FConfigFrame.SimbLst.Selected)) then Exit;
     Result := False;
   end;

   procedure Error(const LangStrId: String);
   begin
      WarningDlg(LangStr(LangStrId));
      SimbEdt.SetFocus;
      Abort;
   end;

begin
  if ModalResult = mrOK then
  begin
    if SimbEdt.Text = '' then Error('NoSymbolName');
    if AnsiPos(' ', SimbEdt.Text) > 0 then Error('NoSymbolSpaces');
    if YaExiste then Error('DuplicateSymbol');
  end;
end;

procedure TEditSimbolFrm.LoadStrings(const ACaption, ALabel1,
  ALabel2: String);
begin                 
   Caption := LangStr(ACaption);
   Label1.Caption := LangStr(ALabel1);
   Label2.Caption := LangStr(ALabel2);
end;

end.
