using System;

namespace DI
{
    public abstract class SourceUnit<T> : ISource 
    {
        public Type Type => typeof(T);
        public object Value => GetValue();

        public abstract T GetValue();
    }
}