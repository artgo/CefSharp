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

; Request application privileges for Windows Vista
RequestExecutionLevel admin

AutoCloseWindow true
;--------------------------------

!macro WaitForDead
loop:
  IntOp $R0 $R0 + 1
  FindWindow $0 "" "${APPWINDOWCLASSNAME}"
  StrCmp $0 "0" done
  StrCmp $R0 "100" message	
  Sleep 200
  Goto loop
  message:
  MessageBox MB_OK "${APPNAME} can't install because there is another version of the application running."
    Abort
done:
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