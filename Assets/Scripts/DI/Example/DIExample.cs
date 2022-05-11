using UnityEngine;

namespace DI.Example
{
    public class DIExample : MonoBehaviour
    {
        [SerializeField]
        private GameObject _gameObject;
        [SerializeField]
        private Transform _transform;
        [SerializeField]
        private MeshRenderer _meshRenderer;
        [SerializeField]
        private MeshFilter _meshFilter;
        [Space]
        [SerializeField]
        private BackedInjectionTarget _rootToInject;

        private void Start()
        {
            DIContainer dIContainer = new DIContainer();

            dIContainer.AddSource(new SingletonSource<GameObject>(_gameObject));
            dIContainer.AddSource(new FuncSource<Camera>(FindObjectOfType<Camera>));
            dIContainer.AddSource(new SingletonSource<Object>(_gameObject));
            dIContainer.AddSource(new SingletonSource<Transform>(_transform));
            dIContainer.AddSource(new SingletonSource<MeshRenderer>(_meshRenderer));
            dIContainer.AddSource(new FuncSource<MeshFilter>(_meshFilter.gameObject.GetComponent<MeshFilter>));

            dIContainer.Inject(_rootToInject);
        }
    }
}