using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Plugins.Shared.Library.Converters
{
    public class DataTableColumnToTypeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Count<object>() < 2 || values[0] == null || values[0].GetType() != typeof(string))
            {
                return Binding.DoNothing;
            }
            else
            {
                try
                {
                    string name = values[0] as string;
                    DataTable dataTable = values[1] as DataTable;
                    if (dataTable.Columns.Contains(name))
                    {
                        DataColumn dataColumn = dataTable.Columns[name];
                        return "(" + dataColumn.DataType.Name + ")";
                    }
                    else
                    {
                        return "undefined";
                    }
                }
                catch
                {
                    throw;
                }
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[]
            {
                Binding.DoNothing,
                Binding.DoNothing
            };
        }
    }
}
