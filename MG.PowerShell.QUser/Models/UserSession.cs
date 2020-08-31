using System;
using System.Collections.Generic;

namespace MG.PowerShell.QUser.Models
{
    public class UserSession
    {
        public string HostName { get; internal set; }
        public int? Id { get; private set; }
        public bool IsCurrentSession { get; }
        public TimeSpan? IdleTime { get; internal set; }
        public DateTime? LogonTime { get; internal set; }
        public string SessionName { get; internal set; }
        public SessionState State { get; internal set; }
        public string UserName { get; internal set; }

        internal UserSession(bool isCurrent)
        {
            this.IsCurrentSession = isCurrent;
        }

        internal void SetId(string possibleId)
        {
            this.Id = !string.IsNullOrEmpty(possibleId) ? Convert.ToInt32(possibleId.Trim()) : (int?)null;
        }
        internal void SetIdleTime(string possibleIdleTime)
        {
            possibleIdleTime = possibleIdleTime.Trim();
            TimeSpan? idleTime = null;
            if (possibleIdleTime != null)
            {
                if (int.TryParse(possibleIdleTime, out int tryInt))
                {
                    idleTime = new TimeSpan(0, tryInt, 0);
                }
                else if (TimeSpan.TryParse(possibleIdleTime, out TimeSpan ts))
                {
                    idleTime = ts;
                }
            }
            this.IdleTime = idleTime;
        }
        internal void SetLogonTime(string possibleLogonTime)
        {
            this.LogonTime = DateTime.TryParse(possibleLogonTime.Trim(), out DateTime dt)
                ? DateTime.SpecifyKind(dt, DateTimeKind.Local)
                : (DateTime?)null;
        }
        internal void SetSessionName(string possibleName)
        {
            if (string.IsNullOrEmpty(possibleName))
                return;

            this.SessionName = possibleName.Trim();
        }
        internal void SetSessionState(string possibleState)
        {
            this.State = EnumFactory.GetSessionState(possibleState.Trim());
        }
        internal void SetUserName(string userName)
        {
            this.UserName = userName.Trim();
        }
    }
}
