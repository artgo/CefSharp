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
    
  EnumRegKey $0 HKLM "SOFTWARE\Wow6432Node" 0
  IfErrors 0 Is64Bit     
  ; Set output path to the installation directory.
  
  ; Files to copy
  File ${COPYFILES}
  File ${COPYDLL32}
  GOTO ENDCOPY
  
  Is64Bit:
  ; Files to copy
  File ${COPYFILES}
  File ${COPYDLL64}
  GOTO ENDCOPY
       
  ENDCOPY:
    
  WriteRegStr HKCU "${REGSTR}" "DisplayVersion" ${VERSION_SHORT} 
     
SectionEnd

Function .onInit
  !insertmacro WaitForDead
FunctionEnd

Function .onInstSuccess
  Exec "${APPEXEPATH}"
FunctionEnd

