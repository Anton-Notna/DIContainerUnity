using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace DI
{
    // TODO:
    // Injecting source when adding
    // Is it possible to improve performance of MethodBase.Invoke()?
    // (?) Add DIConstructor
    public class GameObjectDIContainer : IReadOnlyDIContainer
    {
        private readonly IErrorProvider _errorProvider;
        private readonly ContextAggregator _contextAggregator;
        private readonly InjectableMembersProvider _injectableMembers;
        private readonly ConstructorsProvider _constructorsProvider;

        private readonly Pool<List<Component>> _components = new Pool<List<Component>>(() => new List<Component>());
        private readonly Pool<Dictionary<object, object>> _injected = new Pool<Dictionary<object, object>>(() => new Dictionary<object, object>(999));
        private readonly ReusableArray<object> _reusableArray = new ReusableArray<object>(10);

        public GameObjectDIContainer(IErrorProvider errorProvider, params IReadOnlyContext[] contexts)
        {
            _errorProvider = errorProvider;
            _contextAggregator = new ContextAggregator(_errorProvider);
            _constructorsProvider = new ConstructorsProvider(_errorProvider);
            _injectableMembers = new InjectableMembersProvider();
            _injectableMembers.Prebake();

            if (contexts == null || contexts.Length == 0)
                return;

            for (int i = 0; i < contexts.Length; i++)
                _contextAggregator.Add(contexts[i]);
        }

        public T Instantiate<T>(params object[] subArgs)
        {
            Type type = typeof(T);
            IReadOnlyList<Type> types = _constructorsProvider.GetConstructor(type);
            object[] args = _reusableArray.Get(types.Count);
            for (int i = 0; i < types.Count; i++)
            {
                Type argType = types[i];
                if (TryGet(subArgs, argType, out object arg))
                    args[i] = arg;
                else
                    args[i] = _contextAggregator.GetObject(argType);
            }

            return (T)Inject(Activator.CreateInstance(type, args));
        }

        public GameObject Inject(GameObject target)
        {
            if (target.TryGetComponent(out BackedInjection backedInjection))
                InjectFiltered(backedInjection.Components);
            else
                InjectSimple(target);

            return target;
        }

        public object Inject(object obj)
        {
            Dictionary<object, object> injected = _injected.Get();

            if (_injectableMembers.Injectable(obj.GetType()))
                InjectUnit(obj, obj.GetType(), injected);

            injected.Clear();
            _injected.Release(injected);

            return obj;
        }

        public void AddContext(IReadOnlyContext context) => _contextAggregator.Add(context);

        public void RemoveContext(IReadOnlyContext context) => _contextAggregator.Remove(context);

        private void InjectSimple(GameObject target)
        {
            Dictionary<object, object> injected = _injected.Get();
            List<Component> components = _components.Get();

            target.GetComponentsInChildren(true, components);
            for (int c = 0; c < components.Count; c++)
            {
                Component component = components[c];
                Type type = component.GetType();

                if (_injectableMembers.Injectable(type))
                    InjectUnit(component, type, injected);
            }

            injected.Clear();
            _injected.Release(injected);
            _components.Release(components);
        }

        private void InjectFiltered(IReadOnlyList<Component> components)
        {
            Dictionary<object, object> injected = _injected.Get();

            for (int c = 0; c < components.Count; c++)
                InjectUnit(components[c], components[c].GetType(), injected);

            injected.Clear();
            _injected.Release(injected);
        }

        private void InjectUnit(object target, Type type, Dictionary<object, object> injected)
        {
            if (injected.ContainsKey(target))
                return;

            injected.Add(target, null);

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

        private object GetObject(Type type) => _contextAggregator.GetObject(type);

        private static bool TryGet(object[] args, Type type, out object arg)
        {
            for (int i = 0; i < args.Length; i++)
            {
                object obj = args[i];

                if (obj != null && obj.GetType() == type)
                {
                    arg = obj;
                    return true;
                }
            }

            arg = default;
            return false;
        }
    }
}