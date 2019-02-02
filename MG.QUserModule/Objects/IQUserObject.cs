using System;

namespace MG.QUserModule.Objects
{
    public interface IQUserObject
    {
        string HostName { get; }
        string UserName { get; }
        string SessionName { get; }
        //bool IsCurrentSession { get; }
        SessionState State { get; }
        int Id { get; }
        TimeSpan? IdleTime { get; }
        DateTime LogonTime { get; }
    }
}
