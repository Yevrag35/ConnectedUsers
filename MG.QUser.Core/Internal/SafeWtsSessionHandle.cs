using MG.QUser.Core.Internal.Native;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace MG.QUser.Core.Internal;

internal sealed class SafeWtsSessionHandle : SafeHandle
{
    internal string ComputerName { get; }
    public override bool IsInvalid => IntPtr.Zero == handle;

    private SafeWtsSessionHandle(string computerName)
        : base(IntPtr.Zero, true)
    {
        this.ComputerName = computerName;
    }

    /// <summary>
    /// Opens a connection to the given <paramref name="computerName"/>, returning a <see cref="SafeWtsSessionHandle"/>.
    /// If no name is specified or if the name is considered "local," no new handle is actually opened.
    /// </summary>
    /// <param name="computerName">The remote computer's name, or <see langword="null"/> for the local machine.</param>
    /// <returns>A <see cref="SafeWtsSessionHandle"/> wrapping the server handle.</returns>
    internal static SafeWtsSessionHandle OpenConnection(string? computerName)
    {
        // Decide if local or remote
        string machineName = (computerName ?? ComputerNameHelper.ComputerName).ToUpperInvariant();
        IntPtr rawHandle = IsLocalHost(machineName)
            ? MemoryHelper.WTS_CURRENT_SERVER_HANDLE
            : WTSApi32.WTSOpenServer(machineName);

        // Construct and set
        var safeHandle = new SafeWtsSessionHandle(machineName);
        safeHandle.SetHandle(rawHandle);

        return safeHandle;
    }

    private static bool IsLocalHost([NotNullWhen(false)] string? computerName)
    {
        return string.IsNullOrWhiteSpace(computerName)
            || computerName!.Equals(ComputerNameHelper.ComputerName, StringComparison.OrdinalIgnoreCase)
            || computerName.Equals("localhost", StringComparison.OrdinalIgnoreCase)
            || computerName.Equals(".", StringComparison.Ordinal);
    }
    protected override bool ReleaseHandle()
    {
        // Ensure we only close valid, non-local handles.
        if (!this.IsInvalid && handle != MemoryHelper.WTS_CURRENT_SERVER_HANDLE)
        {
            WTSApi32.WTSCloseServer(handle);
        }

        // Mark handle as closed/invalid
        handle = IntPtr.Zero;
        return true;
    }
}
