using MG.QUserModule.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MG.QUserModule
{
    public interface IQUserRemover : IWildcardMatcher
    {
        void LogoffRemoteUser(IQUserObject sid);
    }
}
