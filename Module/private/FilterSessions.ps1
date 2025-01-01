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