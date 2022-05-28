using System;
using System.Collections.Generic;

namespace DI
{
    public interface IContext : IReadOnlyContext
    {
        public void Add(ISource unit);

        public void AddRange(IEnumerable<ISource> units);

        public void Remove(Type type);

        public void RemoveRange(IEnumerable<Type> types);
    }
}