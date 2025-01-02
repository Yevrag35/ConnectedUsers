using MG.QUser.Core.Internal;
using MG.QUser.Core.Internal.Structs;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MG.QUser.Core;

#nullable enable

public static partial class QUserHelper
{
    // P/Invoke for WTSEnumerateSessions
    [DllImport("wtsapi32.dll", SetLastError = true)]
    private static extern bool WTSEnumerateSessions(
        IntPtr hServer,
        int Reserved,
        int Version,
        out IntPtr ppSessionInfo,
        out int pCount);

    // P/Invoke for WTSFreeMemory
    [DllImport("wtsapi32.dll", SetLastError = true)]
    private static extern void WTSFreeMemory(IntPtr pMemory);

    // P/Invoke for WTSQuerySessionInformation
    [DllImport("wtsapi32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
    private static extern bool WTSQuerySessionInformation(
        IntPtr hServer,
        int sessionId,
        WtsInfoClass wtsInfoClass,
        out IntPtr ppBuffer,
        out int pBytesReturned
    );

    /// <summary>
    /// Enumerates and returns a collection of session information from the local machine.
    /// </summary>
    /// <returns>A list of session information objects.</returns>
    /// <exception cref="Win32Exception"/>
    [DebuggerStepThrough]
    public static WtsSessionInfo[] GetAllSessions()
    {
        return GetAllSessions(computerName: null);
    }

    /// <summary>
    /// Enumerates and returns a collection of session information from the specified server's opened connection.
    /// </summary>
    /// <param name="handle">The opened server handle.</param>
    /// <returns>A list of session information objects.</returns>
    /// <exception cref="Win32Exception"/>
    public static WtsSessionInfo[] GetAllSessions(string? computerName)
    {
        IntPtr pSessions = IntPtr.Zero;
        int count = 0;

        WtsSessionInfo[] sessionList = [];

        WtsSessionHandle session = WtsSessionHandle.OpenConnection(computerName);
        try
        {
            bool result = WTSEnumerateSessions(session,
                Reserved: 0,
                Version: 1,  // Usually is 1.
                out pSessions,
                out count);

            if (!result)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            if (count == 0)
            {
                return sessionList;
            }

            sessionList = new WtsSessionInfo[count];

            // Each session is a block of memory (WTS_SESSION_INFO).
            long dataSize = Marshal.SizeOf(typeof(WTS_SESSION_INFO));
            long current = pSessions.ToInt64();

            for (int i = 0; i < count; i++)
            {
                AddSessionInfo(sessionList, ref session, ref i, ref current, ref dataSize);
            }
        }
        finally
        {
            if (IntPtr.Zero != pSessions)
            {
                WTSFreeMemory(pSessions);
            }

            session.Dispose();
        }

        return sessionList;
    }

    private static void AddSessionInfo(WtsSessionInfo[] sessionList, ref WtsSessionHandle handle, ref int index, ref long current, ref long dataSize)
    {
        // Marshal the current pointer to a WTS_SESSION_INFO struct.
        WTS_SESSION_INFO sessionInfo = MarshalHelper.PtrToStruct<WTS_SESSION_INFO>((IntPtr)current);

        IntPtr buffer = IntPtr.Zero;
        try
        {
            bool success = WTSQuerySessionInformation(
                handle,
                sessionInfo.SessionId,
                WtsInfoClass.WTSSessionInfo,
                out buffer,
                out int bytesReturned
            );

            if (!success || IntPtr.Zero == buffer || bytesReturned < Marshal.SizeOf(typeof(WTSINFO)))
            {
                return;
            }

            WTSINFO wtsInfo = MarshalHelper.PtrToStruct<WTSINFO>(buffer);
            string userName = QuerySessionInfoString(ref sessionInfo.SessionId, ref handle, WtsInfoClass.WTSUserName);
            string domainName = QuerySessionInfoString(ref sessionInfo.SessionId, ref handle, WtsInfoClass.WTSDomainName);
            string clientName = QuerySessionInfoString(ref sessionInfo.SessionId, ref handle, WtsInfoClass.WTSClientName);

            ref long currentTime = ref wtsInfo.CurrentTime;

            sessionList[index] = new WtsSessionInfo
            {
                ClientName = clientName,
                ComputerName = handle.ComputerName,
                DomainName = domainName,
                IdleTime = wtsInfo.LastInputTime > 0
                    ? DateTime.FromFileTimeUtc(currentTime) - DateTime.FromFileTimeUtc(wtsInfo.LastInputTime)
                    : null,
                LogonTime = wtsInfo.LogonTime > 0
                    ? DateTime.FromFileTimeUtc(wtsInfo.LogonTime).ToLocalTime()
                    : null,
                SessionId = sessionInfo.SessionId,
                State = sessionInfo.State,
                UserName = userName,
                WinStationName = sessionInfo.pWinStationName,
            };
        }
        finally
        {
            // Move the pointer to the next structure
            current += dataSize;

            if (buffer != IntPtr.Zero)
            {
                WTSFreeMemory(buffer);
            }
        }
    }
}
