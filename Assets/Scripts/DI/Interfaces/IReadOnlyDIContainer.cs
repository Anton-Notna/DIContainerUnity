using UnityEngine;

namespace DI
{
    public interface IReadOnlyDIContainer
    {
        public GameObject Inject(GameObject target);

        public object Inject(object obj);

        public T Instantiate<T>(params object[] subArgs);
    }
}