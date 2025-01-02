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

    public readonly string ComputerName => _computerName ?? GetEnvironmentMachineName();
    public readonly IntPtr Handle => _handle;
    private WtsSessionHandle(IntPtr handle, string? computerName)
    {
        _computerName = (computerName ?? GetEnvironmentMachineName()).ToUpperInvariant();
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

    private static string? _machineName;
    private static string GetEnvironmentMachineName()
    {
#if !NETSTANDARD1_1
        return _machineName ??= Environment.MachineName;
    }
#else
        if (string.IsNullOrEmpty(_machineName))
        {
            uint uLength = MAX_COMPUTERNAME_LENGTH;
            unsafe
            {
                char* buffer = stackalloc char[(int)uLength];

                _machineName = GetComputerName(buffer, &uLength)
                    ? new string(buffer, 0, (int)uLength)
                    : string.Empty;
            }
        }

        return _machineName!;
    }
    
    private const uint MAX_COMPUTERNAME_LENGTH = 15;

    [DllImport("Kernel32.dll", CharSet = CharSet.Unicode)]
    private static extern unsafe bool GetComputerName(char* lpBuffer, uint* nSize);
#endif

    private static bool IsLocalHost([NotNullWhen(false)] string? computerName)
    {
        return string.IsNullOrWhiteSpace(computerName)
            || computerName!.Equals(GetEnvironmentMachineName(), StringComparison.OrdinalIgnoreCase)
            || computerName.Equals("localhost", StringComparison.OrdinalIgnoreCase)
            || computerName.Equals(".", StringComparison.Ordinal);
    }
    public static WtsSessionHandle OpenConnection(string? computerName)
    {
        if (IsLocalHost(computerName))
        {
            return new WtsSessionHandle(MemoryHelper.WTS_CURRENT_SERVER_HANDLE, GetEnvironmentMachineName());
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
