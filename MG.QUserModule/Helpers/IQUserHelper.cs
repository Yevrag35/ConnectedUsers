using MG.QUserModule.Objects;
using System;
using System.Collections.Generic;

namespace MG.QUserModule
{
    public interface IQUserHelper
    {
        IList<IQUserObject> RunQuery(string computerName);

        IList<IQUserObject> PerformWildcardMatch(IList<IQUserObject> list, string property, string wildcardSearch);
    }
}
