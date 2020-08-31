using MG.PowerShell.QUser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace MG.PowerShell.QUser.Parser
{
    public class LineParser
    {
        private const string REGEX = "(\\S+)";
        private const string TIME = "TIME";
        private static readonly Dictionary<string, Action<UserSession, string>> KVPS = new Dictionary<string, Action<UserSession, string>>(6, StringComparer.InvariantCultureIgnoreCase)
        {
            { "USERNAME", (x, y) => x.SetUserName(y) },
            { "SESSIONNAME", (x, y) => x.SetSessionName(y) },
            { "ID", (x, y) => x.SetId(y) },
            { "STATE", (x, y) => x.SetSessionState(y) },
            { "IDLE", (x, y) => x.SetIdleTime(y) },
            { "LOGON", (x, y) => x.SetLogonTime(y) }
        };

        public IReadOnlyList<NameIndex> Headers { get; private set; }

        public LineParser()
        {
        }

        public void ParseHeaderLine(string header)
        {
            var list = new List<NameIndex>(6);
            MatchCollection collection = Regex.Matches(header, REGEX);
            for (int i = 0; i < collection.Count; i++)
            {
                Group g = collection[i].Groups[1];
                if (!g.Value.Equals(TIME, StringComparison.InvariantCultureIgnoreCase))
                {
                    list.Add(new NameIndex(g.Value, g.Index));
                }
            }
            this.Headers = list;
        }
        public UserSession ParseUserLine(string userLine, bool isCurrent)
        {
            if (this.Headers == null)
                throw new InvalidOperationException("The header line must be parsed first.");

            var userSes = new UserSession(isCurrent);
            for (int i = 0; i < this.Headers.Count; i++)
            {
                NameIndex ni = this.Headers[i];

                string part = userLine.Substring(ni.Index);
                string value = null;

                if (i < (this.Headers.Count - 1))
                {
                    int next = this.Headers[i + 1].Index;
                    value = part.Substring(0, next - ni.Index);
                }
                else
                {
                    value = part;
                }

                KVPS[ni.Name].Invoke(userSes, value);
            }
            return userSes;
        }
    }
}
