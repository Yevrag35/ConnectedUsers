using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MG.PowerShell.QUser.Executor
{
    public class QUserParameter
    {
        private const string SERVER_PARAM = "/server:{0}";
        private const char SPACE = (char)32;

        private StringBuilder _builder;

        public string ServerName { get; set; }
        public string SessionName { get; internal set; }
        public int? SessionId { get; internal set; }
        public string UserName { get; internal set; }
        
        internal QUserParameter()
        {
            _builder = new StringBuilder();
        }
        public QUserParameter(string name, int? id, string userName)
            : this(null, name, id, userName)
        {
        }
        public QUserParameter(string serverName, string name, int? id, string userName)
        {
            this.ServerName = serverName;
            this.SessionName = name;
            this.SessionId = id;
            this.UserName = userName;
            _builder = new StringBuilder();
        }

        public string Build()
        {
            if (!string.IsNullOrEmpty(this.UserName))
                _builder.Append(this.UserName);

            else if (!string.IsNullOrEmpty(this.SessionName))
                _builder.Append(this.SessionName);

            else if (this.SessionId.HasValue)
                _builder.Append(this.SessionId.Value);

            if (!string.IsNullOrEmpty(this.ServerName))
            {
                if (_builder.Length > 0)
                    _builder.Append(SPACE);

                _builder.Append(string.Format(SERVER_PARAM, this.ServerName));
            }

            string result = _builder.ToString();
            _builder.Clear();
            return result;
        }
    }
}
