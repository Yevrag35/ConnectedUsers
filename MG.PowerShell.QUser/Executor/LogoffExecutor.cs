using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MG.PowerShell.QUser.Executor
{
    public class LogoffExecutor : ExecutorBase
    {
        public override string ExePath { get; }

        public LogoffExecutor()
        {
            this.ExePath = string.Format("{0}\\logoff.exe", Environment.GetFolderPath(Environment.SpecialFolder.System));
        }
        protected override void ProcessErrorLine(string line)
        {
        }
    }
}
