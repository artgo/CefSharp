!define APPNAME "Appy"
!define COMPANYNAME "AppDirect"
!define COMPANYDISPLAYNAME "AppDirect Inc."
!define APPDIR "$LOCALAPPDATA\${COMPANYNAME}\${APPNAME}"
!define APPEXE "${APPNAME}.exe"
!define UNINSTALLERNAME "uninstall.exe"
!define REGISTRYPATH "SOFTWARE\${COMPANYNAME}\${APPNAME}"
!define REGSTR "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}"
!define APPICON "AppIcon.ico"
!define APPEXEPATH "${APPDIR}\${APPEXE}"
!define UNINSTALLEXEPATH "${APPDIR}\${UNINSTALLERNAME}"
!define COPYFILES "/r /x Appy\ApplicationData\*.* Appy\*.*"
!define APPCLOSEMESSAGE "AppDirectForceApplicationCloseMessage"
!define APPWINDOWCLASSNAME "AppDirectTaskbarButtonsWindow"
!define SYNC_TERM 0x00100001
!define BROWSERPROCESSNAME "${APPNAME}.Browser.exe"

!define COPY64 "/r 64Bit\*.*"
!define COPY32 "/r 32Bit\*.*"

!searchparse /file version.txt '' VERSION_SHORT

AutoCloseWindow true
;--------------------------------

!macro CloseApplicationIfRunning
  System::Call "user32::RegisterWindowMessage(t'${APPCLOSEMESSAGE}') i.r3"
  FindWindow $0 "" "${APPWINDOWCLASSNAME}"
  ${If} $0 != "0"
  StrCmp $4 "0" gogogo
  MessageBox MB_YESNO "Is it okay if ${APPNAME} closes for a bit while it updates?" IDYES gogogo
    Abort
  gogogo:
  System::Call "user32::GetWindowThreadProcessId(i $0, *i .r1 ) i .r2"
  System::Call "kernel32::OpenProcess(i ${SYNC_TERM}, i 0, i r1)i .r2"
  SendMessage $0 $3 0 0
  System::Call "kernel32::WaitForSingleObject(i r2, i 10000) i.r5"
  StrCmp $5 0 end error
  end:
  System::Call "kernel32::CloseHandle(i r2) i .r1"
  ${EndIf}

  nsExec::Exec "taskkill /f /im ${BROWSERPROCESSNAME}"
  loop:
  IntOp $1 $1 + 1 ;timeout index
  ${FindProcess} BROWSERPROCESSNAME $0 ;sets $0 to 1 if process is found
  StrCmp $0 "0" end2 continue
  continue:
  Sleep 200
  StrCmp $1 "25" error loop ;try for 5 seconds
  end1:

  loop1:
  IntOp $1 $1 + 1 ;timeout index
  IfFileExists $INSTDIR\native.dll deleteFile end2
	deleteFile:
	Delete $INSTDIR\native.dll
    sleep 1000
  StrCmp $1 "5" error loop1 ;try for 5 seconds

  error:
    MessageBox MB_OK "${APPNAME} can not update because the application is currently running or is in a faulted state.  Please uninstall the application before proceeding."
		Abort

  end2:
!macroend

!macro CopyFiles
  ; Files to copy
  File ${COPYFILES}

  EnumRegKey $0 HKLM "SOFTWARE\Wow6432Node" 0
  IfErrors 0 Is64Bit

  File ${COPY32}
  Goto ENDCOPY

  Is64Bit:
  File ${COPY64}

ENDCOPY:
!macroend