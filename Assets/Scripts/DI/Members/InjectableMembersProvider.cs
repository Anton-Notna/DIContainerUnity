using System;
using System.Collections.Generic;
using System.Reflection;

namespace DI
{
    public class InjectableMembersProvider
    {
        private HashSet<Type> _closedTypes = new HashSet<Type>();
        private Dictionary<Type, CashedMembers> _injectableTypes = new Dictionary<Type, CashedMembers>();

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

        public bool Injectable(Type type)
        {
            if (_closedTypes.Contains(type))
                return false;

            if (type.IsDefined(typeof(DITargetAttribute), true))
                return true;

            _closedTypes.Add(type);
            return false;
        }

        public CashedMembers GetMembers(Type type)
        {
            if (_injectableTypes.TryGetValue(type, out CashedMembers members))
                return members;

            return CashInjectableType(type);
        }

        private CashedMembers CashInjectableType(Type type)
        {
            CashedMembers members = new CashedMembers(type);
            _injectableTypes.Add(type, members);
            return members;
        }
    }
}