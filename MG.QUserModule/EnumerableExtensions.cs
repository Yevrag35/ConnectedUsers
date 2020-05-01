using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Management.Automation;

namespace MG.QUserModule
{
    internal static class EnumerableExtensions
    {
        internal static IEnumerable<IQUserObject> ThenFilterBy(this IEnumerable<IQUserObject> objs, Func<IQUserObject, IConvertible> member,
            params string[] wildcardStrings)
        {
            IEnumerable<WildcardPattern> patterns = GetWildcardPatterns();
            return objs
                .Where(x =>
                    member(x) != null
                    &&
                    patterns
                        .Any(p =>
                            p.IsMatch(Convert.ToString(member(x)))));
        }

        private static string GetPropertyName(Expression<Func<IQUserObject, IConvertible>> expression)
        {
            string member = null;
            if (expression.Body is MemberExpression memEx)
            {
                member = memEx.Member.Name;
            }
            else if (expression.Body is UnaryExpression unEx && unEx.Operand is MemberExpression unExMem)
            {
                member = unExMem.Member.Name;
            }
            return member;
        }
        private static IEnumerable<WildcardPattern> GetWildcardPatterns(params string[] wildcardStrs)
        {
            if (wildcardStrs == null || wildcardStrs.Length <= 0)
                yield break;

            foreach (string str in wildcardStrs.Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                yield return new WildcardPattern(str, WildcardOptions.IgnoreCase);
            }
        }
    }
}
