; Remember for Omaha update you need to run this setup with 
; '/verysilent /SP-' argument
; You can set argument for setup in admin panel Omaha server

#define MyAppName "TestAppMFC"
#define MyAppVersion "1.0.0.0"
#define Company "Crystalnix"
#define MyAppURL "https://github.com/Crystalnix/appdater-examples"
#define APPID "{{81A78FC5-54CD-4009-B3AD-4791A11FFF45}"
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
Source: "TestAppMFC.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "Sleep Away.mp3"; DestDir: "{app}"; Flags: ignoreversion

[Registry]
Root: HKLM; Subkey: Software\{#Company}\Update\Clients\{#APPID}; ValueType: string; ValueName: "pv"; ValueData: {#MyAppVersion}
Root: HKLM; Subkey: Software\{#Company}\Update\Clients\{#APPID}; ValueType: string; ValueName: "name"; ValueData: {#MyAppName}
