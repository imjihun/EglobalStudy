library hmne_addfiles;

uses
  Windows,
  SysUtils,
  Classes,
  Dialogs,
  Controls,
  Forms,
  UAddFiles in 'UAddFiles.pas' {AddFilesFrm},
  UEditFiles in 'UEditFiles.pas' {EditFilesFrm},
  PluginsInt in '..\..\..\Source\PluginsInt.pas',
  UEditDirectory in 'UEditDirectory.pas' {EditDirectoryFrm};

var
  Data: TPluginData;


procedure ItemClick(ItemName: PChar); cdecl;
var
  Len, C: Integer;
  LastDir, S: String;
begin
  with TAddFilesFrm.Create(Application) do
  try
    SetLength(RelativeBase, MAX_PATH + 1);
    Len := Data.Command(EC_FILE_GET_CURRENT_FILENAME, Integer(PChar(RelativeBase)),0,0);
    if Len > 0 then
      SetLength(RelativeBase, Len)
    else
      RelativeBase := '';
  
    if ShowModal <> mrOK then Exit;

    S := '';
    LastDir := '';


    for C := 0 to FileLst.Items.Count - 1 do
      with FileLst.Items[C] do
      begin
        if not AnsiSameText(LastDir, SubItems[0]) then
        begin
          LastDir := SubItems[0];
          S := S + Format('  SetOutPath "%s"'#13#10, [LastDir]);
        end;
        if (ExtractFileName(Caption) = '*.*') and (SubItems[1] = '1') then
          S := S + Format('  File /r "%s"'#13#10, [Caption])
        else
          S := S + Format('  File "%s"'#13#10, [Caption]);
      end;

    Data.Command(EC_EDIT_SETSELTEXT, Integer(PChar(S)), 0, 0);  
  finally
    Free;
  end;
end;

procedure UpdateItem(ItemName: PChar); cdecl;
var
  ItemState: Integer;
begin
  ItemState := Data.Command(EC_TB_GETITEMSTATE, Integer(ItemName), 0, 0);
  if Data.Command(EC_EDIT_AVAIL, 0, 0, 0) <> 0 then
    ItemState := ItemState or TBITEM_STATE_ENABLED
  else
    ItemState := ItemState and not TBITEM_STATE_ENABLED;
  Data.Command(EC_TB_SETITEMSTATE, Integer(ItemName), ItemState, 0);
end;

procedure Init; cdecl;
var
  ItemData: TTBItemData;
begin
  ZeroMemory(@ItemData, SizeOf(TTBItemDAta));
  ItemData.Name := 'tmAddFilesItem';
  ItemData.Caption := 'Add files to instalation';
  ItemData.Hint := '???';
  ItemData.OnUpdate := UpdateItem;
  ItemData.OnClick := ItemClick;
  Data.Command(EC_TB_INSERTITEM, Integer(PChar('ToolsMenu')), 2, Integer(@ItemData));
  Application.Handle := Data.ApplicationHandle;
end;

procedure About(ParentWindow: HWND); cdecl;
const
  Msg = '(c) 2004 Hector Mauricio Rodriguez';
begin
  MessageBox(ParentWindow, Msg, Data.Description, MB_OK);
end;

function GetHMNEPluginData: PPluginData; stdcall;
begin
  Data.Version := HMNE_VERSION;
  Data.Description := 'AddFiles v0.0';
  Data.Init := Init;
  Data.About := About;
  Result := @Data;
end;

exports
  GetHMNEPluginData;

begin
end.
