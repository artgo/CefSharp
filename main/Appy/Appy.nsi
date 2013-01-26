; ADWindowsClient.nsi
;--------------------------------

; The name of the installer
Name "Appy"

; The file to write
OutFile "Appy.exe"

; The default installation directory
InstallDir "$WINDIR\Local Application Data"

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKLM "Software\Appy" "Install_Dir"

; Request application privileges for Windows Vista
RequestExecutionLevel admin

SilentInstall silent
;--------------------------------

; Pages

Page components
Page directory
Page instfiles

UninstPage uninstConfirm
UninstPage instfiles

;--------------------------------

; The stuff to install
Section "Appy (required)"

  SectionIn RO
  
  ; Set output path to the installation directory.
  SetOutPath $INSTDIR\Appy
  
  ; Put file there
File /r "Setup"
  
  ; Write the installation path into the registry
  WriteRegStr HKLM SOFTWARE\Appy "Install_Dir" "$INSTDIR"
  
  ; Write the uninstall keys for Windows
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Appy" "DisplayName" "Appy"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Appy" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Appy" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Appy" "NoRepair" 1
  WriteUninstaller "uninstall.exe"
  
SectionEnd

; Optional section (can be disabled by the user)
Section "Start Menu Shortcuts"

  CreateDirectory "$SMPROGRAMS\Appy"
  CreateShortCut "$SMPROGRAMS\Appy\Uninstall.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
  CreateShortCut "$SMPROGRAMS\Appy\Appy.lnk" "$INSTDIR\Appy\Setup\Appy.exe" "" "$INSTDIR\Appy\Setup\AppyIcon.ico" 0
  
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
  Delete $INSTDIR\ADWindowsClient.nsi
  Delete $INSTDIR\uninstall.exe
  Delete "$INSTDIR\Appy\Setup\*.*"
  Delete "$INSTDIR\Appy\Setup\locales\*.*"
  Delete "$INSTDIR\Appy\Setup\ApplicationData\AppDirect\*.*"
  
  ; Remove shortcuts, if any
  Delete "$SMPROGRAMS\Appy\*.*"

  ; Remove directories used
  
  RMDir "$SMPROGRAMS\Appy"
  RMDir "$INSTDIR\Appy\Setup\ApplicationData\AppDirect"
  RMDir "$INSTDIR\Appy\Setup\ApplicationData"
  RMDir "$INSTDIR\Appy\Setup\locales"
  RMDir "$INSTDIR\Appy\Setup"
  RMDir "$INSTDIR\Appy"

SectionEnd
