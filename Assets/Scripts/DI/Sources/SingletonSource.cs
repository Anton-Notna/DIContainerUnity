using System;

namespace DI
{
    public class SingletonSource : ISource
    {
        public SingletonSource(object value)
        {
            Value = value;
            Type = value.GetType();
        }

        public Type Type { get; private set; }

        public object Value { get; private set; }
    }
}