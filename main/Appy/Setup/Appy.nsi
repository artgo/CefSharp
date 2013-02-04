;--------------------------------
!include "MUI.nsh"

; The name of the installer
Name "Appy"

; The file to write
!define OUTFILE "Appy.exe"
OutFile "${OUTFILE}"

; The default installation directory
InstallDir "$LOCALAPPDATA\AppDirect"

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKLM "Software\AppDirect" "Install_Dir"

; Request application privileges for Windows Vista
RequestExecutionLevel admin

;SilentInstall silent
AutoCloseWindow true
;--------------------------------
; Pages

!insertmacro MUI_PAGE_INSTFILES

!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_UNPAGE_FINISH
    
;--------------------------------
;Languages

!insertmacro MUI_LANGUAGE "English"

;--------------------------------

Section "Create directory" Permissions 
   CreateDirectory "$INSTDIR" 
   AccessControl::EnableFileInheritance "$INSTDIR" 
   AccessControl::GrantOnFile "$INSTDIR" "(S-1-5-32-545)" "FullAccess" 
SectionEnd

; The stuff to install
Section "Appy (required)"

  SectionIn RO
  
  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  ; Put file there
  File /r /x Appy\ApplicationData\*.* Appy\*.*
  
  !searchparse /file version.txt '' VERSION_SHORT 
  
  ; Write the installation path into the registry
  WriteRegStr HKLM SOFTWARE\AppDirect "Install_Dir" "$INSTDIR"
  
  ; Write the uninstall keys for Windows
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Appy" "DisplayName" "Appy"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Appy" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Appy" "QuietUninstallString" '"$INSTDIR\uninstall.exe"'
  
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Appy" "Publisher" "AppDirect Inc." 
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Appy" "DisplayVersion" ${VERSION_SHORT} 
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Appy" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Appy" "NoRepair" 1
  ;WriteUninstaller "uninstall.exe"
  
SectionEnd

; Optional section (can be disabled by the user)
Section "Start Menu Shortcuts"

  CreateDirectory "$SMPROGRAMS\Appy"
  CreateShortCut "$SMPROGRAMS\Appy\Uninstall.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
  CreateShortCut "$SMPROGRAMS\Appy\Appy.lnk" "$INSTDIR\Appy.exe" "" "$INSTDIR\AppyIcon.ico" 0
  CreateShortCut "$SMPROGRAMS\Appy.lnk" "$INSTDIR\Appy.exe" "" "$INSTDIR\AppyIcon.ico" 0
  
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
  Delete "$INSTDIR\ApplicationData\AppDirect\*.*"
  Delete "$INSTDIR\ApplicationData\*.*"
  Delete "$INSTDIR\locales\*.*"
  Delete "$INSTDIR\*.*"
  
  ; Remove shortcuts, if any
  Delete "$SMPROGRAMS\Appy\*.*"
  Delete "$SMPROGRAMS\Appy.lnk"

  ; Remove directories used
  
  RMDir "$SMPROGRAMS\Appy"
  RMDir "$INSTDIR\ApplicationData\AppDirect"
  RMDir "$INSTDIR\ApplicationData"
  RMDir "$INSTDIR\locales"
  RMDir "$INSTDIR"

SectionEnd

Function un.onInit
Exec "taskkill /f /t /im appy.exe"
FunctionEnd

Function .onInstSuccess
Exec "$INSTDIR\Appy.exe"
FunctionEnd

