using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Plugins.Shared.Library.Converters
{
    public class DataTableColumnToPropertiesConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Count<object>() < 2 || values[0] == null || values[0].GetType() != typeof(string))
            {
                return Binding.DoNothing;
            }
            try
            {
                string text = values[0] as string;
                DataTable dataTable = values[1] as DataTable;
                if (dataTable.Columns.Contains(text))
                {
                    DataColumn dataColumn = dataTable.Columns[text];
                    string str = (dataColumn.DefaultValue != null) ? dataColumn.DefaultValue.ToString() : string.Empty;
                    return "Column Name : " + text + Environment.NewLine + "Data Type : " + dataColumn.DataType.Name + Environment.NewLine + "Allow Null : " + dataColumn.AllowDBNull.ToString() + Environment.NewLine + "Auto Increment : " + dataColumn.AutoIncrement.ToString() + Environment.NewLine + "Default Value : " + str + Environment.NewLine + "Unique : " + dataColumn.Unique.ToString() + Environment.NewLine + "Maxlength : " + dataColumn.MaxLength.ToString();
                }
            }
            catch
            {
            }
            return Binding.DoNothing;
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
