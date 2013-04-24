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
;--------------------------------
;File Details
VIAddVersionKey "FileDescription" "${APPNAME} Uninstaller"
;--------------------------------

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

  ; Remove shortcuts, if any
  Delete "$SMPROGRAMS\${APPNAME}.lnk"
  Delete "$SMSTARTUP\${APPNAME}.lnk"

  ; Remove directories used
  RMDir /r "$SMPROGRAMS\${APPNAME}"
  RMDir /r "${APPDIR}"
SectionEnd

Function un.onInit
  Push $4
  StrCpy $4 0
  System::Call "user32::RegisterWindowMessage(t'${APPCLOSEMESSAGE}') i.r3"
  FindWindow $0 "" "${APPWINDOWCLASSNAME}"
  StrCmp $0 "0" finalEnd
  System::Call "user32::GetWindowThreadProcessId(i $0, *i .r1 ) i .r2"
  System::Call "kernel32::OpenProcess(i ${SYNC_TERM}, i 0, i r1)i .r2"
  SendMessage $0 $3 0 0
  System::Call "kernel32::WaitForSingleObject(i r2, i 12000) i.r5"
  StrCpy $6 $5
  StrCmp $5 0 end0 error
  end0:
    System::Call "kernel32::CloseHandle(i r2) i .r1"
    Pop $4
      
  error:
    MessageBox MB_OK "${APPNAME} can not uninstall because the application is currently running.  Please close the application before uninstalling. Error: $6"
	  Abort
	  
  finalEnd:  
	nsExec::Exec "taskkill /f /im ${BROWSERPROCESSNAME}"
	sleep 500
FunctionEnd 
