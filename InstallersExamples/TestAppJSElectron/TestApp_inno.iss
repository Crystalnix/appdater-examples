; Remember for Omaha update you need to run this setup with 
; '/verysilent /SP-' argument
; You can set argument for setup in admin panel Omaha server

#define MyAppName "TestAppElectron"
#define MyAppVersion "1.0.0.0"
#define Company "Crystalnix"
#define MyAppURL "https://github.com/Crystalnix/appdater-examples"
#define APPID "{{D55FBE19-2B2A-4C1C-AF0E-F1EC01A7559E}"
[Setup]
AppId= {#APPID}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#Company}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppName}
DisableDirPage=yes
DisableProgramGroupPage=yes
OutputDir=./
OutputBaseFilename={#MyAppName}SetupInno
Compression=lzma
SolidCompression=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "..\..\TestAppJSElectron\out\make\squirrel.windows\x64\TestAppElectron-1.0.0 Setup.exe"; DestDir: "{app}"; Flags: ignoreversion

[Registry]
Root: HKLM; Subkey: Software\{#Company}\Update\Clients\{#APPID}; ValueType: string; ValueName: "pv"; ValueData: {#MyAppVersion}
Root: HKLM; Subkey: Software\{#Company}\Update\Clients\{#APPID}; ValueType: string; ValueName: "name"; ValueData: {#MyAppName}
