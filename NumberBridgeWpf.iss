; 脚本由 Inno Setup 脚本向导生成。
; 有关创建 Inno Setup 脚本文件的详细信息，请参阅帮助文档！

#define MyAppName "Number Bridge Wpf"
#define MyAppVersion "1.0"
#define MyAppPublisher "AuthorZiXi"
#define MyAppURL "https://www.github.com/TeaRed-LeafFall"
#define MyAppExeName "NumberBridgeWpf.exe"

[Setup]
; 注意：AppId 的值唯一标识此应用程序。不要在其他应用程序的安装程序中使用相同的 AppId 值。
; (若要生成新的 GUID，请在 IDE 中单击 "工具|生成 GUID"。)
AppId={{2525B45E-B5BE-436B-B66A-E148843429FA}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
; "ArchitecturesAllowed=x64compatible" 指定安装程序无法运行
; 除 Arm 上的 x64 和 Windows 11 之外的任何平台上。
ArchitecturesAllowed=x64compatible
; "ArchitecturesInstallIn64BitMode=x64compatible" 要求
; 安装可以在 x64 或 Arm 上的 Windows 11 上以“64 位模式”完成，
; 这意味着它应该使用本机 64 位 Program Files 目录和
; 注册表的 64 位视图。
ArchitecturesInstallIn64BitMode=x64compatible
DisableProgramGroupPage=yes
; 取消对以下行的注释以在非管理安装模式下运行(仅针对当前用户进行安装)。
;PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=dialog
OutputDir=C:\Users\leaffall\Desktop
OutputBaseFilename=NumberBridgeWpf-Setup
SetupIconFile=E:\dev\NumberBridge\icon.ico
Compression=lzma
SolidCompression=yes
WizardStyle=modern

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "E:\dev\NumberBridge\NumberBridgeWpf\bin\publish\x64\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\dev\NumberBridge\NumberBridgeWpf\bin\publish\x64\NumberBridge.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\dev\NumberBridge\NumberBridgeWpf\bin\publish\x64\NumberBridge.pdb"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\dev\NumberBridge\NumberBridgeWpf\bin\publish\x64\NumberBridgeWpf.deps.json"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\dev\NumberBridge\NumberBridgeWpf\bin\publish\x64\NumberBridgeWpf.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\dev\NumberBridge\NumberBridgeWpf\bin\publish\x64\NumberBridgeWpf.pdb"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\dev\NumberBridge\NumberBridgeWpf\bin\publish\x64\NumberBridgeWpf.runtimeconfig.json"; DestDir: "{app}"; Flags: ignoreversion
; 注意：不要在任何共享系统文件上使用 "Flags: ignoreversion" 

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

