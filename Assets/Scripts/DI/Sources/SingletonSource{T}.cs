using System;

namespace DI
{
    public class SingletonSource<T> : Source<T>, IDisposable
    {
        private readonly T _value;

        public SingletonSource(T value) => _value = value;

        public override T GetValue() => _value;

        public void Dispose()
        {
            if (_value != null && _value is IDisposable disposable)
                disposable.Dispose();
        }
    }
}