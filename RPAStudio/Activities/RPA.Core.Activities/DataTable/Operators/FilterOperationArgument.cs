using Plugins.Shared.Library.Converters;
using Plugins.Shared.Library.Librarys;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Data;

namespace RPA.Core.Activities.DataTableActivity.Operators
{
    public interface IFilterOperation
    {
        bool Validate(DataRow dataRow, int columnIndex);
        bool ApplyOn(DataRow dataRow, int columnIndex);
    }

    public class FilterOperationArgument
    {
        private static HashSet<FilterOperator> _stringOperations = new HashSet<FilterOperator>
        {
            FilterOperator.EMPTY,
            FilterOperator.NOTEMPTY,
            FilterOperator.STARTSWITH,
            FilterOperator.NOTSTARTSWITH,
            FilterOperator.ENDSWITH,
            FilterOperator.NOTENDSWITH,
            FilterOperator.CONTAINS,
            FilterOperator.NOTCONTAINS
        };

        public InArgument Column
        {
            get;
            set;
        }

        public InArgument Operand
        {
            get;
            set;
        }

        public FilterOperator Operator
        {
            get;
            set;
        }

        public BooleanOperator BooleanOperator
        {
            get;
            set;
        }

        public bool IsStringOperation
        {
            get
            {
                return FilterOperationArgument._stringOperations.Contains(this.Operator);
            }
        }

        public bool IsEmptyOperation
        {
            get
            {
                return FilterOperationArgument.IsEmptyOperator(this.Operator);
            }
        }

        public bool IsFilterEmpty
        {
            get
            {
                return this.Operand == null && this.Column == null;
            }
        }


        public IFilterOperation FilterOperationFactory(DataTable datatable, ActivityContext context)
        {
            InArgument expr_06 = this.Operand;
            object obj = (expr_06 != null) ? expr_06.Get(context) : null;
            if (obj != null)
            {
                GenericValue genericValue = obj as GenericValue;
                obj = GenericValue.GetRawValue(genericValue);
            }
            if (this.Operator == FilterOperator.EQ)
            {
                return new EqualsOperation
                {
                    Operand = obj
                };
            }
            if (this.Operator == FilterOperator.NOTEQ)
            {
                return new NotEqualsOperation
                {
                    Operand = obj
                };
            }
            if (this.IsStringOperation)
            {
                string operand = obj as string;
                switch (this.Operator)
                {
                    case FilterOperator.EMPTY:
                        return new IsEmptyOperation();
                    case FilterOperator.NOTEMPTY:
                        return new IsEmptyOperation
                        {
                            IsNegation = true
                        };
                    case FilterOperator.STARTSWITH:
                        return new StartsWithOperation
                        {
                            Operand = operand
                        };
                    case FilterOperator.ENDSWITH:
                        return new EndsWithOperation
                        {
                            Operand = operand
                        };
                    case FilterOperator.CONTAINS:
                        return new ContainsOperation
                        {
                            Operand = operand
                        };
                    case FilterOperator.NOTSTARTSWITH:
                        return new StartsWithOperation
                        {
                            Operand = operand,
                            IsNegation = true
                        };
                    case FilterOperator.NOTENDSWITH:
                        return new EndsWithOperation
                        {
                            Operand = operand,
                            IsNegation = true
                        };
                    case FilterOperator.NOTCONTAINS:
                        return new ContainsOperation
                        {
                            Operand = operand,
                            IsNegation = true
                        };
                    default:
                        return null;
                }
            }
            else
            {
                IComparable operand2 = obj as IComparable;
                switch (this.Operator)
                {
                    case FilterOperator.LT:
                        return new LessThanOperation
                        {
                            Operand = operand2
                        };
                    case FilterOperator.GT:
                        return new GreaterThanOperation
                        {
                            Operand = operand2
                        };
                    case FilterOperator.LTE:
                        return new LessThanOrEqualOperation
                        {
                            Operand = operand2
                        };
                    case FilterOperator.GTE:
                        return new GreaterThanOrEqualOperation
                        {
                            Operand = operand2
                        };
                    default:
                        return null;
                }
            }
        }

        public static bool IsEmptyOperator(FilterOperator op)
        {
            return op - FilterOperator.EMPTY <= 1;
        }
    }

    internal class IsEmptyOperation : IFilterOperation
    {
        public bool IsNegation
        {
            protected get;
            set;
        }

        public bool Validate(DataRow dataRow, int columnIndex)
        {
            return true;
        }

        public bool ApplyOn(DataRow dataRow, int columnIndex)
        {
            object expr_07 = dataRow[columnIndex];
            bool flag = string.IsNullOrEmpty((expr_07 != null) ? expr_07.ToString() : null);
            if (this.IsNegation)
            {
                return !flag;
            }
            return flag;
        }
    }

    internal abstract class StringOperation : IFilterOperation
    {
        public string Operand
        {
            protected get;
            set;
        }

        public bool IsNegation
        {
            protected get;
            set;
        }

