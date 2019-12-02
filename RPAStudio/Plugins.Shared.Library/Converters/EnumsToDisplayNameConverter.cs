using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Data;

namespace Plugins.Shared.Library.Converters
{
    public class EnumsToDisplayNameConverter : IValueConverter
    {
        public static Func<object, string> fun01;

        public static Func<string, bool> fun02;

        internal string getName(object op)
        {
            Enum expr_06 = op as Enum;
            if (expr_06 == null)
            {
                return null;
            }
            return expr_06.GetDisplayName();
        }
        public static readonly EnumsToDisplayNameConverter c9;


        internal bool b__0_1(string a)
        {
            return a != null;
        }

        public EnumsToDisplayNameConverter()
        {
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IEnumerable source;
            if ((source = (value as IEnumerable)) != null)
            {
                IEnumerable<object> expr_10 = source.Cast<object>();
                object arg_5F_0;
                if (expr_10 == null)
                {
                    arg_5F_0 = null;
                }
                else
                {
                    Func<object, string> arg_36_1 = null;
                    arg_36_1 = fun01;
                    if (fun01 == null)
                    {
                        arg_36_1 = (EnumsToDisplayNameConverter.fun01 = new Func<object, string>(getName));
                    }
                    IEnumerable<string> arg_5A_0 = expr_10.Select(arg_36_1);
                    Func<string, bool> arg_5A_1;
                    if ((arg_5A_1 = EnumsToDisplayNameConverter.fun02) == null)
                    {
                        arg_5A_1 = (EnumsToDisplayNameConverter.fun02 = new Func<string, bool>(b__0_1));
                    }
                    arg_5F_0 = arg_5A_0.Where(arg_5A_1);
                }
                return arg_5F_0 ?? Enumerable.Empty<string>();
            }
            else
            {
                return Enumerable.Empty<string>();
            }
        }


        //public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        //{
        //    IEnumerable enumerable;
        //    if ((enumerable = (value as IEnumerable)) != null)
        //    {
        //        //IEnumerable<object> expr_10 = enumerable.Cast<object>();
        //        //Func<object, string> arg_36_1 = (fun01 = new Func<object, string>(getName));
        //        //IEnumerable<string> arg_5A_0 = expr_10.Select(arg_36_1);

        //        //return arg_5A_0.AsEnumerable();
        //        List<string> sss = new List<string>();
        //        foreach (var f in enumerable)
        //        {
        //            string name = getName(f);
        //            sss.Add(name);
        //        }
        //        IEnumerable<string> mmm = sss.AsEnumerable<string>();
        //        return mmm;
        //    }
        //    else
        //    {
        //        return Enumerable.Empty<string>();
        //    }
        //}
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}