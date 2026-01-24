using System;
using System.Runtime.InteropServices;

namespace MG.QUser.Core.Internal;

internal static class MemoryHelper
{
    // We can use a special handle to indicate the current server
    internal const nint WTS_CURRENT_SERVER_HANDLE = 0;
}
