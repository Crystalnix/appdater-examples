!include nsProcess.nsh ;http://nsis.sourceforge.net/NsProcess_plugin
!include LogicLib.nsh

!define Version 1.0.0.0
!define Company "Crystalnix"
!define AppID {DB9A0020-9969-47A3-8E9B-0BBA1A56C648}
!define AppName TestAppJS

Name ${AppName}
OutFile ${AppName}Setup.exe
SilentInstall silent       ;Important #1
InstallDir $PROGRAMFILES\${AppName}

Section

  SetOutPath $InstDir
  File "..\..\TestAppJS\TestAppJS.js"
  File "Sleep Away.mp3"
  
  WriteRegStr HKLM Software\${Company}\Update\Clients\${AppID} pv ${Version}  ;Important #2
  WriteRegStr HKLM Software\${Company}\Update\Clients\${AppID} name ${AppName} 
  
SectionEnd

Function .onInit

;Important #3
    ${nsProcess::FindProcess} ${AppName}.exe $R0
  ${If} $R0 == 0
	${nsProcess::KillProcess} ${AppName}.exe $R0
	Sleep 1000	
	${nsProcess::Unload} 
  ${EndIf} 
  
FunctionEnd

#1 Silent is mandatory requires for the installer.
#2 There is need to set key value in the registry.
#3 There is need to close Application before install because the installer will can't change main .exe file.