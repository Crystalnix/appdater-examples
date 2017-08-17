!include nsProcess.nsh
!include LogicLib.nsh

!define Version 1.0.0.1
!define Company TestCo
!define AppID {1D99C42C-F620-4853-B2DE-ED5EB7D18C10}
!define AppName TestAppWPF

Name ${AppName}
OutFile TestAppWPFSetup.exe
SilentInstall silent
InstallDir $PROGRAMFILES\${AppName}

Section

  SetOutPath $InstDir
  File "TestAppWPF.exe"
  File "Sleep Away.mp3"
  
  WriteRegStr HKLM Software\${Company}\Update\Clients\${AppID} pv ${Version} 
  WriteRegStr HKLM Software\${Company}\Update\Clients\${AppID} name ${AppName} 
  
SectionEnd

Function .onInit
    ${nsProcess::FindProcess} ${AppName}.exe $R0
  ${If} $R0 == 0
	${nsProcess::KillProcess} ${AppName}.exe $R0
	Sleep 1000	
	${nsProcess::Unload} 
  ${EndIf} 
FunctionEnd