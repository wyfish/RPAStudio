using Plugins.Shared.Library.Librarys;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugins.Shared.Library.Converters
{
    public class GenericValueTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return true;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return this.IsKnownType(destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return new GenericValue(value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value != null && value.GetType().IsAssignableFrom(destinationType))
            {
                return value;
            }
            GenericValue genericValue = (GenericValue)value;
            if (genericValue != null as GenericValue && genericValue.RawValue != null && genericValue.RawValue.GetType().IsAssignableFrom(destinationType))
            {
                return genericValue.RawValue;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        private bool IsKnownType(Type sourceType)
        {
            return sourceType == typeof(GenericValue) || sourceType == typeof(string) || sourceType == typeof(char) || sourceType == typeof(byte) || sourceType == typeof(decimal) || sourceType == typeof(int) || sourceType == typeof(float) || sourceType == typeof(long) || sourceType == typeof(short) || sourceType == typeof(double) || sourceType == typeof(DateTime) || sourceType == typeof(TimeSpan) || sourceType == typeof(bool);
        }
    }
}
