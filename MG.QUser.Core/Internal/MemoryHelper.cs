using System;
using System.Runtime.InteropServices;

namespace MG.QUser.Core.Internal;

internal static class MemoryHelper
{
    // We can use a special handle to indicate the current server
    internal static readonly IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;

    // P/Invoke for WTSFreeMemory
    [DllImport("wtsapi32.dll", SetLastError = true)]
    internal static extern void WTSFreeMemory(IntPtr pMemory);
}
