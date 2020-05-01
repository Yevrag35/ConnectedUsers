using MG.QUserModule.Objects;
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

        public List<IQUserObject> RunQuery(string computerName)
        {
            ProcessStartInfo psi = this.NewProcessStartInfo(computerName);
            List<string> executedLines = this.RunAndRead(psi);
            return this.ParseOutput(executedLines, computerName);
        }
        public async Task<List<IQUserObject>> RunQueryAsync(string computerName)
        {
            return await Task.Run(() =>
            {
                ProcessStartInfo psi = this.NewProcessStartInfo(computerName);
                List<string> executedLines = this.RunAndRead(psi);
                return this.ParseOutput(executedLines, computerName);
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

        public List<string[]> RunAndReadTest(ProcessStartInfo psi)
        {
            var list = new List<string[]>();
            using (var proc = new Process
            {
                StartInfo = psi
            })
            {
                proc.Start();
                proc.WaitForExit(3000);

                string[] allLines = proc.StandardOutput.ReadToEnd().Split(new string[3] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 1; i < allLines.Length; i++)
                {
                    string line = allLines[i];
                    var strList = line.Split(new string[1] { " " }, StringSplitOptions.RemoveEmptyEntries).ToList();

                }
                string errLines = proc.StandardError.ReadToEnd();
                //if (!string.IsNullOrWhiteSpace(errLines) && !errLines.Contains("No User exists for *"))
                if (!string.IsNullOrWhiteSpace(errLines))
                {
                    if (errLines.Contains("Access is denied"))
                        throw new UnauthorizedAccessException(errLines);

                    else if (!errLines.Contains("No User exists for *"))
                        throw new InvalidOperationException(errLines);
                }

                return list;
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

        private List<IQUserObject> ParseOutput(List<string> strs, string hostName)
        {
            var list = new List<IQUserObject>(strs.Count);
            for (int i = 0; i < strs.Count; i++)
            {
                string str = Regex.Replace(strs[i], "\\s{2,}", COMMA.ToString());
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

                list.Add(new QUserObject(isCurrent, hostName, userName, sessionName, state, id, idleTime, logonTime));
            }
            return list;
        }

        #endregion

        #endregion
    }
}
