using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace DI
{
    public class GameObjectDIContainer : IReadOnlyDIContainer<GameObject>
    {
        private class CashedMembers
        {
            public readonly IReadOnlyList<FieldInfo> Fields;
            public readonly IReadOnlyList<MethodInfo> Methods;

            public CashedMembers(IReadOnlyList<FieldInfo> fields, IReadOnlyList<MethodInfo> methods)
            {
                Fields = fields;
                Methods = methods;
            }
        }

        private IReadOnlySource _source;
        private IErrorProvider _errorProvider;
        private List<Component> _components = new List<Component>();
        private HashSet<Type> _badTypes = new HashSet<Type>();
        private Dictionary<Type, CashedMembers> _goodTypes = new Dictionary<Type, CashedMembers>();

        public GameObjectDIContainer(IReadOnlySource source, IErrorProvider errorProvider)
        {
            _source = source;
            _errorProvider = errorProvider;
        }

        public void Inject(GameObject target) 
        {
            if (target.TryGetComponent(out BackedInjection backedInjection))
                InjectFiltred(backedInjection.Components);
            else
                InjectSimple(target);
        }

        private void InjectSimple(GameObject target)
        {
            target.GetComponentsInChildren(true, _components);
            for (int c = 0; c < _components.Count; c++)
            {
                Type type = _components[c].GetType();
                if (_badTypes.Contains(type))
                    continue;

                if (type.IsDefined(typeof(DITargetAttribute), true) == false)
                {
                    _badTypes.Add(type);
                    continue;
                }

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
            if (_goodTypes.TryGetValue(type, out CashedMembers members) == false)
            {
                List<FieldInfo> cashedFields = new List<FieldInfo>();
                List<MethodInfo> cashedMethods = new List<MethodInfo>();

                FieldInfo[] fields = type.GetFields(Constants.Flags);
                for (int f = 0; f < fields.Length; f++)
                    if (fields[f].IsDefined(typeof(DIFIeldAttribute), true))
                        cashedFields.Add(fields[f]);

                MethodInfo[] methods = type.GetMethods(Constants.Flags);
                for (int m = 0; m < methods.Length; m++)
                    if (methods[m].IsDefined(typeof(DIMethodAttribute), true))
                        cashedMethods.Add(methods[m]);

                members = new CashedMembers(cashedFields, cashedMethods);
                _goodTypes.Add(type, members);
            }

            for (int i = 0; i < members.Fields.Count; i++)
                SetField(members.Fields[i], component);

            for (int i = 0; i < members.Methods.Count; i++)
                InvokeMethod(members.Methods[i], component);
        }

        private void SetField(FieldInfo field, object target) => field.SetValue(target, GetObject(field.FieldType));

        private void InvokeMethod(MethodInfo method, object target)
        {
            ParameterInfo[] parameters = method.GetParameters();

            if (parameters.Length == 0)
            {
                method.Invoke(target, null);
                return;
            }

            object[] objects = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
                objects[i] = GetObject(parameters[i].ParameterType);

            method.Invoke(target, objects);
        }

        private object GetObject(Type type)
        {
            if (_source.TryGet(type, out object value))
                return value;

            _errorProvider.Throw(new NullReferenceException($"Try to inject from non existing source typeof {type}"));
            return GetDefault(type);
        }

        private static object GetDefault(Type type) => type.IsValueType ? Activator.CreateInstance(type) : null;
    }
}