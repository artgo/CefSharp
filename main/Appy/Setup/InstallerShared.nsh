!define APPNAME "Appy"
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
!define COPYFILES "/r /x Appy\ApplicationData\*.* Appy\*.*"
!define APPCLOSEMESSAGE "AppDirectForceApplicationCloseMessage"
!define APPWINDOWCLASSNAME "AppDirectTaskbarButtonsWindow"
!define SYNC_TERM 0x00100001
!define BROWSERPROCESSNAME "${APPNAME}.Browser.exe"
!define NATIVEDLLPATH "$INSTDIR\native.dll"
!define COMMONDLLPATH "$INSTDIR\${APPNAME}.Common.dll"
!define BROWSERCACHEPATH "$INSTDIR\CACHE"

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
  FindWindow $0 "" "${APPWINDOWCLASSNAME}"
  ${If} $0 != "0"
  StrCmp $4 "0" gogogo
  MessageBox MB_YESNO "Is it okay if ${APPNAME} closes for a bit while it updates?" IDYES gogogo
    Abort
  gogogo:
  System::Call "user32::GetWindowThreadProcessId(i $0, *i .r1 ) i .r2"
  System::Call "kernel32::OpenProcess(i ${SYNC_TERM}, i 0, i r1)i .r2"
  SendMessage $0 $3 0 0
  System::Call "kernel32::WaitForSingleObject(i r2, i 12000) i.r5"
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
    MessageBox MB_OK "${APPNAME} can not update because the application is currently running or is in a faulted state.  Please uninstall the application before proceeding. Error: $6"
	  Abort

  finalEnd:
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