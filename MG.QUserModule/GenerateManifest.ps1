param
(
    [parameter(Mandatory=$true, Position=0)]
    [string] $DebugDirectory,

    [parameter(Mandatory=$true, Position=2)]
    [string] $AssemblyInfo,

    [parameter(Mandatory=$true, Position=3)]
    [string] $TargetFileName
)

Get-ChildItem $DebugDirectory *.ps1xml -File | Remove-Item -Force;
$manifestFileName = "ConnectedUsers.psd1";
$manifestFile = Join-Path $DebugDirectory $manifestFileName;
if ((Test-Path $manifestFile))
{
	Remove-Item $manifestFile -Force;
	Get-ChildItem "$DebugDirectory" *.ps1xml -File | Remove-Item -Force;
}

## Get Module Version
$assInfo = Get-Content $AssemblyInfo;
foreach ($line in $assInfo)
{
    if ($line -like "*AssemblyFileVersion(*")
    {
        $vers = $line -replace '^\s*\[assembly\:\sAssemblyFileVersion\(\"(.*?)\"\)\]$', '$1';
    }
}

$References = Join-Path "$DebugDirectory\..\.." "ReferenceAssemblies";
$Formats = Join-Path "$DebugDirectory\..\.." "TypeFormats";
$dlls = Get-ChildItem $References -Include *.dll -Exclude 'System.Management.Automation.dll' -Recurse;
$formatFiles = Get-ChildItem $Formats -Include *.ps1xml -Recurse;
[string[]]$allDlls = $dlls.Name;
[string[]]$allFormats = $formatFiles.Name;
@($dlls, $formatFiles) | Copy-Item -Destination $DebugDirectory -Force;

$manifest = @{
    Path               = $manifestFile
    Guid               = '0cd92751-7c76-4b31-ae4a-48d344f9b786';
    Description        = 'A set of PowerShell cmdlets to query and disconnect local and remote sessions.'
    Author             = 'Mike Garvey'
    CompanyName        = 'Yevrag35, LLC.'
    Copyright          = '(c) 2019 Yevrag35, LLC.  All rights reserved.'
    ModuleVersion      = $vers.Trim()
    PowerShellVersion  = '4.0'
	DotNetFrameworkVersion = '4.7'
    RootModule         = $TargetFileName
	RequiredAssemblies = $allDlls
	FormatsToProcess   = $allFormats
    AliasesToExport    = @('Get-ConnectedUser', 'Remove-ConnectedUser', 'gcu', 'rcu')
    CmdletsToExport    = @('Get-QUser', 'Remove-QUser')
    FunctionsToExport  = @()
    VariablesToExport  = ''
	ProjectUri         = 'https://git.yevrag35.com/gityev/qusermodule.git'
	Tags               = @('QUser', 'RDP', 'Console', 'query', 'get', 'connected', 'user', 'query', 'remove', 'remote', 'logoff', 'disconnect')
};

New-ModuleManifest @manifest;