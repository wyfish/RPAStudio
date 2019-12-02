using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
namespace Plugins.Shared.Library.Converters
{
    public class IntCollectionToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IEnumerable<int> enumerable = value as IEnumerable<int>;
            if (enumerable == null)
            {
                return null;
            }
            string text = string.Empty;
            string text2 = parameter as string;
            if (string.IsNullOrWhiteSpace(text2))
            {
                text2 = ",";
            }
            foreach (int current in enumerable)
            {
                if (!string.IsNullOrWhiteSpace(text))
                {
                    text = text + text2 + " ";
                }
                text += current.ToString();
            }
            return text;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string text = value as string;
            List<int> list = new List<int>();
            if (string.IsNullOrWhiteSpace(text))
            {
                return list;
            }
            string text2 = parameter as string;
            if (string.IsNullOrWhiteSpace(text2))
            {
                text2 = ",";
            }
            string[] array = text.Split(text2.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (array == null || array.Count<string>() == 0)
            {
                return list;
            }
            string[] array2 = array;
            for (int i = 0; i < array2.Length; i++)
            {
                string s = array2[i];
                try
                {
                    int item = int.Parse(s);
                    list.Add(item);
                }
                catch (Exception)
                {
                }
            }
            return list;
        }
    }
}
