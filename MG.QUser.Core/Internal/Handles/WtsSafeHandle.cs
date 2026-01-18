using System;
using System.Runtime.InteropServices;

namespace MG.QUser.Core.Internal.Handles;

public abstract class WtsSafeHandle : SafeHandle
{
    public sealed override bool IsInvalid => IntPtr.Zero == handle;

    protected private WtsSafeHandle() : base(IntPtr.Zero, ownsHandle: true)
    {
    }

    protected private virtual void OnHandleReleased() { }
    protected sealed override bool ReleaseHandle()
    {
        if (!this.IsInvalid)
        {
            this.ReleaseHandle(handle);
        }

        handle = IntPtr.Zero;
        this.OnHandleReleased();
        return true;
    }
    protected private abstract void ReleaseHandle(IntPtr handle);
}