using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MG.QUser.Core.Internal.Structs;

/// <summary>
/// WTSINFO (ANSI version) structure definition, as documented at:
/// https://learn.microsoft.com/windows/win32/api/wtsapi32/ns-wtsapi32-wtsinfoa
/// 
/// If you want the Unicode (WTSINFOW), adjust CharSet and string field sizes accordingly.
/// </summary>
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
internal struct WTSINFOW
{
    public WtsConnectState State;
    public int SessionId;
    public int IncomingBytes;
    public int OutgoingBytes;
    public int IncomingFrames;
    public int OutgoingFrames;
    public int IncomingCompressedBytes;
    public int OutgoingCompressedBytes;

    // According to docs, these are null-terminated ANSI strings of fixed length.
    // You can adjust size as documented in wtsapi32.h:
    //   WinStationName: 32 chars
    //   Domain: 17 chars
    //   UserName: 21 chars
    // If you see unexpected results, verify you have the correct size.
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    public string WinStationName;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 17)]
    public string Domain;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
    public string UserName;

    // These LARGE_INTEGER fields are actually 64-bit file times (100-ns intervals since 1601).
    public long ConnectTime;
    public long DisconnectTime;
    public long LastInputTime;
    public long LogonTime;
    public long CurrentTime;
}
