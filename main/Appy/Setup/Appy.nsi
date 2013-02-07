; Appy.nsi
;--------------------------------
!include "MUI.nsh"
!include "InstallerShared.nsh"

; The name of the installer
Name "${APPNAME}"

; The file to write
!define OUTFILE ${APPEXE}
OutFile "${OUTFILE}"

; The default installation directory
InstallDir "${APPDIR}"

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKCU "${REGISTRYPATH}" "Install_Dir"
;--------------------------------
; Pages
!insertmacro MUI_PAGE_INSTFILES
;--------------------------------
;Languages
!insertmacro MUI_LANGUAGE "English"


Section "Create directory" Permissions 
   CreateDirectory "$INSTDIR" 
   AccessControl::EnableFileInheritance "$INSTDIR" 
   AccessControl::GrantOnFile "$INSTDIR" "(S-1-5-32-545)" "FullAccess" 
SectionEnd

; The stuff to install
Section "Create"

  SectionIn RO
  
  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  ; Files to copy
  File ${COPYFILES}
    
  ; Write the installation path into the registry
  WriteRegStr HKCU ${REGISTRYPATH} "Install_Dir" "$INSTDIR"
  
  ; Write the uninstall keys for Windows
  WriteRegStr HKCU "${REGSTR}" "DisplayName" "${APPNAME}"
  WriteRegStr HKCU "${REGSTR}" "UninstallString" "${UNINSTALLEXEPATH}"
  WriteRegStr HKCU "${REGSTR}" "QuietUninstallString"  "${UNINSTALLEXEPATH}"
 
  WriteRegStr HKCU "${REGSTR}" "Publisher" "${COMPANYNAME}"
  WriteRegStr HKCU "${REGSTR}" "DisplayVersion" ${VERSION_SHORT}
  WriteRegStr HKCU "${REGSTR}" "DisplayIcon" "$INSTDIR\${APPICON}"
  WriteRegDWORD HKCU "${REGSTR}" "NoModify" 1
  WriteRegDWORD HKCU "${REGSTR}" "NoRepair" 1
  
SectionEnd

Section "Start Menu Shortcuts"

  CreateDirectory "$SMPROGRAMS\${APPNAME}"
  CreateShortCut "$SMPROGRAMS\${APPNAME}\Uninstall.lnk" "${UNINSTALLEXEPATH}" "" "${UNINSTALLEXEPATH}" 0
  CreateShortCut "$SMPROGRAMS\${APPNAME}\${APPNAME}.lnk" "${APPEXEPATH}" "" "$INSTDIR\${APPICON}" 0
  CreateShortCut "$SMPROGRAMS\${APPNAME}.lnk" "${APPEXEPATH}" "" "$INSTDIR\${APPICON}" 0
  
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
Exec "${APPEXEPATH}"
FunctionEnd

