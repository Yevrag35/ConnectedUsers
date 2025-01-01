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