using System;
using System.Collections.Generic;
using System.Reflection;

namespace DI
{
    public class CashedMembers
    {
        public readonly IReadOnlyList<FieldInfo> Fields;
        public readonly IReadOnlyList<CashedMethod> Methods;
        public readonly IReadOnlyList<CashedTarget> Targets;

        public CashedMembers(Type type)
        {
            List<FieldInfo> cashedFields = new List<FieldInfo>();
            List<CashedMethod> cashedMethods = new List<CashedMethod>();
            List<CashedTarget> cashedTargets = new List<CashedTarget>();

            IReadOnlyList<Type> types = GetBases(type);
            for (int t = 0; t < types.Count; t++)
            {
                Type current = types[t];

                FieldInfo[] fields = current.GetFields(Constants.Flags);
                for (int f = 0; f < fields.Length; f++)
                {
                    if (fields[f].IsDefined(typeof(DIFIeldAttribute), true))
                        cashedFields.Add(fields[f]);
                    else if (fields[f].IsDefined(typeof(DITargetAttribute), true) &&
                        fields[f].FieldType.IsDefined(typeof(DITargetAttribute), true))
                        cashedTargets.Add(new CashedTarget(fields[f]));
                }

                MethodInfo[] methods = current.GetMethods(Constants.Flags);
                for (int m = 0; m < methods.Length; m++)
                {
                    if (methods[m].IsDefined(typeof(DIMethodAttribute), true))
                        cashedMethods.Add(new CashedMethod(methods[m]));
                }
            }

            Fields = cashedFields;
            Methods = cashedMethods;
            Targets = cashedTargets;
        }

        private IReadOnlyList<Type> GetBases(Type source)
        {
            List<Type> types = new List<Type>();
            Type current = source;
            while (current != null)
            {
                types.Add(current);
                current = current.BaseType;
            }

            types.Reverse();
            return types;
        }
    }
}