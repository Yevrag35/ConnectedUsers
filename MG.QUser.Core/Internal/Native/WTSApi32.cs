using MG.QUser.Core.Internal.Handles;
using System;
using System.Runtime.InteropServices;

namespace MG.QUser.Core.Internal.Native;

internal static class WTSApi32
{
    #region SERVER CONNECTIONS
    // WTSOpenServer opens a handle to a remote (or local) server
    [DllImport("wtsapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern nint WTSOpenServer(string pServerName);

    // WTSCloseServer closes that handle
    [DllImport("wtsapi32.dll", SetLastError = true)]
    internal static extern void WTSCloseServer(nint hServer);

    #endregion

    internal static WtsSessionArraySafeHandle WTSEnumerateSessions(WtsSessionSafeHandle hServer)
    {
        bool result = WTSEnumerateSessions(hServer, Reserved: 0, Version: 1, out nint pSessions, out int count);
        return WtsSessionArraySafeHandle.Create(pSessions, count, result);
    }

    // P/Invoke for WTSEnumerateSessions
    [DllImport("wtsapi32.dll", SetLastError = true)]
    internal static extern bool WTSEnumerateSessions(
        WtsSessionSafeHandle hServer,
        int Reserved,
        int Version,
        out nint pSessionInfo,
        out int pCount);

    // P/Invoke for WTSFreeMemory
    [DllImport("wtsapi32.dll", SetLastError = true)]
    internal static extern void WTSFreeMemory(nint pMemory);

    internal static WtsInfoSafeHandle WTSQuerySessionInformation(int sessionId, WtsSessionSafeHandle sessionHandle)
    {
        bool result = WTSQuerySessionInformation(
            sessionHandle,
            sessionId,
            WtsInfoClass.WTSSessionInfo,
            out nint buffer,
            out int pBytesReturned);

        return WtsInfoSafeHandle.Create(buffer, result, pBytesReturned);
    }
    internal static WtsBufferHandle WTSQuerySessionInformation(int sessionId, WtsSessionSafeHandle sessionHandle, WtsInfoClass infoClass)
    {
        bool result = WTSQuerySessionInformation(
            sessionHandle,
            sessionId,
            infoClass,
            out nint buffer,
            out int pBytesReturned);

        return WtsBufferHandle.Create(buffer, result, pBytesReturned);
    }

    // P/Invoke for WTSQuerySessionInformation
    [DllImport("wtsapi32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
    private static extern bool WTSQuerySessionInformation(
        WtsSessionSafeHandle hServer,
        int sessionId,
        WtsInfoClass wtsInfoClass,
        out nint ppBuffer,
        out int pBytesReturned);
}
