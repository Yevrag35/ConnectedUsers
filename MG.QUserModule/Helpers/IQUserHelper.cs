using MG.QUserModule.Objects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MG.QUserModule
{
    public interface IQUserHelper : IWildcardMatcher
    {
        IList<IQUserObject> RunQuery(string computerName);
        Task<IList<IQUserObject>> RunQueryAsync(string computerName);
    }
}
