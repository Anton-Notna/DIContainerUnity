using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace DI
{
    public static class Constants
    {
        public const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
    }

    public class DIContainer : IDIInjector
    {
        private Dictionary<Type, ISource> _sources = new Dictionary<Type, ISource>();

        public void Inject(GameObject root) => Inject(root.GetTarget());

        public void Inject(IInjectionTarget target)
        {
            foreach ((FieldInfo, object) pair in target.GetFields())
                pair.Item1.SetValue(pair.Item2, GetObject(pair.Item1.FieldType));

            foreach ((MethodInfo, object) pair in target.GetMethods())
                InjectMethod(pair.Item1, pair.Item2);
        }

        public void AddSource(ISource source)
        {
            if (_sources.ContainsKey(source.Type))
                throw new InvalidOperationException($"Source typeof {source.Type} already added");

            _sources.Add(source.Type, source);
        }

        public void RemoveSource<T>() => RemoveSource(typeof(T));

        public void RemoveSource(Type type)
        {
            _sources.Remove(type);
        }

        public bool ContainsSource<T>() => ContainsSource(typeof(T));

        public bool ContainsSource(Type type) => _sources.ContainsKey(type);

        public void ClearSources() => _sources.Clear();

        private void InjectMethod(MethodInfo method, object target)
        {
            ParameterInfo[] parameters = method.GetParameters();
            object[] args = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
                args[i] = GetObject(parameters[i].ParameterType);

            method.Invoke(target, args);
        }

        private object GetObject(Type type)
        {
            if (_sources.TryGetValue(type, out ISource source) == false)
                throw new NullReferenceException($"Try to inject from non existing source typeof {type}");

            return source.Value;
        }
    }
}