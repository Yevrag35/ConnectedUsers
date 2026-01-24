using MG.QUser.Core.Internal.Native;
using MG.QUser.Core.Internal.Structs;
using System;
using System.Runtime.InteropServices;

namespace MG.QUser.Core.Internal.Handles;

internal sealed class WtsInfoSafeHandle : WtsSafeHandle
{
    private static readonly int s_WTSINFO_SIZE = Marshal.SizeOf(typeof(WTSINFO));

    internal bool Result { get; private set; }

    private WtsInfoSafeHandle(bool result) : base()
    {
        this.Result = result;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="WtsInfoSafeHandle"/> class based on the specified native handle and result
    /// information.
    /// </summary>
    /// <remarks>This method combines the provided result, handle, and byte count to determine whether the
    /// resulting <see cref="WtsInfoSafeHandle"/> should be marked as valid. Callers should ensure that the parameters accurately
    /// reflect the outcome of the native operation to avoid wrapping an invalid handle.</remarks>
    /// <param name="handle">The native handle to be wrapped by the WtsInfoSafeHandle. Must not be zero for the handle to be considered
    /// valid.</param>
    /// <param name="result">A value indicating whether the handle is valid prior to additional checks. This value is further evaluated with
    /// other parameters to determine final validity.</param>
    /// <param name="pBytesReturned">The number of bytes returned by the native operation. Must be greater than or equal to the required WTSINFO
    /// structure size for the handle to be considered valid.</param>
    /// <returns>A <see cref="WtsInfoSafeHandle"/> instance representing the specified handle. The handle is considered valid only if all input
    /// conditions are met; otherwise, the returned handle is invalid.</returns>
    internal static WtsInfoSafeHandle Create(nint handle, bool result, int pBytesReturned)
    {
        result = result && ZERO != handle && pBytesReturned >= s_WTSINFO_SIZE;
        WtsInfoSafeHandle safeHandle = new(result);
        safeHandle.SetHandle(handle);
        return safeHandle;
    }

    /// <summary>
    /// Reads the session information from the handle into a <see cref="WTSINFO"/> structure.
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
    protected private override void ReleaseHandle(nint handle)
    {
        WTSApi32.WTSFreeMemory(handle);
    }
}