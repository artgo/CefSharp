!include "InstallerShared.nsh"
!include "LogicLib.nsh"

;--------------------------------

; The name of the installer
Name "updater"

; The file to write
!define OUTFILE "updater.exe"
OutFile "${OUTFILE}"

; The default installation directory
InstallDir "${APPDIR}"

SilentInstall silent
;--------------------------------

; Pages

;--------------------------------

; The stuff to install
Section "Create"

  SectionIn RO
  
  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
    
 !insertmacro COPYFILES
    
  WriteRegStr HKCU "${REGSTR}" "DisplayVersion" ${VERSION_SHORT} 
     
SectionEnd

Function .onInit
  !insertmacro WaitForDead
FunctionEnd

Function .onInstSuccess
  Exec "${APPEXEPATH}"
FunctionEnd

