!define APPNAME "Appy"
!define COMPANYNAME "AppDirect"
!define APPDIR "$LOCALAPPDATA\${COMPANYNAME}\${APPNAME}"
!define APPEXE "${APPNAME}.exe"
!define UNINSTALLERNAME "uninstall.exe"
!define REGISTRYPATH "SOFTWARE\${COMPANYNAME}\${APPNAME}" 
!define REGSTR "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}"
!define APPICON "AppIcon.ico"
!define APPEXEPATH "${APPDIR}\${APPEXE}"
!define UNINSTALLEXEPATH "${APPDIR}\${UNINSTALLERNAME}"
!define COPYFILES "/r /x Appy\ApplicationData\*.* Appy\*.*"

!define COPYDLL64 "/r 64BitDLL\*.*"
!define COPYDLL32 "/r 32BitDLL\*.*"

!searchparse /file version.txt '' VERSION_SHORT 

; Request application privileges for Windows Vista
RequestExecutionLevel admin

AutoCloseWindow true
;--------------------------------

!macro WaitForDead
;Processes::FindProcess sets $R0 to "1" if the process is found 
loop:
  IntOp $0 $0 + 1
  Processes::FindProcess "${APPNAME}.Browser.exe"
  ${If} $R0 == "0"
  Processes::FindProcess "${APPEXE}"
  StrCmp $R0 "0" done
  StrCmp $0 "100" message	
  ${EndIf}	
  Sleep 200
  Goto loop
  message:
  MessageBox MB_OK "${APPNAME} can't install because there is another version of the application running."
    Abort
done:
!macroend