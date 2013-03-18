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
; Request application privileges for Windows Vista
RequestExecutionLevel admin

;Version Information
VIProductVersion "${VERSION_SHORT}"
VIAddVersionKey /LANG=${LANG_ENGLISH} "ProductName" "${APPNAME}"
VIAddVersionKey /LANG=${LANG_ENGLISH} "CompanyName" "${COMPANYDISPLAYNAME}"
VIAddVersionKey /LANG=${LANG_ENGLISH} "LegalCopyright" "${COMPANYDISPLAYNAME}"
VIAddVersionKey /LANG=${LANG_ENGLISH} "FileDescription" "${APPNAME} Installer"
VIAddVersionKey /LANG=${LANG_ENGLISH} "FileVersion" "${VERSION_SHORT}"
VIAddVersionKey /LANG=${LANG_ENGLISH} "ProductVersion" "${VERSION_SHORT}"
;--------------------------------

!define MIN_FRA_MAJOR "3"
!define MIN_FRA_MINOR "5"
!define MIN_FRA_BUILD "*"
!define NETInstaller "dotNetFx35setup.exe"
Section "MS .NET Framework v${MIN_FRA_MAJOR}.${MIN_FRA_MINOR}" SecFramework
  
 ;Save the variables in case something else is using them
  Push $0
  Push $1
  Push $2
  Push $3
  Push $4
  Push $R1
  Push $R2
  Push $R3
  Push $R4
  Push $R5
  Push $R6
  Push $R7
  Push $R8
 
  StrCpy $R5 "0"
  StrCpy $R6 "0"
  StrCpy $R7 "0"
  StrCpy $R8 "0.0.0"
  StrCpy $0 0
 
  loop:
 
  ;Get each sub key under "SOFTWARE\Microsoft\NET Framework Setup\NDP"
  EnumRegKey $1 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP" $0
  StrCmp $1 "" done ;jump to end if no more registry keys
  IntOp $0 $0 + 1
  StrCpy $2 $1 1 ;Cut off the first character
  StrCpy $3 $1 "" 1 ;Remainder of string
 
  ;Loop if first character is not a 'v'
  StrCmpS $2 "v" start_parse loop
 
  ;Parse the string
  start_parse:
  StrCpy $R1 ""
  StrCpy $R2 ""
  StrCpy $R3 ""
  StrCpy $R4 $3
 
  StrCpy $4 1
 
  parse:
  StrCmp $3 "" parse_done ;If string is empty, we are finished
  StrCpy $2 $3 1 ;Cut off the first character
  StrCpy $3 $3 "" 1 ;Remainder of string
  StrCmp $2 "." is_dot not_dot ;Move to next part if it's a dot
 
  is_dot:
  IntOp $4 $4 + 1 ; Move to the next section
  goto parse ;Carry on parsing
 
  not_dot:
  IntCmp $4 1 major_ver
  IntCmp $4 2 minor_ver
  IntCmp $4 3 build_ver
  IntCmp $4 4 parse_done
 
  major_ver:
  StrCpy $R1 $R1$2
  goto parse ;Carry on parsing
 
  minor_ver:
  StrCpy $R2 $R2$2
  goto parse ;Carry on parsing
 
  build_ver:
  StrCpy $R3 $R3$2
  goto parse ;Carry on parsing
 
  parse_done:
 
  IntCmp $R1 $R5 this_major_same loop this_major_more
  this_major_more:
  StrCpy $R5 $R1
  StrCpy $R6 $R2
  StrCpy $R7 $R3
  StrCpy $R8 $R4
 
  goto loop
 
  this_major_same:
  IntCmp $R2 $R6 this_minor_same loop this_minor_more
  this_minor_more:
  StrCpy $R6 $R2
  StrCpy $R7 R3
  StrCpy $R8 $R4
  goto loop
 
  this_minor_same:
  IntCmp R3 $R7 loop loop this_build_more
  this_build_more:
  StrCpy $R7 $R3
  StrCpy $R8 $R4
  goto loop
 
  done:
 
  ;Have we got the framework we need?
  IntCmp $R5 ${MIN_FRA_MAJOR} max_major_same no_framework end
  max_major_same:
  IntCmp $R6 ${MIN_FRA_MINOR} max_minor_same no_framework end
  max_minor_same:
  IntCmp $R7 ${MIN_FRA_BUILD} end no_framework end
 
  no_framework:
    File /oname=$TEMP\${NETInstaller} ${NETInstaller}
	ExecWait "$TEMP\${NETInstaller} /passive" $R0	
	StrCmp "$R0" "0" end
	Abort	
  end:
 
  ;Pop the variables we pushed earlier
  Pop $R8
  Pop $R7
  Pop $R6
  Pop $R5
  Pop $R4
  Pop $R3
  Pop $R2
  Pop $R1
  Pop $4
  Pop $3
  Pop $2
  Pop $1
  Pop $0
 
SectionEnd


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

Function .onInstSuccess
  Exec "${APPEXEPATH}"
FunctionEnd

Function .onInit 
  Push $4
  StrCpy $4 0
  !insertmacro CloseApplicationIfRunning
  Pop $4
FunctionEnd 