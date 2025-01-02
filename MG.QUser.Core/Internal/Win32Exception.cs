#if !NETSTANDARD1_1
using System.ComponentModel;
using System.Runtime.CompilerServices;

[assembly: TypeForwardedTo(typeof(Win32Exception))]

#else

using System;

namespace System.ComponentModel;

/// <summary>
/// Throws an exception for a Win32 error code.
/// </summary>
public class Win32Exception : Exception
{
    private readonly int _nativeErrorCode;

    /// <summary>
    /// Gets the Win32 error code associated with this exception.
    /// </summary>
    public int NativeErrorCode => _nativeErrorCode;

    public Win32Exception(int errorCode)
        : base("Win32 Error Code: " + errorCode)
    {
        _nativeErrorCode = errorCode;
    }
}

#endif
