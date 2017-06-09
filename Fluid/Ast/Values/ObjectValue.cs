﻿using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Encodings.Web;

namespace Fluid.Ast.Values
{
    public class ObjectValue : FluidValue, INamedSet
    {
        private readonly object _value;

        public ObjectValue(object value)
        {
            _value = value;
        }

        public override bool Equals(FluidValue other)
        {
            return other is ObjectValue && ((ObjectValue)other)._value == _value;
        }

        public FluidValue GetProperty(string name)
        {
            var propertyInfo = _value.GetType().GetTypeInfo().GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
            return Create(propertyInfo.GetValue(_value));
        }

        public FluidValue GetIndex(FluidValue index)
        {
            PropertyInfo indexProperty = _value.GetType().GetProperties().FirstOrDefault(p => p.GetIndexParameters().Length == 1 && p.GetIndexParameters()[0].ParameterType == typeof(string));

            if (indexProperty == null)
            {
                return FluidValue.Nil;
            }

            return Create(indexProperty.GetValue(_value, new[] { index.ToObjectValue() }));
        }

        public override bool ToBooleanValue()
        {
            return _value != null;
        }

        public override double ToNumberValue()
        {
            return 0;
        }

        public override void WriteTo(TextWriter writer, TextEncoder encoder)
        {
            writer.Write(encoder.Encode(ToStringValue()));
        }

        public override string ToStringValue()
        {
            return _value.ToString();
        }

        public override object ToObjectValue()
        {
            return _value;
        }
    }
}
