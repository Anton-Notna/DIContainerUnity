using System;

namespace DI
{
    public abstract class SourceUnit<T> : ISourceUnit 
    {
        public Type Type => typeof(T);
        public object Value => GetValue();

        public abstract T GetValue();
    }
}