; Remember for Omaha update you need to run this setup with 
; '/verysilent /SP-' argument
; You can set argument for setup in admin panel Omaha server

#define MyAppName "TestAppJavaFX"
#define MyAppVersion "1.0.0.0"
#define Company "Crystalnix"
#define MyAppURL "https://github.com/Crystalnix/appdater-examples"
#define APPID "{{D639B7EF-7F72-4182-95C2-128CB4B90199}"
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
Source: "..\..\TestAppJavaFX\dist\*"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\..\TestAppJavaFX\dist\web-files\*"; DestDir: "{app}"; Flags: ignoreversion
Source: "Sleep Away.mp3"; DestDir: "{app}"; Flags: ignoreversion

[Registry]
Root: HKLM; Subkey: Software\{#Company}\Update\Clients\{#APPID}; ValueType: string; ValueName: "pv"; ValueData: {#MyAppVersion}
Root: HKLM; Subkey: Software\{#Company}\Update\Clients\{#APPID}; ValueType: string; ValueName: "name"; ValueData: {#MyAppName}
