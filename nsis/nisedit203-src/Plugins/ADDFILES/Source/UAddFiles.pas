unit UAddFiles;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  StdCtrls, ComCtrls;

type
  TAddFilesFrm = class(TForm)
    FileLst: TListView;
    Button1: TButton;
    Button3: TButton;
    Button4: TButton;
    Button5: TButton;
    Button6: TButton;
    procedure Button1Click(Sender: TObject);
    procedure Button6Click(Sender: TObject);
    procedure Button3Click(Sender: TObject);
    procedure FormCreate(Sender: TObject);
    procedure FormDestroy(Sender: TObject);
  private
    DirList: TStringList;
    function GetFileName(const S: String; const Relative: Boolean): String;
  public
    RelativeBase: String;
  end;

var
  AddFilesFrm: TAddFilesFrm;
  LastDir: String = '$INSTDIR';

const
  SYes = 'Yes';
  SNo = 'No';

  BoolStrs: array[Boolean] of String = (SNo, SYes);

  SDirs =
  '$INSTDIR'#13#10 +
  '$PROGRAMFILES'#13#10 +
  '$PLUGINSDIR'#13#10 +
  '$TEMP'#13#10 +
  '$DESKTOP'#13#10 +
  '$SYSDIR'#13#10 +
  '$EXEDIR'#13#10 +
  '$WINDIR'#13#10 +
  '$STARTMENU'#13#10 +
  '$SMPROGRAMS'#13#10 +
  '$QUICKLAUNCH'#13#10 +
  '$COMMONFILES'#13#10 +
  '$DOCUMENTS'#13#10 +
  '$SENDTO'#13#10 +
  '$RECENT'#13#10 +
  '$FAVORITES'#13#10 +
  '$MUSIC'#13#10 +
  '$PICTURES'#13#10 +
  '$VIDEOS'#13#10 +
  '$NETHOOD'#13#10 +
  '$FONTS'#13#10 +
  '$TEMPLATES'#13#10 +
  '$APPDATA'#13#10 +
  '$PRINTHOOD'#13#10 +
  '$INTERNET_CACHE'#13#10 +
  '$COOKIES'#13#10 +
  '$HISTORY'#13#10 +
  '$PROFILE'#13#10 +
  '$ADMINTOOLS'#13#10 +
  '$RESOURCES'#13#10 +
  '$RESOURCES_LOCALIZED'#13#10 +
  '$CDBURN_AREA';


implementation

uses UEditFiles, UEditDirectory;

{$R *.DFM}

function ExtractStr(var S: String; Delim: String): String;
var
  I: Integer;
begin
  I := AnsiPos(Delim, S);
  if I = 0 then I := Length(S)+1;
  Result := Copy(S, 1, I-1);
  S := Copy(S, I+1, Maxint);
end;

function TAddFilesFrm.GetFileName(const S: String; const Relative: Boolean): String;
begin
  if Relative and (RelativeBase <> '') and (AnsiPos('${NSISDIR}', S) = 0) then
    Result := ExtractRelativePath(RelativeBase, S)
  else
    Result := ExpandFileName(S);
end;

procedure TAddFilesFrm.Button1Click(Sender: TObject);
var
  S: String;
begin
  with TEditFilesFrm.Create(Self) do
  try
    RelativeChk.Checked := (RelativeBase <> '') and FileExists(RelativeBase);
    RelativeChk.Enabled := RelativeChk.Checked;

    DestinoEdt.Items.Text := SDirs;
    DestinoEdt.Items.AddStrings(DirList);
    DestinoEdt.Text := LastDir;

    if ShowModal <> mrOK then Exit;

    LastDir := DestinoEdt.Text;
    if Pos(LastDir, SDirs) = 0 then
      DirList.Add(LastDir);

    S := FilesEdt.Text;
    FileLst.Items.BeginUpdate;
    try
      while S <> '' do
        with FileLst.Items.Add do
        begin
          Caption := GetFileName(ExtractStr(S, ';'), RelativeChk.Checked);
          SubItems.Add(DestinoEdt.Text);
        end;
     finally
       FileLst.Items.EndUpdate;
     end;
  finally
    Free;
  end;
end;

procedure TAddFilesFrm.Button6Click(Sender: TObject);
var
  Recurse, Relative: Boolean;

  procedure AddFiles(const SourceDir, OutDir: String);
  var
    Found: Integer;
    SR: TSearchRec;
  begin
    Found := FindFirst(SourceDir, faAnyFile, SR);
    while Found = 0 do
    begin
      if Recurse and (SR.Attr and faDirectory <> 0) and (SR.Name <> '.') and (SR.Name <> '..') then
        AddFiles(ExtractFilePath(SourceDir) + SR.Name + '\*.*', IncludeTrailingBackSlash(OutDir) + SR.Name)
      else if SR.Attr and faDirectory = 0 then
      with FileLst.Items.Add do
      begin
        Caption := GetFileName(ExtractFilePath(SourceDir) + SR.Name, Relative);
        SubItems.Add(OutDir);
      end;
      Found := FindNext(SR);
    end;
    FindClose(SR);
  end;

begin
  with TEditDirectoryFrm.Create(Application) do
  try
    RelativeChk.Checked := (RelativeBase <> '') and FileExists(RelativeBase);
    RelativeChk.Enabled := RelativeChk.Checked;

    DestinoEdt.Items.Text := SDirs;
    DestinoEdt.Items.AddStrings(DirList);
    DestinoEdt.Text := LastDir;

    if ShowModal <> mrOK then Exit;

    LastDir := DestinoEdt.Text;
    if Pos(LastDir, SDirs) = 0 then
      DirList.Add(LastDir);

    if IndividualChk.Checked then
    begin
      Relative := RelativeChk.Checked;
      Recurse := RecurseChk.Checked;
      FileLst.Items.BeginUpdate;
      try
        AddFiles(IncludeTrailingBackSlash(DirEdt.Text) + '*.*', DestinoEdt.Text);
      finally
        FileLst.Items.EndUpdate;
      end;
    end else
    with FileLst.Items.Add do
    begin
      Caption := GetFileName(IncludeTrailingBackSlash(DirEdt.Text) + '*.*', RelativeChk.Checked);
      SubItems.Add(DestinoEdt.Text);
      SubItems.Add(IntToStr(Ord(RecurseChk.Checked)));
    end;
  finally
    Free;
  end;
end;

procedure TAddFilesFrm.Button3Click(Sender: TObject);
var
  Item: TListItem;
  I: Integer;
begin
  if FileLst.SelCount > 1 then
  begin
    I := 0;
    FileLst.Items.BeginUpdate;
    try
      while I < FileLst.Items.Count do
      begin
        if FileLst.Items[I].Selected then
          FileLst.Items[I].Delete
        else
          Inc(I);
      end;
    finally
      FileLst.Items.EndUpdate;
    end;
  end else
  begin
    Item := FileLst.Selected;
    if Item <> nil then
    begin
      I := Item.Index;
      Item.Delete;
      if I >= FileLst.Items.Count then Dec(I);
      if I in [0..FileLst.Items.Count-1] then
        FileLst.Selected := FileLst.Items[I];
    end;
  end;  
end;

procedure TAddFilesFrm.FormCreate(Sender: TObject);
begin
  DirList := TStringList.Create;
  DirList.Sorted := True;
  DirList.Duplicates := dupIgnore;
end;

procedure TAddFilesFrm.FormDestroy(Sender: TObject);
begin
  DirList.Free;
end;

end.