        public virtual bool Validate(DataRow dataRow, int columnIndex)
        {
            return true;
        }

        public bool ApplyOn(DataRow dataRow, int columnIndex)
        {
            bool flag = this.Apply(dataRow, columnIndex);
            if (this.IsNegation)
            {
                return !flag;
            }
            return flag;
        }

        public abstract bool Apply(DataRow dataRow, int columnIndex);
    }

    internal class StartsWithOperation : StringOperation
    {
        public override bool Apply(DataRow dataRow, int columnIndex)
        {
            return (dataRow[columnIndex] as string).StartsWith(base.Operand);
        }
    }

    internal class EndsWithOperation : StringOperation
    {
        public override bool Apply(DataRow dataRow, int columnIndex)
        {
            return (dataRow[columnIndex] as string).EndsWith(base.Operand);
        }
    }

    internal class ContainsOperation : StringOperation
    {
        public override bool Apply(DataRow dataRow, int columnIndex)
        {
            return (dataRow[columnIndex] as string).Contains(base.Operand);
        }
    }

    internal class LessThanOperation : ComparableOperation
    {
        public override bool ApplyOn(DataRow dataRow, int columnIndex)
        {
            return OperationComparer.Compare(dataRow[columnIndex] as IComparable, base.Operand, (IComparable a, IComparable b) => a.CompareTo(b) < 0);
        }

        public override bool ApplyOn(DataRow leftRow, int column1Index, DataRow rowRight, int column2Index)
        {
            IComparable operand = leftRow[column1Index] as IComparable;
            IComparable operand2 = rowRight[column2Index] as IComparable;
            return OperationComparer.Compare(operand, operand2, (IComparable a, IComparable b) => a.CompareTo(b) < 0);
        }

        public LessThanOperation()
        {
        }
    }

    internal class GreaterThanOperation : ComparableOperation
    {
        public override bool ApplyOn(DataRow dataRow, int columnIndex)
        {
            return OperationComparer.Compare(dataRow[columnIndex] as IComparable, base.Operand, (IComparable a, IComparable b) => a.CompareTo(b) > 0);
        }

        public override bool ApplyOn(DataRow leftRow, int column1Index, DataRow rowRight, int column2Index)
        {
            IComparable operand = leftRow[column1Index] as IComparable;
            IComparable operand2 = rowRight[column2Index] as IComparable;
            return OperationComparer.Compare(operand, operand2, (IComparable a, IComparable b) => a.CompareTo(b) > 0);
        }

        public GreaterThanOperation()
        {
        }
    }

    internal class LessThanOrEqualOperation : ComparableOperation
    {
        // Token: 0x06000342 RID: 834 RVA: 0x00008F58 File Offset: 0x00007158
        public override bool ApplyOn(DataRow dataRow, int columnIndex)
        {
            return OperationComparer.Compare(dataRow[columnIndex] as IComparable, base.Operand, (IComparable a, IComparable b) => a.CompareTo(b) <= 0);
        }

        // Token: 0x06000343 RID: 835 RVA: 0x00008F90 File Offset: 0x00007190
        public override bool ApplyOn(DataRow leftRow, int column1Index, DataRow rowRight, int column2Index)
        {
            IComparable operand = leftRow[column1Index] as IComparable;
            IComparable operand2 = rowRight[column2Index] as IComparable;
            return OperationComparer.Compare(operand, operand2, (IComparable a, IComparable b) => a.CompareTo(b) <= 0);
        }

        // Token: 0x06000344 RID: 836 RVA: 0x00008F50 File Offset: 0x00007150
        public LessThanOrEqualOperation()
        {
        }
    }

    internal class GreaterThanOrEqualOperation : ComparableOperation
    {
        // Token: 0x06000348 RID: 840 RVA: 0x00009060 File Offset: 0x00007260
        public override bool ApplyOn(DataRow dataRow, int columnIndex)
        {
            return OperationComparer.Compare(dataRow[columnIndex] as IComparable, base.Operand, (IComparable a, IComparable b) => a.CompareTo(b) >= 0);
        }

        // Token: 0x06000349 RID: 841 RVA: 0x00009098 File Offset: 0x00007298
        public override bool ApplyOn(DataRow leftRow, int column1Index, DataRow rowRight, int column2Index)
        {
            IComparable operand = leftRow[column1Index] as IComparable;
            IComparable operand2 = rowRight[column2Index] as IComparable;
            return OperationComparer.Compare(operand, operand2, (IComparable a, IComparable b) => a.CompareTo(b) >= 0);
        }

        // Token: 0x0600034A RID: 842 RVA: 0x00008F50 File Offset: 0x00007150
        public GreaterThanOrEqualOperation()
        {
        }
    }

    public class Operation : IFilterOperation
    {
        public bool ApplyOn(DataRow dataRow, int columnIndex)
        {
            return true;
        }

        public bool Validate(DataRow dataRow, int columnIndex)
        {
            return true;
        }
    }
    public struct SourceListItem
    {
        public int ColumnIndex;
        public IFilterOperation Operation;
    }
}
