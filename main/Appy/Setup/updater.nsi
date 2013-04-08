!include "InstallerShared.nsh"
!include "MUI.nsh"
!include "LogicLib.nsh"
!include "FindProcess.nsh"

;--------------------------------

; The name of the installer
Name "updater"

;--------------------------------
;Replace Default Installer Icon
!define MUI_ICON "${INSTALLERICON}"
;--------------------------------

; The file to write
!define OUTFILE "updater.exe"
OutFile "${OUTFILE}"

; The default installation directory
InstallDir "${APPDIR}"
;--------------------------------
;Languages
!insertmacro MUI_LANGUAGE "English"
;--------------------------------
;File Details
VIAddVersionKey "FileDescription" "${APPNAME} Updater"
;--------------------------------
RequestExecutionLevel user
;--------------------------------
SilentInstall silent

; The stuff to install
Section "Create"

  SectionIn RO
  
  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
    
 !insertmacro COPYFILES
    
  WriteRegStr HKCU "${REGSTR}" "DisplayVersion" ${VERSION_SHORT} 
     
SectionEnd

Function .onInit
  Push $4
  StrCpy $4 0
  !insertmacro CloseApplicationIfRunning
  Pop $4
FunctionEnd

Function .onInstSuccess
  Sleep 200
  Exec "${APPEXEPATH}"
FunctionEnd

