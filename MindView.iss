[Setup]
AppName=MindView
AppVerName=MindView v0.5
AppPublisher=NeuroSky, Inc.
DefaultDirName={pf}\NeuroSky\MindView
DefaultGroupName=NeuroSky
OutputBaseFilename="MindView Setup"
Compression=lzma
SolidCompression=yes
OutputDir="C:\Documents and Settings\Developer\My Documents\Visual Studio 2008\Projects"

[Languages]
Name: "eng"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: checkablealone

[Files]
Source: "C:\Documents and Settings\Developer\My Documents\Visual Studio 2008\Projects\GUIWindows\bin\Debug\GUIWindows.exe"; DestDir: "{app}"
Source: "C:\Documents and Settings\Developer\My Documents\Visual Studio 2008\Projects\GUIWindows\bin\Debug\ThinkGear.dll"; DestDir: "{app}"


[Icons]
Name: "{group}\MindView"; Filename: "{app}\GUIWindows.exe"
Name: "{userdesktop}\MindView"; Filename: "{app}\GUIWindows.exe"; Tasks: desktopicon
