using System;

namespace DI
{
    public class FuncSource<T> : Source<T>
    {
        private Func<T> _func;

        public FuncSource(Func<T> func) => _func = func;

        public override T GetValue() => _func.Invoke();
    }
}