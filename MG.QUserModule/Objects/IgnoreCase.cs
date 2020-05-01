using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MG.QUserModule
{
    internal class IgnoreCase : IEqualityComparer<string>
    {
        private IgnoreCase() { }

        public bool Equals(string x, string y) => x.Equals(y, StringComparison.InvariantCultureIgnoreCase);
        public int GetHashCode(string x) => x.ToLower().GetHashCode();

        internal static IEqualityComparer<string> GetComparer() => new IgnoreCase();
    }
}
