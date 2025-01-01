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