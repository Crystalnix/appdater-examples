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
  CreateShortCut "$SMPROGRAMS\${AppName}.lnk" "$InstDir\${AppName}.exe"  
  WriteUninstaller "uninstall.exe"
SectionEnd

Section "uninstall"
  Delete "$InstDir\uninstall.exe"
  Delete "$InstDir\${AppName}.exe"
  Delete "$InstDir\Sleep Away.mp3"
  Delete "$InstDir\Update.ini"
  Delete "c:\Update.ini"
  Delete "$InstDir\UpdateLib.dll" 
  Delete "$InstDir"
  Delete "$SMPROGRAMS\${AppName}.lnk" 
  DeleteRegKey HKLM Software\${Company}\Update\Clients\${AppID}
  DeleteRegKey HKLM Software\Microsoft\Windows\CurrentVersion\Uninstall\${AppName}
SectionEnd

Function .onInit
  WriteRegStr HKLM Software\${Company}\Update\Clients\${AppID} update 1 
  Sleep 5000 ; Time to close all instances of application.
FunctionEnd

Function .onInstSuccess
  WriteRegStr HKLM Software\${Company}\Update\Clients\${AppID} update 0 
  WriteRegStr HKLM Software\${Company}\Update\Clients\${AppID} pv ${Version}  
  WriteRegStr HKLM Software\${Company}\Update\Clients\${AppID} name ${AppName} 
  
  WriteRegStr HKLM Software\Microsoft\Windows\CurrentVersion\Uninstall\${AppName} "DisplayName" ${AppName}
  WriteRegStr HKLM Software\Microsoft\Windows\CurrentVersion\Uninstall\${AppName} "UninstallString" "$\"$INSTDIR\uninstall.exe$\""
  WriteRegStr HKLM Software\Microsoft\Windows\CurrentVersion\Uninstall\${AppName} "QuietUninstallString" "$\"$INSTDIR\uninstall.exe$\" /S"
  WriteRegStr HKLM Software\Microsoft\Windows\CurrentVersion\Uninstall\${AppName} "Publisher" "${Company}"
  WriteRegStr HKLM Software\Microsoft\Windows\CurrentVersion\Uninstall\${AppName} "DisplayVersion" "${Version}"
FunctionEnd

#1 Silent is mandatory requires for the installer.
#2 There is need to set key value in the registry.
#3 There is need to close Application before install because the installer will can't change executed file. 
