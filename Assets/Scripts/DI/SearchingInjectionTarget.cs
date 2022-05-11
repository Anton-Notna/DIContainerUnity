using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace DI
{
    public class SearchingInjectionTarget : IInjectionTarget
    {
        private GameObject _root;

        public SearchingInjectionTarget(GameObject root) => _root = root;

        public IEnumerable<(FieldInfo, Component)> GetFields()
        {
            foreach (Component component in _root.GetComponentsInChildren<Component>())
                foreach (FieldInfo field in component.GetType().GetFields(Constants.Flags))
                    if (field.GetCustomAttribute<DIFIeldAttribute>() != null)
                        yield return (field, component);
        }

        public IEnumerable<(MethodInfo, Component)> GetMethods()
        {
            foreach (Component component in _root.GetComponentsInChildren<Component>())
                foreach (MethodInfo method in component.GetType().GetMethods(Constants.Flags))
                    if (method.GetCustomAttribute<DIMethodAttribute>() != null)
                        yield return (method, component);
        }
    }
}