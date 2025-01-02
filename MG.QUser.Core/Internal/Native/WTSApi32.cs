using System;
using System.Runtime.InteropServices;

namespace MG.QUser.Core.Internal.Native;

internal static class WTSApi32
{
    #region SERVER CONNECTIONS
    // WTSOpenServer opens a handle to a remote (or local) server
    [DllImport("wtsapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern IntPtr WTSOpenServer(string pServerName);

    // WTSCloseServer closes that handle
    [DllImport("wtsapi32.dll", SetLastError = true)]
    internal static extern void WTSCloseServer(IntPtr hServer);

    #endregion

    // P/Invoke for WTSEnumerateSessions
    [DllImport("wtsapi32.dll", SetLastError = true)]
    internal static extern bool WTSEnumerateSessions(
        SafeWtsSessionHandle hServer,
        int Reserved,
        int Version,
        out IntPtr ppSessionInfo,
        out int pCount);

    // P/Invoke for WTSFreeMemory
    [DllImport("wtsapi32.dll", SetLastError = true)]
    internal static extern void WTSFreeMemory(IntPtr pMemory);

    // P/Invoke for WTSQuerySessionInformation
    [DllImport("wtsapi32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
    internal static extern bool WTSQuerySessionInformation(
        SafeWtsSessionHandle hServer,
        int sessionId,
        WtsInfoClass wtsInfoClass,
        out IntPtr ppBuffer,
        out int pBytesReturned
    );
}
