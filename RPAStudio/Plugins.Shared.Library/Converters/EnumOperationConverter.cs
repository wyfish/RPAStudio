using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Windows.Data;

namespace Plugins.Shared.Library.Converters
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumObj)
        {
            object[] customAttributes = enumObj.GetType().GetField(enumObj.ToString()).GetCustomAttributes(false);
            if (customAttributes.Length == 0)
            {
                return enumObj.ToString();
            }
            return (customAttributes[0] as DisplayAttribute).Name;
        }
    }

    public class EnumOperationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Enum enumObj = value as Enum;
            if (value == null)
            {
                return Binding.DoNothing;
            }
            return enumObj.GetDisplayName();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string b = (string)value;
            IEnumerator enumerator = Enum.GetValues(targetType).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Enum @enum = enumerator.Current as Enum;
                    if (@enum != null && @enum.GetDisplayName() == b)
                    {
                        return @enum;
                    }
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
            return Binding.DoNothing;
        }
    }
}
