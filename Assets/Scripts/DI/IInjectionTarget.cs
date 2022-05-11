using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace DI
{
    public interface IInjectionTarget
    {
        public IEnumerable<(FieldInfo, Component)> GetFields();
        public IEnumerable<(MethodInfo, Component)> GetMethods();
    }
}