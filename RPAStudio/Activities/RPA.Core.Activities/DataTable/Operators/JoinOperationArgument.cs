using Plugins.Shared.Library.Librarys;
using System;
using System.Activities;
using System.Data;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Plugins.Shared.Library.Converters;
using Plugins.Shared.Library;

namespace RPA.Core.Activities.DataTableActivity.Operators
{
    public interface IJoinOperation
    {
        bool Validate(DataRow leftRow, int column1Index, DataRow rightRow, int column2Index);
        bool ApplyOn(DataRow leftRow, int column1Index, DataRow rightRow, int column2Index);
    }


    internal static class OperationComparer
    {
        private static readonly Dictionary<Type, int> _typePrecedence = new Dictionary<Type, int>
        {
            {
                typeof(sbyte),
                0
            },
            {
                typeof(byte),
                1
            },
            {
                typeof(short),
                2
            },
            {
                typeof(int),
                3
            },
            {
                typeof(long),
                4
            },
            {
                typeof(float),
                4
            },
            {
                typeof(double),
                5
            },
            {
                typeof(decimal),
                6
            }
        };

        public static bool Compare(IComparable operand1, IComparable operand2, Func<IComparable, IComparable, bool> compareFunc)
        {
            if (operand1 == null || operand2 == null)
            {
                return false;
            }
            if (!OperationComparer.IsNumeric(operand1) || !OperationComparer.IsNumeric(operand2))
            {
                return compareFunc(operand1, operand2);
            }
            Type type = operand1.GetType();
            Type type2 = operand2.GetType();
            int num;
            int num2;
            if (!OperationComparer._typePrecedence.TryGetValue(type, out num) || !OperationComparer._typePrecedence.TryGetValue(type2, out num2))
            {
                return compareFunc(operand1, operand2);
            }
            if (num < num2)
            {
                object obj;
                if (!OperationComparer.TryConvertOperand(operand1, type2, out obj))
                {
                    SharedObject.Instance.Output(SharedObject.enOutputType.Error, "FilterTypeWarning operand:" + operand1  + "type:" + type2);
                    return false;
                }
                return compareFunc((IComparable)obj, operand2);
            }
            else
            {
                if (num <= num2)
                {
                    object obj2;
                    OperationComparer.TryConvertOperand(operand1, typeof(double), out obj2);
                    object obj3;
                    OperationComparer.TryConvertOperand(operand2, typeof(double), out obj3);
                    return compareFunc((IComparable)obj2, (IComparable)obj3);
                }
                object obj4;
                if (!OperationComparer.TryConvertOperand(operand2, type, out obj4))
                {
                    SharedObject.Instance.Output(SharedObject.enOutputType.Error, "FilterTypeWarning operand:" + operand1 + "type:" + type2);
                    return false;
                }
                return compareFunc(operand1, (IComparable)obj4);
            }
        }

        public new static bool Equals(object operand1, object operand2)
        {
            if (operand1 == null && operand2 == null)
            {
                return true;
            }
            if (operand1 == null || operand2 == null)
            {
                return false;
            }
            if (!OperationComparer.IsNumeric(operand1) && !OperationComparer.IsNumeric(operand2))
            {
                return operand1.Equals(operand2);
            }
            Type type = operand1.GetType();
            Type type2 = operand2.GetType();
            int num;
            int num2;
            if (!OperationComparer._typePrecedence.TryGetValue(type, out num) || !OperationComparer._typePrecedence.TryGetValue(type2, out num2))
            {
                return operand1.Equals(operand2);
            }
            if (num < num2)
            {
                object obj;
                if (!OperationComparer.TryConvertOperand(operand1, type2, out obj))
                {
                    SharedObject.Instance.Output(SharedObject.enOutputType.Error, "FilterTypeWarning operand:" + operand1 + "type:" + type2);
                    return false;
                }
                return operand2.Equals(obj);
            }
            else
            {
                if (num <= num2)
                {
                    return operand1.Equals(operand2);
                }
                object obj2;
                if (!OperationComparer.TryConvertOperand(operand2, type, out obj2))
                {
                    SharedObject.Instance.Output(SharedObject.enOutputType.Error, "FilterTypeWarning operand:" + operand1 + "type:" + type2);
                    return false;
                }
                return operand1.Equals(obj2);
            }
        }

        public static bool IsNumeric(object value)
        {
            return OperationComparer.IsIntegral(value) || OperationComparer.IsFloatingPoint(value);
        }

