unit Unit2;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  ExtCtrls, StdCtrls;

type
  TForm2 = class(TForm)
    Button1: TButton;
    Label1: TLabel;
    Label2: TLabel;
    Bevel1: TBevel;
    Label3: TLabel;
    procedure Label3Click(Sender: TObject);
    procedure FormCreate(Sender: TObject);
  private
    { Private declarations }
  public
    { Public declarations }
  end;

var
  Form2: TForm2;

implementation

uses ShellAPI, Unit1;

{$R *.DFM}

procedure TForm2.Label3Click(Sender: TObject);
begin
  ShellExecute(Handle, 'open', PChar('mailto:' + Label3.Caption),
    nil, nil, SW_NORMAL);
end;

procedure TForm2.FormCreate(Sender: TObject);
begin
  Label1.Caption := Label1.Caption + VersionString;
end;

end.
