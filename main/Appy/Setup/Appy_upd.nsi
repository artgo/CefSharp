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
InstallDir "${APPDIR}"

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKCU "${REGISTRYPATH}" "Install_Dir"

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
!insertmacro WaitForDead
FunctionEnd

Function .onInstSuccess
Exec "$INSTDIR\${APPNAME}.exe"
FunctionEnd

