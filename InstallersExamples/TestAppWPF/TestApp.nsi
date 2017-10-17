!define Version 1.0.0.1
!define Company "Crystalnix"
!define AppID {5A7490AC-F314-4BD3-9591-7B6E9CEA5560}
!define AppName TestAppWPF
!define LinkDir $PROGRAMFILES\${Company}\${AppName}

Name ${AppName}
OutFile ${AppName}Setup.exe
SilentInstall silent       ;Important #1
InstallDir ${LinkDir}\${Version}


Section 
  SetOutPath $InstDir
  File "${AppName}.exe"
  File "Sleep Away.mp3"
  File "..\..\${AppName}\bin\Debug\Update.ini"
  File "..\..\${AppName}\bin\Debug\UpdateLib.dll" 
  CreateShortCut "${LinkDir}\${AppName}.lnk" "$InstDir\${AppName}.exe"   
  Delete "$SMPROGRAMS\${AppName}.lnk" 
  CreateShortCut "$SMPROGRAMS\${AppName}.lnk" "${LinkDir}\${AppName}.lnk"  
  WriteUninstaller "uninstall.exe"
SectionEnd

Section "uninstall"
  RMDir /r /REBOOTOK ${LinkDir}
  Delete "$SMPROGRAMS\${AppName}.lnk" 

  DeleteRegKey HKLM Software\${Company}\Update\Clients\${AppID}
  DeleteRegKey HKLM Software\Microsoft\Windows\CurrentVersion\Uninstall\${AppName}
SectionEnd

Function .onInstSuccess
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
