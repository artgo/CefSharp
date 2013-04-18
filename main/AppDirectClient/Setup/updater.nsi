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
  
  SetOutPath $INSTDIR
  
  !insertmacro COPYFILES
        
  ; Write the installation path into the registry
  WriteRegStr HKCU ${REGISTRYPATH} "Install_Dir" "$INSTDIR"
  
  ; Write the uninstall keys for Windows
  WriteRegStr HKCU "${REGSTR}" "DisplayName" "${APPNAME}"
  WriteRegStr HKCU "${REGSTR}" "UninstallString" "${UNINSTALLEXEPATH}"
  WriteRegStr HKCU "${REGSTR}" "QuietUninstallString"  "${UNINSTALLEXEPATH}"
 
  WriteRegStr HKCU "${REGSTR}" "Publisher" "${COMPANYDISPLAYNAME}"
  WriteRegStr HKCU "${REGSTR}" "DisplayVersion" ${VERSION_SHORT}
  WriteRegStr HKCU "${REGSTR}" "DisplayIcon" "$INSTDIR\${APPICON}"
  WriteRegDWORD HKCU "${REGSTR}" "NoModify" 1
  WriteRegDWORD HKCU "${REGSTR}" "NoRepair" 1
  
SectionEnd

Section "Start Menu Shortcuts"
  CreateDirectory "$SMPROGRAMS\${APPNAME}"
  CreateShortCut "$SMPROGRAMS\${APPNAME}\Uninstall.lnk" "${UNINSTALLEXEPATH}" "" "${UNINSTALLEXEPATH}" 0
  CreateShortCut "$SMPROGRAMS\${APPNAME}\${APPNAME}.lnk" "${APPEXEPATH}" "" "${APPEXEPATH}" 0
  CreateShortCut "$SMPROGRAMS\${APPNAME}.lnk" "${APPEXEPATH}" "" "${APPEXEPATH}" 0
  CreateShortCut "$SMSTARTUP\${APPNAME}.lnk" "${APPEXEPATH}" "" "${APPEXEPATH}" 0
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

