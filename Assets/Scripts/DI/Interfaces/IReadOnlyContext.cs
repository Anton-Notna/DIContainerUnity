using System;
using System.Collections.Generic;

namespace DI
{
    public interface IReadOnlyContext
    {
        public event Action<Type, ISource> SourceAdded;

        public event Action<Type, ISource> SourceRemoved;

        public IEnumerable<KeyValuePair<Type, ISource>> GetSources();

        public bool Contains(Type type);

        public bool TryGet(Type type, out object value);
    }
}