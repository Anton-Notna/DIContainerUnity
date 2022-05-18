using UnityEngine;

namespace DI.Example
{
    public class SomeDummyClass
    {
        private static bool _log = false;

        public SomeDummyClass(float value)
        {
            if(_log)
                Debug.Log($"SomeDummyClass created with value {value}");
        }
    }

    [DITarget]
    public class SomeComponent : MonoBehaviour
    {
        private static bool _log = false;

        [DIFIeld]
        private GameObject _someObject2;
        [DIFIeld]
        private Transform _someTransform;
        [DIFIeld]
        private Camera _camera;

        [DIMethod]
        private void InjectEmpty()
        {
            if (_log)
                Debug.Log("InjectEmpty invoked");
        }

        [DIMethod]
        private void InjectArg(MeshRenderer meshRenderer)
        {
            if (_log) 
                Debug.Log($"InjectArg invoked, meshRenderer: {meshRenderer.name}");
        }

        [DIMethod]
        private void InjectArgs(MeshRenderer meshRenderer, MeshFilter meshFilter)
        {
            if (_log)
                Debug.Log($"InjectArgs invoked, meshRenderer: {meshRenderer.name}, meshFilter: {meshFilter.name}");
        }

        [DIMethod]
        private void InjectDummyClass(SomeDummyClass dummy)
        {
            if (_log)
                Debug.Log($"InjectDummyClass invoked");
        }
    }
}