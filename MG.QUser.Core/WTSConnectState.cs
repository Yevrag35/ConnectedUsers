namespace MG.QUser.Core;

/// <summary>
/// Session connection states recognized by Terminal Services
/// </summary>
/// <remarks>
/// For more detailed states, see <see cref="WtsConnectState"/>.
/// </remarks>
public enum WtsConnectState
{
    Active,
    Connected,
    ConnectQuery,
    Shadow,
    Disconnected,
    Idle,
    Listening,
    Reset,
    Down,
    Init
}