        private static bool IsIntegral(object value)
        {
            return value is sbyte || value is byte || value is short || value is int || value is long;
        }

        private static bool IsFloatingPoint(object value)
        {
            return value is float || value is double || value is decimal;
        }

        private static bool TryConvertOperand(object operand, Type toType, out object newOperand)
        {
            newOperand = null;
            IConvertible convertible;
            if ((convertible = (operand as IConvertible)) == null)
            {
                return false;
            }
            try
            {
                newOperand = convertible.ToType(toType, CultureInfo.InvariantCulture);
            }
            catch (Exception arg_20_0)
            {
                Trace.TraceWarning(arg_20_0.ToString());
                return false;
            }
            return true;
        }
    }

    internal abstract class ComparableOperation : IFilterOperation, IJoinOperation
    {
        public IComparable Operand
        {
            protected get;
            set;
        }

        public virtual bool Validate(DataRow dataRow, int columnIndex)
        {
            IComparable comparable = dataRow[columnIndex] as IComparable;
            if (this.Operand == null || comparable == null)
            {
                return false;
            }
            if (comparable.GetType() != this.Operand.GetType())
            {
                if (OperationComparer.IsNumeric(comparable) && OperationComparer.IsNumeric(this.Operand))
                {
                    return true;
                }
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "FilterTypeWarning operand:" + this.Operand.GetType() + "comparable:" + comparable.GetType());
                return false;
            }
            else
            {
                if (comparable is string)
                {
                    SharedObject.Instance.Output(SharedObject.enOutputType.Error, "FilterTypeWarning operand:" + this.Operand.GetType() + "comparable:" + comparable.GetType());
                    return false;
                }
                return true;
            }
        }

        public abstract bool ApplyOn(DataRow dataRow, int columnIndex);

