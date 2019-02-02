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
    public class QUserHelper : IQUserHelper
    {
        private const string QUSER_EXE = "quser.exe";
        private const string REGEX_EXP = @"^(\s|\>)(\S*)\s{1,}(console|rdp\S*|\s)\s{1,}((?:[0-9]){1,})\s{1,}(Active|Disc)\s{1,}(none|\.|\d|\d{1,2}\:\d{1,2}|\d{1,2}\:\d{1,2}\:\d{1,2})\s{1,}(.{1,})$";

        private static readonly string QUSER_PATH = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.System),
            QUSER_EXE
        );

        public QUserHelper() { }

        public IList<IQUserObject> RunQuery(string computerName)
        {
            ProcessStartInfo psi = this.NewProcessStartInfo(computerName);
            IList<string> executedLines = this.RunAndRead(psi);
            return this.ParseOutput(executedLines, computerName);
        }

        #region BACKEND/PRIVATE METHODS
        public ProcessStartInfo NewProcessStartInfo(string computerName)
        {
            string cmdArgs = null;
            if (!string.IsNullOrEmpty(computerName) && computerName != "localhost" && computerName != Environment.GetEnvironmentVariable("COMPUTERNAME"))
                cmdArgs = string.Format("/SERVER:{0}", computerName);

            return new ProcessStartInfo
            {
                Arguments = cmdArgs,
                CreateNoWindow = true,
                FileName = QUSER_PATH,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };
        }

        public IList<string> RunAndRead(ProcessStartInfo psi)
        {
            var list = new List<string>();
            using (var proc = new Process
            {
                StartInfo = psi
            })
            {
                proc.Start();
                while (!proc.StandardOutput.EndOfStream)
                    list.Add(proc.StandardOutput.ReadLine());

                string errLines = proc.StandardError.ReadToEnd();
                if (!string.IsNullOrWhiteSpace(errLines))
                    throw new InvalidOperationException(errLines);

                list.RemoveAt(0);

                return list;
            }
        }

        private IList<IQUserObject> ParseOutput(IList<string> strs, string hostName)
        {
            IList<IQUserObject> list = new List<IQUserObject>(strs.Count);
            for (int i = 0; i < strs.Count; i++)
            {
                var str = strs[i];
                Match mtc = Regex.Match(str, REGEX_EXP);
                if (mtc.Success)
                {
                    string sesn = mtc.Groups[3].Value;

                    //bool isCur = mtc.Groups[1].Value.Equals(">");
                    string userName = mtc.Groups[2].Value.Trim();
                    string session = !string.IsNullOrWhiteSpace(sesn) ? sesn : null;
                    int id = Convert.ToInt32(mtc.Groups[4].Value.Trim());
                    string state = mtc.Groups[5].Value.Trim();
                    string idleTime = mtc.Groups[6].Value.Trim();
                    string logonTime = mtc.Groups[7].Value.Trim();

                    list.Add(new QUserObject(hostName, userName, session, state, id, idleTime, logonTime));
                }
            }
            return list;
        }

        public IList<IQUserObject> PerformWildcardMatch(IList<IQUserObject> list, string property, string wildcardSearch)
        {
            if (property != "SessionName" && property != "UserName")
                throw new ArgumentException("Only 'SessionName' and 'UserName' support wildcard searching!");

            var search = new WildcardPattern(wildcardSearch, WildcardOptions.IgnoreCase);
            PropertyInfo pi = typeof(IQUserObject).GetProperty(property, BindingFlags.Public | BindingFlags.Instance);
            for (int i = list.Count - 1; i >= 0; i--)
            {
                var obj = list[i];
                string value = pi.GetValue(obj) as string;
                if (!search.IsMatch(value))
                    list.Remove(obj);
            }
            return list;
        }

        #endregion
    }
}
