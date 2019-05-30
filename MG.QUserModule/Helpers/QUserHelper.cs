using MG.QUserModule.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MG.QUserModule
{
    public class QUserHelper : WildcardMatcher, IQUserHelper
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
            var psi = this.NewProcessStartInfo(computerName);
            var executedLines = this.RunAndRead(psi);
            return this.ParseOutput(executedLines, computerName);
        }
        public async Task<IList<IQUserObject>> RunQueryAsync(string computerName)
        {
            return await Task.Run(() =>
            {
                var psi = this.NewProcessStartInfo(computerName);
                var executedLines = this.RunAndRead(psi);
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
                proc.WaitForExit(3000);
                //while (!proc.StandardOutput.EndOfStream)
                //    list.Add(proc.StandardOutput.ReadLine());

                string[] allLines = proc.StandardOutput.ReadToEnd().Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                list.AddRange(allLines);
                var errLines = proc.StandardError.ReadToEnd();
                if (!string.IsNullOrWhiteSpace(errLines) && !errLines.Contains("No User exists for *"))
                    throw new InvalidOperationException(errLines);

                if (list.Count > 1)
                    list.RemoveAt(0);

                return list;
            }
        }

        private IList<IQUserObject> ParseOutput(IList<string> strs, string hostName)
        {
            IList<IQUserObject> list = new List<IQUserObject>(strs.Count);
            for (var i = 0; i < strs.Count; i++)
            {
                var str = strs[i];
                var mtc = Regex.Match(str, REGEX_EXP);
                if (mtc.Success)
                {
                    var sesn = mtc.Groups[3].Value;

                    //bool isCur = mtc.Groups[1].Value.Equals(">");
                    var userName = mtc.Groups[2].Value.Trim();
                    var session = !string.IsNullOrWhiteSpace(sesn) ? sesn : null;
                    var id = Convert.ToInt32(mtc.Groups[4].Value.Trim());
                    var state = mtc.Groups[5].Value.Trim();
                    var idleTime = mtc.Groups[6].Value.Trim();
                    var logonTime = mtc.Groups[7].Value.Trim();

                    list.Add(new QUserObject(hostName, userName, session, state, id, idleTime, logonTime));
                }
            }
            return list;
        }

        #endregion
    }
}
