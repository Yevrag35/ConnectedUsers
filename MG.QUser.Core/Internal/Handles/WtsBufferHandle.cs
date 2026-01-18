using MG.QUser.Core.Internal.Native;
using System;
using System.Runtime.InteropServices;

namespace MG.QUser.Core.Internal.Handles;

internal sealed class WtsBufferHandle : WtsSafeHandle
{
    internal int BytesReturned { get; private set; }
    internal bool Result { get; private set; }

    internal WtsBufferHandle(bool result, int pBytesReturned) : base()
    {
        this.Result = result;
        this.BytesReturned = pBytesReturned;
    }

    internal static WtsBufferHandle Create(IntPtr handle, bool result, int pBytesReturned)
    {
        WtsBufferHandle safeHandle = new(result, pBytesReturned);
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
    protected private override void ReleaseHandle(IntPtr handle)
    {
        WTSApi32.WTSFreeMemory(handle);
    }
}