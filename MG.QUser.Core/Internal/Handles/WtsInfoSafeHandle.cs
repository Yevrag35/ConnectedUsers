using MG.QUser.Core.Internal.Native;
using MG.QUser.Core.Internal.Structs;
using System;
using System.Runtime.InteropServices;

namespace MG.QUser.Core.Internal.Handles;

internal sealed class WtsInfoSafeHandle : WtsSafeHandle
{
    private static readonly int WTSINFO_SIZE = Marshal.SizeOf(typeof(WTSINFO));

    internal bool Result { get; private set; }

    internal WtsInfoSafeHandle(bool result) : base()
    {
        this.Result = result;
    }

    internal static WtsInfoSafeHandle Create(IntPtr handle, bool result, int pBytesReturned)
    {
        result = result && IntPtr.Zero != handle && pBytesReturned >= WTSINFO_SIZE;
        WtsInfoSafeHandle safeHandle = new(result);
        safeHandle.SetHandle(handle);
        return safeHandle;
    }

    /// <summary>
    /// Reads the session information from the handle into a <see cref="WTS_SESSION_INFO"/> structure.
    /// </summary>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ObjectDisposedException"></exception>
    public WTSINFO GetInfo()
    {
        if (this.IsInvalid)
        {
            throw new ObjectDisposedException(nameof(WtsInfoSafeHandle));
        }

        return MarshalHelper.PtrToStruct<WTSINFO>(handle);
    }
    protected private override void OnHandleReleased()
    {
        this.Result = false;
    }
    protected private override void ReleaseHandle(IntPtr handle)
    {
        WTSApi32.WTSFreeMemory(handle);
    }
}