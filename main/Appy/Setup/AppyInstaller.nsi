; Appy.nsi
;--------------------------------
!include "MUI.nsh"
!include "InstallerShared.nsh"
!include WinMessages.nsh
;--------------------------------
; The name of the installer
Name "${APPNAME}"
;--------------------------------
;Replace Default Installer Icon
!define MUI_ICON "${APPICON}"
;--------------------------------
; The file to write
!define OUTFILE "${APPNAME}Installer.exe"
OutFile "${OUTFILE}"
;--------------------------------
; The default installation directory
InstallDir "${APPDIR}"
;--------------------------------
; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKCU "${REGISTRYPATH}" "Install_Dir"
;--------------------------------
; Pages
!insertmacro MUI_PAGE_INSTFILES
;--------------------------------
;Languages
!insertmacro MUI_LANGUAGE "English"
;--------------------------------
;Version Information
VIProductVersion "${VERSION_SHORT}"
VIAddVersionKey /LANG=${LANG_ENGLISH} "ProductName" "${APPNAME}"
VIAddVersionKey /LANG=${LANG_ENGLISH} "CompanyName" "${COMPANYDISPLAYNAME}"
VIAddVersionKey /LANG=${LANG_ENGLISH} "LegalCopyright" "${COMPANYDISPLAYNAME}"
VIAddVersionKey /LANG=${LANG_ENGLISH} "FileDescription" "${APPNAME} Installer"
VIAddVersionKey /LANG=${LANG_ENGLISH} "FileVersion" "${VERSION_SHORT}"
VIAddVersionKey /LANG=${LANG_ENGLISH} "ProductVersion" "${VERSION_SHORT}"
;--------------------------------

Section "Create directory" Permissions 
   CreateDirectory "$INSTDIR" 
   AccessControl::EnableFileInheritance "$INSTDIR" 
   AccessControl::GrantOnFile "$INSTDIR" "(S-1-5-32-545)" "FullAccess" 
SectionEnd

; The stuff to install
Section "Create"

  SectionIn RO
  
  SetOutPath $INSTDIR
  
  !insertmacro COPYFILES
        
  ; Write the installation path into the registry
  WriteRegStr HKCU ${REGISTRYPATH} "Install_Dir" "$INSTDIR"
  
  ; Write the uninstall keys for Windows
  WriteRegStr HKCU "${REGSTR}" "DisplayName" "${APPNAME}"
  WriteRegStr HKCU "${REGSTR}" "UninstallString" "${UNINSTALLEXEPATH}"
  WriteRegStr HKCU "${REGSTR}" "QuietUninstallString"  "${UNINSTALLEXEPATH}"
 
  WriteRegStr HKCU "${REGSTR}" "Publisher" "${COMPANYDISPLAYNAME}"
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
  CreateShortCut "$SMSTARTUP\${APPNAME}.lnk" "${APPEXEPATH}" "" "$INSTDIR\${APPICON}" 0
  
SectionEnd

!define NETVersion "3.5"
!define NETInstaller "dotNetFx35setup.exe"
Section "MS .NET Framework v${NETVersion}" SecFramework
  IfFileExists "$WINDIR\Microsoft.NET\Framework\v${NETVersion}" NETFrameworkInstalled 0
  File /oname=$TEMP\${NETInstaller} ${NETInstaller}
 
  DetailPrint "Starting Microsoft .NET Framework v${NETVersion} Setup..."
  ExecWait "$TEMP\${NETInstaller} /passive" 
  Return
 
  NETFrameworkInstalled: 
SectionEnd

Function .onInstSuccess
  Exec "${APPEXEPATH}"
FunctionEnd

Function .onInit
	System::Call "user32::RegisterWindowMessage(t'${APPCLOSEMESSAGE}') i .r3"
	FindWindow $0 "${APPWINDOWCLASSNAME}"
	${If} $R0 == "1"
	MessageBox MB_YESNO "Is it okay if ${APPNAME} closes for a bit while it updates?" IDYES gogogo
      Abort
    gogogo:
	SendMessage $0 $R3 0 0
	!insertmacro WaitForDead
	${EndIf}
FunctionEnd 