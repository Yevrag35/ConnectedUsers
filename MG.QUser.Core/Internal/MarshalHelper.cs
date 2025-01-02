using System;
using System.Runtime.InteropServices;

namespace MG.QUser.Core.Internal;

internal static class MarshalHelper
{
    internal static T PtrToStruct<T>(IntPtr pointer) where T : struct
    {
#if !NETSTANDARD1_1
        return Marshal.PtrToStructure<T>(pointer);
#else
        return (T)Marshal.PtrToStructure(pointer, typeof(T));
#endif
    }
}
