using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace MG.QUser.Core.Internal.Structs;

#nullable enable

[StructLayout(LayoutKind.Auto)]
internal ref struct WtsSessionHandle
{
    private string? _computerName;
    private IntPtr _handle;

    public readonly string ComputerName => _computerName ?? Environment.MachineName;
    public readonly IntPtr Handle => _handle;
    private WtsSessionHandle(IntPtr handle, string? computerName)
    {
        _computerName = (computerName ?? Environment.MachineName).ToUpperInvariant();
        _handle = handle;
    }
    public void Dispose()
    {
        IntPtr handle = _handle;
        this = default;

        if (MemoryHelper.WTS_CURRENT_SERVER_HANDLE != handle)
        {
            WTSCloseServer(handle);
        }
    }
    private static bool IsLocalHost([NotNullWhen(false)] string? computerName)
    {
        return string.IsNullOrWhiteSpace(computerName)
            || computerName!.Equals(Environment.MachineName, StringComparison.OrdinalIgnoreCase)
            || computerName.Equals("localhost", StringComparison.OrdinalIgnoreCase)
            || computerName.Equals(".", StringComparison.Ordinal);
    }
    public static WtsSessionHandle OpenConnection(string? computerName)
    {
        if (IsLocalHost(computerName))
        {
            return new WtsSessionHandle(MemoryHelper.WTS_CURRENT_SERVER_HANDLE, Environment.MachineName);
        }

        IntPtr serverHandle = WTSOpenServer(computerName);
        return new WtsSessionHandle(serverHandle, computerName);
    }

    public static implicit operator IntPtr(WtsSessionHandle handle) => handle.Handle;

    // WTSOpenServer opens a handle to a remote (or local) server
    [DllImport("wtsapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr WTSOpenServer(string pServerName);

    // WTSCloseServer closes that handle
    [DllImport("wtsapi32.dll", SetLastError = true)]
    private static extern void WTSCloseServer(IntPtr hServer);
}
