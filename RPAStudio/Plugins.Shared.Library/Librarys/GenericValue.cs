using Newtonsoft.Json;
using Plugins.Shared.Library.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace Plugins.Shared.Library.Librarys
{
    public interface GenericValueFormatProvider : IFormatProvider
    {
        string Pattern
        {
            get;
        }

        string DisplayName
        {
            get;
        }
    }

    [JsonConverter(typeof(GenericValueJsonConverter)), TypeConverter(typeof(GenericValueTypeConverter))]
    public class GenericValue : IComparable, IFormattable, IComparable<GenericValue>, IEquatable<GenericValue>, IConvertible
    {
        internal object RawValue
        {
            get;
            private set;
        }

        public IFormatProvider FormatProvider
        {
            get;
            set;
        }

        public static object GetRawValue(GenericValue value)
        {
            return value.RawValue;
        }

        public GenericValue(object value)
        {
            this.RawValue = value;
        }

        private T GetRawValue<T>()
        {
            return (T)((object)this.RawValue);
        }

        public static implicit operator bool(GenericValue value)
        {
            if (value == null as GenericValue || value.RawValue == null)
            {
                return false;
            }
            if (value.RawValue is string && string.IsNullOrWhiteSpace(value.GetRawValue<string>()))
            {
                return false;
            }
            bool result;
            try
            {
                result = Convert.ToBoolean(value.RawValue);
            }
            catch
            {
                result = true;
            }
            return result;
        }

        public static implicit operator int(GenericValue value)
        {
            if (value == null as GenericValue || value.RawValue == null)
            {
                return 0;
            }
            return (int)GenericValue.ConvertGeneric(value, typeof(int));
        }

        public static implicit operator double(GenericValue value)
        {
            if (value == null as GenericValue || value.RawValue == null)
            {
                return 0.0;
            }
            return (double)GenericValue.ConvertGeneric(value, typeof(double));
        }

        public static implicit operator decimal(GenericValue value)
        {
            if (value == null as GenericValue || value.RawValue == null)
            {
                return decimal.Zero;
            }
            return (decimal)GenericValue.ConvertGeneric(value, typeof(decimal));
        }

        public static implicit operator string(GenericValue value)
        {
            if (value == null as GenericValue || value.RawValue == null)
            {
                return null;
            }
            return value.ToString();
        }

        public static implicit operator DateTime(GenericValue value)
        {
            if (value == null as GenericValue || value.RawValue == null)
            {
                return default(DateTime);
            }
            if (value.RawValue.GetType() == typeof(double))
            {
                return DateTime.FromOADate((double)value.RawValue);
            }
            return (DateTime)GenericValue.ConvertGeneric(value, typeof(DateTime));
        }

        public static implicit operator TimeSpan(GenericValue value)
        {
            if (value == null as GenericValue || value.RawValue == null)
            {
                return default(TimeSpan);
            }
            return (TimeSpan)GenericValue.ConvertGeneric(value, typeof(TimeSpan));
        }

        private static object ConvertGeneric(GenericValue value, Type resultType)
        {
            object result;
            try
            {
                if (value.RawValue.GetType() == resultType)
                {
                    result = value.RawValue;
                }
                else if (value.FormatProvider == null || !(value.RawValue is string))
                {
                    result = Convert.ChangeType(value.RawValue, resultType);
                }
                else
                {
                    string text = value.RawValue.ToString();
                    if (resultType == typeof(TimeSpan))
                    {
                        DateTimeFormatInfo dateTimeFormatInfo;
                        if ((dateTimeFormatInfo = (value.FormatProvider.GetFormat(typeof(DateTimeFormatInfo)) as DateTimeFormatInfo)) != null)
                        {
                            result = TimeSpan.ParseExact(text, dateTimeFormatInfo.ShortTimePattern, value.FormatProvider);
                        }
                        else
                        {
                            result = TimeSpan.Parse(text, value.FormatProvider);
                        }
                    }
                    else
                    {
                        TypeCode typeCode = Type.GetTypeCode(resultType);
                        try
                        {
                            NumberFormatInfo expr_D7 = value.FormatProvider.GetFormat(typeof(NumberFormatInfo)) as NumberFormatInfo;
                            string text2 = (expr_D7 != null) ? expr_D7.PercentSymbol : null;
                            if (text.Contains(text2))
                            {
                                text = text.Replace(text2, string.Empty);
                            }
                        }
                        catch
                        {
                        }
                        switch (typeCode)
                        {
                            case TypeCode.Int16:
                            case TypeCode.Int32:
                            case TypeCode.Int64:
                                result = int.Parse(text, NumberStyles.Any, value.FormatProvider);
                                return result;
                            case TypeCode.Double:
                                result = double.Parse(text, NumberStyles.Any, value.FormatProvider);
                                return result;
                            case TypeCode.Decimal:
                                result = decimal.Parse(text, NumberStyles.Any, value.FormatProvider);
                                return result;
                            case TypeCode.DateTime:
                                {
                                    DateTimeFormatInfo dateTimeFormatInfo2 = value.FormatProvider.GetFormat(typeof(DateTimeFormatInfo)) as DateTimeFormatInfo;
                                    if (dateTimeFormatInfo2 != null)
                                    {
                                        result = DateTime.ParseExact(text, dateTimeFormatInfo2.ShortDatePattern, value.FormatProvider);
                                        return result;
                                    }
                                    result = DateTime.Parse(text, value.FormatProvider);
                                    return result;
                                }
                        }
                        result = Convert.ChangeType(value.RawValue, resultType);
                    }
                }
            }
            catch
            {
                throw new InvalidOperationException(string.Format("Cannot convert Generic Value to {0}", resultType));
            }
            return result;
        }

        public static implicit operator GenericValue(bool value)
        {
            return new GenericValue(value);
        }

        public static implicit operator GenericValue(int value)
        {
            return new GenericValue(value);
        }

        public static implicit operator GenericValue(double value)
        {
            return new GenericValue(value);
        }

        public static implicit operator GenericValue(decimal value)
        {
            return new GenericValue(value);
        }

        public static implicit operator GenericValue(string value)
        {
            return new GenericValue(value);
        }

        public static implicit operator GenericValue(DateTime value)
        {
            return new GenericValue(value);
        }

        public static implicit operator GenericValue(TimeSpan value)
        {
            return new GenericValue(value);
        }

        public static bool operator ==(GenericValue lvalue, GenericValue rvalue)
        {
            if (lvalue == rvalue)
            {
                return true;
            }
            if (lvalue != null as GenericValue && rvalue == null as GenericValue)
            {
                object expr_12 = lvalue.RawValue;
                if (string.IsNullOrEmpty((expr_12 != null) ? expr_12.ToString() : null))
                {
                    return true;
                }
            }
            if (lvalue == null as GenericValue && rvalue != null as GenericValue)
            {
                object expr_33 = rvalue.RawValue;
                if (string.IsNullOrEmpty((expr_33 != null) ? expr_33.ToString() : null))
                {
                    return true;
                }
            }
            try
            {
                return GenericValue.Compare(lvalue, rvalue) == 0;
            }
            catch
            {
            }
            return lvalue != null as GenericValue && lvalue.RawValue.Equals((rvalue != null as GenericValue) ? rvalue.RawValue : null);
        }

        public static bool operator ==(GenericValue lvalue, string rvalue)
        {
            return lvalue == rvalue;
        }

        public static bool operator ==(GenericValue lvalue, DateTime rvalue)
        {
            return lvalue == rvalue;
        }

        public static bool operator ==(GenericValue lvalue, int rvalue)
        {
            return lvalue == rvalue;
        }

        public static bool operator ==(GenericValue lvalue, double rvalue)
        {
            return lvalue == rvalue;
        }

        public static bool operator ==(GenericValue lvalue, decimal rvalue)
        {
            return lvalue == rvalue;
        }

        public static bool operator ==(string lvalue, GenericValue rvalue)
        {
            return lvalue == rvalue;
        }

        public static bool operator ==(double lvalue, GenericValue rvalue)
        {
            return lvalue == rvalue;
        }

        public static bool operator ==(int lvalue, GenericValue rvalue)
        {
            return lvalue == rvalue;
        }

        public static bool operator ==(decimal lvalue, GenericValue rvalue)
        {
            return lvalue == rvalue;
        }

        public static bool operator ==(DateTime lvalue, GenericValue rvalue)
        {
            return lvalue == rvalue;
        }

        public static bool operator !=(GenericValue lvalue, GenericValue rvalue)
        {
            return !(lvalue == rvalue);
        }

        public static bool operator !=(GenericValue lvalue, string rvalue)
        {
            return !(lvalue == rvalue);
        }

        public static bool operator !=(GenericValue lvalue, DateTime rvalue)
        {
            return !(lvalue == rvalue);
        }

        public static bool operator !=(GenericValue lvalue, int rvalue)
        {
            return !(lvalue == rvalue);
        }

        public static bool operator !=(GenericValue lvalue, double rvalue)
        {
            return !(lvalue == rvalue);
        }

        public static bool operator !=(GenericValue lvalue, decimal rvalue)
        {
            return !(lvalue == rvalue);
        }

        public static bool operator !=(string lvalue, GenericValue rvalue)
        {
            return lvalue != rvalue;
        }

        public static bool operator !=(double lvalue, GenericValue rvalue)
        {
            return lvalue != rvalue;
        }

        public static bool operator !=(int lvalue, GenericValue rvalue)
        {
            return lvalue != rvalue;
        }

        public static bool operator !=(decimal lvalue, GenericValue rvalue)
        {
            return lvalue != rvalue;
        }

        public static bool operator !=(DateTime lvalue, GenericValue rvalue)
        {
            return lvalue != rvalue;
        }

        public static GenericValue operator +(GenericValue value1, GenericValue value2)
        {
            GenericValue result;
            try
            {
                switch (Type.GetTypeCode(value1.RawValue.GetType()))
                {
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                        {
                            int rawValue = value1.GetRawValue<int>();
                            if (value2.RawValue is int)
                            {
                                result = rawValue + value2;
                                return result;
                            }
                            if (value2.RawValue is decimal)
                            {
                                result = rawValue + value2;
                                return result;
                            }
                            result = (double)value1.GetRawValue<int>() + value2;
                            return result;
                        }
                    case TypeCode.Double:
                        result = value1.GetRawValue<double>() + value2;
                        return result;
                    case TypeCode.Decimal:
                        result = value1.GetRawValue<decimal>() + value2;
                        return result;
                    case TypeCode.DateTime:
                        result = (GenericValue)value1.GetRawValue<DateTime>() + value2;
                        return result;
                    case TypeCode.String:
                        result = value1.GetRawValue<string>() + value2;
                        return result;
                }
                throw new Exception();
            }
            catch
            {
                throw new InvalidOperationException(string.Format("Cannot add {0} to {1} ", value1.RawValue.GetType(), value2.RawValue.GetType()));
            }
            //return result;
        }

        public static GenericValue operator +(GenericValue value1, decimal value2)
        {
            return value1 + value2;
        }

        public static GenericValue operator +(GenericValue value1, int value2)
        {
            return value1 + value2;
        }

        public static GenericValue operator +(GenericValue value1, TimeSpan value2)
        {
            return value1 + value2;
        }

        public static GenericValue operator -(GenericValue value1, GenericValue value2)
        {
            GenericValue result;
            try
            {
                switch (Type.GetTypeCode(value1.RawValue.GetType()))
                {
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                        {
                            int rawValue = value1.GetRawValue<int>();
                            if (value2.RawValue is decimal)
                            {
                                result = rawValue - value2.GetRawValue<decimal>();
                                return result;
                            }
                            if (value2.RawValue is int)
                            {
                                result = rawValue - value2.GetRawValue<int>();
                                return result;
                            }
                            result = (double)value1.GetRawValue<int>() - value2;
                            return result;
                        }
                    case TypeCode.Double:
                        result = value1.GetRawValue<double>() - value2;
                        return result;
                    case TypeCode.Decimal:
                        result = value1.GetRawValue<decimal>() - value2;
                        return result;
                    case TypeCode.DateTime:
                        if (value2 != null as GenericValue && value2.RawValue is DateTime)
                        {
                            result = value1.GetRawValue<DateTime>() - value2.GetRawValue<DateTime>();
                            return result;
                        }
                        result = (GenericValue)value1.GetRawValue<DateTime>() - value2;
                        return result;
                }
                throw new Exception();
            }
            catch
            {
                throw new InvalidOperationException(string.Format("cannot substract a {0} from a {1}", (value2 != (GenericValue)null) ? value2.RawValue.GetType() : null, value1.RawValue.GetType()));
            }
         //   return result;
        }

        public static GenericValue operator -(GenericValue value1, decimal value2)
        {
            return value1 - value2;
        }

        public static GenericValue operator -(GenericValue value1, int value2)
        {
            return value1 - value2;
        }

        public static GenericValue operator -(GenericValue value1, TimeSpan value2)
        {
            return value1 - value2;
        }

        public static GenericValue operator -(GenericValue value1, DateTime value2)
        {
            return value1 - value2;
        }

        public static GenericValue operator *(GenericValue value1, GenericValue value2)
        {
            GenericValue result;
            try
            {
                switch (value1.GetTypeCode())
                {
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                        {
                            int rawValue = value1.GetRawValue<int>();
                            if (value2.RawValue is int)
                            {
                                result = rawValue * value2;
                                return result;
                            }
                            if (value2.RawValue is decimal)
                            {
                                result = rawValue * value2;
                                return result;
                            }
                            result = (double)value1.GetRawValue<int>() * value2;
                            return result;
                        }
                    case TypeCode.Double:
                        result = value1.GetRawValue<double>() * value2;
                        return result;
                    case TypeCode.Decimal:
                        result = value1.GetRawValue<decimal>() * value2;
                        return result;
                }
                throw new Exception();
            }
            catch
            {
                throw new InvalidOperationException(string.Format("Cannot multiply a {0} with a {1}", value1.RawValue.GetType(), value2.RawValue.GetType()));
            }
            //return result;
        }

        public static GenericValue operator /(GenericValue value1, GenericValue value2)
        {
            GenericValue result;
            try
            {
                switch (value1.GetTypeCode())
                {
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                        {
                            int rawValue = value1.GetRawValue<int>();
                            if (value2.RawValue is int)
                            {
                                result = rawValue / value2;
                                return result;
                            }
                            if (value2.RawValue is decimal)
                            {
                                result = rawValue / value2;
                                return result;
                            }
                            result = (double)rawValue / value2;
                            return result;
                        }
                    case TypeCode.Double:
                        result = value1.GetRawValue<double>() / value2;
                        return result;
                    case TypeCode.Decimal:
                        result = value1.GetRawValue<decimal>() / value2;
                        return result;
                }
                throw new Exception();
            }
            catch
            {
                throw new InvalidOperationException(string.Format("Cannot divde a {0} with a {1}", value1.RawValue.GetType(), value2.RawValue.GetType()));
            }
          //  return result;
        }

        public static bool operator !(GenericValue value)
        {
            return !value;
        }

        public static bool operator <(GenericValue value1, GenericValue value2)
        {
            return GenericValue.Compare(value1, value2) < 0;
        }

        public static bool operator >(GenericValue value1, GenericValue value2)
        {
            return GenericValue.Compare(value1, value2) > 0;
        }

        public static bool operator <(GenericValue value1, DateTime value2)
        {
            return GenericValue.Compare(value1, value2) < 0;
        }

        public static bool operator >(GenericValue value1, DateTime value2)
        {
            return GenericValue.Compare(value1, value2) > 0;
        }

        public static bool operator <(GenericValue value1, double value2)
        {
            return GenericValue.Compare(value1, value2) < 0;
        }

        public static bool operator >(GenericValue value1, double value2)
        {
            return GenericValue.Compare(value1, value2) > 0;
        }

        public static bool operator <(GenericValue value1, decimal value2)
        {
            return GenericValue.Compare(value1, value2) < 0;
        }

        public static bool operator >(GenericValue value1, decimal value2)
        {
            return GenericValue.Compare(value1, value2) > 0;
        }

        public static bool operator <(GenericValue value1, int value2)
        {
            return GenericValue.Compare(value1, value2) < 0;
        }

        public static bool operator >(GenericValue value1, int value2)
        {
            return GenericValue.Compare(value1, value2) > 0;
        }

        public static bool operator <(double value1, GenericValue value2)
        {
            return GenericValue.Compare(value1, value2) < 0;
        }

        public static bool operator >(double value1, GenericValue value2)
        {
            return GenericValue.Compare(value1, value2) > 0;
        }

        public static bool operator <(int value1, GenericValue value2)
        {
            return GenericValue.Compare(value1, value2) < 0;
        }

        public static bool operator >(int value1, GenericValue value2)
        {
            return GenericValue.Compare(value1, value2) > 0;
        }

        public static bool operator <(decimal value1, GenericValue value2)
        {
            return GenericValue.Compare(value1, value2) < 0;
        }

        public static bool operator >(decimal value1, GenericValue value2)
        {
            return GenericValue.Compare(value1, value2) > 0;
        }

        public static bool operator <=(GenericValue value1, GenericValue value2)
        {
            return GenericValue.Compare(value1, value2) <= 0;
        }

        public static bool operator >=(GenericValue value1, GenericValue value2)
        {
            return GenericValue.Compare(value1, value2) >= 0;
        }

        public static bool operator <=(GenericValue value1, DateTime value2)
        {
            return GenericValue.Compare(value1, value2) <= 0;
        }

        public static bool operator >=(GenericValue value1, DateTime value2)
        {
            return GenericValue.Compare(value1, value2) >= 0;
        }

        public static bool operator <=(GenericValue value1, double value2)
        {
            return GenericValue.Compare(value1, value2) <= 0;
        }

        public static bool operator >=(GenericValue value1, double value2)
        {
            return GenericValue.Compare(value1, value2) >= 0;
        }

        public static bool operator <=(GenericValue value1, decimal value2)
        {
            return GenericValue.Compare(value1, value2) <= 0;
        }

        public static bool operator >=(GenericValue value1, decimal value2)
        {
            return GenericValue.Compare(value1, value2) >= 0;
        }

        public static bool operator <=(GenericValue value1, int value2)
        {
            return GenericValue.Compare(value1, value2) <= 0;
        }

        public static bool operator >=(GenericValue value1, int value2)
        {
            return GenericValue.Compare(value1, value2) >= 0;
        }

        public static bool operator <=(int value1, GenericValue value2)
        {
            return GenericValue.Compare(value1, value2) <= 0;
        }

        public static bool operator >=(int value1, GenericValue value2)
        {
            return GenericValue.Compare(value1, value2) >= 0;
        }

        public static bool operator <=(double value1, GenericValue value2)
        {
            return GenericValue.Compare(value1, value2) <= 0;
        }

        public static bool operator >=(double value1, GenericValue value2)
        {
            return GenericValue.Compare(value1, value2) >= 0;
        }

        public static bool operator <=(decimal value1, GenericValue value2)
        {
            return GenericValue.Compare(value1, value2) <= 0;
        }

        public static bool operator >=(decimal value1, GenericValue value2)
        {
            return GenericValue.Compare(value1, value2) >= 0;
        }

        public static int Compare(GenericValue value1, GenericValue value2)
        {
            if (value1 == (GenericValue)null && value2 == (GenericValue)null)
            {
                return 0;
            }
            if (value1 == (GenericValue)null && value2 != (GenericValue)null)
            {
                return -1;
            }
            int result;
            try
            {
                switch (Type.GetTypeCode(value1.RawValue.GetType()))
                {
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.Double:
                    case TypeCode.Decimal:
                        result = Comparer<decimal>.Default.Compare(value1, value2);
                        return result;
                    case TypeCode.DateTime:
                        result = Comparer<DateTime>.Default.Compare(GenericValue.TrimMilliseconds(value1), GenericValue.TrimMilliseconds(value2));
                        return result;
                    case TypeCode.String:
                        result = Comparer<string>.Default.Compare(value1, value2);
                        return result;
                }
                throw new Exception();
            }
            catch
            {
                throw new InvalidOperationException(string.Format("Compare not implemented for {0} with {1}", value1.RawValue.GetType(), value2.RawValue.GetType()));
            }
        //    return result;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            GenericValue genericValue = obj as GenericValue;
            if (genericValue == (GenericValue)null)
            {
                return obj.Equals(this.RawValue);
            }
            if (this.RawValue == null)
            {
                return genericValue.RawValue == null;
            }
            return this.RawValue.Equals(genericValue.RawValue);
        }

        public override int GetHashCode()
        {
            if (this.RawValue == null)
            {
                return 42;
            }
            return this.RawValue.GetHashCode();
        }

        public override string ToString()
        {
            return GenericValue.ToString(this);
        }

        public static int ToInt(GenericValue value, IFormatProvider formatProvider)
        {
            return int.Parse(value, formatProvider);
        }

        public static double ToDouble(GenericValue value, IFormatProvider formatProvider)
        {
            return double.Parse(value, formatProvider);
        }

        public static decimal ToDecimal(GenericValue value, IFormatProvider formatProvider)
        {
            return decimal.Parse(value, formatProvider);
        }

        public static DateTime ToDateTime(GenericValue value, IFormatProvider formatProvider)
        {
            return DateTime.Parse(value, formatProvider);
        }

        public static DateTime ToDateTime(GenericValue value, string format, IFormatProvider formatProvider)
        {
            return DateTime.ParseExact(value, format, formatProvider ?? CultureInfo.InvariantCulture);
        }

        public static TimeSpan ToTimeSpan(GenericValue value, IFormatProvider formatProvider)
        {
            return TimeSpan.Parse(value, formatProvider);
        }

        public static TimeSpan ToTimeSpan(GenericValue value, string format, IFormatProvider formatProvider)
        {
            return TimeSpan.ParseExact(value, format, formatProvider ?? CultureInfo.InvariantCulture);
        }

        public TimeSpan ToTimeSpan(IFormatProvider formatProvider)
        {
            return GenericValue.ToTimeSpan(this, formatProvider);
        }

        public double ToDouble(NumberStyles format)
        {
            return double.Parse(this, format);
        }

        public decimal ToDecimal(NumberStyles format)
        {
            return decimal.Parse(this, format);
        }

        public DateTime ToDateTime(DateTimeStyles format)
        {
            return DateTime.Parse(this, null, format);
        }

        public DateTime ToDateTime(string format)
        {
            return DateTime.ParseExact(this, format, CultureInfo.InvariantCulture);
        }

        public TimeSpan ToTimeSpan(string format)
        {
            return TimeSpan.ParseExact(this, format, CultureInfo.CurrentCulture);
        }

        public static string ToString(GenericValue value)
        {
            if (value == (GenericValue)null || value.RawValue == null)
            {
                return null;
            }
            if (value.FormatProvider == null)
            {
                return value.RawValue.ToString();
            }
            GenericValueFormatProvider genericValueFormatProvider = value.FormatProvider as GenericValueFormatProvider;
            if (genericValueFormatProvider != null)
            {
                return GenericValue.ToString(value, genericValueFormatProvider.Pattern, value.FormatProvider);
            }
            return GenericValue.ToString(value, value.FormatProvider);
        }

        public static string ToString(GenericValue value, string format, IFormatProvider formatProvider)
        {
            if (value == (GenericValue)null || value.RawValue == null)
            {
                return null;
            }
            if (value.RawValue is TimeSpan)
            {
                return value.ToString(format, formatProvider);
            }
            switch (Type.GetTypeCode(value.RawValue.GetType()))
            {
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return value.GetRawValue<int>().ToString(format, formatProvider);
                case TypeCode.Double:
                    return value.GetRawValue<double>().ToString(format, formatProvider);
                case TypeCode.Decimal:
                    return value.GetRawValue<decimal>().ToString(format, formatProvider);
                case TypeCode.DateTime:
                    return value.GetRawValue<DateTime>().ToString(format, formatProvider);
                case TypeCode.String:
                    return value.GetRawValue<string>();
            }
            return value.RawValue.ToString();
        }

        public static string ToString(GenericValue value, GenericValueFormatProvider customFormatProvider)
        {
            return GenericValue.ToString(value, customFormatProvider.Pattern, customFormatProvider);
        }

        public static string ToString(GenericValue value, IFormatProvider formatProvider)
        {
            if (value == null as GenericValue || value.RawValue == null)
            {
                return null;
            }
            switch (Type.GetTypeCode(value.RawValue.GetType()))
            {
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return value.GetRawValue<int>().ToString(formatProvider);
                case TypeCode.Double:
                    return value.GetRawValue<double>().ToString(formatProvider);
                case TypeCode.Decimal:
                    return value.GetRawValue<decimal>().ToString(formatProvider);
                case TypeCode.DateTime:
                    return value.GetRawValue<DateTime>().ToString(formatProvider);
                case TypeCode.String:
                    return value.RawValue as string;
            }
            return value.RawValue.ToString();
        }

        public string ToString(IFormatProvider formatProvider)
        {
            if (formatProvider == null)
            {
                return this.ToString();
            }
            return GenericValue.ToString(this, formatProvider);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return GenericValue.ToString(this, format, formatProvider);
        }

        public int CompareTo(object obj)
        {
            return GenericValue.Compare(this, new GenericValue(obj));
        }

        public TypeCode GetTypeCode()
        {
            if (this.RawValue == null)
            {
                return TypeCode.Object;
            }
            return Type.GetTypeCode(this.RawValue.GetType());
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            return Convert.ToBoolean(this.RawValue, provider);
        }

        public byte ToByte(IFormatProvider provider)
        {
            return Convert.ToByte(this.RawValue, provider);
        }

        public char ToChar(IFormatProvider provider)
        {
            return Convert.ToChar(this.RawValue, provider);
        }

        public short ToInt16(IFormatProvider provider)
        {
            return Convert.ToInt16(this.RawValue, provider);
        }

        public int ToInt32(IFormatProvider provider)
        {
            return Convert.ToInt32(this.RawValue, provider);
        }

        public long ToInt64(IFormatProvider provider)
        {
            return Convert.ToInt64(this.RawValue, provider);
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            return Convert.ToSByte(this.RawValue, provider);
        }

        public float ToSingle(IFormatProvider provider)
        {
            return Convert.ToSingle(this.RawValue, provider);
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            if (conversionType == typeof(GenericValue))
            {
                return this;
            }
            return Convert.ChangeType(this.RawValue, conversionType, provider);
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            return Convert.ToUInt16(this.RawValue, provider);
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            return Convert.ToUInt32(this.RawValue, provider);
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            return Convert.ToUInt64(this.RawValue, provider);
        }

        public int CompareTo(GenericValue other)
        {
            return GenericValue.Compare(this, other);
        }

        public bool Equals(GenericValue other)
        {
            return this.Equals(other);
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            return GenericValue.ToDateTime(this, provider);
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            return GenericValue.ToDecimal(this, provider);
        }

        public double ToDouble(IFormatProvider provider)
        {
            return GenericValue.ToDouble(this, provider);
        }

        private static DateTime TrimMilliseconds(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, 0, dt.Kind);
        }

        public string[] Split(char[] separator, StringSplitOptions options)
        {
            return (this.ToString() ?? string.Empty).Split(separator, options);
        }

        public string[] Split(params char[] separator)
        {
            return this.Split(separator, StringSplitOptions.None);
        }

        public string[] Split(string[] separator, StringSplitOptions options)
        {
            return (this.ToString() ?? string.Empty).Split(separator, options);
        }

        public string[] Split(params string[] separator)
        {
            return this.Split(separator, StringSplitOptions.None);
        }

        public string Replace(string oldValue, string newValue)
        {
            return (this.ToString() ?? string.Empty).Replace(oldValue, newValue);
        }

        public string Substring(int startIndex, int length)
        {
            return (this.ToString() ?? string.Empty).Substring(startIndex, length);
        }

        public string Substring(int startIndex)
        {
            return (this.ToString() ?? string.Empty).Substring(startIndex);
        }

        public int Length()
        {
            return (this.ToString() ?? string.Empty).Length;
        }

        public bool Contains(string other)
        {
            return (this.ToString() ?? string.Empty).Contains(other);
        }

        public string Trim()
        {
            return (this.ToString() ?? string.Empty).Trim();
        }

        public string Trim(params char[] trimChars)
        {
            return (this.ToString() ?? string.Empty).Trim(trimChars);
        }

        public string TrimStart(params char[] trimChars)
        {
            return (this.ToString() ?? string.Empty).TrimStart(trimChars);
        }

        public string TrimEnd(params char[] trimChars)
        {
            return (this.ToString() ?? string.Empty).TrimEnd(trimChars);
        }

        public int IndexOf(string value, int startIndex, StringComparison comparisonType)
        {
            return (this.ToString() ?? string.Empty).IndexOf(value, startIndex, comparisonType);
        }

        public int IndexOf(string value)
        {
            return this.IndexOf(value, 0, StringComparison.CurrentCulture);
        }

        public int IndexOf(string value, StringComparison comparisonType)
        {
            return this.IndexOf(value, 0, comparisonType);
        }

        public int IndexOf(string value, int startIndex)
        {
            return this.IndexOf(value, startIndex, StringComparison.CurrentCulture);
        }

        public string ToUpper()
        {
            return (this.ToString() ?? string.Empty).ToUpper();
        }

        public string ToUpperInvariant()
        {
            return (this.ToString() ?? string.Empty).ToUpperInvariant();
        }

        public string ToLower()
        {
            return (this.ToString() ?? string.Empty).ToLower();
        }

        public string ToLowerInvariant()
        {
            return (this.ToString() ?? string.Empty).ToLowerInvariant();
        }

        public int ToInt(IFormatProvider culture)
        {
            object rawValue = this.GetRawValue<object>();
            IConvertible convertible;
            if (!(rawValue is string) && (convertible = (rawValue as IConvertible)) != null)
            {
                return convertible.ToInt32(culture);
            }
            return int.Parse(this.ToString(culture), NumberStyles.None, culture);
        }

        public int ToInt()
        {
            return this.ToInt(CultureInfo.CurrentCulture);
        }
    }
}
