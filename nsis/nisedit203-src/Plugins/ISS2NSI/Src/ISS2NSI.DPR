{
   HM ISS2NSI
   (c) 2003 Hector Mauricio Rodriguez Segura, http://www.espanol.geocities.com/ranametalcr
   <ranametal@blistering.net>

   Includes portions derived from Inno Setup, (c) 1998-2002 Jordan Russell, www.jrsoftware.org.

}

{$APPTYPE CONSOLE}

program iss2nsi;
uses
  SysUtils, Classes,
  Compile in 'Compile.pas';

var
  InnoSetupScript, NSISScript: String;
begin
  try
    InnoSetupScript := ParamStr(1);
    NSISScript := ParamStr(2);

    if NSISScript = '' then
      NSISScript := ChangeFileExt(InnoSetupScript, '.nsi');

    Writeln;
    Writeln('HM ISS2NSI v' + Iss2Nsi_Version);
    Writeln;
    Writeln('(c) 2003 Hector Mauricio Rodriguez Segura, http://www.espanol.geocities.com/ranametalcr.');
    Writeln('Includes portions derived from Inno Setup, (c) 1998-2002 Jordan Russell,');
    Writeln('www.jrsoftware.org.');
    if InnoSetupScript = '' then
    begin
      Writeln;
      Writeln('Convert Inno Setup Script files Into NSIS script files.');
      Writeln;
      Writeln('Usage:    iss2nsi isscript.iss [nsiscript.nsi]');
      Halt(1);
    end;
    Writeln;

    ISConvertFileToFile(NSISScript, InnoSetupScript);
    Writeln(Format('"%s" successfully converted to "%s"', [InnoSetupScript, NSISScript]));

  except
    on E: Exception do
    begin
      Writeln('Fatal error: ' + E.Message);
      Halt(2);
    end;
  end;
end.
