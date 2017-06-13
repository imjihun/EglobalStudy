unit UEditDirectory;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  StdCtrls;

type
  TEditDirectoryFrm = class(TForm)
    DirEdt: TEdit;
    Button1: TButton;
    StaticText2: TStaticText;
    DestinoEdt: TComboBox;
    StaticText1: TStaticText;
    RelativeChk: TCheckBox;
    IndividualChk: TCheckBox;
    RecurseChk: TCheckBox;
    Button4: TButton;
    Button5: TButton;
    procedure Button1Click(Sender: TObject);
  private
    { Private declarations }
  public
    { Public declarations }
  end;

var
  EditDirectoryFrm: TEditDirectoryFrm;

implementation
uses ShellAPI, ActiveX, ComObj, ShlObj;

{$R *.DFM}

function SelectDirectory(const Caption: string; out Directory: string): Boolean;
var
  WindowList: Pointer;
  BrowseInfo: TBrowseInfo;
  Buffer: PChar;
  ItemIDList: PItemIDList;
  ShellMalloc: IMalloc;
  CurFrm: TForm;
begin
  Result := False;
  Directory := '';
  FillChar(BrowseInfo, SizeOf(BrowseInfo), 0);
  if (ShGetMalloc(ShellMalloc) = S_OK) and (ShellMalloc <> nil) then
  begin
    Buffer := ShellMalloc.Alloc(MAX_PATH);
    try
      with BrowseInfo do
      begin
        CurFrm := Screen.ActiveForm;
        if Assigned(CurFrm) then
          hwndOwner := CurFrm.Handle
        else
          hwndOwner := Application.Handle;
        pidlRoot := nil;
        pszDisplayName := Buffer;
        lpszTitle := PChar(Caption);
        ulFlags := BIF_RETURNONLYFSDIRS;
      end;
      WindowList := DisableTaskWindows(0);
      try
        EnableWindow(Application.Handle, False);
        try
          ItemIDList := ShBrowseForFolder(BrowseInfo);
        finally
          EnableWindow(Application.Handle, True);
        end;
      finally
        EnableTaskWindows(WindowList);
      end;
      Result :=  ItemIDList <> nil;
      if Result then
      begin
        ShGetPathFromIDList(ItemIDList, Buffer);
        ShellMalloc.Free(ItemIDList);
        Directory := Buffer;
      end;
    finally
      ShellMalloc.Free(Buffer);
    end;
  end;
end;

procedure TEditDirectoryFrm.Button1Click(Sender: TObject);
var
  Dir: String;
begin
  if SelectDirectory('Please select the directory to add:', Dir) then
    DirEdt.Text := Dir;
end;

end.
