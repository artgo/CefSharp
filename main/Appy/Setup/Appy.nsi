;--------------------------------
!include "MUI.nsh"
!include "InstallerShared.nsh"

; The name of the installer
Name "${APPNAME}"

; The file to write
!define OUTFILE "${APPNAME}.exe"
OutFile "${OUTFILE}"

; The default installation directory
InstallDir "$LOCALAPPDATA\AppDirect\${APPNAME}"

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKLM "Software\AppDirect" "Install_Dir"
;--------------------------------
; Pages

!insertmacro MUI_PAGE_INSTFILES
    
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
  File /r /x ${APPNAME}\ApplicationData\*.* ${APPNAME}\*.*
  
  !searchparse /file version.txt '' VERSION_SHORT 
  
  ; Write the installation path into the registry
  WriteRegStr HKLM SOFTWARE\AppDirect "Install_Dir" "$INSTDIR"
  
  ; Write the uninstall keys for Windows
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "DisplayName" "${APPNAME}"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "QuietUninstallString" '"$INSTDIR\uninstall.exe"'
  
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "Publisher" "AppDirect Inc." 
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "DisplayVersion" ${VERSION_SHORT} 
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "NoRepair" 1
  
SectionEnd

Section "Start Menu Shortcuts"

  CreateDirectory "$SMPROGRAMS\${APPNAME}"
  CreateShortCut "$SMPROGRAMS\${APPNAME}\Uninstall.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
  CreateShortCut "$SMPROGRAMS\${APPNAME}\${APPNAME}.lnk" "$INSTDIR\${APPNAME}.exe" "" "$INSTDIR\AppyIcon.ico" 0
  CreateShortCut "$SMPROGRAMS\${APPNAME}.lnk" "$INSTDIR\${APPNAME}.exe" "" "$INSTDIR\AppyIcon.ico" 0
  
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

Function .onInstSuccess
Exec "$INSTDIR\${APPNAME}.exe"
FunctionEnd

