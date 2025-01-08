using MG.QUser.Core.Internal.Native;
using System;
using System.Runtime.InteropServices;

namespace MG.QUser.Core.Internal.Handles;

internal sealed class WtsBufferHandle : WtsSafeHandle
{
    internal int BytesReturned { get; private set; }
    internal bool Result { get; private set; }

    internal WtsBufferHandle(ref bool result, ref int pBytesReturned) : base()
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
    protected private override void OnHandleReleased()
    {
        this.BytesReturned = 0;
        this.Result = false;
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
    protected private override void ReleaseHandle(ref IntPtr handle)
    {
        WTSApi32.WTSFreeMemory(handle);
    }
}