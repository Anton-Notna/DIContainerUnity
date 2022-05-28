using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace DI
{
    // TODO:
    // Compare with Zenject
    // Non component injection
    // Many contexts
    // (?) Injecting source when adding
    // Move _injectableTypes/_closedTypes/Prebake/CashInjectableType/IsInjectableType to another module
    // (?) Remove generic from IReadOnlyDIContainer
    // Is it possible to improve perfomance of MethodBase.Invoke()/FieldInfo.SetValue()?

    public class GameObjectDIContainer : IReadOnlyDIContainer<GameObject>
    {
        private class CashedMethod
        {
            public readonly MethodInfo Method;
            public readonly IReadOnlyList<Type> Parameters;

            public CashedMethod(MethodInfo methodInfo)
            {
                Method = methodInfo;

                ParameterInfo[] parameters = Method.GetParameters();
                List<Type> parameterTypes = new List<Type>(parameters.Length);
                for (int i = 0; i < parameters.Length; i++)
                    parameterTypes.Add(parameters[i].ParameterType);

                Parameters = parameterTypes;
            }
        }

        private class CashedMembers
        {
            public readonly IReadOnlyList<FieldInfo> Fields;
            public readonly IReadOnlyList<CashedMethod> Methods;

            public CashedMembers(Type type)
            {
                List<FieldInfo> cashedFields = new List<FieldInfo>();
                List<CashedMethod> cashedMethods = new List<CashedMethod>();

                FieldInfo[] fields = type.GetFields(Constants.Flags);
                for (int f = 0; f < fields.Length; f++)
                    if (fields[f].IsDefined(typeof(DIFIeldAttribute), true))
                        cashedFields.Add(fields[f]);

                MethodInfo[] methods = type.GetMethods(Constants.Flags);
                for (int m = 0; m < methods.Length; m++)
                    if (methods[m].IsDefined(typeof(DIMethodAttribute), true))
                        cashedMethods.Add(new CashedMethod(methods[m]));

                Fields = cashedFields;
                Methods = cashedMethods;
            }
        }

        private IReadOnlyContext _context;
        private IErrorProvider _errorProvider;

        private HashSet<Type> _closedTypes = new HashSet<Type>();
        private Dictionary<Type, CashedMembers> _injectableTypes = new Dictionary<Type, CashedMembers>();

        private List<Component> _components = new List<Component>();
        private ReusableArray<object> _reusableArray = new ReusableArray<object>(10);

        public GameObjectDIContainer(IReadOnlyContext context, IErrorProvider errorProvider)
        {
            _context = context;
            _errorProvider = errorProvider;
        }

        public void Prebake()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int a = 0; a < assemblies.Length; a++)
            {
                Type[] types = assemblies[a].GetTypes();
                for (int t = 0; t < types.Length; t++)
                {
                    Type type = types[t];
                    if (type.IsDefined(typeof(DITargetAttribute), true))
                        CashInjectableType(type);
                    else
                        _closedTypes.Add(type);
                }
            }
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

                if (IsInjectableType(type))
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
            if (_injectableTypes.TryGetValue(type, out CashedMembers members) == false)
                members = CashInjectableType(type);

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
                method.Method.Invoke(target, null);
                return;
            }

            object[] objects = _reusableArray.Get(parameters.Count);
            for (int i = 0; i < parameters.Count; i++)
                objects[i] = GetObject(parameters[i]);

            method.Method.Invoke(target, objects);
        }

        private bool IsInjectableType(Type type)
        {
            if (_closedTypes.Contains(type))
                return false;

            if (type.IsDefined(typeof(DITargetAttribute), true))
                return true;
                       
            _closedTypes.Add(type);
            return false;
        }

        private CashedMembers CashInjectableType(Type type)
        {
            CashedMembers members = new CashedMembers(type);
            _injectableTypes.Add(type, members);
            return members;
        }

        private object GetObject(Type type)
        {
            if (_context.TryGet(type, out object value))
                return value;

            _errorProvider.Throw(new NullReferenceException($"Try to inject from non existing source typeof {type}"));
            return GetDefault(type);
        }

        private static object GetDefault(Type type) => type.IsValueType ? Activator.CreateInstance(type) : null;
    }
}