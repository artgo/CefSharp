;--------------------------------
!include "MUI.nsh"
!include "InstallerShared.nsh"

Name "${AppName}"

; The file to write
!define OUTFILE "build_uninstaller.exe"
OutFile "${OUTFILE}"

; Pages

!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_UNPAGE_FINISH

;--------------------------------
;Languages
!insertmacro MUI_LANGUAGE "English"

RequestExecutionLevel user

; The default installation directory
InstallDir $EXEDIR

; The stuff to install
Section "Create"

  SectionIn RO
  
  ; Set output path to the installation directory.
  SetOutPath "$0"
  
  WriteUninstaller "${UNINSTALLERNAME}"

SectionEnd

; Uninstaller
Section "Uninstall"
  
  ; Remove registry keys
  DeleteRegKey HKCU "${REGSTR}"
  DeleteRegKey HKCU ${REGISTRYPATH}

  ; Remove files and uninstaller
  Delete "${APPDIR}\ApplicationData\*.*"
  Delete "${APPDIR}\components\*.*"
  Delete "${APPDIR}\dictionaries\*.*"
  Delete "${APPDIR}\*.*"
  
  ; Remove shortcuts, if any
  Delete "$SMPROGRAMS\${APPNAME}\*.*"
  Delete "$SMPROGRAMS\${APPNAME}.lnk"
  Delete "$SMSTARTUP\${APPNAME}.lnk"

  ; Remove directories used
  
  RMDir "$SMPROGRAMS\${APPNAME}"
  RMDir "${APPDIR}\ApplicationData"
  RMDir "${APPDIR}\components"
  RMDir "${APPDIR}\dictionaries"
  RMDir "${APPDIR}"

SectionEnd

Function un.onInit
  Push $4
  StrCpy $4 0
  !insertmacro CloseApplicationIfRunning
  Pop $4
FunctionEnd 
