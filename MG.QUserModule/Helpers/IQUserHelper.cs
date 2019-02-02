using MG.QUserModule.Objects;
using System;
using System.Collections.Generic;

namespace MG.QUserModule
{
    public interface IQUserHelper : IWildcardMatcher
    {
        IList<IQUserObject> RunQuery(string computerName);
    }
}
