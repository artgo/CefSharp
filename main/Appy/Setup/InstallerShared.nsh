;--------------------------------
; The name of the installer

!define APPNAME "Appy"
!define COMPANYNAME "AppDirect"
!define APPDIR "$LOCALAPPDATA\${COMPANYNAME}\${APPNAME}"
!define APPEXECUTABLENAME "${APPNAME}.exe"
!define UNINSTALLERNAME "uninstall.exe"
!define REGISTRYPATH "SOFTWARE\${COMPANYNAME}\${APPNAME}" 
!define REGSTR "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}"
!define APPICON "AppIcon.ico"

; Request application privileges for Windows Vista
RequestExecutionLevel admin

;SilentInstall silent
AutoCloseWindow true
;--------------------------------

!macro WaitForDead
loop:
	IntOp $0 $0 + 1
	Processes::FindProcess "${APPNAME}.Browser.exe"
	${If} $R0 == "0"
	Processes::FindProcess "${APPEXECUTABLENAME}"
	StrCmp $R0 "0" done
	StrCmp $0 "100" done
	${EndIf}	
	Sleep 200
	Goto loop
done:
!macroend