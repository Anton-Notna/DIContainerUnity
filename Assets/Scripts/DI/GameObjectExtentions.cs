using UnityEngine;

namespace DI
{
    public static class GameObjectExtentions
    {
        public static IInjectionTarget GetTarget(this GameObject gameObject)
        {
            if (gameObject.TryGetComponent(out BackedInjectionTarget target))
                return target;

            return new SearchingInjectionTarget(gameObject);
        }
    }
}