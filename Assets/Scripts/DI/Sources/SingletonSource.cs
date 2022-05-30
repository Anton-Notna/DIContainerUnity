namespace DI
{
    public class SingletonSource<T> : Source<T>
    {
        private T _value;

        public SingletonSource(T value) => _value = value;

        public override T GetValue() => _value;
    }
}