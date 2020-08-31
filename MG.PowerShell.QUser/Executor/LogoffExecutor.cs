using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MG.PowerShell.QUser.Executor
{
    public class LogoffExecutor : ExecutorBase
    {
        private const string PATH_FORMAT = "{0}\\logoff.exe";
        public override string ExePath { get; }

        public LogoffExecutor()
        {
            this.ExePath = string.Format(PATH_FORMAT, Environment.GetFolderPath(Environment.SpecialFolder.System));
        }
    }
}
