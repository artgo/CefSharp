; ADWindowsClient.nsi
;--------------------------------
!include "MUI.nsh"
;!include "PostExec.nsh"

; The name of the installer
Name "Appy"

; The file to write
!define OUTFILE "Appy.exe"
OutFile "${OUTFILE}"

;${PostExec5} signtool.exe sign /t http://timestamp.verisign.com/scripts/timstamp.dll ${OUTFILE}

; The default installation directory
InstallDir "$WINDIR\Local Application Data"

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKLM "Software\Appy" "Install_Dir"

; Request application privileges for Windows Vista
RequestExecutionLevel admin

;SilentInstall silent
AutoCloseWindow true
;--------------------------------

; Pages

!insertmacro MUI_PAGE_INSTFILES

!insertmacro MUI_UNPAGE_WELCOME
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_UNPAGE_FINISH
  
;--------------------------------
;Languages

  !insertmacro MUI_LANGUAGE "English"

;--------------------------------

Section "Create directory" Permissions 
   CreateDirectory "$INSTDIR\Appy" 
   AccessControl::EnableFileInheritance "$INSTDIR\Appy" 
   AccessControl::GrantOnFile "$INSTDIR\Appy" "(S-1-5-32-545)" "FullAccess" 
SectionEnd

; The stuff to install
Section "Appy (required)"

  SectionIn RO
  
  ; Set output path to the installation directory.
  SetOutPath $INSTDIR\Appy
  
  !searchparse /file version.txt '' VERSION_SHORT
  
  ; Put file there
File /r "Appy"
File "AppyIcon.ico"
  
  ; Write the installation path into the registry
  WriteRegStr HKLM SOFTWARE\Appy "Install_Dir" "$INSTDIR"
 
  
  ; Write the uninstall keys for Windows
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Appy" "DisplayName" "Appy"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Appy" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Appy" "QuietUninstallString" '"$INSTDIR\uninstall.exe"'
  
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Appy" "Publisher" "AppDirect Inc." 
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Appy" "DisplayVersion" ${VERSION_SHORT} 
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Appy" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Appy" "NoRepair" 1
  WriteUninstaller "uninstall.exe"
  
SectionEnd

; Optional section (can be disabled by the user)
Section "Start Menu Shortcuts"

  CreateDirectory "$SMPROGRAMS\Appy"
  CreateShortCut "$SMPROGRAMS\Appy\Uninstall.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
  CreateShortCut "$SMPROGRAMS\Appy\Appy.lnk" "$INSTDIR\Appy\Appy.exe" "" "$INSTDIR\Appy\AppyIcon.ico" 0
  
SectionEnd

!define NETVersion "3.5"
!define NETInstaller "dotNetFx35setup.exe"
Section "MS .NET Framework v${NETVersion}" SecFramework
  IfFileExists "$WINDIR\Microsoft.NET\Framework\v${NETVersion}" NETFrameworkInstalled 0
  File /oname=$TEMP\${NETInstaller} ${NETInstaller}
 
  DetailPrint "Starting Microsoft .NET Framework v${NETVersion} Setup..."
  ExecWait "$TEMP\${NETInstaller}"
  Return
 
  NETFrameworkInstalled:
  DetailPrint "Microsoft .NET Framework is already installed!"
 
SectionEnd

;--------------------------------

; Uninstaller

Section "Uninstall"
  
  ; Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Appy"
  DeleteRegKey HKLM SOFTWARE\Appy

  ; Remove files and uninstaller
  Delete $INSTDIR\uninstall.exe
  Delete "$INSTDIR\Appy\ApplicationData\AppDirect\*.*"
  Delete "$INSTDIR\Appy\locales\*.*"
  Delete "$INSTDIR\Appy\*.*"
  Delete "$INSTDIR\AppyIcon.ico"
  
  ; Remove shortcuts, if any
  Delete "$SMPROGRAMS\Appy\*.*"

  ; Remove directories used
  
  RMDir "$SMPROGRAMS\Appy"
  RMDir "$INSTDIR\Appy\ApplicationData\AppDirect"
  RMDir "$INSTDIR\Appy\ApplicationData"
  RMDir "$INSTDIR\Appy\locales"
  RMDir "$INSTDIR\Appy"

SectionEnd

Function .onInstSuccess
Exec "$INSTDIR\Appy\Appy.exe"
FunctionEnd

