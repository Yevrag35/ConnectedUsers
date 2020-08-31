using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MG.PowerShell.QUser.Executor
{
    public abstract class ExecutorBase
    {
        public abstract string ExePath { get; }
        public IList<string> ErrorLines { get; }
        public IList<string> StandardLines { get; }

        public ExecutorBase()
        {
            this.ErrorLines = new List<string>();
            this.StandardLines = new List<string>(1);
        }

        public bool Execute(IParameterBuilder parameter)
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
                        this.StandardLines.Add(process.StandardOutput.ReadLine());

                    if (!process.StandardError.EndOfStream)
                    {
                        string errLine = process.StandardError.ReadLine();
                        this.ProcessErrorLine(errLine);

                        this.ErrorLines.Add(errLine);
                    }
                }

                return this.ErrorLines.Count <= 0;
            }
        }
        protected virtual ProcessStartInfo GenerateStartInfo(IParameterBuilder parameter)
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
        protected abstract void ProcessErrorLine(string line);
    }
}
