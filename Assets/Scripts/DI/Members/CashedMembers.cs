using System;
using System.Collections.Generic;
using System.Reflection;

namespace DI
{
    public class CashedMembers
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
}