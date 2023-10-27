using System;

namespace DI
{
    public interface ISource
    {
        public Type Type { get; }

        public object Value { get; }
    }
}