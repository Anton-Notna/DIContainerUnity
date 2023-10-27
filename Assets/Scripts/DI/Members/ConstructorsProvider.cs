using System;
using System.Collections.Generic;
using System.Reflection;

namespace DI
{
    public class ConstructorsProvider
    {
        private readonly IErrorProvider _errorProvider;

        public ConstructorsProvider(IErrorProvider errorProvider) => _errorProvider = errorProvider;

        public IReadOnlyList<Type> GetConstructor(Type type)
        {
            ConstructorInfo[] constructors = type.GetConstructors(Constants.Flags);

            if(constructors.Length != 1)
            {
                _errorProvider.Throw(new IndexOutOfRangeException($"Wrong constructors count in {type}"));
                return null;
            }

            ParameterInfo[] parameters = constructors[0].GetParameters();
            List<Type> result = new List<Type>(parameters.Length);
            for (int i = 0; i < parameters.Length; i++)
                result.Add(parameters[i].ParameterType);

            return result;
        }
    }
}