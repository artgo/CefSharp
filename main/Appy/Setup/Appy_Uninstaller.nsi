;--------------------------------
!include "MUI.nsh"
!include "InstallerShared.nsh"

Name "Uninstaller"

; The file to write
!define OUTFILE "Appy_Uninstaller.exe"
OutFile "${OUTFILE}"

; The default installation directory
!define APPDIR "$LOCALAPPDATA\AppDirect"
; Pages

!insertmacro MUI_PAGE_INSTFILES

!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_UNPAGE_FINISH

;--------------------------------
;Languages

  !insertmacro MUI_LANGUAGE "English"

;--------------------------------

; The default installation directory
InstallDir $EXEDIR

; The stuff to install
Section "Appy (required)"

  SectionIn RO
  
  ; Set output path to the installation directory.
  SetOutPath "$0"
  
  WriteUninstaller "uninstall.exe"

SectionEnd

; Uninstaller
Section "Uninstall"
  
  ; Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}"
  DeleteRegKey HKLM SOFTWARE\Appy

  ; Remove files and uninstaller
  Delete "${APPDIR}\ApplicationData\AppDirect\*.*"
  Delete "${APPDIR}\ApplicationData\*.*"
  Delete "${APPDIR}\locales\*.*"
  Delete "${APPDIR}\*.*"
  
  ; Remove shortcuts, if any
  Delete "$SMPROGRAMS\${APPNAME}\*.*"
  Delete "$SMPROGRAMS\${APPNAME}.lnk"

  ; Remove directories used
  
  RMDir "$SMPROGRAMS\${APPNAME}"
  RMDir "${APPDIR}\ApplicationData\AppDirect"
  RMDir "${APPDIR}\ApplicationData"
  RMDir "${APPDIR}\locales"
  RMDir "${APPDIR}"

SectionEnd

Function un.onInit
Exec "taskkill /f /t /im appy.exe"
FunctionEnd
