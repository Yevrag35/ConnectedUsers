using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MG.PowerShell.QUser.Parser
{
    public struct NameIndex
    {
        public string Name;
        public int Index;

        public NameIndex(string name, int index)
        {
            this.Name = name;
            this.Index = index;
        }
    }
}
