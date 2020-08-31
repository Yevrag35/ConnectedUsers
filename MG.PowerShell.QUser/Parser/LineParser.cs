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
        private const int HEADER_COUNT = 6;

        private static readonly Dictionary<string, Action<UserSession, string>> KVPS = new Dictionary<string, Action<UserSession, string>>(HEADER_COUNT, StringComparer.InvariantCultureIgnoreCase)
        {
            { Constants.USERNAME, (x, y) => x.SetUserName(y) },
            { Constants.SESSION_NAME, (x, y) => x.SetSessionName(y) },
            { Constants.ID, (x, y) => x.SetId(y) },
            { Constants.STATE, (x, y) => x.SetSessionState(y) },
            { Constants.IDLE, (x, y) => x.SetIdleTime(y) },
            { Constants.LOGON, (x, y) => x.SetLogonTime(y) }
        };

        public IReadOnlyList<NameIndex> Headers { get; private set; }

        public LineParser()
        {
        }

        public void ParseHeaderLine(string header)
        {
            var list = new List<NameIndex>(HEADER_COUNT);
            MatchCollection collection = Regex.Matches(header, Constants.LINE_PARSING_REGEX_STATEMENT);

            for (int i = 0; i < collection.Count; i++)
            {
                Group g = collection[i].Groups[1];

                if (!g.Value.Equals(Constants.TIME, StringComparison.InvariantCultureIgnoreCase))
                {
                    list.Add(new NameIndex(g.Value, g.Index));
                }
            }
            this.Headers = list;
        }
        public UserSession ParseUserLine(string userLine, bool isCurrent)
        {
            if (this.Headers == null)
                throw new InvalidOperationException(Constants.HEADER_MUST_BE_PARSE_FIRST);

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
