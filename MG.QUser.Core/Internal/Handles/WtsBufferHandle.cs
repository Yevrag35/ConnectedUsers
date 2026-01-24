using MG.QUser.Core.Internal.Native;
using System;
using System.Runtime.InteropServices;

namespace MG.QUser.Core.Internal.Handles;

internal sealed class WtsBufferHandle : WtsSafeHandle
{
    internal int BytesReturned { get; private set; }
    internal bool Result { get; private set; }

    private WtsBufferHandle(bool result, int pBytesReturned) : base()
    {
        this.Result = result;
        this.BytesReturned = pBytesReturned;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="WtsBufferHandle"/> class using the specified native handle, result status, and byte
    /// count.
    /// </summary>
    /// <param name="handle">The native handle to associate with the <see cref="WtsBufferHandle"/> instance.</param>
    /// <param name="result">A value indicating whether the handle is valid. Set to <see langword="true"/> if the handle is valid; otherwise,
    /// <see langword="false"/>.</param>
    /// <param name="pBytesReturned">The number of bytes returned by the operation associated with the handle.</param>
    /// <returns>A <see cref="WtsBufferHandle"/> instance initialized with the specified handle, result status, and byte count.</returns>
    internal static WtsBufferHandle Create(nint handle, bool result, int pBytesReturned)
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
    protected private override void ReleaseHandle(nint handle)
    {
        WTSApi32.WTSFreeMemory(handle);
    }
}