using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace DI
{
    // TODO:
    // (?) Injecting source when adding
    // (?) Remove generic from IReadOnlyDIContainer
    // Is it possible to improve perfomance of MethodBase.Invoke()/FieldInfo.SetValue()?

    public class GameObjectDIContainer : IReadOnlyDIContainer<GameObject>
    {
        private IErrorProvider _errorProvider;
        private ContextAgregator _contextAgregator;
        private InjectableMembersProvider _injectableMembers;

        private List<Component> _components = new List<Component>();
        Dictionary<object, object> _injected = new Dictionary<object, object>(999);
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

        public void InjectObject(object obj)
        {
            if (_injectableMembers.Injectable(obj.GetType()))
                InjectUnit(obj, obj.GetType(), _injected);

            _injected.Clear();
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
                    InjectUnit(_components[c], type, _injected);
            }
            //Debug.Log(_injected.Count);
            _injected.Clear();
        }

        private void InjectFiltred(IReadOnlyList<Component> components)
        {
            for (int c = 0; c < components.Count; c++)
                InjectUnit(components[c], components[c].GetType(), _injected);

            _injected.Clear();
        }

        private void InjectUnit(object target, Type type, Dictionary<object, object> injected)
        {
            /*if (injected.ContainsKey(target))
                return;

            injected.Add(target, null);*/

            CashedMembers members = _injectableMembers.GetMembers(type);

            for (int i = 0; i < members.Targets.Count; i++)
                InjectChildTarget(members.Targets[i], target, injected);

            for (int i = 0; i < members.Fields.Count; i++)
                SetField(members.Fields[i], target);

            for (int i = 0; i < members.Methods.Count; i++)
                InvokeMethod(members.Methods[i], target);
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

        private void InjectChildTarget(CashedTarget cashed, object parent, Dictionary<object, object> injected)
        {
            object target = cashed.GetTarget(parent);
            InjectUnit(target, cashed.Type, injected);
        }

        private object GetObject(Type type) => _contextAgregator.GetObject(type);
    }

    public static class HashSetExtensions
    {
        private static class HashSetDelegateHolder<T>
        {
            private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic;
            public static MethodInfo InitializeMethod { get; } = typeof(HashSet<T>).GetMethod("Initialize", Flags);
        }

        public static void SetCapacity<T>(this HashSet<T> hs, int capacity)
        {
            HashSetDelegateHolder<T>.InitializeMethod.Invoke(hs, new object[] { capacity });
        }

        public static HashSet<T> GetHashSet<T>(int capacity)
        {
            var hashSet = new HashSet<T>();
            hashSet.SetCapacity(capacity);
            return hashSet;
        }
    }
}