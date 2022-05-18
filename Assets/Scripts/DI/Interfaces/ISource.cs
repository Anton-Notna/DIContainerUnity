using System;

namespace DI
{
    public interface ISource : IReadOnlySource
    {
        public void Add(ISourceUnit unit);
        public void Remove(Type type);
    }
}