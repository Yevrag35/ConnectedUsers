using System.Runtime.InteropServices;

namespace MG.QUser.Core.Internal.Structs;

/// <summary>
/// Represents the structure returned by WTSEnumerateSessions
/// </summary>
/// <remarks>
/// Holds information such as Session ID, the station name, and current session state.
/// <para>
/// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/wtsapi32/ns-wtsapi32-wts_session_infoa"/>
/// </para>
/// </remarks>
[StructLayout(LayoutKind.Sequential)]
internal struct WTS_SESSION_INFO
{
    public int SessionId;

    [MarshalAs(UnmanagedType.LPStr)]
    public string pWinStationName;

    public WtsConnectState State;
}
