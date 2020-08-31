using System;
using System.Collections.Generic;
using System.Linq;

namespace MG.PowerShell.QUser.Executor
{
    public interface IExecutor
    {
        IList<string> ErrorLines { get; }
        IList<string> StandardLines { get; }

        bool Execute(IParameterBuilder parameter);
        void Reset();
    }
}
