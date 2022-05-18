using System;

namespace DI
{
    public interface IReadOnlySource
    {
        public bool Contains(Type type);
        public bool TryGet(Type type, out object value);
    }
}