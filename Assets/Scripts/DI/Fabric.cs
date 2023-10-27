using UnityEngine;

namespace DI
{
    [DITarget]
    public abstract class Fabric<TBase> where TBase : MonoBehaviour
    {
        private GameObjectDIContainer _container;

        [DIMethod]
        public void Init(GameObjectDIContainer container) =>
            _container = container;

        public T Instantiate<T>(T original) where T : TBase =>
            Instantiate(original, original.transform.position, original.transform.rotation, null);

        public T Instantiate<T>(T original, Transform parent) where T : TBase =>
            Instantiate(original, original.transform.position, original.transform.rotation, parent);

        public T Instantiate<T>(T original, Vector3 position, Quaternion rotation) where T : TBase =>
            Instantiate(original, position, rotation, null);

        public T Instantiate<T>(T original, Vector3 position, Transform parent) where T : TBase =>
            Instantiate(original, position, original.transform.rotation, parent);

        public T Instantiate<T>(T original, Vector3 position, Quaternion rotation, Transform parent) where T : TBase
        {
            T component = GameObject.Instantiate(original, position, rotation, parent);
            _container.Inject(component.gameObject);
            return component;
        }

    }
}