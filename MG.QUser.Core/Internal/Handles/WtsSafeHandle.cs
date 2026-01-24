using System;
using System.Runtime.InteropServices;

namespace MG.QUser.Core.Internal.Handles;

/// <summary>
/// Provides a base class for safe handle implementations that manage Windows Terminal Services (WTS) native handles.
/// </summary>
/// <remarks>This class derives from <see cref="SafeHandle"/> and ensures that native WTS handles are released
/// reliably and safely. Derived classes must implement the <see cref="ReleaseHandle(nint)"/> method to specify how the
/// native handle should be released. The handle is considered invalid when it is zero. This class is intended for use
/// with interop scenarios involving WTS APIs.</remarks>
public abstract class WtsSafeHandle : SafeHandle
{
    /// <summary>
    /// Represents a platform-specific zero pointer value.
    /// </summary>
    /// <remarks>This constant can be used when a native integer zero value is required, such as for pointer
    /// initialization or interop scenarios.</remarks>
    protected const nint ZERO = 0;

    /// <summary>
    /// Gets a value indicating whether the handle is invalid.
    /// </summary>
    /// <remarks>An invalid handle typically indicates that the resource has not been initialized or has been
    /// released. Use this property to check the validity of the handle before performing operations that require a
    /// valid handle.</remarks>
    public sealed override bool IsInvalid => ZERO == handle;

    /// <summary>
    /// Initializes a new instance of the <see cref="WtsSafeHandle"/> class with a handle value of zero and ownership of the handle.
    /// </summary>
    /// <remarks>This constructor is intended for use by derived classes or within the assembly to create a
    /// safe handle that represents an invalid or uninitialized handle. The handle is considered owned and will be
    /// released when the object is disposed or finalized.</remarks>
    protected private WtsSafeHandle() : base(ZERO, ownsHandle: true)
    {
    }

    /// <summary>
    /// Called when the underlying handle is released. Override this method to perform custom cleanup or resource
    /// release logic when the handle is no longer needed.
    /// </summary>
    /// <remarks>This method is invoked as part of the handle release process. Derived classes can override
    /// this method to implement additional actions that should occur when the handle is released. The base
    /// implementation does nothing.</remarks>
    protected private virtual void OnHandleReleased() { }
    /// <summary>
    /// Releases the handle associated with the current instance.
    /// </summary>
    /// <returns>true in all cases.</returns>
    protected sealed override bool ReleaseHandle()
    {
        if (!this.IsInvalid)
        {
            this.ReleaseHandle(handle);
        }

        handle = ZERO;
        this.OnHandleReleased();
        return true;
    }
    /// <summary>
    /// Releases the unmanaged resource associated with the specified handle.
    /// </summary>
    /// <remarks>Override this method to implement custom logic for releasing unmanaged resources. This method
    /// is typically called during cleanup or disposal operations. After this method is called, the handle should not be
    /// used again.</remarks>
    /// <param name="handle">The handle to the unmanaged resource to release. This value must be valid and must not have been previously
    /// released.</param>
    protected private abstract void ReleaseHandle(nint handle);
}