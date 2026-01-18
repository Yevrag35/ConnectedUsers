using MG.QUser.Core.Internal.Structs;
using System;

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

    /// <summary>
    /// Gets or sets the name of the computer the session is associated with.
    /// </summary>
    public string ComputerName { get; set; } = string.Empty;

    /// <summary>
    /// The user's domain name (if any).
    /// </summary>
    public string? DomainName { get; set; }
    /// <summary>
    /// Gets or sets the duration of inactivity the session has experienced.
    /// </summary>
    public TimeSpan? IdleTime { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the user last logged on.
    /// </summary>
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
        return other is null ? -1 : this.SessionId.CompareTo(other.SessionId);
    }

    /// <summary>
    /// Creates a new instance of the <see cref="WtsSessionInfo"/> class using the specified computer name and session information.
    /// </summary>
    /// <param name="computerName">The name of the computer associated with the session.</param>
    /// <param name="wtsSessionInfo">A reference to a <see cref="WTS_SESSION_INFO"/> structure containing basic session details. The structure is used to populate
    /// session-related properties.</param>
    /// <param name="wtsInfo">A reference to a <see cref="WTSINFO"/> structure containing extended session information, such as logon and idle times. The
    /// structure is used to populate time-related properties.</param>
    /// <returns>A <see cref="WtsSessionInfo"/> object initialized with the provided session and computer information.</returns>
    internal static WtsSessionInfo Create(string computerName, ref WTS_SESSION_INFO wtsSessionInfo, ref WTSINFO wtsInfo)
    {
        return new WtsSessionInfo
        {
            ComputerName = computerName,
            IdleTime = wtsInfo.LastInputTime > 0
                    ? DateTime.FromFileTimeUtc(wtsInfo.CurrentTime) - DateTime.FromFileTimeUtc(wtsInfo.LastInputTime)
                    : null,
            LogonTime = wtsInfo.LogonTime > 0
                    ? DateTime.FromFileTimeUtc(wtsInfo.LogonTime).ToLocalTime()
                    : null,
            SessionId = wtsSessionInfo.SessionId,
            State = wtsSessionInfo.State,
            WinStationName = wtsSessionInfo.pWinStationName,
        };
    }
}
