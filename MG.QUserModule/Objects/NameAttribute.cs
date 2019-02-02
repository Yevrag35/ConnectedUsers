using System;
using System.Collections;
using System.Linq;

namespace MG.QUserModule.Objects
{
    public class NameAttribute : Attribute, IAttribute
    {
        private readonly object _val;
        public object Value => _val;
        public Type ValueType => _val.GetType();
        public bool ValueIsArray => this.ValueType.IsArray;
        public bool ValueIsOneItemArray => this.ValueIsArray && this.ValueCount == 1;
        public long ValueCount => !this.ValueIsArray ? 1 : ((IEnumerable)_val).Cast<object>().ToArray().LongLength;

        public NameAttribute(string name) =>
            _val = name;
    }
}
