; Inno Setup script for GameScout.
; Packages the published output (see docs/RELEASING.md) into GameScoutSetup-<version>.exe.
; Build with:  iscc /DAppVersion=0.3.0 installer\GameScout.iss
; Requires the app to be published first to: publish\  (self-contained win-x64).

#ifndef AppVersion
  #define AppVersion "0.0.0"
#endif

#define AppName "GameScout"
#define AppPublisher "Lukas Rejci"
#define AppExe "GameScout.exe"
#define AppUrl "https://github.com/lukr-99/GameScout"

[Setup]
AppId={{7C7F5B9E-2E2E-4B7E-9C2A-2A6E5F4D1A20}
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher={#AppPublisher}
AppSupportURL={#AppUrl}
DefaultDirName={autopf}\{#AppName}
DefaultGroupName={#AppName}
DisableProgramGroupPage=yes
OutputDir=dist
OutputBaseFilename=GameScoutSetup-{#AppVersion}
SetupIconFile=..\src\GameScout.App\Assets\gamescout.ico
UninstallDisplayIcon={app}\{#AppExe}
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
; Per-user install: no admin prompt, matches the app's HKCU startup registration.
PrivilegesRequired=lowest
ArchitecturesInstallIn64BitMode=x64compatible

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "Create a desktop shortcut"; GroupDescription: "Additional shortcuts:"; Flags: unchecked

[Files]
; Everything produced by `dotnet publish ... -o publish`.
Source: "..\publish\*"; DestDir: "{app}"; Flags: recursesubdirs createallsubdirs ignoreversion

[Icons]
Name: "{group}\{#AppName}"; Filename: "{app}\{#AppExe}"
Name: "{userdesktop}\{#AppName}"; Filename: "{app}\{#AppExe}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#AppExe}"; Description: "Launch GameScout"; Flags: nowait postinstall skipifsilent
