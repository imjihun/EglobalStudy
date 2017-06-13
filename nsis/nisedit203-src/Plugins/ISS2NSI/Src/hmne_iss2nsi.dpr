{
   HM ISS2NSI (NIS Edit plugin version)
   (c) 2003 Hector Mauricio Rodriguez Segura, http://www.espanol.geocities.com/ranametalcr
   <ranametal@blistering.net>

   Includes portions derived from Inno Setup, (c) 1998-2002 Jordan Russell, www.jrsoftware.org.

}
library hmne_iss2nsi;


uses
  Windows, SysUtils, Classes, Forms, Dialogs,
  Compile in 'Compile.pas',
  PluginsInt in '..\..\..\Source\PluginsInt.pas';

var
  Data: TPluginData;

procedure Convert(const InnoSetupScript: String);
var
  Lines: TStrings;
begin
  Lines := TStringList.Create;
  try
    ISConvertFileToLines(InnoSetupScript, Lines);
    Data.Command(EC_FILE_OPEN, 0, 0, 0);
    Data.Command(EC_EDIT_SETTEXT, Integer(PChar(Lines.Text)), 0, 0);
  finally
    Lines.Free;
  end;
end;

procedure ItemClick(ItemName: PChar); cdecl;
var
  C: Integer;
begin
  with TOpenDialog.Create(nil) do
  try
    Filter := 'InnoSetup script files (*.iss)|*.iss';
    Options := Options + [ofAllowMultiSelect];
    if Execute then
      for C := 0 to Files.Count - 1 do
        try
          Convert(Files[C]);
        except
          Application.HandleException(Application);
        end;
  finally
    Free;
  end;
end;

procedure Init; cdecl;
var
  ItemData: TTBItemData;
begin
  ZeroMemory(@ItemData, SizeOf(TTBItemDAta));
  ItemData.Name := 'tmIss2NsisItem';
  ItemData.Caption := 'Convert InnoSetup script';
  ItemData.Hint := 'Convert InnoSetup scripts into NSIS scripts';
  ItemData.OnClick := ItemClick;
  Data.Command(EC_TB_INSERTITEM, Integer(PChar('ToolsMenu')), 2, Integer(@ItemData));
  Application.Handle := Data.ApplicationHandle;
end;

procedure About(ParentWindow: HWND); cdecl;
const
  Msg = 'ISS2NSI (c) 2003 Hector Mauricio Rodriguez'#13 +
        'Convert InnoSetup Scripts to NSIS scripts.'#13#13 +
        'Based on InnoSetup Compiler 3.0.6 (c) 2003 Jordan Russell.';
begin
  MessageBox(ParentWindow, Msg, Data.Description, MB_OK);
end;

function GetHMNEPluginData: PPluginData; stdcall;
begin
  Data.Version := HMNE_VERSION;
  Data.Description := 'ISS2NSI v' + Iss2Nsi_Version;
  Data.Init := Init;
  Data.About := About;
  Result := @Data;
end;

exports
  GetHMNEPluginData;

begin
end.
