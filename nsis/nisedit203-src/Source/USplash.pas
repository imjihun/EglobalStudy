{
  HM NIS Edit (c) 2003-2004 Héctor Mauricio Rodríguez Segura <ranametal@users.sourceforge.net>
  For conditions of distribution and use, see license.txt

  Splash screen form

}
unit USplash;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  ExtCtrls, jpeg, UIStateForm;

type
  TSplashFrm = class(TUIStateForm)
    Image1: TImage;
  private
    { Private declarations }
  public
    { Public declarations }
  end;

var
  SplashFrm: TSplashFrm = nil;

procedure ShowSplash;
procedure HideSplash;
implementation

uses Utils;

{$R *.DFM}

procedure ShowSplash;
begin
  if not SwitchPassed('ForceShowSplash') then
    if SwitchPassed('ns') or OptionsIni.ReadBool('Options', 'NoSplash', True) then
      Exit;
      
  if SplashFrm = nil then
    SplashFrm := TSplashFrm.Create(Application);
  SplashFrm.Show;
  SplashFrm.Update;
  Sleep(OptionsIni.ReadInteger('Options', 'SplashDelay', 1));
end;

procedure HideSplash;
begin
  if SplashFrm = nil then Exit;
  SplashFrm.Hide;
  FreeAndNil(SplashFrm);
end;

end.
