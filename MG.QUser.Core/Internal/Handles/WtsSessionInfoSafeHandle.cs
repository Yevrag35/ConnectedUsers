using MG.QUser.Core.Internal.Structs;
using System;
using System.Runtime.InteropServices;

namespace MG.QUser.Core.Internal.Handles;

internal sealed class WtsSessionInfoSafeHandle : SafeHandle
{
    internal WtsSessionInfoSafeHandle() : base(IntPtr.Zero, ownsHandle: true)
    {
    }

    public override bool IsInvalid => IntPtr.Zero == handle;

    /// <summary>
    /// Reads the session information from the handle into a <see cref="WTS_SESSION_INFO"/> structure.
    /// </summary>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ObjectDisposedException"></exception>
    internal WTS_SESSION_INFO GetSessionInfo()
    {
        if (this.IsInvalid)
        {
            throw new ObjectDisposedException(nameof(WtsSessionInfoSafeHandle));
        }

        return MarshalHelper.PtrToStruct<WTS_SESSION_INFO>(handle);
    }

    protected override bool ReleaseHandle()
    {
        if (!this.IsInvalid)
        {

        }

        handle = IntPtr.Zero;
        return true;
    }
}