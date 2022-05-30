using System;

namespace DI
{
    public class InstanceSource<T> : Source<T>
    {
        private object[] _args;

        public InstanceSource(params object[] args) => _args = args;

        public override T GetValue() => (T)Activator.CreateInstance(typeof(T), _args);
    }
}