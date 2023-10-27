using System;

namespace DI
{
    public class SingletonConstructorSource<T> : Source<T>, IDisposable where T : new()
    {
        private readonly T _value;

        public SingletonConstructorSource() => _value = new T();

        public override T GetValue() => _value;

        public void Dispose()
        {
            if (_value != null && _value is IDisposable disposable)
                disposable.Dispose();
        }
    }
}