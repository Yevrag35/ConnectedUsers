using MG.QUserModule.Objects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MG.QUserModule
{
    public class QUserRemover : WildcardMatcher, IQUserRemover
    {
        private const string LOGOFF_EXE = "logoff.exe";
        private const string LO_ARGS = "{0}";
        private const string LO_ARGS_SERVER = LO_ARGS + " /SERVER:{1}";

        private static readonly string LOGOFF_PATH = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.System),
            LOGOFF_EXE);

        public QUserRemover() { }

        public void LogoffRemoteUser(IQUserObject sid)
        {
            var psi = this.NewProcessStartInfo(sid.Id, sid.HostName);
            this.Run(psi);
        }

        #region BACKEND/PRIVATE METHODS
        private ProcessStartInfo NewProcessStartInfo(int sessionId, string serverName)
        {
            var cmdArgs = !string.IsNullOrWhiteSpace(serverName) && serverName != "localhost" && serverName != Environment.GetEnvironmentVariable("COMPUTERNAME")
                ? string.Format(LO_ARGS_SERVER, sessionId, serverName)
                : string.Format(LO_ARGS, sessionId);

            return new ProcessStartInfo
            {
                Arguments = cmdArgs,
                CreateNoWindow = true,
                FileName = LOGOFF_PATH,
                RedirectStandardError = true,
                UseShellExecute = false
            };
        }

        private void Run(ProcessStartInfo psi)
        {
            using (var proc = new Process
            {
                StartInfo = psi
            })
            {
                proc.Start();
                var errLines = proc.StandardError.ReadToEnd();
                if (!string.IsNullOrWhiteSpace(errLines))
                    throw new InvalidOperationException(errLines);
            }
        }

        #endregion
    }
}
