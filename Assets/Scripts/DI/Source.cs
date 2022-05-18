using System;
using System.Collections.Generic;

namespace DI
{
    public class Source : ISource
    {
        private Dictionary<Type, ISourceUnit> _sources = new Dictionary<Type, ISourceUnit>();

        public void Add(ISourceUnit unit)
        {
            if (Contains(unit.Type))
                throw new InvalidOperationException($"Source typeof {unit.Type} already added");

            _sources.Add(unit.Type, unit);
        }

        public bool Contains(Type type) => _sources.ContainsKey(type);

        public void Remove(Type type)
        {
            if(_sources.ContainsKey(type) == false)
                throw new InvalidOperationException($"There is no {type}");

            _sources.Remove(type);
        }

        public bool TryGet(Type type, out object value)
        {
            if (_sources.TryGetValue(type, out ISourceUnit unit))
            {
                value = unit.Value;
                return true;
            }

            value = default;
            return false;
        }
    }
}