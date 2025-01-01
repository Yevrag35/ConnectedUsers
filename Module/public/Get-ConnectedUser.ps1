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