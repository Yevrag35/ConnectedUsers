using MG.QUserModule.Objects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MG.QUserModule
{
    public interface IQUserHelper : IWildcardMatcher
    {
        List<IQUserObject> RunQuery(string computerName, int timeoutInMs);
        Task<List<IQUserObject>> RunQueryAsync(string computerName, int timeoutInMs);
    }
}
