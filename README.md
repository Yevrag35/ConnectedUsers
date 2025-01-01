# ConnectedUsers

[![version](https://img.shields.io/powershellgallery/v/ConnectedUsers.svg?include_prereleases)](https://www.powershellgallery.com/packages/ConnectedUsers)
[![downloads](https://img.shields.io/powershellgallery/dt/ConnectedUsers.svg?label=downloads)](https://www.powershellgallery.com/stats/packages/ConnectedUsers?groupby=Version)

ConnectedUsers is a PowerShell module that provides cmdlets to query and disconnect local and remote user sessions.

## Cmdlets

- `Get-ConnectedUser` (aliases: `Get-QUser`, `gcu`): Retrieves information about user sessions.
- `Remove-ConnectedUser` (aliases: `Remove-QUser`, `rcu`): Disconnects a specified user session.

## Installation

You can install the ConnectedUsers module from the PowerShell Gallery: [Link to the Gallery Page](https://www.powershellgallery.com/packages/ConnectedUsers)

## Usage

After installation, import the module:
```powershell
Import-Module ConnectedUsers
```

### Retrieving User Sessions

To retrieve all user sessions on the local machine:

```powershell
Get-ConnectedUser
```

To retrieve sessions on remote machines:

```powershell
Get-ConnectedUser -ComputerName "RemotePC1", "RemotePC2"
```

### Logoff a User Session

To logoff a user session by specifying the Session ID:

```powershell
Remove-ConnectedUser 1
```

To disconnect a user session on a remote machine:
```powershell
Remove-ConnectedUser -SessionId 2 -ComputerName "RemotePC"
```

To logoff a specific user from multiple machines:
```powershell
$userName = "thisguy"
Get-ConnectedUser -ComputerName "RemotePC1", "RemoteServer1" | Where-Object { $_.UserName -eq $thisGuy } | Remove-ConnectedUser
```