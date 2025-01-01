Function FilterSessions() {
    [CmdletBinding()]
    [OutputType([MG.QUser.Core.WtsSessionInfo])]
    param (
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [AllowEmptyCollection()]
        [MG.QUser.Core.WtsSessionInfo[]] $InputObject,

        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [AllowEmptyString()]
        [AllowNull()]
        [string[]] $AvoidStationNames,

        [Parameter(Mandatory = $true)]
        [ValidateRange(65535, [int]::MaxValue)]
        [int] $ExclusiveMaxSessionId
    )

    Process {
        $InputObject | Where-Object {
            $_.SessionId -lt $ExclusiveMaxSessionId -and $AvoidStationNames -notcontains $_.WinStationName
        }
    }
}

Function GetShouldMessage() {
    [CmdletBinding(DefaultParameterSetName = "ByComputerName")]
    [OutputType([string])]
    param (
        [Parameter(Mandatory = $true, ParameterSetName = "ByInputObject")]
        [MG.QUser.Core.WtsSessionInfo] $InputObject,

        [Parameter(Mandatory = $true, ParameterSetName = "ByComputerName")]
        [AllowEmptyString()]
        [AllowNull()]
        [string] $ComputerName,

        [Parameter(Mandatory = $true, ParameterSetName = "ByComputerName")]
        [int] $SessionId
    )
    Process {
        if ($PSCmdlet.ParameterSetName -eq 'ByInputObject') {
            return "Session ID $($InputObject.SessionId): $($InputObject.UserName) on '$($InputObject.ComputerName)'"
        }
        elseif ([string]::IsNullOrEmpty($ComputerName)) {
            $ComputerName = "$($env:COMPUTERNAME) (localhost)"
        }

        return "Session ID $($SessionId) on '$ComputerName'"
    }
}

Function Get-ConnectedUser() {
    [CmdletBinding()]
    [Alias("Get-QUser", "gcu")]
    [OutputType([MG.QUser.Core.WtsSessionInfo])]
    param (
        [Parameter(Mandatory = $false, ValueFromPipelineByPropertyName = $true, ValueFromPipeline = $true)]
        [AllowEmptyCollection()]
        [AllowEmptyString()]
        [AllowNull()]
        [string[]] $ComputerName
    )
    Begin {
        [string[]] $AvoidName = @("Services")
        $exclusiveMaxSessionId = 65536
    }
    Process {
        if ($null -eq $ComputerName -or $ComputerName.Count -eq 0) {

            [MG.QUser.Core.QUserHelper]::GetAllSessions() | FilterSessions -AvoidStationNames $AvoidName -ExclusiveMaxSessionId $exclusiveMaxSessionId

            return
        }

        foreach ($computer in $ComputerName) {

            [MG.QUser.Core.QUserHelper]::GetAllSessions($computer) | FilterSessions -AvoidStationNames $AvoidName -ExclusiveMaxSessionId $exclusiveMaxSessionId
        }
    }
}

Function Remove-ConnectedUser() {
    [CmdletBinding(DefaultParameterSetName = "BySessionId", SupportsShouldProcess = $true, ConfirmImpact = "High")]
    [Alias("Remove-QUser", "rcu")]
    param (
        [Parameter(Mandatory = $true, ValueFromPipeline = $true, ParameterSetName = "ByInputObject")]
        [AllowEmptyCollection()]
        [MG.QUser.Core.WtsSessionInfo[]] $InputObject,

        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = "BySessionId")]
        [ValidateRange(1, 65534)]
        [int[]] $SessionId,

        [Parameter(Mandatory = $false, ParameterSetName = "BySessionId")]
        [AllowNull()]
        [AllowEmptyString()]
        [string] $ComputerName,

        [Parameter(Mandatory = $false)]
        [switch] $Force
    )
    Begin {
        $logoffExe = "$env:WINDIR\System32\logoff.exe"
        $areYouSure = "Are you sure you want to log off the user?"

        if (-not (Test-Path $logoffExe -PathType Leaf)) {
            throw "Logoff.exe not found at '$logoffExe'"
        }

        $yesToAll = $false
        $noToAll = $false
    }
    Process {
        if ($PSCmdlet.ParameterSetName -eq 'ByInputObject') {

            foreach ($session in $InputObject) {

                $msg = GetShouldMessage -InputObject $session
                if ($Force -or $PSCmdlet.ShouldContinue($msg, $areYouSure, [ref]$yesToAll, [ref]$noToAll)) {
                    & $logoffExe $($session.SessionId) "/SERVER:$($session.ComputerName)"

                    Write-Verbose "Logged off session $($session.SessionId) on '$($session.ComputerName)'"
                }
            }
        }
        elseif ($PSCmdlet.ParameterSetName -eq 'BySessionId') {

            foreach ($id in $SessionId) {

                $msg = GetShouldMessage -ComputerName $ComputerName -SessionId $id
                if ($Force -or $PSCmdlet.ShouldContinue($msg, $areYouSure, [ref]$yesToAll, [ref]$noToAll)) {

                    & $logoffExe $id "/SERVER:$($ComputerName)"

                    Write-Verbose "Logged off session $id on '$ComputerName'"
                }
            }
        }
    }
}


