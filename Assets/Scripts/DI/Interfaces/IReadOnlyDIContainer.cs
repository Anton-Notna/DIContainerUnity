namespace DI
{
    public interface IReadOnlyDIContainer<T>
    {
        public void Inject(T target);
    }
}