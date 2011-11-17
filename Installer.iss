#define VERSION "1.0.4"

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
AlwaysShowDirOnReadyPage=yes
DisableFinishedPage=no
DefaultGroupName=NeuroSky

;Output file configuration
OutputBaseFilename="BMD100 Setup"
OutputDir="CD"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: checkablealone

[Files]
Source: "CD\*"; DestDir: "{app}"; Flags: recursesubdirs ignoreversion


[Icons]
Name: "{group}\BMD100"; Filename: "{app}\"
Name: "{userdesktop}\BMD100 PC Starter Software"; Filename: "{app}\PC Starter Software\BMD100.exe"; Tasks: desktopicon; IconFilename: "{app}\PC Starter Software\BMD100.ico"


[Run]
Filename: "{app}\Bluetooth Driver\Setup.exe"


