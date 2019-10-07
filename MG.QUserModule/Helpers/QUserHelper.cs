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
        private const string QUSER_EXE = "quser.exe";
        private const string REGEX_EXP = @"^(?<IsCurrent>\s|>)(?<UserName>[^\s]*)\s*(?<SessionName>rdp[^\s]*)?\s*(?<ID>\d*)\s*(?<STATE>[^\s]*)\s*(?<IdleTime>[^\s]*)\s*(?<LogonTime>.+)";

        private static readonly string QUSER_PATH = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.System),
            QUSER_EXE
        );

        public QUserHelper() { }

        public List<IQUserObject> RunQuery(string computerName, int timeoutInMs)
        {
            ProcessStartInfo psi = this.NewProcessStartInfo(computerName);
            List<string> executedLines = this.RunAndRead(psi, timeoutInMs);
            return this.ParseOutput(executedLines, computerName);
        }
        public async Task<List<IQUserObject>> RunQueryAsync(string computerName, int timeoutInMs)
        {
            return await Task.Run(() =>
            {
                ProcessStartInfo psi = this.NewProcessStartInfo(computerName);
                List<string> executedLines = this.RunAndRead(psi, timeoutInMs);
                return this.ParseOutput(executedLines, computerName);
            });
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
                StandardOutputEncoding = Encoding.ASCII,
                StandardErrorEncoding = Encoding.ASCII,
                UseShellExecute = false
            };
        }

        public List<string[]> RunAndReadTest(ProcessStartInfo psi, int timeoutInMs)
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
                if (!string.IsNullOrWhiteSpace(errLines) && !errLines.Contains("No User exists for *"))
                    throw new InvalidOperationException(errLines);

                return list;
            }
        }

        public List<string> RunAndRead(ProcessStartInfo psi, int timeoutInMs)
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

        private List<IQUserObject> ParseOutput(IList<string> strs, string hostName)
        {
            var list = new List<IQUserObject>(strs.Count);
            for (int i = 0; i < strs.Count; i++)
            {
                string str = strs[i];
                Match mtc = Regex.Match(str, REGEX_EXP);
                if (mtc.Success)
                {
                    Group grp = mtc.Groups["SessionName"];
                    string sesn = !string.IsNullOrEmpty(grp.Value)
                        ? grp.Value.Trim()
                        : null;

                    bool isCur = !string.IsNullOrWhiteSpace(mtc.Groups["IsCurrent"].Value) &&
                        mtc.Groups["IsCurrent"].Value.Contains(">");

                    string userName = !string.IsNullOrWhiteSpace(mtc.Groups["UserName"].Value)
                        ? mtc.Groups["UserName"].Value.Trim()
                        : null;

                    int? id = !string.IsNullOrWhiteSpace(mtc.Groups["ID"].Value)
                        ? Convert.ToInt32(mtc.Groups["ID"].Value.Trim())
                        : (int?)null;
                    
                    string state = !string.IsNullOrWhiteSpace(mtc.Groups["STATE"].Value)
                        ? mtc.Groups["STATE"].Value.Trim()
                        : null;

                    string idleTime = !string.IsNullOrWhiteSpace(mtc.Groups["IdleTime"].Value)
                        ? mtc.Groups["IdleTime"].Value.Trim()
                        : null;

                    string logonTime = !string.IsNullOrWhiteSpace(mtc.Groups["LogonTime"].Value)
                        ? mtc.Groups["LogonTime"].Value.Trim()
                        : null;

                    list.Add(new QUserObject(isCur, hostName, userName, sesn, state, id, idleTime, logonTime));
                }
            }
            return list;
        }

        #endregion
    }
}
