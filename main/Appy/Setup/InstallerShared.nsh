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

!searchparse /file version.txt '' VERSION_SHORT 

; Request application privileges for Windows Vista
RequestExecutionLevel admin

AutoCloseWindow true
;--------------------------------

!macro WaitForDead
loop:
	IntOp $0 $0 + 1
	Processes::FindProcess "${APPNAME}.Browser.exe"
	${If} $R0 == "0"
	Processes::FindProcess "${APPEXE}"
	StrCmp $R0 "0" 0 done
	StrCmp $0 "100" done
	${EndIf}	
	Sleep 200
	Goto loop
done:
!macroend