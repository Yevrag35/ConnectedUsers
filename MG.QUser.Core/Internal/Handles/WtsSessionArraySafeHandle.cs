using MG.QUser.Core.Internal.Native;
using MG.QUser.Core.Internal.Structs;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace MG.QUser.Core.Internal.Handles;

internal sealed class WtsSessionArraySafeHandle : SafeHandle
{
    internal static readonly long SessionInfoSize = Marshal.SizeOf(typeof(WTS_SESSION_INFO));

    private int _count;
    private long _current;
    private int _index;

    internal int Count => _count;
    public override bool IsInvalid => IntPtr.Zero == handle;
    internal bool Result { get; private set; }

    private WtsSessionArraySafeHandle(ref int arrayLength, ref bool result)
        : base(IntPtr.Zero, true)
    {
        _count = arrayLength;
        this.Result = result;
        _index = -1;
    }

    internal static WtsSessionArraySafeHandle Create(ref IntPtr ppSessionInfo, ref int count, ref bool result)
    {
        WtsSessionArraySafeHandle handle = new(ref count, ref result);
        handle.SetHandle(ppSessionInfo);
        handle.SetCurrentValue(ref ppSessionInfo);

        return handle;
    }

    protected override bool ReleaseHandle()
    {
        if (!this.IsInvalid)
        {
            WTSApi32.WTSFreeMemory(handle);
        }

        handle = IntPtr.Zero;
        _current = 0;
        return true;
    }
    private void SetCurrentValue(ref IntPtr pointer)
    {
        _current = pointer.ToInt64();
    }

    internal bool TryReadNext(WtsSessionSafeHandle sessionHandle, [NotNullWhen(true)] out WtsSessionInfo? sessionInfo)
    {
        int index = _index + 1;
        if ((uint)index >= (uint)_count)
        {
            _index = this.Count;
            sessionInfo = null;
            return false;
        }

        _index = index;

        long current = _current;
        _current += SessionInfoSize;
        WTS_SESSION_INFO wtsSessionInfo = ReadNext(ref current);
        ref int sessionId = ref wtsSessionInfo.SessionId;

        using WtsInfoSafeHandle infoHandle = WTSApi32.WTSQuerySessionInformation(ref sessionId, sessionHandle);

        if (!infoHandle.Result)
        {
            sessionInfo = null;
            return false;
        }

        WTSINFO wtsInfo = infoHandle.GetInfo();
        sessionInfo = WtsSessionInfo.Create(sessionHandle.ComputerName, ref wtsSessionInfo, ref wtsInfo);

        SetDomainName(sessionHandle, sessionInfo, ref sessionId);
        SetClientName(sessionHandle, sessionInfo, ref sessionId);
        SetUserName(sessionHandle, sessionInfo, ref sessionId);

        return true;
    }

    private static WTS_SESSION_INFO ReadNext(ref long current)
    {
        return MarshalHelper.PtrToStruct<WTS_SESSION_INFO>((IntPtr)current);
    }
    private static void SetClientName(WtsSessionSafeHandle sessionHandle, WtsSessionInfo sessionInfo, ref int sessionId)
    {
        using (WtsBufferHandle clientNameBuffer = WTSApi32.WTSQuerySessionInformation(ref sessionId, sessionHandle, WtsInfoClass.WTSClientName))
        {
            sessionInfo.ClientName = clientNameBuffer.ReadStringAnsi();
        }
    }
    private static void SetDomainName(WtsSessionSafeHandle sessionHandle, WtsSessionInfo sessionInfo, ref int sessionId)
    {
        using (WtsBufferHandle domainNameBuffer = WTSApi32.WTSQuerySessionInformation(ref sessionId, sessionHandle, WtsInfoClass.WTSDomainName))
        {
            sessionInfo.DomainName = domainNameBuffer.ReadStringAnsi();
        }
    }
    private static void SetUserName(WtsSessionSafeHandle sessionHandle, WtsSessionInfo sessionInfo, ref int sessionId)
    {
        using (WtsBufferHandle userNameBuffer = WTSApi32.WTSQuerySessionInformation(ref sessionId, sessionHandle, WtsInfoClass.WTSUserName))
        {
            sessionInfo.UserName = userNameBuffer.ReadStringAnsi();
        }
    }

    protected override void Dispose(bool disposing)
    {
        _count = 0;
        _index = 0;
        _current = 0;
        base.Dispose(disposing);
    }
}