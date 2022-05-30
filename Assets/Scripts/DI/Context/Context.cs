using System;
using System.Collections.Generic;

namespace DI
{
    public class Context : IContext
    {
        private Dictionary<Type, ISource> _sources = new Dictionary<Type, ISource>();

        public event Action<Type, ISource> SourceAdded;
        public event Action<Type, ISource> SourceRemoved;

        public IEnumerable<KeyValuePair<Type, ISource>> GetSources() => _sources;

        public void AddRange(IEnumerable<ISource> units)
        {
            foreach (ISource source in units)
                Add(source);
        }

        public void Add(ISource unit)
        {
            if (Contains(unit.Type))
                throw new InvalidOperationException($"Source typeof {unit.Type} already added");

            _sources.Add(unit.Type, unit);
            SourceAdded?.Invoke(unit.Type, unit);
        }

        public bool Contains(Type type) => _sources.ContainsKey(type);

        public void RemoveRange(IEnumerable<Type> types)
        {
            foreach (Type source in types)
                Remove(source);
        }

        public void Remove(Type type)
        {
            if(_sources.TryGetValue(type,out ISource source) == false)
                throw new InvalidOperationException($"There is no {type}");

            _sources.Remove(type);
            SourceRemoved?.Invoke(type, source);
        }

        public bool TryGet(Type type, out object value)
        {
            if (_sources.TryGetValue(type, out ISource unit))
            {
                value = unit.Value;
                return true;
            }

            value = default;
            return false;
        }
    }
}