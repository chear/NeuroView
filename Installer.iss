#define VERSION "1.0.0"

[Setup]
;Application name configuration
AppName=BMD100 Developer Tool
AppVersion = {#VERSION}
AppPublisher=NeuroSky, Inc.
UsePreviousAppDir = yes

Compression=lzma
SolidCompression=yes

;Setup wizard configuration
DefaultDirName={pf}\NeuroSky\BMD100
DisableWelcomePage=no
DisableProgramGroupPage=yes
DisableDirPage=yes
DisableReadyPage=no
DisableFinishedPage=no
DefaultGroupName=NeuroSky

;Output file configuration
OutputBaseFilename="BMD100 PC Starter Software"
OutputDir="Installer"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: checkablealone

[Files]
Source: "GUIWindows\bin\Release\*"; DestDir: "{app}"; Flags: recursesubdirs ignoreversion


[Icons]
Name: "{group}\Toshiba Wake Project"; Filename: "{app}\BMD100.exe"
Name: "{userdesktop}\Toshiba Wake Project"; Filename: "{app}\BMD100.exe"; Tasks: desktopicon; IconFilename: "{app}\BMD100.ico"





