using System.Collections;
using UnityEngine;
using Zenject;

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
        private GameObject _rootToInject;
        [SerializeField]
        private GameObject _rootToInject2;

        GameObjectDIContainer _dIContainer;

        private IEnumerator Start()
        {
            Context context1 = new Context();
            Context context2 = new Context();

            context1.Add(_gameObject.AsSources<GameObject, GameObject, Object>());
            context1.Add(FindObjectOfType<Camera>().AsSource());
            context1.Add(_gameObject.AsSource<Object>());
            context2.Add(new SingletonSource<Transform>(_transform));
            context2.Add(new SingletonSource<MeshRenderer>(_meshRenderer));
            context2.Add(new SingletonSource<MeshFilter>(_meshFilter.gameObject.GetComponent<MeshFilter>()));
            context2.Add(new SingletonSource<SomeDummyClass>(new SomeDummyClass(5f)));

            _dIContainer = new GameObjectDIContainer(new UnityLogErrorProvider());
            _dIContainer.AddContext(context1);
            _dIContainer.AddContext(context2);

            DiContainer diContainer = new DiContainer();
            diContainer.Bind<GameObject>().FromInstance(_gameObject).AsSingle();
            diContainer.Bind<Camera>().FromInstance(FindObjectOfType<Camera>()).AsSingle();
            diContainer.Bind<Object>().FromInstance(_gameObject).AsSingle();
            diContainer.Bind<Transform>().FromInstance(_transform).AsSingle();

            diContainer.Bind<MeshRenderer>().FromInstance(_meshRenderer).AsSingle();
            diContainer.Bind<MeshFilter>().FromInstance(_meshFilter.gameObject.GetComponent<MeshFilter>()).AsSingle();
            diContainer.Bind<SomeDummyClass>().FromInstance(new SomeDummyClass(5f)).AsSingle();


            yield return new WaitForSeconds(0.5f);
            //_dIContainer.InjectObject(new SomeRecursevlyClass3());
            _dIContainer.Inject(_rootToInject2);
            yield return new WaitForSeconds(0.5f);
            diContainer.InjectGameObject(_rootToInject2);

            yield return new WaitForSeconds(0.5f);
            _dIContainer.Inject(_rootToInject);
            yield return new WaitForSeconds(0.5f);
            diContainer.InjectGameObject(_rootToInject);

            yield return new WaitForSeconds(0.2f);
            Debug.LogError("Pause");

        }

        [ContextMenu(nameof(Inject))]
        private void Inject()
        {
            _dIContainer.Inject(_rootToInject);
        }
    }
}