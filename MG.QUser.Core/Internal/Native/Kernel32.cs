#if NETSTANDARD1_1

using System.Runtime.InteropServices;

namespace MG.QUser.Core.Internal.Native;

internal static class Kernel32
{
    [DllImport("Kernel32.dll", CharSet = CharSet.Unicode)]
    internal static extern unsafe bool GetComputerName(char* lpBuffer, uint* nSize);
}

#endif