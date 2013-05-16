!include "ga.nsh"
!include LogicLib.nsh

!define ___X64__NSH___
!define APPNAME "AppDirectClient"
!define COMPANYNAME "AppDirect"
!define COMPANYDISPLAYNAME "AppDirect Inc."
!define APPDIR "$LOCALAPPDATA\${COMPANYNAME}\${APPNAME}"
!define APPEXE "${APPNAME}.exe"
!define UNINSTALLERNAME "uninstall.exe"
!define REGISTRYPATH "SOFTWARE\${COMPANYNAME}\${APPNAME}"
!define REGSTR "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}"
!define APPICON "AppIcon.ico"
!define INSTALLERICON "install.ico"
!define APPEXEPATH "${APPDIR}\${APPEXE}"
!define UNINSTALLEXEPATH "${APPDIR}\${UNINSTALLERNAME}"
!define COPYFILES "/r /x ${APPNAME}\ApplicationData\*.* AppDirectClient\*.*"
!define APPCLOSEMESSAGE "AppDirectForceApplicationCloseMessage"
!define NATIVETERMINATEMESSAGE "AppDirectNativeTerminateMessage"
!define APPWINDOWCLASSNAME "AppDirectTaskbarButtonsWindow"
!define SHELLTRAYWINDOWCLASSNAME "Shell_TrayWnd"
!define SYNC_TERM 0x00100001
!define BROWSERPROCESSNAME "BrowserManager.exe"
!define NATIVEDLLPATH "$INSTDIR\native.dll"
!define COMMONDLLPATH "$INSTDIR\Common.dll"
!define BROWSERCACHEPATH "$INSTDIR\CACHE"
!define GAACCOUNT "UA-33544164-4"

!define COPY64 "/r 64Bit\*.*"
!define COPY32 "/r 32Bit\*.*"

!searchparse /file version.txt '' VERSION_SHORT

AutoCloseWindow true
;--------------------------------

;Version Information
VIProductVersion "${VERSION_SHORT}"
VIAddVersionKey "ProductName" "${APPNAME}"
VIAddVersionKey "CompanyName" "${COMPANYDISPLAYNAME}"
VIAddVersionKey "LegalCopyright" "${COMPANYDISPLAYNAME}"
VIAddVersionKey "FileVersion" "${VERSION_SHORT}"
VIAddVersionKey "ProductVersion" "${VERSION_SHORT}"
;--------------------------------

!macro CloseApplicationIfRunning
  System::Call "user32::RegisterWindowMessage(t'${APPCLOSEMESSAGE}') i.r3"
  System::Call "user32::RegisterWindowMessage(t'${NATIVETERMINATEMESSAGE}') i.r4"
  FindWindow $0 "" "${APPWINDOWCLASSNAME}"
  FindWindow $2 "" "${SHELLTRAYWINDOWCLASSNAME}"
  ${If} $0 != "0"
  StrCmp $4 "0" gogogo
  MessageBox MB_YESNO "Is it okay if ${APPNAME} closes for a bit while it updates?" IDYES gogogo
    Abort
  gogogo:
  System::Call "user32::GetWindowThreadProcessId(i $0, *i .r1 ) i .r2"
  System::Call "kernel32::OpenProcess(i ${SYNC_TERM}, i 0, i r1)i .r2"
  SendMessage $0 $3 0 0
  SendMessage $2 $4 0 0
  System::Call "kernel32::WaitForSingleObject(i r2, i 30000) i.r5"
  StrCpy $6 $5
  StrCmp $5 0 end0 error
  end0:
  System::Call "kernel32::CloseHandle(i r2) i .r1"
  ${EndIf}

  StrCpy $1 0 
  StrCpy $6 "Browser Process Is Still Running"  
  nsExec::Exec "taskkill /f /im ${BROWSERPROCESSNAME}"
  loop1:
	IntOp $1 $1 + 1 ;timeout index
	${FindProcess} BROWSERPROCESSNAME $0 ;sets $0 to 1 if process is found
	StrCmp $0 "0" end1 continue
	continue:
	sleep 500
	StrCmp $1 "20" error loop1 ;try for 10 seconds
  end1:
  
  StrCpy $1 0
  StrCpy $6 "${NATIVEDLLPATH} could not be removed" 
  loop2:
	IntOp $1 $1 + 1 ;timeout index
	IfFileExists ${NATIVEDLLPATH} deleteFile end2
	deleteFile:
	  Delete ${NATIVEDLLPATH} 
      sleep 200
	  StrCmp $1 "40" error loop2 ;try for 8 seconds
  end2:
  
  StrCpy $1 0
  StrCpy $6 "${COMMONDLLPATH} could not be removed" 
  loop3:
	IntOp $1 $1 + 1 ;timeout index
	IfFileExists ${COMMONDLLPATH} deleteFile1 end3
	deleteFile1:
	  Delete ${COMMONDLLPATH} 
	  sleep 200
      StrCmp $1 "40" error loop3 ;try for 8 seconds
  end3:
  
  StrCpy $1 0
  StrCpy $6 "${BROWSERCACHEPATH}} could not be removed" 
  loop4:
    IntOp $1 $1 + 1 ;timeout index
    IfFileExists ${BROWSERCACHEPATH} deleteCache finalEnd
	deleteCache:
	  RMDir /r "${BROWSERCACHEPATH}" 
      sleep 200
      StrCmp $1 "50" error loop4 ;try for 10 seconds

  error:
    !insertmacro GoogleAnalytics "${GAACCOUNT}" "Install" "Retry" "" ""
    WriteRegStr HKCU "SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce" "updater" $EXEPATH
	Delete "$SMSTARTUP\${APPNAME}.lnk"	
	MessageBox MB_YESNO "${APPNAME} has updates ready.  You must restart your computer to complete these updates. Would you like to restart now?" IDYES restart
	Abort
	restart:
	Reboot
	
  finalEnd:
!macroend

!macro CopyFiles
  ; Files to copy
  File ${COPYFILES}

 ${If} ${RunningX64}
 File ${COPY64}
 goto ENDCOPY
 ${EndIf}
 
 File ${COPY32}
 Goto ENDCOPY

  ENDCOPY:
!macroend

!macro _RunningX64 _a _b _t _f
  !insertmacro _LOGICLIB_TEMP
  System::Call kernel32::GetCurrentProcess()i.s
  System::Call kernel32::IsWow64Process(is,*i.s)
  Pop $_LOGICLIB_TEMP
  !insertmacro _!= $_LOGICLIB_TEMP 0 `${_t}` `${_f}`
!macroend

!define RunningX64 `"" RunningX64 ""`

!macro DisableX64FSRedirection

  System::Call kernel32::Wow64EnableWow64FsRedirection(i0)

!macroend

!define DisableX64FSRedirection "!insertmacro DisableX64FSRedirection"

!macro EnableX64FSRedirection

  System::Call kernel32::Wow64EnableWow64FsRedirection(i1)

!macroend

!define EnableX64FSRedirection "!insertmacro EnableX64FSRedirection"
