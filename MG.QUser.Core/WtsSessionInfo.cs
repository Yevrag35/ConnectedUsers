using System;
using System.Collections.Generic;
using System.Text;

#nullable enable

namespace MG.QUser.Core;

/// <summary>
/// Simple model class to hold session information results
/// </summary>
/// <remarks>
/// Represents the data for a single user session on the machine.
/// </remarks>
public sealed class WtsSessionInfo : IComparable<WtsSessionInfo>
{
    /// <summary>
    /// The session's unique identifier.
    /// </summary>
    public int SessionId { get; set; }

    /// <summary>
    /// The client's machine name if connected via Remote Desktop.
    /// </summary>
    public string? ClientName { get; set; }

    public string ComputerName { get; set; } = Environment.MachineName;

    /// <summary>
    /// The user's domain name (if any).
    /// </summary>
    public string? DomainName { get; set; }

    public TimeSpan? IdleTime { get; set; }

    public DateTimeOffset? LogonTime { get; set; }

    /// <summary>
    /// The current connection state (e.g., Active, Disconnected).
    /// </summary>
    public WtsConnectState State { get; set; }

    /// <summary>
    /// The username associated with this session (if any).
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// Name of the RDP/Terminal Services session.
    /// </summary>
    public string? WinStationName { get; set; }

    public int CompareTo(WtsSessionInfo? other)
    {
        return this.SessionId.CompareTo(other?.SessionId);
    }

    public override string ToString()
    {
        return $"[Session ID: {this.SessionId}, " +
               $"WinStation: {this.WinStationName}, " +
               $"State: {this.State}, " +
               $"User: {this.DomainName}\\{this.UserName}, " +
               $"Client: {this.ClientName}]";
    }
}
