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
        //public QUserObject(string[] strings)
        //{

        //}

        public QUserObject(bool? isCurrent, string hn, string un, string sn, string state, int? id, string it, string lot)
        {
            if (!string.IsNullOrWhiteSpace(it) && int.TryParse(it, out int tryint))
            {
                it = string.Format("0:{0}", tryint);
            }
            if (!string.IsNullOrWhiteSpace(it) && TimeSpan.TryParse(it, out TimeSpan ts))
            {
                this.IdleTime = ts;
            }

            this.HostName = hn;
            this.UserName = un;
            this.SessionName = sn;
            this.IsCurrentSession = isCurrent;
            this.State = state.Equals("Disc", StringComparison.CurrentCultureIgnoreCase)
                ? SessionState.Disconnected
                : SessionState.Active;
            this.Id = id;
            this.LogonTime = DateTime.Parse(lot);
        }

        #endregion
    }
}
