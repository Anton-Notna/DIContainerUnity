using UnityEngine;

namespace DI.Example
{
    [DITarget]
    public class SomeRecursevlyComponent : MonoBehaviour
    {
        [SerializeField]
        private SomeRecursevlyComponent _another;

        private SomeRecursevlyClass _recursevlyClass = new SomeRecursevlyClass();

        [DIMethod]
        private void Inject()
        {
            Debug.Log(gameObject.name);
        }

    }
}