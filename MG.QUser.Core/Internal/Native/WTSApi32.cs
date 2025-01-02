using MG.QUser.Core.Internal.Handles;
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

    internal static WtsSessionArraySafeHandle WTSEnumerateSessions(WtsSessionSafeHandle hServer)
    {
        bool result = WTSEnumerateSessions(hServer, Reserved: 0, Version: 1, out IntPtr ppSessions, out int count);
        return WtsSessionArraySafeHandle.Create(ref ppSessions, ref count, ref result);
    }

    // P/Invoke for WTSEnumerateSessions
    [DllImport("wtsapi32.dll", SetLastError = true)]
    internal static extern bool WTSEnumerateSessions(
        WtsSessionSafeHandle hServer,
        int Reserved,
        int Version,
        out IntPtr pSessionInfo,
        out int pCount);

    // P/Invoke for WTSFreeMemory
    [DllImport("wtsapi32.dll", SetLastError = true)]
    internal static extern void WTSFreeMemory(IntPtr pMemory);

    internal static WtsInfoSafeHandle WTSQuerySessionInformation(ref int sessionId, WtsSessionSafeHandle sessionHandle)
    {
        bool result = WTSQuerySessionInformation(
            sessionHandle,
            sessionId,
            WtsInfoClass.WTSSessionInfo,
            out IntPtr buffer,
            out int pBytesReturned);

        return WtsInfoSafeHandle.Create(ref buffer, ref result, ref pBytesReturned);
    }
    internal static WtsBufferHandle WTSQuerySessionInformation(ref int sessionId, WtsSessionSafeHandle sessionHandle, WtsInfoClass infoClass)
    {
        bool result = WTSQuerySessionInformation(
            sessionHandle,
            sessionId,
            infoClass,
            out IntPtr buffer,
            out int pBytesReturned);

        return WtsBufferHandle.Create(ref buffer, ref result, ref pBytesReturned);
    }

    // P/Invoke for WTSQuerySessionInformation
    [DllImport("wtsapi32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
    private static extern bool WTSQuerySessionInformation(
        WtsSessionSafeHandle hServer,
        int sessionId,
        WtsInfoClass wtsInfoClass,
        out IntPtr ppBuffer,
        out int pBytesReturned
    );
}
