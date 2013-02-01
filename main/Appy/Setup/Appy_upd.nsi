; Appy_upd.nsi
;--------------------------------

; The name of the installer
Name "Appy_upd"

; The file to write
!define OUTFILE "Appy_upd.exe"
OutFile "${OUTFILE}"

; The default installation directory
InstallDir "$LOCALAPPDATA\Appy"

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKLM "Software\Appy" "Install_Dir"

; Request application privileges for Windows Vista
RequestExecutionLevel admin

;SilentInstall silent
;--------------------------------

; Pages

;--------------------------------

; The stuff to install
Section "Appy (required)"

  SectionIn RO
  
  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  !searchparse /file version.txt '' VERSION_SHORT
  
  ; Put file there
  File /r Appy\*.*
  
  ; Write the installation path into the registry
  WriteRegStr HKLM SOFTWARE\Appy "Install_Dir" "$INSTDIR"
 
  
  ; Write the uninstall keys for Windows
  ; WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Appy" "DisplayName" "Appy"
  ; WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Appy" "UninstallString" '"$INSTDIR\uninstall.exe"'
  ; WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Appy" "QuietUninstallString" '"$INSTDIR\uninstall.exe"'
  
  ; WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Appy" "Publisher" "AppDirect Inc." 
  ; WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Appy" "DisplayVersion" ${VERSION_SHORT} 
  ; WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Appy" "NoModify" 1
  ; WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Appy" "NoRepair" 1
  ; WriteUninstaller "uninstall.exe"
  
SectionEnd

Function .onInstSuccess
Exec "$INSTDIR\Appy.exe"
FunctionEnd

