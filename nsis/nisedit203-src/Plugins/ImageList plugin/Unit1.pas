unit Unit1;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  ComCtrls, PluginsInt, StdCtrls, ExtCtrls, ExtDlgs;

type
  TForm1 = class(TForm)
    ListView: TListView;
    ImageList: TImageList;
    Panel1: TPanel;
    Button1: TButton;
    Button3: TButton;
    SaveDlg: TSavePictureDialog;
    Button4: TButton;
    procedure Button1Click(Sender: TObject);
    procedure Button3Click(Sender: TObject);
    procedure Button4Click(Sender: TObject);
  private
    function GetSelectedImages(Bmp: TBitmap): Boolean;
  public
    procedure InitImages;
  end;

var
  Form1: TForm1;
  Data: TPluginData;

const
  Version = '1.0';
  VersionString = 'v' + Version;
  
function GetHMNEPluginData: PPluginData; stdcall;
implementation

uses Unit2, Clipbrd;

{$R *.DFM}

procedure TBItemClick(ItemName: PChar); cdecl;
begin
  if Form1 = nil then
    Form1 := TForm1.Create(Application);
  Form1.InitImages;
  Form1.Show;
end;

procedure Init; cdecl;
var
  ItemData: TTBItemData;
begin
  ZeroMemory(@ItemData, SizeOf(TTBItemData));
  ItemData.Name := 'ShowImageListItem';
  ItemData.Caption := 'Image List';
  ItemData.Hint := 'Show the application main image list.';
  ItemData.OnClick := TBItemClick;
  Data.Command(EC_TB_INSERTITEM, Integer(PChar('ViewMenu')), 0, 0);
  Data.Command(EC_TB_INSERTITEM, Integer(PChar('ViewMenu')), 0, Integer(@ItemData));
  Application.Handle := Data.ApplicationHandle;
end;

procedure About(ParentHandle: HWND); cdecl;
begin
  with TForm2.Create(Application) do
  try
    ShowModal;
  finally
    Free;
  end;
end;

function GetHMNEPluginData: PPluginData; stdcall;
begin
  // Set up data
  Data.Version := HMNE_VERSION; // Version number (required)
  Data.Description := 'ImageList Plugin ' + VersionString; // Plugin description (required)
  Data.Init := Init; // Initialization procedure (optional)
  Data.About := About; // For show the about box (optional)
  Result := @Data;
end;

{ TForm1 }

function TForm1.GetSelectedImages(Bmp: TBitmap): Boolean;
var
  SelCount, C: Integer;
  ImgBmp: TBitmap;
begin
  SelCount := ListView.SelCount;
  Result := SelCount > 0;
  if not Result then Exit;
  Bmp.Height := ImageList.Height;
  Bmp.Width := ImageList.Width * SelCount;
  SelCount := 0;
  for C := 0 to ListView.Items.Count - 1 do
    if ListView.Items[C].Selected then
    begin
      ImgBmp := TBitmap.Create;
      try
        ImageList.GetBitmap(C, ImgBmp);
        Bmp.Canvas.Draw(SelCount * ImageList.Width, 0, ImgBmp);
      finally
        ImgBmp.Free;
      end;
      Inc(SelCount);
    end;
end;

procedure TForm1.InitImages;
var
  C: Integer;
begin
  ImageList.Handle := Data.Command(EC_GETIMAGELIST, 0, 0, 0);
  ListView.Items.BeginUpdate;
  try
    ListView.Items.Clear;
    for C := 0 to ImageList.Count - 1 do
      with ListView.Items.Add do
      begin
        Caption := IntToStr(C);
        ImageIndex := C;
      end;
  finally
    ListView.Items.EndUpdate;
  end;
end;

procedure TForm1.Button1Click(Sender: TObject);
var
  Bmp: TBitmap;
begin
  if ListView.Selected <> nil then
  if SaveDlg.Execute then
  begin
    Bmp := TBitmap.Create;
    try
      GetSelectedImages(Bmp);
      Bmp.SaveToFile(SaveDlg.FileName);
    finally
      Bmp.Free;
    end;
  end;
end;

procedure TForm1.Button3Click(Sender: TObject);
var
  Bmp: TBitmap;
begin
  Bmp := TBitmap.Create;
  try
    GetSelectedImages(Bmp);
    Clipboard.Assign(Bmp);
  finally
    Bmp.Free;
  end;
end;

procedure TForm1.Button4Click(Sender: TObject);
begin
  Close;
end;

end.
