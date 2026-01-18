using MG.QUser.Core.Internal.Native;
using MG.QUser.Core.Internal.Structs;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace MG.QUser.Core.Internal.Handles;

/// <summary>
/// Represent an owning handle for an array of session information structures returned by the platform.
/// </summary>
/// <remarks>
/// <para>Instances own unmanaged memory that must be released exactly once.</para>
/// <para><b>Thread-safety:</b> This type is not thread-safe; enumeration state is stored on the instance.</para>
/// </remarks>
internal sealed class WtsSessionArraySafeHandle : SafeHandle
{
    /// <summary>
    /// Get the size, in bytes, of a single session information structure in unmanaged memory.
    /// </summary>
    /// <remarks>
    /// <para>This value is computed using <see cref="Marshal.SizeOf(System.Type)"/> for the underlying structure.</para>
    /// </remarks>
    /// <returns>The size, in bytes, of one element in the unmanaged array.</returns>
    internal static readonly long SessionInfoSize = Marshal.SizeOf(typeof(WTS_SESSION_INFO));

    private int _count;
    private long _current;
    private int _index;

    /// <summary>
    /// Get the number of elements available in the unmanaged array.
    /// </summary>
    /// <returns>The element count.</returns>
    /// <value>The number of session entries in the unmanaged buffer.</value>
    internal int Count => _count;

    /// <summary>
    /// Get a value indicating whether this handle is invalid.
    /// </summary>
    /// <remarks>
    /// <para>The handle is considered invalid when it is equal to <see cref="IntPtr.Zero"/>.</para>
    /// </remarks>
    /// <returns><see langword="true"/> when the handle is invalid; otherwise, <see langword="false"/>.</returns>
    public override bool IsInvalid => IntPtr.Zero == handle;

    /// <summary>
    /// Get a value indicating whether the underlying native operation that produced this handle reported success.
    /// </summary>
    /// <returns><see langword="true"/> when the native operation succeeded; otherwise, <see langword="false"/>.</returns>
    /// <value><see langword="true"/> when creation succeeded; otherwise, <see langword="false"/>.</value>
    internal bool Result { get; private set; }

