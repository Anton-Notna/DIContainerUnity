using System;
using System.Collections.Generic;

namespace DI
{
    public class Pool<T> where T : class
    {
        private class Pair
        {
            public T Value { get; private set; }

            public bool Free { get; set; }

            public Pair(T value, bool free)
            {
                Value = value;
                Free = free;
            }
        }

        private readonly Func<T> _source;
        private readonly List<Pair> _units = new List<Pair>();

        public Pool(Func<T> source) => _source = source;

        public T Get()
        {
            for (int i = 0; i < _units.Count; i++)
            {
                Pair unit = _units[i];
                if (unit.Free)
                {
                    unit.Free = false;
                    return unit.Value;
                }
            }

            Pair next = new Pair(_source.Invoke(), false);
            _units.Add(next);
            return next.Value;
        }

        public void Release(T value)
        {
            for (int i = 0; i < _units.Count; i++)
            {
                Pair unit = _units[i];
                if (unit.Value == value)
                {
                    unit.Free = true;
                    return;
                }
            }
        }
    }
}