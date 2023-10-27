using System;
using System.Reflection;

namespace DI
{
    public class CashedTarget
    {
        public readonly Type Type;
        private readonly FieldInfo _field;

        public CashedTarget(FieldInfo field)
        {
            _field = field;
            Type = _field.FieldType;
        }

        public object GetTarget(object parent) => _field.GetValue(parent);
    }
}