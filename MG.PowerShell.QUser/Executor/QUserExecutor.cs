using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MG.PowerShell.QUser.Executor
{
    public class QUserExecutor
    {
        private const string PATH_FORMAT = "{0}\\quser.exe";
        public string ExePath { get; }
        public bool NoUserExists { get; private set; }
        public List<string> ErrorLines { get; }
        public List<string> UserLines { get; }

        public QUserExecutor()
        {
            this.ErrorLines = new List<string>();
            this.ExePath = string.Format(PATH_FORMAT, Environment.GetFolderPath(Environment.SpecialFolder.System));
            this.UserLines = new List<string>(1);
        }

        public bool Execute(QUserParameter parameter)
        {
            using (Process process = new Process
            {
                StartInfo = this.GenerateStartInfo(parameter)
            })
            {
                process.Start();
                while (!process.StandardOutput.EndOfStream || !process.StandardError.EndOfStream)
                {
                    if (!process.StandardOutput.EndOfStream)
                        this.UserLines.Add(process.StandardOutput.ReadLine());

                    if (!process.StandardError.EndOfStream)
                    {
                        string errLine = process.StandardError.ReadLine();
                        if (errLine.StartsWith("No User exists", StringComparison.InvariantCultureIgnoreCase))
                            this.NoUserExists = true;

                        this.ErrorLines.Add(errLine);
                    }
                }

                return this.ErrorLines.Count <= 0;
            }
        }
        private ProcessStartInfo GenerateStartInfo(QUserParameter parameter)
        {
            Encoding encoding = Encoding.ASCII;
            return new ProcessStartInfo(this.ExePath, parameter.Build())
            {
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                StandardErrorEncoding = encoding,
                StandardOutputEncoding = encoding,
                UseShellExecute = false
            };
        }
    }
}
