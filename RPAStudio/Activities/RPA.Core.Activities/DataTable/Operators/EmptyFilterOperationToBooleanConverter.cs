using System;
using System.Globalization;
using System.Windows.Data;


namespace RPA.Core.Activities.DataTableActivity.Operators
{
    public class EmptyFilterOperationToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            FilterOperator ope = (FilterOperator)value;
            if (ope == FilterOperator.NOTEMPTY || ope == FilterOperator.EMPTY)
                return false;
            else
                return true;
            //if (value == null)
            //    return Binding.DoNothing;
            //else
            //     return !FilterOperationArgument.IsEmptyOperator((FilterOperator)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
