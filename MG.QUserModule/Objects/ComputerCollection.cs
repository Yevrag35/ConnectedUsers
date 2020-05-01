using System;
using System.Collections.Generic;
using System.Linq;

namespace MG.QUserModule
{
    public class ComputerCollection : HashSet<string>
    {
        public string this[int index] => this.ElementAtOrDefault(index);

        public ComputerCollection() : base(IgnoreCase.GetComparer())
        {
        }
        public ComputerCollection(IEnumerable<string> computerNames)
            : base(computerNames, IgnoreCase.GetComparer())
        {
        }
    }
}
