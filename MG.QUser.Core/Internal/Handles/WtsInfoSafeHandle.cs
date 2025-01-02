using MG.QUser.Core.Internal.Native;
using MG.QUser.Core.Internal.Structs;
using System;
using System.Runtime.InteropServices;

namespace MG.QUser.Core.Internal.Handles;

internal sealed class WtsInfoSafeHandle : SafeHandle
{
    private static readonly int WTSINFO_SIZE = Marshal.SizeOf(typeof(WTSINFO));

    internal bool Result { get; private set; }

    internal WtsInfoSafeHandle(ref bool result) : base(IntPtr.Zero, ownsHandle: true)
    {
        this.Result = result;
    }

    public override bool IsInvalid => IntPtr.Zero == handle;

    internal static WtsInfoSafeHandle Create(ref IntPtr handle, ref bool result, ref int pBytesReturned)
    {
        result = result && IntPtr.Zero != handle && pBytesReturned >= WTSINFO_SIZE;
        WtsInfoSafeHandle safeHandle = new(ref result);
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

    protected override bool ReleaseHandle()
    {
        if (!this.IsInvalid)
        {
            WTSApi32.WTSFreeMemory(handle);
        }

        handle = IntPtr.Zero;
        this.Result = false;
        return true;
    }
}