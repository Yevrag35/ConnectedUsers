using MG.QUserModule.Objects;
using System;
using System.Collections.Generic;

namespace MG.QUserModule
{
    public interface IWildcardMatcher
    {
        IList<IQUserObject> PerformWildcardMatch(IList<IQUserObject> list, string property, string wildcardSearch);
    }
}
