; This script generate a Inno Setup installer which install the iss2nsi tool, and is
; used to generate his NSIS equivalent. See sample.nsi

[Setup]
AppName=ISS2NSI
AppVerName=ISS2NSI 1.0a1
AppVersion=1.0a1
AppPublisher=Héctor Mauricio Rodríguez Segura
AppPublisherURL=http://www.geocities.com/hmsoftcr
AppSupportURL=http://www.geocities.com/hmsoftcr
AppUpdatesURL=http://www.geocities.com/hmsoftcr
DefaultDirName={pf}\HMSoft\ISS2NSI
DisableProgramGroupPage=yes
LicenseFile=license.txt
OutputDir=.

[Files]
Source: "iss2nsi.exe"; DestDir: "{app}";
Source: "license.txt"; DestDir: "{app}";
Source: "readme.txt"; DestDir: "{app}";
Source: "sample.nsi"; DestDir: "{app}";
Source: "sample.iss"; DestDir: "{app}";
Source: "Src\iss2nsi.dof"; DestDir: "{app}\src";
Source: "Src\Compile.pas"; DestDir: "{app}\src";
Source: "Src\iss2nsi.dpr"; DestDir: "{app}\src";
Source: "Src\is-src\Version.inc"; DestDir: "{app}\src\iss-src";
Source: "Src\is-src\CompMsgs.pas"; DestDir: "{app}\src\iss-src";
Source: "Src\is-src\is-license.txt"; DestDir: "{app}\src\iss-src";
Source: "Src\is-src\Struct.pas"; DestDir: "{app}\src\iss-src";
Source: "Src\is-src\CompInt.pas"; DestDir: "{app}\src\iss-src";

[Registry]
Root: HKCR; Subkey: "InnoSetupScriptFile\Shell\ConvertToNSIS"; ValueType: string; ValueName: ""; ValueData: "Convert to NSIS script"; Flags: uninsdeletekey
Root: HKCR; Subkey: "InnoSetupScriptFile\Shell\ConvertToNSIS\Command"; ValueType: string; ValueName: ""; ValueData: """{app}\iss2nsi"" ""%1"""; Flags: uninsdeletekey

