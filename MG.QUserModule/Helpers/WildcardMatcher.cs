using MG.QUserModule.Objects;
using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Reflection;

namespace MG.QUserModule
{
    public abstract class WildcardMatcher : IWildcardMatcher
    {
        IList<IQUserObject> IWildcardMatcher.PerformWildcardMatch(IList<IQUserObject> list, string property, string wildcardSearch)
        {
            if (property != "SessionName" && property != "UserName")
                throw new ArgumentException("Only 'SessionName' and 'UserName' support wildcard searching!");

            var search = new WildcardPattern(wildcardSearch, WildcardOptions.IgnoreCase);
            var pi = typeof(IQUserObject).GetProperty(property, BindingFlags.Public | BindingFlags.Instance);
            for (var i = list.Count - 1; i >= 0; i--)
            {
                var obj = list[i];
                var value = pi.GetValue(obj) as string;
                if (!search.IsMatch(value))
                    list.Remove(obj);
            }
            return list;
        }
    }
}
