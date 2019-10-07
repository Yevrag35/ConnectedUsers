using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace MG.QUserModule.Objects
{
    public class QUserObject : IQUserObject
    {
        private const string GenericAttsGetMethod = "GetAttribute";

        public string HostName { get; }
        public string UserName { get; }
        public string SessionName { get; }
        public bool? IsCurrentSession { get; }
        public SessionState? State { get; }
        public int? Id { get; }
        public TimeSpan? IdleTime { get; }
        public DateTime? LogonTime { get; }

        #region CONSTRUCTORS

        public QUserObject(bool? isCurrent, string hn, string un, string sn, string state, int? id, string it, string lot)
        {
            this.IdleTime = this.FormatIdleTime(it);
            this.HostName = hn;
            this.UserName = un;
            this.SessionName = sn;
            this.IsCurrentSession = isCurrent;
            this.State = state.Equals("Active", StringComparison.CurrentCultureIgnoreCase)
                ? SessionState.Active
                : SessionState.Disconnected;
            this.Id = id;
            this.LogonTime = this.FormatLogonTime(lot);
        }

        #endregion

        #region PRIVATE/BACKEND METHODS
        private DateTime? FormatLogonTime(string possibleLogonTime)
        {
            return DateTime.TryParse(possibleLogonTime, out DateTime dt)
                ? dt
                : (DateTime?)null;
        }

        private TimeSpan? FormatIdleTime(string possibleIdleTime)
        {
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
            return idleTime;
        }

        #endregion
    }
}
