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
  SendMessage $0 $3 0 0
  ExecCmd::wait $0
  ${EndIf}
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