        public bool Validate(DataRow leftRow, int column1Index, DataRow rowRight, int column2Index)
        {
            IComparable comparable = leftRow[column1Index] as IComparable;
            IComparable comparable2 = rowRight[column2Index] as IComparable;
            if (comparable == null || comparable2 == null)
            {
                return false;
            }
            if (comparable is string || comparable2 is string)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "FilterTypeWarning operand:" + this.Operand.GetType() + "comparable:" + comparable.GetType());
                return false;
            }
            return true;
        }

        public abstract bool ApplyOn(DataRow leftRow, int column1Index, DataRow rowRight, int column2Index);
    }

    public class EqualsOperation : IFilterOperation, IJoinOperation
    {
        public object Operand
        {
            get;
            set;
        }

        public bool Validate(DataRow dataRow, int columnIndex)
        {
            object obj = dataRow[columnIndex];
            return (this.Operand == null && obj == null) || (this.Operand != null && obj != null && (obj.GetType() == this.Operand.GetType() || (OperationComparer.IsNumeric(obj) && OperationComparer.IsNumeric(this.Operand))));
        }

        public bool ApplyOn(DataRow dataRow, int columnIndex)
        {
            return OperationComparer.Equals(dataRow[columnIndex], this.Operand);
        }

        public bool Validate(DataRow leftRow, int column1Index, DataRow rightRow, int column2Index)
        {
            object obj = leftRow[column1Index];
            object obj2 = rightRow[column2Index];
            return (obj == null && obj2 == null) || (obj != null && obj2 != null && (obj.GetType() == obj2.GetType() || (OperationComparer.IsNumeric(obj) && OperationComparer.IsNumeric(obj2))));
        }

        public bool ApplyOn(DataRow leftRow, int column1Index, DataRow rightRow, int column2Index)
        {
            return OperationComparer.Equals(leftRow[column1Index], rightRow[column2Index]);
        }
    }

    internal class NotEqualsOperation : IFilterOperation, IJoinOperation
    {
        public object Operand
        {
            get;
            set;
        }

        public bool Validate(DataRow dataRow, int columnIndex)
        {
            return dataRow[columnIndex] != null || this.Operand != null;
        }

        public bool ApplyOn(DataRow dataRow, int columnIndex)
        {
            return !OperationComparer.Equals(dataRow[columnIndex], this.Operand);
        }

        public bool Validate(DataRow leftRow, int column1Index, DataRow rightRow, int column2Index)
        {
            bool arg_10_0 = leftRow[column1Index] != null;
            object obj = rightRow[column2Index];
            return arg_10_0 || obj != null;
        }

        public bool ApplyOn(DataRow leftRow, int column1Index, DataRow rightRow, int column2Index)
        {
            return !OperationComparer.Equals(leftRow[column1Index], rightRow[column2Index]);
        }
    }


  //  internal class LessThanOperation : ComparableOperation
  //  {
  //      [CompilerGenerated]
  //      [Serializable]
  //      private sealed class <>c
		//{
		//	public static readonly LessThanOperation.<>c<>9 = new LessThanOperation.<>c();

  //      public static Func<IComparable, IComparable, bool> <>9__0_0;

		//	public static Func<IComparable, IComparable, bool> <>9__1_0;

		//	internal bool <ApplyOn>b__0_0(IComparable a, IComparable b)
  //      {
  //          return a.CompareTo(b) < 0;
  //      }

  //      internal bool <ApplyOn>b__1_0(IComparable a, IComparable b)
  //      {
  //          return a.CompareTo(b) < 0;
  //      }
  //  }

  //  public override bool ApplyOn(DataRow dataRow, int columnIndex)
  //  {
  //      IComparable arg_31_0 = dataRow[columnIndex] as IComparable;
  //      IComparable arg_31_1 = base.Operand;
  //      Func<IComparable, IComparable, bool> arg_31_2;
  //      if ((arg_31_2 = LessThanOperation.<> c.<> 9__0_0) == null)
		//	{
  //          arg_31_2 = (LessThanOperation.<> c.<> 9__0_0 = new Func<IComparable, IComparable, bool>(LessThanOperation.<> c.<> 9.< ApplyOn > b__0_0));
  //      }
  //      return OperationComparer.Compare(arg_31_0, arg_31_1, arg_31_2);
  //  }

  //  public override bool ApplyOn(DataRow leftRow, int column1Index, DataRow rowRight, int column2Index)
  //  {
  //      IComparable arg_3A_0 = leftRow[column1Index] as IComparable;
  //      IComparable comparable = rowRight[column2Index] as IComparable;
  //      IComparable arg_3A_1 = comparable;
  //      Func<IComparable, IComparable, bool> arg_3A_2;
  //      if ((arg_3A_2 = LessThanOperation.<> c.<> 9__1_0) == null)
		//	{
  //          arg_3A_2 = (LessThanOperation.<> c.<> 9__1_0 = new Func<IComparable, IComparable, bool>(LessThanOperation.<> c.<> 9.< ApplyOn > b__1_0));
  //      }
  //      return OperationComparer.Compare(arg_3A_0, arg_3A_1, arg_3A_2);
  //  }


    public class JoinOperationArgument
    {
        public InArgument Column1
        {
            get;
            set;
        }

        public InArgument Column2
        {
            get;
            set;
        }

        public InArgument Operand
        {
            get;
            set;
        }

        public JoinOperator Operator
        {
            get;
            set;
        }

        public BooleanOperator BooleanOperator
        {
            get;
            set;
        }

        public bool IsJoinEmpty
        {
            get
            {
                return this.Operand == null && this.Column1 == null && this.Column2 == null;
            }
        }


    
        public IJoinOperation JoinOperationFactory(ActivityContext context)
        {
            InArgument expr_06 = this.Operand;
            object obj = (expr_06 != null) ? expr_06.Get(context) : null;
            GenericValue value;
            if ((value = (obj as GenericValue)) != (GenericValue)null)
            {
                obj = GenericValue.GetRawValue(value);
            }
            if (this.Operator == JoinOperator.EQ)
            {
                return new EqualsOperation
                {
                    Operand = obj
                };
            }
            if (this.Operator == JoinOperator.NOTEQ)
            {
                return new NotEqualsOperation
                {
                    Operand = obj
                };
            }
            IComparable operand = obj as IComparable;
            switch (this.Operator)
            {
                //case JoinOperator.LT:
                //    return new LessThanOperation
                //    {
                //        Operand = operand
                //    };
                //case JoinOperator.GT:
                //    return new GreaterThanOperation
                //    {
                //        Operand = operand
                //    };
                //case JoinOperator.LTE:
                //    return new LessThanOrEqualOperation
                //    {
                //        Operand = operand
                //    };
                //case JoinOperator.GTE:
                //    return new GreaterThanOrEqualOperation
                //    {
                //        Operand = operand
                //    };
                default:
                    return null;
            }
        }
    }
}
