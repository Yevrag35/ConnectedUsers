using MG.QUser.Core.Internal;
using MG.QUser.Core.Internal.Handles;
using MG.QUser.Core.Internal.Native;
using MG.QUser.Core.Internal.Structs;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MG.QUser.Core;

#nullable enable

public static partial class QUserHelper
{
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

        WtsSessionInfo[] sessionList = [];

        using WtsSessionSafeHandle session = WtsSessionSafeHandle.OpenConnection(computerName);
        using WtsSessionArraySafeHandle sessionArray = WTSApi32.WTSEnumerateSessions(session);

        if (!sessionArray.Result)
        {
            return sessionList;
        }

        sessionList = new WtsSessionInfo[sessionArray.Count];
        for (int i = 0; sessionArray.TryReadNext(session, out WtsSessionInfo? sessionInfo); i++)
        {
            sessionList[i] = sessionInfo;
        }

        return sessionList;
    }

    //private static void AddSessionInfo(WtsSessionInfo[] sessionList, WtsSessionSafeHandle handle, ref int index, ref long current, ref long dataSize)
    //{
    //    // Marshal the current pointer to a WTS_SESSION_INFO struct.
    //    WTS_SESSION_INFO sessionInfo = MarshalHelper.PtrToStruct<WTS_SESSION_INFO>((IntPtr)current);

    //    IntPtr buffer = IntPtr.Zero;
    //    try
    //    {
    //        bool success = WTSApi32.WTSQuerySessionInformation(
    //            handle,
    //            sessionInfo.SessionId,
    //            WtsInfoClass.WTSSessionInfo,
    //            out buffer,
    //            out int bytesReturned
    //        );

    //        if (!success || IntPtr.Zero == buffer || bytesReturned < Marshal.SizeOf(typeof(WTSINFO)))
    //        {
    //            return;
    //        }

    //        WTSINFO wtsInfo = MarshalHelper.PtrToStruct<WTSINFO>(buffer);
    //        string userName = QuerySessionInfoString(ref sessionInfo.SessionId, handle, WtsInfoClass.WTSUserName);
    //        string domainName = QuerySessionInfoString(ref sessionInfo.SessionId, handle, WtsInfoClass.WTSDomainName);
    //        string clientName = QuerySessionInfoString(ref sessionInfo.SessionId, handle, WtsInfoClass.WTSClientName);

    //        ref long currentTime = ref wtsInfo.CurrentTime;

    //        sessionList[index] = new WtsSessionInfo
    //        {
    //            ClientName = clientName,
    //            ComputerName = handle.ComputerName,
    //            DomainName = domainName,
    //            IdleTime = wtsInfo.LastInputTime > 0
    //                ? DateTime.FromFileTimeUtc(currentTime) - DateTime.FromFileTimeUtc(wtsInfo.LastInputTime)
    //                : null,
    //            LogonTime = wtsInfo.LogonTime > 0
    //                ? DateTime.FromFileTimeUtc(wtsInfo.LogonTime).ToLocalTime()
    //                : null,
    //            SessionId = sessionInfo.SessionId,
    //            State = sessionInfo.State,
    //            UserName = userName,
    //            WinStationName = sessionInfo.pWinStationName,
    //        };
    //    }
    //    finally
    //    {
    //        // Move the pointer to the next structure
    //        current += dataSize;

    //        if (buffer != IntPtr.Zero)
    //        {
    //            WTSApi32.WTSFreeMemory(buffer);
    //        }
    //    }
    //}
}
