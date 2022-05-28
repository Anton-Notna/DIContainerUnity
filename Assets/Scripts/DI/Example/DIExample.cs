using System.Collections;
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
        private GameObject _rootToInject;
        [SerializeField]
        private GameObject _rootToInject2;

        GameObjectDIContainer _dIContainer;

        private IEnumerator Start()
        {
            Context source = new Context();

            source.Add(new SingletonSource<GameObject>(_gameObject));
            source.Add(new SingletonSource<Camera>(FindObjectOfType<Camera>()));
            source.Add(new SingletonSource<Object>(_gameObject));
            source.Add(new SingletonSource<Transform>(_transform));
            source.Add(new SingletonSource<MeshRenderer>(_meshRenderer));
            //source.Add(new FuncSource<MeshFilter>(_meshFilter.gameObject.GetComponent<MeshFilter>));
            source.Add(new SingletonSource<MeshFilter>(_meshFilter.gameObject.GetComponent<MeshFilter>()));
            //source.Add(new InstanceSource<SomeDummyClass>(5f));
            source.Add(new SingletonSource<SomeDummyClass>(new SomeDummyClass(5f)));

            _dIContainer = new GameObjectDIContainer(source, new UnityLogErrorProvider());
            _dIContainer.Prebake();

            yield return new WaitForSeconds(1f);


            _dIContainer.Inject(_rootToInject2);
            yield return new WaitForSeconds(1f);
            _dIContainer.Inject(_rootToInject);

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