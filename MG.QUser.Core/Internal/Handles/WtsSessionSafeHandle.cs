using MG.QUser.Core.Internal.Native;
using System;
using System.Diagnostics.CodeAnalysis;

namespace MG.QUser.Core.Internal.Handles;

internal sealed class WtsSessionSafeHandle : WtsSafeHandle
{
    internal string ComputerName { get; }

    private WtsSessionSafeHandle(string computerName)
    {
        this.ComputerName = computerName;
    }

    /// <summary>
    /// Opens a connection to the given <paramref name="computerName"/>, returning a <see cref="WtsSessionSafeHandle"/>.
    /// If no name is specified or if the name is considered "local," no new handle is actually opened.
    /// </summary>
    /// <param name="computerName">The remote computer's name, or <see langword="null"/> for the local machine.</param>
    /// <returns>A <see cref="WtsSessionSafeHandle"/> wrapping the server handle.</returns>
    internal static WtsSessionSafeHandle OpenConnection(string? computerName)
    {
        // Decide if local or remote
        string machineName = (computerName ?? ComputerNameHelper.ComputerName).ToUpperInvariant();
        nint rawHandle = IsLocalHost(machineName)
            ? MemoryHelper.WTS_CURRENT_SERVER_HANDLE
            : WTSApi32.WTSOpenServer(machineName);

        // Construct and set
        WtsSessionSafeHandle safeHandle = new(machineName);
        safeHandle.SetHandle(rawHandle);

        return safeHandle;
    }

    private static bool IsLocalHost([NotNullWhen(false)] string? computerName)
    {
        return string.IsNullOrWhiteSpace(computerName)
            || computerName!.Equals(".", StringComparison.Ordinal)
            || computerName.Equals("LOCALHOST", StringComparison.Ordinal)
            || computerName.Equals(ComputerNameHelper.ComputerName, StringComparison.OrdinalIgnoreCase);
    }
    private protected override void ReleaseHandle(nint handle)
    {
        // Ensure we only close valid, non-local handles.
        if (handle != MemoryHelper.WTS_CURRENT_SERVER_HANDLE)
        {
            WTSApi32.WTSCloseServer(handle);
        }
    }
}
