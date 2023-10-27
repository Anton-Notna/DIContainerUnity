using System;
using System.Collections.Generic;
using System.Reflection;

namespace DI
{
    public class CashedMethod
    {
        public readonly IReadOnlyList<Type> Parameters;
        private readonly MethodInfo _method;

        public CashedMethod(MethodInfo methodInfo)
        {
            _method = methodInfo;

            ParameterInfo[] parameters = _method.GetParameters();
            List<Type> parameterTypes = new List<Type>(parameters.Length);
            for (int i = 0; i < parameters.Length; i++)
                parameterTypes.Add(parameters[i].ParameterType);

            Parameters = parameterTypes;
        }

        public void Invoke(object target) => _method.Invoke(target, null);

        public void Invoke(object target, object[] args) => _method.Invoke(target, args);
    }
   
}