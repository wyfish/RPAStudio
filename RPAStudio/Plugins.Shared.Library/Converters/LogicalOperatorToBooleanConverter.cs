using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace Plugins.Shared.Library.Converters
{
    public enum BooleanOperator
    {
        And,
        Or
    }
    public class LogicalOperatorToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (BooleanOperator)value == BooleanOperator.Or;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return BooleanOperator.Or;
            else
                return BooleanOperator.And;
        }
    }
}
