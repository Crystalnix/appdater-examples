!define Version 1.0.0.0
!define Company "Crystalnix"
!define AppID {5A7490AC-F314-4BD3-9591-7B6E9CEA5560}
!define AppName TestAppWPF

Name ${AppName}
OutFile ${AppName}Setup.exe
SilentInstall silent       ;Important #1
InstallDir $PROGRAMFILES\${AppName}

Section 
  SetOutPath $InstDir
  File "${AppName}.exe"
  File "Sleep Away.mp3"
  File "..\..\${AppName}\bin\Debug\Update.ini"
  File "..\..\${AppName}\bin\Debug\UpdateLib.dll" 
SectionEnd

Function .onInit
  WriteRegStr HKLM Software\${Company}\Update\Clients\${AppID} update 1 ; Important #3. Set up update flag. This causes to close the all instances of application.
  Sleep 5000 ; Time to close all instances of application.
FunctionEnd

Function .onInstSuccess
  WriteRegStr HKLM Software\${Company}\Update\Clients\${AppID} pv ${Version}  ;Important #2
  WriteRegStr HKLM Software\${Company}\Update\Clients\${AppID} name ${AppName};Important #2
  
  WriteRegStr HKLM Software\${Company}\Update\Clients\${AppID} update 0 ; Prepare for update flag reset.
FunctionEnd

#1 Silent is mandatory requires for the installer.
#2 There is need to set key value in the registry.
#3 There is need to close Application before install because the installer will can't change executed file. 
