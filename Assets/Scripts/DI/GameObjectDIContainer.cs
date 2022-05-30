using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace DI
{
    // TODO:
    // Non component injection
    // (?) Injecting source when adding
    // (?) Remove generic from IReadOnlyDIContainer
    // Is it possible to improve perfomance of MethodBase.Invoke()/FieldInfo.SetValue()?

    public class GameObjectDIContainer : IReadOnlyDIContainer<GameObject>
    {
        private IErrorProvider _errorProvider;
        private ContextAgregator _contextAgregator;
        private InjectableMembersProvider _injectableMembers;

        private List<Component> _components = new List<Component>();
        private ReusableArray<object> _reusableArray = new ReusableArray<object>(10);

        public GameObjectDIContainer(IErrorProvider errorProvider, params IReadOnlyContext[] contexts)
        {
            _errorProvider = errorProvider;
            _contextAgregator = new ContextAgregator(_errorProvider);
            _injectableMembers = new InjectableMembersProvider();
            _injectableMembers.Prebake();

            if (contexts == null || contexts.Length == 0)
                return;

            for (int i = 0; i < contexts.Length; i++)
                _contextAgregator.Add(contexts[i]);
        }

        public void Inject(GameObject target) 
        {
            if (target.TryGetComponent(out BackedInjection backedInjection))
                InjectFiltred(backedInjection.Components);
            else
                InjectSimple(target);
        }

        public void AddContext(IReadOnlyContext context) => _contextAgregator.Add(context);

        public void RemoveContext(IReadOnlyContext context) => _contextAgregator.Remove(context);

        private void InjectSimple(GameObject target)
        {
            target.GetComponentsInChildren(true, _components);
            for (int c = 0; c < _components.Count; c++)
            {
                Type type = _components[c].GetType();

                if (_injectableMembers.Injectable(type))
                    InjectUnit(_components[c], type);
            }
        }

        private void InjectFiltred(IReadOnlyList<Component> components)
        {
            for (int c = 0; c < components.Count; c++)
                InjectUnit(components[c], components[c].GetType());
        }

        private void InjectUnit(Component component, Type type)
        {
            CashedMembers members = _injectableMembers.GetMembers(type);

            for (int i = 0; i < members.Fields.Count; i++)
                SetField(members.Fields[i], component);

            for (int i = 0; i < members.Methods.Count; i++)
                InvokeMethod(members.Methods[i], component);
        }

        private void SetField(FieldInfo field, object target) => field.SetValue(target, GetObject(field.FieldType));

        private void InvokeMethod(CashedMethod method, object target)
        {
            IReadOnlyList<Type> parameters = method.Parameters;

            if (parameters.Count == 0)
            {
                method.Invoke(target);
                return;
            }

            object[] objects = _reusableArray.Get(parameters.Count);
            for (int i = 0; i < parameters.Count; i++)
                objects[i] = GetObject(parameters[i]);

            method.Invoke(target, objects);
        }

        private object GetObject(Type type) => _contextAgregator.GetObject(type);
    }
}