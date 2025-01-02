using MG.QUser.Core.Internal.Native;
using System;
using System.Collections.Generic;
using System.Text;

namespace MG.QUser.Core.Internal;

internal static class ComputerNameHelper
{
    private static string? _machineName;

    internal static string ComputerName => _machineName ??= GetEnvironmentMachineName();

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

                _machineName = Kernel32.GetComputerName(buffer, &uLength)
                    ? new string(buffer, 0, (int)uLength)
                    : string.Empty;
            }
        }

        return _machineName!;
    }

    private const uint MAX_COMPUTERNAME_LENGTH = 15;
#endif
}
