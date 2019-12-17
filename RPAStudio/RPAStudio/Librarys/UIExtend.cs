using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RPAStudio.Librarys
{
    public static class UIUtils
    {
        public static DependencyObject VisualUpwardSearch<T>(DependencyObject source)
        {
            while (source != null && source.GetType() != typeof(T))
                source = VisualTreeHelper.GetParent(source);

            return source;
        }

        public static ScrollViewer GetListBoxScrollViewer(ListBox listBox)
        {
            if (listBox != null && VisualTreeHelper.GetChildrenCount(listBox) > 0)
            {
                Border border = (Border)VisualTreeHelper.GetChild(listBox, 0);
                ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
                return scrollViewer;
            }

            return null;
        }
    }


    public class BoolVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value == false)
            {
                return Visibility.Hidden;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility vis = (Visibility)value;
            if (vis == Visibility.Hidden || vis == Visibility.Collapsed)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    public class BoolCollapsedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value == false)
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility vis = (Visibility)value;
            if (vis == Visibility.Hidden || vis == Visibility.Collapsed)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    public class BoolNotConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value == false)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !(bool)value;
        }
    }

    public class BoolNotVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value == false)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Hidden;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility vis = (Visibility)value;
            if (vis == Visibility.Hidden || vis == Visibility.Collapsed)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class BoolNotCollapsedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value == false)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility vis = (Visibility)value;
            if (vis == Visibility.Hidden || vis == Visibility.Collapsed)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }



    public class NullImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class SingleLineTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string s = (string)value;
            if(!string.IsNullOrEmpty(s))
            {
               s = s.Replace(Environment.NewLine, " ");
               s = s.Replace("\n", " ");
            }
           
            return s;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class SingleLineWithSpaceCombineTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string s = (string)value;
            if (!string.IsNullOrEmpty(s))
            {
                s = s.Replace(Environment.NewLine, " ");
                s = s.Replace("\n", " ");
                s = Regex.Replace(s, "\\s{2,}", " ");//s{2,} 中的s表示空格，数字2表示两个或以上的空格
            }
            
            return s;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }






}
