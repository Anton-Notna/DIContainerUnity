using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DI
{
    public class BackedInjectionTarget : MonoBehaviour, IInjectionTarget
    {
        [Serializable]
        private abstract class Member
        {
            [SerializeField]
            private Component _component;
            [SerializeField]
            private string _name;

            protected Component Component => _component;
            protected string Name => _name;

            public Member((MemberInfo, Component) field)
            {
                _name = field.Item1.Name;
                _component = field.Item2;
            }
        }

        [Serializable]
        private class Field : Member
        {
            public Field((FieldInfo, Component) field) : base(field) { }

            public (FieldInfo, Component) GetField() => 
                (Component.GetType().GetField(Name, Constants.Flags), Component);
        }

        [Serializable]
        private class Method : Member
        {
            public Method((MethodInfo, Component) field) : base(field) { }

            public (MethodInfo, Component) GetMethod() =>
                (Component.GetType().GetMethod(Name, Constants.Flags), Component);
        }

        [SerializeField]
        private List<Field> _fields = new List<Field>();
        [SerializeField]
        private List<Method> _methods = new List<Method>();

        public IEnumerable<(FieldInfo, Component)> GetFields()
        {
            for (int i = 0; i < _fields.Count; i++)
                yield return _fields[i].GetField();
        }

        public IEnumerable<(MethodInfo, Component)> GetMethods()
        {
            for (int i = 0; i < _methods.Count; i++)
                yield return _methods[i].GetMethod();
        }

        [ContextMenu(nameof(Bake))]
        public void Bake()
        {
            SearchingInjectionTarget searching = new SearchingInjectionTarget(gameObject);

            _fields.Clear();
            _fields.AddRange(searching.GetFields().Select(field => new Field(field)));

            _methods.Clear();
            _methods.AddRange(searching.GetMethods().Select(method => new Method(method)));
        }
    }
}