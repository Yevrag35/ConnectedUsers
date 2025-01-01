$private:curDir = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent

if ($PSEdition -eq 'Core' -or $PSVersionTable.PSVersion.Major -gt 5) {

    $windows = [System.Runtime.InteropServices.OSPlatform]::Windows
    if (-not [System.Runtime.InteropServices.RuntimeInformation]::IsOSPlatform($windows)) {
        throw "This module is only supported on Windows Operating Systems."
    }

    Import-Module "$private:curDir\assemblies\netstandard20\MG.QUser.Core.dll" -ErrorAction Stop
}
else {
    Import-Module "$private:curDir\assemblies\net452\MG.QUser.Core.dll" -ErrorAction Stop
}

Import-Module "$private:curDir\ConnectedUsers.psm1" -ErrorAction Stop