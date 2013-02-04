; Appy_upd.nsi
;--------------------------------

; The name of the installer
Name "Appy_upd"

; The file to write
!define OUTFILE "Appy_upd.exe"
OutFile "${OUTFILE}"

; The default installation directory
InstallDir "$LOCALAPPDATA\Appy"

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKLM "Software\Appy" "Install_Dir"

; Request application privileges for Windows Vista
RequestExecutionLevel admin

SilentInstall silent
;--------------------------------

; Pages

;--------------------------------

; The stuff to install
Section "Appy (required)"

  SectionIn RO
  
  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  !searchparse /file version.txt '' VERSION_SHORT
  
  ; Put file there
  File /r /x Appy\ApplicationData\*.* Appy\*.*
     
SectionEnd

Function .onInit
loop:
	Processes::FindProcess "Appy.Browser"
	StrCmp $R0 "0" done
	Sleep 500
	Goto loop
done:
FunctionEnd

Function .onInstSuccess
Exec "$INSTDIR\Appy.exe"
FunctionEnd

