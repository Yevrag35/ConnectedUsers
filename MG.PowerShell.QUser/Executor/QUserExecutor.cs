using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MG.PowerShell.QUser.Executor
{
    public class QUserExecutor : ExecutorBase
    {
        private const string PATH_FORMAT = "{0}\\quser.exe";
        public sealed override string ExePath { get; }
        public bool NoUserExists { get; private set; }

        public QUserExecutor()
            : base()
        {
            this.ExePath = string.Format(PATH_FORMAT, Environment.GetFolderPath(Environment.SpecialFolder.System));
        }

        protected override void ProcessErrorLine(string line)
        {
            if (line.StartsWith(Constants.NO_USER_EXISTS, StringComparison.InvariantCultureIgnoreCase))
                this.NoUserExists = true;
        }

        public override void Reset()
        {
            this.NoUserExists = false;
            base.Reset();
        }
    }
}
