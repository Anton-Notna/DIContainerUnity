using System;

namespace DI
{
    public interface ISourceUnit
    {
        public Type Type { get; }
        public object Value { get; }
    }
}