using MG.QUser.Core.Internal;
using MG.QUser.Core.Internal.Handles;
using MG.QUser.Core.Internal.Native;
using MG.QUser.Core.Internal.Structs;
using System;
using System.Runtime.InteropServices;

namespace MG.QUser.Core;

#nullable enable

public static partial class QUserHelper
{
    private delegate bool BytesReturnPredicate(ref int bytesReturned);

    #region Private Helpers

    //private static bool CallQuerySessionInfo(ref IntPtr buffer, WtsSessionSafeHandle sessionHandle, ref int sessionId, ref WtsInfoClass infoClass,
    //    BytesReturnPredicate predicate)
    //{
    //    bool success = WTSApi32.WTSQuerySessionInformation(
    //        sessionHandle,
    //        sessionId,
    //        infoClass,
    //        out buffer,
    //        out int bytesReturned
    //    );

    //    return success && IntPtr.Zero != buffer && predicate(ref bytesReturned);
    //}

    ///// <summary>
    ///// Retrieves a string value (e.g., username) from WTSQuerySessionInformation.
    ///// </summary>
    ///// <remarks>
    ///// This can be extended for domain, client name, etc.
    ///// </remarks>
    //private static string QuerySessionInfoString(ref int sessionId, SafeWtsSessionHandle sessionHandle, WtsInfoClass infoClass)
    //{
    //    IntPtr buffer = IntPtr.Zero;

    //    try
    //    {
    //        return CallQuerySessionInfo(ref buffer, sessionHandle, ref sessionId, ref infoClass, (ref int bytesReturned) => bytesReturned > 1)
    //            ? Marshal.PtrToStringAnsi(buffer) ?? string.Empty
    //            : string.Empty;
    //    }
    //    finally
    //    {
    //        if (buffer != IntPtr.Zero)
    //        {
    //            WTSApi32.WTSFreeMemory(buffer);
    //        }
    //    }
    //}

    ///// <summary>
    ///// Retrieves an integer value (e.g., idle time in seconds) from WTSQuerySessionInformation.
    ///// </summary>
    //private static int QuerySessionInfoInt(ref int sessionId, SafeWtsSessionHandle sessionHandle, WtsInfoClass infoClass)
    //{
    //    IntPtr buffer = IntPtr.Zero;

    //    try
    //    {
    //        return CallQuerySessionInfo(ref buffer, sessionHandle, ref sessionId, ref infoClass, (ref int bytesReturned) => bytesReturned >= sizeof(int))
    //            ? Marshal.ReadInt32(buffer)
    //            : 0;
    //    }
    //    finally
    //    {
    //        if (buffer != IntPtr.Zero)
    //        {
    //            WTSApi32.WTSFreeMemory(buffer);
    //        }
    //    }
    //}

    ///// <summary>
    ///// Retrieves a long value (often used for FILETIME) from WTSQuerySessionInformation.
    ///// </summary>
    //private static long QuerySessionInfoLong(ref int sessionId, SafeWtsSessionHandle sessionHandle, WtsInfoClass infoClass)
    //{
    //    IntPtr buffer = IntPtr.Zero;

    //    try
    //    {
    //        return CallQuerySessionInfo(ref buffer, sessionHandle, ref sessionId, ref infoClass, (ref int bytesReturned) => bytesReturned >= sizeof(long))
    //            ? Marshal.ReadInt64(buffer)
    //            : 0;
    //    }
    //    finally
    //    {
    //        if (buffer != IntPtr.Zero)
    //        {
    //            WTSApi32.WTSFreeMemory(buffer);
    //        }
    //    }
    //}

    #endregion
}