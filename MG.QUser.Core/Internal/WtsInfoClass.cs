using System;

namespace MG.QUser.Core.Internal;

/// <summary>
/// Contains values that indicate the type of session information to retrieve in a call to the <c>WTSQuerySessionInformation</c> function.
/// </summary>
/// <remarks>
/// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/wtsapi32/ne-wtsapi32-wts_info_class">WTS_INFO_CLASS</see>.
/// </remarks>
public enum WtsInfoClass
{
    /// <summary>
    /// A null-terminated <see cref="string"/> that contains the name of the initial program that Remote Desktop Services runs when the
    /// user logs on.
    /// </summary>
    WTSInitialProgram,
    /// <summary>
    /// A null-terminated <see cref="string"/> that contains the published name of the application that the session is running.
    /// </summary>
    /// <remarks>
    /// Windows Server 2008 R2, Windows 7, Windows Server 2008 and Windows Vista:  This value is not supported.
    /// </remarks>
    WTSApplicationName,
    /// <summary>
    /// A null-terminated <see cref="string"/> that contains the default directory used when launching the initial program.
    /// </summary>
    WTSWorkingDirectory,
    /// <summary>
    /// This value is not used.
    /// </summary>
    [Obsolete("This value is not used.")]
    WTSOEMId,
    /// <summary>
    /// A <see cref="ulong"/> value that contains the session identifier.
    /// </summary>
    WTSSessionId,
    /// <summary>
    /// A null-terminated <see cref="string"/> that contains the name of the user associated with the session.
    /// </summary>
    WTSUserName,
    /// <summary>
    /// A null-terminated <see cref="string"/> that contains the name of the Remote Desktop Services session.
    /// </summary>
    /// <remarks>
    /// Note  Despite its name, specifying this type does not return the window station name. Rather, it returns the
    /// name of the Remote Desktop Services session. Each Remote Desktop Services session is associated with an interactive window
    /// station. Because the only supported window station name for an interactive window station is
    /// "<c>WinSta0</c>", each session is associated with its own <c>WinSta0</c> window station. For more information, 
    /// see <see href="https://learn.microsoft.com/en-us/windows/desktop/winstation/window-stations">Window Stations</see>.
    /// </remarks>
    WTSWinStationName,
    /// <summary>
    /// A null-terminated <see cref="string"/> that contains the name of the domain to which the logged-on user belongs.
    /// </summary>
    WTSDomainName,
    /// <summary>
    /// The session's current connection state. For more information, see <see cref="WtsConnectState"/>.
    /// </summary>
    WTSConnectState,
    /// <summary>
    /// A <see cref="ulong"/> value that contains the build number of the client.
    /// </summary>
    WTSClientBuildNumber,
    /// <summary>
    /// A null-terminated <see cref="string"/> that contains the name of the client.
    /// </summary>
    WTSClientName,
    /// <summary>
    /// A null-terminated <see cref="string"/> that contains the directory in which the client is installed.
    /// </summary>
    WTSClientDirectory,
    /// <summary>
    /// A <see cref="ushort"/> client-specific product identifier.
    /// </summary>
    WTSClientProductId,
    /// <summary>
    /// A <see cref="ulong"/> value that contains a client-specific hardware identifier.
    /// </summary>
    /// <remarks>
    ///  This option is reserved for future use. 
    /// <para>WTSQuerySessionInformation will always return a value of 0.</para>
    /// </remarks>
    WTSClientHardwareId,
    /// <summary>
    /// The network type and network address of the client.
    /// </summary>
    /// <remarks>
    /// For more information, see <see href="https://learn.microsoft.com/en-us/windows/desktop/api/wtsapi32/ns-wtsapi32-wts_client_address">WTS_CLIENT_ADDRESS</see>.
    /// <para>The IP address is offset by two bytes from the start of the Address member of the WTS_CLIENT_ADDRESS
    /// structure.</para>
    /// </remarks>
    WTSClientAddress,
    /// <summary>
    /// Information about the display resolution of the client.
    /// </summary>
    /// <remarks>
    /// For more information, see
    /// <see href="https://learn.microsoft.com/en-us/windows/desktop/api/wtsapi32/ns-wtsapi32-wts_client_display">WTS_CLIENT_DISPLAY</see>.
    /// </remarks>
    WTSClientDisplay,
    /// <summary>
    /// A USHORT value that specifies information about the protocol type for the
    /// session.
    /// </summary>
    /// <remarks>
    /// This is one of the following values:
    /// <para>
    /// <code>0 = Console session</code>
    /// <code>1 = Retained for legacy purposes</code>
    /// <code>2 = RDP protocol</code>
    /// </para>
    /// </remarks>
    WTSClientProtocolType,
    /// <summary>
    /// This value returns <see langword="false"/>.
    /// </summary>
    /// <remarks>
    /// <para>If you call GetLastError to get extended error information, GetLastError returns ERROR_NOT_SUPPORTED.</para>
    /// To caculate session idle time, use the LastInputTime structure field.
    /// Windows Server 2008 and Windows Vista:  This value is not used.
    /// </remarks>
    WTSIdleTime,
    /// <summary>
    /// This value returns <see langword="false"/>.
    /// </summary>
    /// <remarks>
    /// If you call GetLastError to get extended error information, GetLastError returns ERROR_NOT_SUPPORTED.
    /// <para>Windows Server 2008 and Windows Vista:  This value is not used.</para>
    /// </remarks>
    WTSLogonTime,
    /// <summary>
    /// This value returns <see langword="false"/>.
    /// </summary>
    /// <remarks>
    /// If you call GetLastError to get extended error information, GetLastError returns ERROR_NOT_SUPPORTED.
    /// <para>Windows Server 2008 and Windows Vista:  This value is not used.</para>
    /// </remarks>
    WTSIncomingBytes,
    /// <summary>
    /// This value returns <see langword="false"/>.
    /// </summary>
    /// <remarks>
    /// If you call GetLastError to get extended error information, GetLastError returns ERROR_NOT_SUPPORTED.
    /// <para>Windows Server 2008 and Windows Vista:  This value is not used.</para>
    /// </remarks>
    WTSOutgoingBytes,
    /// <summary>
    /// This value returns <see langword="false"/>.
    /// </summary>
    /// <remarks>
    /// If you call GetLastError to get extended error information, GetLastError returns ERROR_NOT_SUPPORTED.
    /// <para>Windows Server 2008 and Windows Vista:  This value is not used.</para>
    /// </remarks>
    WTSIncomingFrames,
    /// <summary>
    /// This value returns <see langword="false"/>.
    /// </summary>
    /// <remarks>
    /// If you call GetLastError to get extended error information, GetLastError returns ERROR_NOT_SUPPORTED.
    /// <para>Windows Server 2008 and Windows Vista:  This value is not used.</para>
    /// </remarks>
    WTSOutgoingFrames,
    /// <summary>
    /// Information about a Remote Desktop Connection (RDC) client.
    /// </summary>
    /// <remarks>
    /// For more information, see <see href="https://learn.microsoft.com/en-us/windows/desktop/api/wtsapi32/ns-wtsapi32-wtsclienta">WTSCLIENT</see>.
    /// </remarks>
    WTSClientInfo,
    /// <summary>
    /// Information about a client session on a RD Session Host server.
    /// </summary>
    /// <remarks>
    /// For more information, <see href="https://learn.microsoft.com/en-us/windows/desktop/api/wtsapi32/ns-wtsapi32-wtsinfoa">WTSINFO</see>.
    /// </remarks>
    WTSSessionInfo,
    /// <summary>
    /// Extended information about a session on a RD Session Host server. 
    /// </summary>
    /// <remarks>
    /// For more information, 
    /// see <see href="https://learn.microsoft.com/en-us/windows/desktop/api/wtsapi32/ns-wtsapi32-wtsinfoexa">WTSINFOEX</see>.
    /// <para>Windows Server 2008 and Windows Vista:  This value is not supported.</para>
    /// </remarks>
    WTSSessionInfoEx,
    /// <summary>
    /// A structure that contains information about the configuration of a RD Session Host server.
    /// </summary>
    /// <remarks>
    /// For more information, see <see href="https://learn.microsoft.com/en-us/windows/desktop/api/wtsapi32/ns-wtsapi32-wtsconfiginfoa">WTSCONFIGINFO</see>.
    /// <para>Windows Server 2008 and Windows Vista:  This value is not supported.</para>
    /// </remarks>
    WTSConfigInfo,
    /// <summary>
    /// This value is not supported.
    /// </summary>
    [Obsolete("This value is not supported.")]
    WTSValidationInfo,
    /// <summary>
    /// A structure that contains the IPv4 address assigned to the session.
    /// </summary>
    /// <remarks>
    /// <para>If the session does not have a virtual IP address, the WTSQuerySessionInformation function returns ERROR_NOT_SUPPORTED.</para>
    /// For more information, <see href="https://learn.microsoft.com/en-us/windows/desktop/api/wtsapi32/ns-wtsapi32-wts_session_address">WTS_SESSION_ADDRESS </see>.
    /// <para>Windows Server 2008 and Windows Vista:  This value is not supported.</para>
    /// </remarks>
    WTSSessionAddressV4,
    /// <summary>
    /// Determines whether the current session is a remote session. 
    /// </summary>
    /// <remarks>
    /// The WTSQuerySessionInformation function returns a value of <see langword="true"/> to indicate that the current session 
    /// is a remote session, and <see langword="false"/> to indicate that the current session is a local session.
    /// This value can only be used for the local machine, so the hServer parameter of the WTSQuerySessionInformation 
    /// function must contain <c>WTS_CURRENT_SERVER_HANDLE</c>.
    /// <para>Windows Server 2008 and Windows Vista:  This value is not supported.</para>
    /// </remarks>
    WTSIsRemoteSession,
}