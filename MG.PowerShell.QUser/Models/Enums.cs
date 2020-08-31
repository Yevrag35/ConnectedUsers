using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MG.PowerShell.QUser.Models
{
    public enum SessionState
    {
        Unknown,
        Active,
        Disconnected
    }

    public static class EnumFactory
    {
        private const string ACTIVE = "Active";
        private const string DISCONNECTED = "Disc";

        public static SessionState GetSessionState(string possibleState)
        {
            if (string.IsNullOrEmpty(possibleState))
                return SessionState.Unknown;

            if (possibleState.StartsWith(ACTIVE, StringComparison.InvariantCultureIgnoreCase))
                return SessionState.Active;

            else if (possibleState.StartsWith(DISCONNECTED, StringComparison.InvariantCultureIgnoreCase))
                return SessionState.Disconnected;

            else
                return SessionState.Unknown;
        }
    }
}
