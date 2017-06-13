unit UEditFiles;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  StdCtrls;

type
  TEditFilesFrm = class(TForm)
    StaticText1: TStaticText;
    FilesEdt: TEdit;
    Button1: TButton;
    DestinoEdt: TComboBox;
    StaticText2: TStaticText;
    Button4: TButton;
    Button5: TButton;
    OpenDlg: TOpenDialog;
    RelativeChk: TCheckBox;
    procedure Button1Click(Sender: TObject);
  private
    { Private declarations }
  public
    { Public declarations }
  end;

var
  EditFilesFrm: TEditFilesFrm;

implementation

{$R *.DFM}

procedure TEditFilesFrm.Button1Click(Sender: TObject);
var
  C: Integer;
  S: String;
begin
  if OpenDlg.Execute then
  begin
    S := '';
    for C := 0 to OpenDlg.Files.Count - 1 do
      S := S + OpenDlg.Files[C] + ';';
    if (S <> '') and (AnsiLastChar(S)^ = ';') then
      SetLength(S, Length(S) - 1); 
    FilesEdt.Text := S;
  end;
end;

end.