    private WtsSessionArraySafeHandle(int arrayLength, bool result)
        : base(IntPtr.Zero, true)
    {
        _count = arrayLength;
        this.Result = result;
        _index = -1;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="WtsSessionArraySafeHandle"/> class using the specified session 
    /// information pointer session count, and result status.
    /// </summary>
    /// <param name="pSessionInfo">A pointer to the session information structure to be associated with the handle. Must not be null.</param>
    /// <param name="count">The number of session entries referenced by the session information pointer. Must be zero or greater.</param>
    /// <param name="result">A value indicating whether the session information retrieval was successful. Set to <see langword="true"/> if
    /// successful; otherwise, <see langword="false"/>.</param>
    /// <returns>A <see cref="WtsSessionArraySafeHandle"/> that encapsulates the specified session information.</returns>
    internal static WtsSessionArraySafeHandle Create(IntPtr pSessionInfo, int count, bool result)
    {
        WtsSessionArraySafeHandle handle = new(count, result);
        handle.SetHandle(pSessionInfo);
        handle.SetCurrentValue(pSessionInfo);

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
    private void SetCurrentValue(IntPtr pointer)
    {
        _current = pointer.ToInt64();
    }

    /// <summary>
    /// Attempts to read the next available session from the specified session handle.
    /// </summary>
    /// <remarks>This method advances the internal session enumeration. If no more sessions are available, the
    /// method returns false and sets sessionInfo to null. The caller should check the return value before using the
    /// sessionInfo output parameter.</remarks>
    /// <param name="sessionHandle">A handle to the session from which to read session information. Must be valid and open for the duration of the
    /// call.</param>
    /// <param name="sessionInfo">When this method returns, contains the next available session information if the operation succeeds; otherwise,
    /// null.</param>
    /// <returns>true if the next session was successfully read and session information is available; otherwise, false.</returns>
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
        WTS_SESSION_INFO wtsSessionInfo = ReadNext(current);
        int sessionId = wtsSessionInfo.SessionId;

        using WtsInfoSafeHandle infoHandle = WTSApi32.WTSQuerySessionInformation(sessionId, sessionHandle);

        if (!infoHandle.Result)
        {
            sessionInfo = null;
            return false;
        }

        WTSINFO wtsInfo = infoHandle.GetInfo();
        sessionInfo = WtsSessionInfo.Create(sessionHandle.ComputerName, ref wtsSessionInfo, ref wtsInfo);

        SetDomainName(sessionHandle, sessionInfo, sessionId);
        SetClientName(sessionHandle, sessionInfo, sessionId);
        SetUserName(sessionHandle, sessionInfo, sessionId);

        return true;
    }

    /// <summary>
    /// Read a session information structure from the specified unmanaged address.
    /// </summary>
    /// <param name="current">The unmanaged address of the structure to read.</param>
    /// <returns>The marshaled session information structure.</returns>
    private static WTS_SESSION_INFO ReadNext(long current)
    {
        return MarshalHelper.PtrToStruct<WTS_SESSION_INFO>((IntPtr)current);
    }
    /// <summary>
    /// Set the client name on the provided session information by querying the current session.
    /// </summary>
    /// <param name="sessionHandle">A session handle used to query session properties.</param>
    /// <param name="sessionInfo">The session information instance to update.</param>
    /// <param name="sessionId">The session identifier to query.</param>
    private static void SetClientName(WtsSessionSafeHandle sessionHandle, WtsSessionInfo sessionInfo, int sessionId)
    {
        using (WtsBufferHandle clientNameBuffer = WTSApi32.WTSQuerySessionInformation(sessionId, sessionHandle, WtsInfoClass.WTSClientName))
        {
            sessionInfo.ClientName = clientNameBuffer.ReadStringAnsi();
        }
    }
    /// <summary>
    /// Set the domain name on the provided session information by querying the current session.
    /// </summary>
    /// <param name="sessionHandle">A session handle used to query session properties.</param>
    /// <param name="sessionInfo">The session information instance to update.</param>
    /// <param name="sessionId">The session identifier to query.</param>
    private static void SetDomainName(WtsSessionSafeHandle sessionHandle, WtsSessionInfo sessionInfo, int sessionId)
    {
        using (WtsBufferHandle domainNameBuffer = WTSApi32.WTSQuerySessionInformation(sessionId, sessionHandle, WtsInfoClass.WTSDomainName))
        {
            sessionInfo.DomainName = domainNameBuffer.ReadStringAnsi();
        }
    }
    /// <summary>
    /// Set the user name on the provided session information by querying the current session.
    /// </summary>
    /// <param name="sessionHandle">A session handle used to query session properties.</param>
    /// <param name="sessionInfo">The session information instance to update.</param>
    /// <param name="sessionId">The session identifier to query.</param>
    private static void SetUserName(WtsSessionSafeHandle sessionHandle, WtsSessionInfo sessionInfo, int sessionId)
    {
        using (WtsBufferHandle userNameBuffer = WTSApi32.WTSQuerySessionInformation(sessionId, sessionHandle, WtsInfoClass.WTSUserName))
        {
            sessionInfo.UserName = userNameBuffer.ReadStringAnsi();
        }
    }

    /// <summary>
    /// Dispose the handle and clear iteration state.
    /// </summary>
    /// <remarks>
    /// <para>This method clears managed fields before delegating to the base implementation.</para>
    /// </remarks>
    /// <param name="disposing"><see langword="true"/> when called from <see cref="IDisposable.Dispose"/>; otherwise, <see langword="false"/>.</param>
    protected override void Dispose(bool disposing)
    {
        _count = 0;
        _index = 0;
        _current = 0;
        base.Dispose(disposing);
    }
}