using MG.QUser.Core.Internal.Native;
using System;
using System.Runtime.InteropServices;

namespace MG.QUser.Core.Internal.Handles;

internal sealed class WtsBufferHandle : SafeHandle
{
    public override bool IsInvalid => IntPtr.Zero == handle;
    internal int BytesReturned { get; private set; }
    internal bool Result { get; private set; }

    internal WtsBufferHandle(ref bool result, ref int pBytesReturned) : base(IntPtr.Zero, ownsHandle: true)
    {
        this.Result = result;
        this.BytesReturned = pBytesReturned;
    }

    internal static WtsBufferHandle Create(ref IntPtr handle, ref bool result, ref int pBytesReturned)
    {
        WtsBufferHandle safeHandle = new(ref result, ref pBytesReturned);
        safeHandle.SetHandle(handle);
        return safeHandle;
    }
    /// <summary>
    /// Reads the buffer handle as a <see cref="string"/> using the ANSI encoding.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ObjectDisposedException"></exception>
    internal string ReadStringAnsi()
    {
        if (this.IsInvalid)
        {
            throw new ObjectDisposedException(nameof(WtsBufferHandle));
        }

        return this.Result && this.BytesReturned > 1
            ? Marshal.PtrToStringAnsi(handle)
            : string.Empty;
    }

    protected override bool ReleaseHandle()
    {
        if (!this.IsInvalid)
        {
            WTSApi32.WTSFreeMemory(handle);
        }

        handle = IntPtr.Zero;
        return true;
    }
}