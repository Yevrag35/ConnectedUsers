using System;

namespace MG.PowerShell.QUser
{
    public static class Constants
    {
        #region LINE PARSING
        internal const string LINE_PARSING_REGEX_STATEMENT = "(\\S+)";
        internal const string ID = "ID";
        internal const string IDLE = "IDLE";
        internal const string LOGON = "LOGON";
        internal const string SESSION_NAME = "SESSIONNAME";
        internal const string STATE = "STATE";
        internal const string TIME = "TIME";
        internal const string USERNAME = "USERNAME";

        internal const string GREATER_THAN = ">";
        internal const char SPACE = (char)32;

        // EXCEPTION TEXT
        internal const string HEADER_MUST_BE_PARSE_FIRST = "The header line must be parsed first.";
        internal const string NO_USER_EXISTS = "No user exists";

        #endregion

        internal const string ENV_COMPUTER_NAME = "COMPUTERNAME";

        #region FORMATS
        internal const string FORMAT_1 = "{0} - {1}";

        #endregion

        #region LOGOFF CONSTANTS
        internal const string SERVER_PARAM = "/server:";
        internal const string VERBOSE = "/V";

        #endregion
    }
}
