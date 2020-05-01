using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MG.QUserModule
{
    public interface IQUserHelper : IWildcardMatcher
    {
        //List<IQUserObject> RunQuery(string computerName);
        QUserResult RunQuery(string computerName);
        //Task<List<IQUserObject>> RunQueryAsync(string computerName);
        Task<QUserResult> RunQueryAsync(string computerName);
    }
}
