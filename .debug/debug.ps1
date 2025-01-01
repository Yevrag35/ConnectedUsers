$curDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
if ([string]::IsNullOrEmpty($curDir)) {
    $curDir = Get-Location | Select -ExpandProperty "Path"
}

foreach ($dll in $(Get-ChildItem -Path "$curDir\..\Module\assemblies" -Filter *.dll -Recurse -ea 0)) {
    Import-Module $dll.FullName
}

foreach ($priv in $(Get-ChildItem -Path "$curDir\..\Module\private" -Filter *.ps1 -Recurse)) {

    . $priv.FullName
}

foreach ($pub in $(Get-ChildItem -Path "$curDir\..\Module\public" -Filter *.ps1)) {

    . $pub.FullName
}