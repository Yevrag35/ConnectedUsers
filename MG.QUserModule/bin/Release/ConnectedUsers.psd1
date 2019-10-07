﻿#
# Module manifest for module 'ConnectedUsers'
#
# Generated by: Mike Garvey
#
# Generated on: 10/7/2019
#

@{

# Script module or binary module file associated with this manifest.
RootModule = 'MG.QUserModule.dll'

# Version number of this module.
ModuleVersion = '1.2.1'

# Supported PSEditions
# CompatiblePSEditions = @()

# ID used to uniquely identify this module
GUID = '0cd92751-7c76-4b31-ae4a-48d344f9b786'

# Author of this module
Author = 'Mike Garvey'

# Company or vendor of this module
CompanyName = 'Yevrag35, LLC.'

# Copyright statement for this module
Copyright = '(c) 2019 Yevrag35, LLC.  All rights reserved.'

# Description of the functionality provided by this module
Description = 'A set of PowerShell cmdlets to query and disconnect local and remote sessions.'

# Minimum version of the Windows PowerShell engine required by this module
PowerShellVersion = '4.0'

# Name of the Windows PowerShell host required by this module
# PowerShellHostName = ''

# Minimum version of the Windows PowerShell host required by this module
# PowerShellHostVersion = ''

# Minimum version of Microsoft .NET Framework required by this module. This prerequisite is valid for the PowerShell Desktop edition only.
DotNetFrameworkVersion = '4.7'

# Minimum version of the common language runtime (CLR) required by this module. This prerequisite is valid for the PowerShell Desktop edition only.
# CLRVersion = ''

# Processor architecture (None, X86, Amd64) required by this module
# ProcessorArchitecture = ''

# Modules that must be imported into the global environment prior to importing this module
# RequiredModules = @()

# Assemblies that must be loaded prior to importing this module
RequiredAssemblies = 'Microsoft.ActiveDirectory.Management.dll'

# Script files (.ps1) that are run in the caller's environment prior to importing this module.
# ScriptsToProcess = @()

# Type files (.ps1xml) to be loaded when importing this module
# TypesToProcess = @()

# Format files (.ps1xml) to be loaded when importing this module
FormatsToProcess = 'QUserObject.ps1xml'

# Modules to import as nested modules of the module specified in RootModule/ModuleToProcess
# NestedModules = @()

# Functions to export from this module, for best performance, do not use wildcards and do not delete the entry, use an empty array if there are no functions to export.
FunctionsToExport = @()

# Cmdlets to export from this module, for best performance, do not use wildcards and do not delete the entry, use an empty array if there are no cmdlets to export.
CmdletsToExport = 'Get-QUser', 'Remove-QUser'

# Variables to export from this module
VariablesToExport = @()

# Aliases to export from this module, for best performance, do not use wildcards and do not delete the entry, use an empty array if there are no aliases to export.
AliasesToExport = 'Get-ConnectedUser', 'Remove-ConnectedUser', 'gcu', 'rcu'

# DSC resources to export from this module
# DscResourcesToExport = @()

# List of all modules packaged with this module
# ModuleList = @()

# List of all files packaged with this module
FileList = @(
    'ConnectedUsers.psd1',
    'MG.QUserModule.dll',
    'Microsoft.ActiveDirectory.Management.dll',
    'QUserObject.ps1xml'
)

# Private data to pass to the module specified in RootModule/ModuleToProcess. This may also contain a PSData hashtable with additional module metadata used by PowerShell.
PrivateData = @{

    PSData = @{

        # Tags applied to this module. These help with module discovery in online galleries.
        Tags = 'QUser', 'RDP', 'Console', 'query', 'get', 'connected', 'user', 'query', 'remove',
               'remote', 'logoff', 'disconnect'

        # A URL to the license for this module.
        # LicenseUri = ''

        # A URL to the main website for this project.
        ProjectUri = 'https://github.com/Yevrag35/ConnectedUsers'

        # A URL to an icon representing this module.
        # IconUri = ''

        # ReleaseNotes of this module
        ReleaseNotes = 'Better Regex parsing to account for more variations.'

    } # End of PSData hashtable

} # End of PrivateData hashtable

# HelpInfo URI of this module
HelpInfoURI = 'https://github.com/Yevrag35/ConnectedUsers/issues'

# Default prefix for commands exported from this module. Override the default prefix using Import-Module -Prefix.
# DefaultCommandPrefix = ''

}

