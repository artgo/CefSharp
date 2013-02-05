; Appy_upd.nsi
!include "InstallerShared.nsh"
!include "LogicLib.nsh"

;--------------------------------

; The name of the installer
Name "${AppName}_upd"

; The file to write
!define OUTFILE "${AppName}_upd.exe"
OutFile "${OUTFILE}"

; The default installation directory
InstallDir "${APPDIR}\${APPNAME}"

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKLM "Software\Appy" "Install_Dir"

SilentInstall silent
;--------------------------------

; Pages

;--------------------------------

; The stuff to install
Section "Appy (required)"

  SectionIn RO
  
  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
    
  ; Files to copy
  File /r /x Appy\ApplicationData\*.* Appy\*.*
     
SectionEnd

Function .onInit
loop:
	IntOp $0 $0 + 1
	Processes::FindProcess "${APPNAME}.Browser.exe"
	${If} $R0 == "0"
	Processes::FindProcess "${APPNAME}.exe"
	StrCmp $R0 "0" done
	StrCmp $0 "50" done
	${EndIf}	
	Sleep 200
	Goto loop
done:
FunctionEnd

Function .onInstSuccess
Exec "$INSTDIR\${APPNAME}.exe"
FunctionEnd

