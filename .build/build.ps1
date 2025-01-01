$builder = New-Object -TypeName "System.Text.StringBuilder"

foreach ($priv in $(Get-ChildItem -Path "$PSScriptRoot\..\Module\private" -Filter *.ps1 -Exclude "ComparerBuilder.ps1" -Recurse))
{
    [void] $builder.AppendLine((Get-Content -Path $priv.FullName -Raw))
    [void] $builder.AppendLine()
}

foreach ($pub in Get-ChildItem -Path "$PSScriptRoot\..\Module\public" -Filter *.ps1)
{
    [void] $builder.AppendLine((Get-Content -Path $pub.FullName -Raw))
    [void] $builder.AppendLine()
}

Set-Content -Path "$PSScriptRoot\..\Module\ConnectedUsers.psm1" -Value $builder.ToString() -Force

# Build DotNet Project
dotnet build "$PSScriptRoot\..\QUserModule.sln" -c Release

$copyToRoot = "$PSScriptRoot\..\Module\assemblies"
foreach ($dll in $(Get-ChildItem -Path "$PSScriptRoot\..\MG.QUser.Core\bin\Release" -Filter *.dll -Recurse)) {

    $parent = [System.IO.Path]::GetDirectoryName($dll.FullName)
    $parentName = Split-Path -Path $parent -Leaf
    Write-Host $parentName
    $copyTo = "$copyToRoot\$($parentName.Replace('-windows', '').Replace('.', '').Replace('framework', ''))"
    New-Item -Path $copyTo -ItemType Directory -ea 0 | Out-Null
    $dll | Copy-Item -Destination $copyTo -Force
}