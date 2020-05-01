using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MG.QUserModule
{
    public class QUserHelper : WildcardMatcher, IQUserHelper
    {
        private const char COMMA = (char)44;

        private const string QUSER_EXE = "quser.exe";
        private const string REGEX_EXP = @"^(?<IsCurrent>\s|>)(?<UserName>[^\s]*)\s*(?<SessionName>[^\s]*)?\s*(?<ID>\d*)\s*(?<STATE>[^\s]*)\s*(?<IdleTime>[^\s]*)\s*(?<LogonTime>.+)";

        private static readonly string QUSER_PATH = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.System),
            QUSER_EXE
        );

        public QUserHelper() { }

        public QUserResult RunQuery(string computerName)
        {
            ProcessStartInfo psi = this.NewProcessStartInfo(computerName);
            //List<string> executedLines = this.RunAndRead(psi);
            QUserResult result = this.RunAndReadTest(psi);
            
            this.ParseOutput(computerName, ref result);
            return result;
        }
        public async Task<QUserResult> RunQueryAsync(string computerName)
        {
            return await Task.Run(() =>
            {
                ProcessStartInfo psi = this.NewProcessStartInfo(computerName);
                //List<string> executedLines = this.RunAndRead(psi);
                QUserResult result = this.RunAndReadTest(psi);
                this.ParseOutput(computerName, ref result);
                return result;
            });
        }

        #region BACKEND/PRIVATE METHODS

        #region PROCESS
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
                StandardOutputEncoding = Encoding.ASCII,
                StandardErrorEncoding = Encoding.ASCII,
                UseShellExecute = false
            };
        }

        public QUserResult RunAndReadTest(ProcessStartInfo psi)
        {
            using (var process = new Process
            {
                StartInfo = psi
            })
            {
                try
                {
                    process.Start();
                }
                catch (Exception e)
                {
                    return QUserResult.FromException(e);
                }

                if (!process.WaitForExit(30000))
                {
                    return QUserResult.FromException(new TimeoutException(string.Format("Process Id: \"{0}\" timed out", process.Id)));
                }

                return QUserResult.ReadProcess(process);
            }
        }
        public List<string> RunAndRead(ProcessStartInfo psi)
        {
            var list = new List<string>();
            using (var proc = new Process
            {
                StartInfo = psi
            })
            {
                proc.Start();
                proc.WaitForExit(3000);

                string[] allLines = proc.StandardOutput.ReadToEnd().Split(new string[3] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                list.AddRange(allLines);
                string errLines = proc.StandardError.ReadToEnd();
                if (!string.IsNullOrWhiteSpace(errLines) && !errLines.Contains("No User exists for *"))
                    throw new InvalidOperationException(errLines);

                if (list.Count > 1)
                    list.RemoveAt(0);

                return list;
            }
        }

        #endregion

        #region PARSING
        private string CountSplit(string orig, string[] split)
        {
            if (split.Length == 5)
                return Regex.Replace(orig, "(^[^,]+)", new MatchEvaluator(MatchReplace));

            else
                return orig;
        }
        private string MatchReplace(Match m)
        {
            return m.Groups[1].Value + COMMA.ToString();
        }

        private void ParseOutput(string hostName, ref QUserResult result)
        {
            result.Users = new List<IQUserObject>(result.StandardOutput.Count);
            for (int i = 0; i < result.StandardOutput.Count; i++)
            {
                string str = Regex.Replace(result.StandardOutput[i], "\\s{2,}", COMMA.ToString());
                string[] split = str.Split(COMMA);

                string output = this.CountSplit(str, split);

                split = output.Split(COMMA);
                bool isCurrent = false;

                string idleTime = split[4];
                string userName = split[0];
                if (userName.StartsWith(COMMA.ToString()))
                {
                    userName = userName.Substring(1);
                    isCurrent = true;
                }
                string sessionName = split[1];
                int? id = null;
                if (int.TryParse(split[2], out int foundId))
                    id = foundId;

                string state = split[3];
                string logonTime = split[5];

                result.Users.Add(new QUserObject(isCurrent, hostName, userName, sessionName, state, id, idleTime, logonTime));
            }
        }
        //private List<IQUserObject> ParseOutput(IReadOnlyList<string> strs, string hostName)
        //{
        //    var list = new List<IQUserObject>(strs.Count);
        //    for (int i = 0; i < strs.Count; i++)
        //    {
        //        string str = Regex.Replace(strs[i], "\\s{2,}", COMMA.ToString());
        //        string[] split = str.Split(COMMA);

        //        string output = this.CountSplit(str, split);

        //        split = output.Split(COMMA);
        //        bool isCurrent = false;

        //        string idleTime = split[4];
        //        string userName = split[0];
        //        if (userName.StartsWith(COMMA.ToString()))
        //        {
        //            userName = userName.Substring(1);
        //            isCurrent = true;
        //        }
        //        string sessionName = split[1];
        //        int? id = null;
        //        if (int.TryParse(split[2], out int foundId))
        //            id = foundId;

        //        string state = split[3];
        //        string logonTime = split[5];

        //        list.Add(new QUserObject(isCurrent, hostName, userName, sessionName, state, id, idleTime, logonTime));
        //    }
        //    return list;
        //}

        #endregion

        #endregion
    }
}
