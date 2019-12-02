using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace Plugins.Shared.Library.Converters
{
    public static class DataTableSeparatorsHelper
    {
        public static readonly IReadOnlyDictionary<string, string> SeparatorsDictionary = new Dictionary<string, string>
        {
        };
    }
    public class SeparatorsToFriendlyStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string text = value as string;
            if (string.IsNullOrEmpty(text))
            {
                return value;
            }
            foreach (KeyValuePair<string, string> current in DataTableSeparatorsHelper.SeparatorsDictionary)
            {
                text = text.Replace(current.Value, "[" + current.Key + "]");
            }
            return text;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string text = value as string;
            if (string.IsNullOrEmpty(text))
            {
                return value;
            }
            text = text.Trim();
            foreach (KeyValuePair<string, string> current in DataTableSeparatorsHelper.SeparatorsDictionary)
            {
                text = text.Replace("[" + current.Key + "]", current.Value);
            }
            return text;
        }
    }
}
