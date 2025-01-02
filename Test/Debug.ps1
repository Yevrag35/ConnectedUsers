$curDir = Split-Path -Parent $MyInvocation.MyCommand.Definition;
$myDesktop = [System.Environment]::GetFolderPath("Desktop")

Import-Module "$curDir\MG.QUser.Core.dll" -ErrorAction Stop

Push-Location $([System.Environment]::GetFolderPath("Desktop"))