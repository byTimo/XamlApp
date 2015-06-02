;contribute on github.com/stfx/innodependencyinstaller or codeproject.com/Articles/20868/NET-Framework-1-1-2-0-3-5-Installer-for-InnoSetup

#define use_dotnetfx40

#define MyAppName "Zappchat"
#define MyAppVersion "3.0.0.0"
#define MyAppPublisher "Zappcat, LLC."
#define MyAppURL "http://zappchat.ru/"
#define MyAppExeName "ZappChat.exe"
#define MyAppSetupName "Zappchat"

[Setup]
AppId={{9E752155-2C5F-4C70-BDB2-10CD42FF57DB}
AppName={#MyAppSetupName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppSetupName} {#MyAppVersion}
AppCopyright=Copyright © Zappcat LLC 2013-2015
VersionInfoVersion={#MyAppVersion}
VersionInfoCompany=Zappcat LLC
AppPublisher=Zappcat LLC
AppPublisherURL=http://zappchat.ru/
;AppSupportURL=http://...
;AppUpdatesURL=http://...
UsePreviousAppDir=no
UsePreviousGroup=no
OutputBaseFilename={#MyAppSetupName}-{#MyAppVersion}
DefaultGroupName={#MyAppSetupName}
DefaultDirName={userappdata}\ZappChat\Client
OutputDir=bin
SourceDir=.
AllowNoIcons=yes
;SetupIconFile=MyProgramIcon
SolidCompression=yes
DisableDirPage=yes
DisableProgramGroupPage=yes

;MinVersion default value: "0,5.0 (Windows 2000+) if Unicode Inno Setup, else 4.0,4.0 (Windows 95+)"
;MinVersion=0,5.0
PrivilegesRequired=admin
ArchitecturesAllowed=x86 x64 ia64
ArchitecturesInstallIn64BitMode=x64 ia64

[Languages]
Name: "en"; MessagesFile: "compiler:Default.isl"
Name: "ru"; MessagesFile: "compiler:Languages\Russian.isl"
Name: "de"; MessagesFile: "compiler:Languages\German.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Files]
Source: "..\ZappChat\bin\Release\ZappChat.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\ZappChat\bin\Release\Zappchat.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\ZappChat\bin\Release\Newtonsoft.Json.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\ZappChat\bin\Release\NAppUpdate.Framework.dll"; DestDir: "{app}"; Flags: ignoreversion
;Source: "..\ZappChat\bin\Release\de\Microsoft.Expression.Drawing.resources.dll"; DestDir: "{app}\ru"; Flags: ignoreversion
;Source: "..\ZappChat\bin\Release\es\Microsoft.Expression.Drawing.resources.dll"; DestDir: "{app}\ru"; Flags: ignoreversion
;Source: "..\ZappChat\bin\Release\fr\Microsoft.Expression.Drawing.resources.dll"; DestDir: "{app}\ru"; Flags: ignoreversion
;Source: "..\ZappChat\bin\Release\it\Microsoft.Expression.Drawing.resources.dll"; DestDir: "{app}\ru"; Flags: ignoreversion
;Source: "..\ZappChat\bin\Release\ja\Microsoft.Expression.Drawing.resources.dll"; DestDir: "{app}\ru"; Flags: ignoreversion
;Source: "..\ZappChat\bin\Release\ko\Microsoft.Expression.Drawing.resources.dll"; DestDir: "{app}\ru"; Flags: ignoreversion
;Source: "..\ZappChat\bin\Release\ru\Microsoft.Expression.Drawing.resources.dll"; DestDir: "{app}\ru"; Flags: ignoreversion
;Source: "..\ZappChat\bin\Release\zh-Hans\Microsoft.Expression.Drawing.resources.dll"; DestDir: "{app}\ru"; Flags: ignoreversion
;Source: "..\ZappChat\bin\Release\zh-Hant\Microsoft.Expression.Drawing.resources.dll"; DestDir: "{app}\ru"; Flags: ignoreversion
Source: "..\ZappChat\bin\Release\Hardcodet.Wpf.TaskbarNotification.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\ZappChat\bin\Release\Hardcodet.Wpf.TaskbarNotification.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\ZappChat\bin\Release\Microsoft.Expression.Drawing.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\ZappChat\bin\Release\nunit.framework.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\ZappChat\bin\Release\nunit.framework.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\ZappChat\bin\Release\WebSocket4Net.dll"; DestDir: "{app}"; Flags: ignoreversion

; Source: "..\ZappchatClient\bin\Release\support\TeamViewerQS_ru.exe"; DestDir: "{app}\support"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[Registry]
Root: "HKCU"; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Run"; ValueType: string; ValueName: "Zappchat"; ValueData: """{app}\{#MyAppExeName}"""; Flags: uninsdeletevalue

[UninstallDelete]
Type: filesandordirs; Name: "{localappdata}\Zappchat_LLC"
Type: filesandordirs; Name: "{userappdata}\ZappChat"

#include "scripts\products.iss"

#include "scripts\products\stringversion.iss"
#include "scripts\products\winversion.iss"
#include "scripts\products\fileversion.iss"
#include "scripts\products\dotnetfxversion.iss"

#ifdef use_iis
#include "scripts\products\iis.iss"
#endif

#ifdef use_kb835732
#include "scripts\products\kb835732.iss"
#endif

#ifdef use_msi20
#include "scripts\products\msi20.iss"
#endif
#ifdef use_msi31
#include "scripts\products\msi31.iss"
#endif
#ifdef use_msi45
#include "scripts\products\msi45.iss"
#endif

#ifdef use_ie6
#include "scripts\products\ie6.iss"
#endif

#ifdef use_dotnetfx11
#include "scripts\products\dotnetfx11.iss"
#include "scripts\products\dotnetfx11sp1.iss"
#ifdef use_dotnetfx11lp
#include "scripts\products\dotnetfx11lp.iss"
#endif
#endif

#ifdef use_dotnetfx20
#include "scripts\products\dotnetfx20.iss"
#include "scripts\products\dotnetfx20sp1.iss"
#include "scripts\products\dotnetfx20sp2.iss"
#ifdef use_dotnetfx20lp
#include "scripts\products\dotnetfx20lp.iss"
#include "scripts\products\dotnetfx20sp1lp.iss"
#include "scripts\products\dotnetfx20sp2lp.iss"
#endif
#endif

#ifdef use_dotnetfx35
//#include "scripts\products\dotnetfx35.iss"
#include "scripts\products\dotnetfx35sp1.iss"
#ifdef use_dotnetfx35lp
//#include "scripts\products\dotnetfx35lp.iss"
#include "scripts\products\dotnetfx35sp1lp.iss"
#endif
#endif

#ifdef use_dotnetfx40
#include "scripts\products\dotnetfx40client.iss"
#include "scripts\products\dotnetfx40full.iss"
#endif

#ifdef use_dotnetfx45
#include "scripts\products\dotnetfx45.iss"
#endif

#ifdef use_wic
#include "scripts\products\wic.iss"
#endif

#ifdef use_vc2010
#include "scripts\products\vcredist2010.iss"
#endif

#ifdef use_mdac28
#include "scripts\products\mdac28.iss"
#endif
#ifdef use_jet4sp8
#include "scripts\products\jet4sp8.iss"
#endif

#ifdef use_sqlcompact35sp2
#include "scripts\products\sqlcompact35sp2.iss"
#endif

#ifdef use_sql2005express
#include "scripts\products\sql2005express.iss"
#endif
#ifdef use_sql2008express
#include "scripts\products\sql2008express.iss"
#endif

[CustomMessages]
win_sp_title=Windows %1 Service Pack %2

[Code]
const
WM_CLOSE = 16;

function InitializeSetup(): boolean;
var winHwnd: longint;
    retVal : boolean;
    strProg: string;
begin

  Result := true;
  try
    //Either use FindWindowByClassName. ClassName can be found with Spy++ included with Visual C++. 
    strProg := 'Notepad';
    winHwnd := FindWindowByClassName(strProg);
    
    Log('winHwnd: ' + inttostr(winHwnd));
    if winHwnd <> 0 then
      retVal:=postmessage(winHwnd,WM_CLOSE,0,0);
      if retVal then
        Result := True
      else
        Result := False;

  except
  end;





	//init windows version
	initwinversion();

#ifdef use_iis
	if (not iis()) then exit;
#endif

#ifdef use_msi20
	msi20('2.0');
#endif
#ifdef use_msi31
	msi31('3.1');
#endif
#ifdef use_msi45
	msi45('4.5');
#endif
#ifdef use_ie6
	ie6('5.0.2919');
#endif

#ifdef use_dotnetfx11
	dotnetfx11();
#ifdef use_dotnetfx11lp
	dotnetfx11lp();
#endif
	dotnetfx11sp1();
#endif

	//install .netfx 2.0 sp2 if possible; if not sp1 if possible; if not .netfx 2.0
#ifdef use_dotnetfx20
	//check if .netfx 2.0 can be installed on this OS
	if not minwinspversion(5, 0, 3) then begin
		msgbox(fmtmessage(custommessage('depinstall_missing'), [fmtmessage(custommessage('win_sp_title'), ['2000', '3'])]), mberror, mb_ok);
		exit;
	end;
	if not minwinspversion(5, 1, 2) then begin
		msgbox(fmtmessage(custommessage('depinstall_missing'), [fmtmessage(custommessage('win_sp_title'), ['XP', '2'])]), mberror, mb_ok);
		exit;
	end;

	if minwinversion(5, 1) then begin
		dotnetfx20sp2();
#ifdef use_dotnetfx20lp
		dotnetfx20sp2lp();
#endif
	end else begin
		if minwinversion(5, 0) and minwinspversion(5, 0, 4) then begin
#ifdef use_kb835732
			kb835732();
#endif
			dotnetfx20sp1();
#ifdef use_dotnetfx20lp
			dotnetfx20sp1lp();
#endif
		end else begin
			dotnetfx20();
#ifdef use_dotnetfx20lp
			dotnetfx20lp();
#endif
		end;
	end;
#endif

#ifdef use_dotnetfx35
	//dotnetfx35();
	dotnetfx35sp1();
#ifdef use_dotnetfx35lp
	//dotnetfx35lp();
	dotnetfx35sp1lp();
#endif
#endif

#ifdef use_wic
	wic();
#endif

	// if no .netfx 4.0 is found, install the client (smallest)
#ifdef use_dotnetfx40
	if (not netfxinstalled(NetFx40Client, '') and not netfxinstalled(NetFx40Full, '')) then
		dotnetfx40client();
#endif

#ifdef use_dotnetfx45
    //dotnetfx45(2); // min allowed version is .netfx 4.5.2
    dotnetfx45(0); // min allowed version is .netfx 4.5.0
#endif

#ifdef use_vc2010
	vcredist2010();
#endif

#ifdef use_mdac28
	mdac28('2.7');
#endif
#ifdef use_jet4sp8
	jet4sp8('4.0.8015');
#endif

#ifdef use_sqlcompact35sp2
	sqlcompact35sp2();
#endif

#ifdef use_sql2005express
	sql2005express();
#endif
#ifdef use_sql2008express
	sql2008express();
#endif

	Result := true;
end;