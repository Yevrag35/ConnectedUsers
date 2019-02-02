using System;

namespace MG.QUserModule.Objects
{
    public interface IAttribute
    {
        Type ValueType { get; }
        object Value { get; }
        bool ValueIsArray { get; }
        bool ValueIsOneItemArray { get; }
        long ValueCount { get; }
    }
}
