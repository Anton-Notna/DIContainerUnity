using UnityEngine;

namespace DI.Example
{
    public class SomeComponent : MonoBehaviour
    {
        [DIFIeld]
        private GameObject _someObject2;
        [DIFIeld]
        private Transform _someTransform;
        [DIFIeld]
        private Camera _camera;

        [DIMethod]
        private void InjectEmpty() =>
            Debug.Log("InjectEmpty invoked");

        [DIMethod]
        private void InjectArg(MeshRenderer meshRenderer) =>
            Debug.Log($"InjectArg invoked, meshRenderer: {meshRenderer.name}");

        [DIMethod]
        private void InjectArgs(MeshRenderer meshRenderer, MeshFilter meshFilter) =>
            Debug.Log($"InjectArgs invoked, meshRenderer: {meshRenderer.name}, meshFilter: {meshFilter.name}");
    }
}