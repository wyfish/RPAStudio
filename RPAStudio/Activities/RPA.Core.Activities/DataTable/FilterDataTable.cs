using RPA.Core.Activities.DataTableActivity.Operators;
using Plugins.Shared.Library.Converters;
using Plugins.Shared.Library.Librarys;
using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace RPA.Core.Activities.DataTableActivity
{
    [Designer(typeof(FilterDataTableDesigner))]
    public sealed class FilterDataTable : AsyncCodeActivity
    {
        public FilterDataTable()
        {
            
        }

        public new string DisplayName
        {
            get
            {
                return "FilterDataTable";
            }
        }

        public enum filterTypes
        {
            保留,
            删除
        }


        [Browsable(false)]
        public List<FilterOperationArgument> Filters
        {
            get;
            set;
        }

        [Browsable(false)]
        public List<InArgument> SelectColumns
        {
            get;
            set;
        }



        [Category("输入")]
        [DisplayName("DataTable")]
        [Description("要筛选的DataTable变量。该字段仅支持DataTable变量")]
        public InArgument<DataTable> DataTable { get; set; }

        [Category("选项")]
        [DisplayName("行过滤模式")]
        [Description("指定是否通过保留或删除目标行来过滤表")]
        public SelectMode FilterRowsMo​​de { get; set; }

        [Category("选项")]
        [DisplayName("列过滤模式")]
        [Description("指定是通过保留还是删除目标列来过滤表")]
        public SelectMode SelectColumnsMode { get; set; }


        [Category("输出")]
        [DisplayName("DataTable")]
        [Description("过滤结果DataTable")]
        public OutArgument<DataTable> OutDataTable { get; set; }


        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.Core.Activities;Component/Resources/DataTable/datatable.png";
            }
        }

        private delegate string runDelegate();
        private runDelegate m_Delegate;
        public string Run()
        {
            return DisplayName;
        }

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);
            this.ValidateFilters(metadata);
            this.ValidateSelects(metadata);
        }

        private void ValidateFilters(CodeActivityMetadata metadata)
        {
            if (this.Filters == null)
            {
                return;
            }
            if (this.Filters.Count == 1 && this.Filters[0].IsFilterEmpty)
            {
                return;
            }
            int num = 0;
            foreach (FilterOperationArgument current in this.Filters)
            {
                Type type = typeof(object);
                if (current.Column == null)
                {
                    metadata.AddValidationError("FilterInvalidColumn");
                    break;
                }
                type = current.Column.ArgumentType;
                if (!this.CheckColumnType(type))
                {
                    metadata.AddValidationError("ColumnInvalidType");
                    break;
                }
                RuntimeArgument argument = new RuntimeArgument("Arg" + num, type, ArgumentDirection.In);
                metadata.Bind(current.Column, argument);
                metadata.AddArgument(argument);
                num++;
                if (!current.IsEmptyOperation)
                {
                    if (current.Operand == null)
                    {
                        metadata.AddValidationError("FilterInvalidOperand");
                        break;
                    }
                    type = current.Operand.ArgumentType;
                    argument = new RuntimeArgument("Arg" + num, type, ArgumentDirection.In);
                    metadata.Bind(current.Operand, argument);
                    metadata.AddArgument(argument);
                    num++;
                }
            }
        }


        private void ValidateSelects(CodeActivityMetadata metadata)
        {
            if (this.SelectColumns == null)
            {
                return;
            }
            if (this.SelectColumns.Count == 1 && this.SelectColumns[0] == null)
            {
                return;
            }
            int num = 0;
            foreach (InArgument current in this.SelectColumns)
            {
                if (current == null)
                {
                    metadata.AddValidationError("SelectInvalidColumn");
                    break;
                }
                Type argumentType = current.ArgumentType;
                if (!this.CheckColumnType(argumentType))
                {
                    metadata.AddValidationError("ColumnInvalidType");
                    break;
                }
                RuntimeArgument argument = new RuntimeArgument("SelectArg" + num, argumentType, ArgumentDirection.In);
                metadata.Bind(current, argument);
                metadata.AddArgument(argument);
                num++;
            }
        }

        private bool CheckColumnType(Type type)
        {
            return type == typeof(string) || type == typeof(int) || type == typeof(DataColumn) || type == typeof(GenericValue);
        }


        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            DataTable dataTable = this.DataTable.Get(context);
            if (dataTable == null)
            {
                throw new ArgumentException("");
            }
            DataTable dataTable2 = dataTable;
            DataTable dataTable3 = this.FilterRows(dataTable2, context);
            this.OutDataTable.Set(context, this.Select(dataTable3, context));

            m_Delegate = new runDelegate(Run);
            return m_Delegate.BeginInvoke(callback, state);
        }

        private void SetFirstFilterOperator(List<FilterOperationArgument> filters, BooleanOperator booleanOperator)
        {
            FilterOperationArgument argument = (filters != null) ? filters.FirstOrDefault<FilterOperationArgument>() : null;
            if (argument != null)
            {
                argument.BooleanOperator = booleanOperator;
            }
        }
        public static int? GetColumnIndex( DataTable dt, InArgument arg, ActivityContext context, bool throwIfNotFound = false)
        {
	        object argumentValue = (arg != null) ? arg.Get<object>(context) : null;
	        if (dt == null)
	        {
		        return null;
	        }
	        return dt.GetColumnIndex(argumentValue, throwIfNotFound);
        }


        private DataTable FilterRows(DataTable dataTable, ActivityContext context)
        {
            List<FilterOperationArgument> filters = this.Filters;
            bool? flag;
            if (filters == null)
            {
                flag = null;
            }
            else
            {
                flag = new bool?(filters.All((FilterOperationArgument filter) => filter == null || filter.IsFilterEmpty));
            }
            bool? flag2 = flag;
            if (flag2 == null || flag2.Value)
            {
                return dataTable.AsDataView().ToTable();
            }
            this.Filters.SetFirstFilterOperator(BooleanOperator.And);
            List<int> second = (from f in this.Filters
                                select dataTable.GetColumnIndex(f.Column, context, true).Value).ToList<int>();
            var source = (from f in this.Filters
                          select f.FilterOperationFactory(dataTable, context)).Zip(second, 
                          (IFilterOperation op, int i) => new
                          {
                              ColumnIndex = i,
                              Operation = op
                          });
            IEnumerable<BooleanOperator> filtersBooleanOperator = from f in this.Filters
                                                                  select f.BooleanOperator;
            DataTable dataTable2 = dataTable.Clone();
            bool flag3 = this.FilterRowsMode == SelectMode.Keep;
            int rowIndex;
            int rowIndex2;
            for (rowIndex = 0; rowIndex < dataTable.Rows.Count; rowIndex = rowIndex2)
            {
                List<bool> list = new List<bool>();
                DataRow dataRow = dataTable.Rows[rowIndex];
                foreach(var item in source)
                {
                    if (item.Operation == null || !item.Operation.Validate(dataRow, item.ColumnIndex))
                    {
                        list.Add(false);
                    }
                    list.Add(item.Operation.ApplyOn(dataRow, item.ColumnIndex));
                }
                IEnumerable<bool> filtersResult = list.AsEnumerable();
                //IEnumerable<bool> filtersResult = source.Select(delegate (f, int i)
                //{
                //    if (f.Operation == null || !f.Operation.Validate(dataRow, f.ColumnIndex))
                //    {
                //        return false;
                //    }
                //    return f.Operation.ApplyOn(dataRow, f.ColumnIndex);
                //});

                bool flag4 = this.ApplyFiltersOnRow(filtersResult, filtersBooleanOperator);
                if ((flag4 && flag3) || (!flag4 && !flag3))
                {
                    dataTable2.ImportRow(dataTable.Rows[rowIndex]);
                }
                rowIndex2 = rowIndex + 1;
            }
            return dataTable2;
        }

        private bool ApplyFiltersOnRow(IEnumerable<bool> filtersResult, IEnumerable<BooleanOperator> filtersBooleanOperator)
        {

            bool filterResult = true;
            foreach (var type in filtersResult.Zip(filtersBooleanOperator, (fr, op) => new {
                Operator = op,
                FilterResult = fr
            }))
            {
                if (type.Operator == BooleanOperator.And)
                {
                    filterResult = !filterResult ? false : type.FilterResult;

                }
                else
                {
                    if (filterResult)
                    {
                        return true;
                    }
                    filterResult = type.FilterResult;
                }
            }
            return filterResult;
        }

        private DataTable Select(DataTable dataTable, ActivityContext context)
        {
            DataView dataView = dataTable.AsDataView();
            List<InArgument> selectColumns = this.SelectColumns;
            IEnumerable<InArgument> enumerable;
            if (selectColumns == null)
            {
                enumerable = null;
            }
            else
            {
                enumerable = from a in selectColumns
                             where a != null
                             select a;
            }
            IEnumerable<InArgument> enumerable2 = enumerable;
            if (enumerable2 == null || !enumerable2.Any<InArgument>())
            {
                return dataView.ToTable();
            }
            List<string> columnNamesToKeep = new List<string>();
            if (this.SelectColumnsMode == SelectMode.Remove)
            {
                HashSet<int> removeColumnsIndex = new HashSet<int>();
                this.SelectColumns.ForEach(delegate (InArgument col)
                {
                    int? num = null;
                    try
                    {
                        num = dataTable.GetColumnIndex(col, context, true);
                    }
                    catch (ArgumentException ex)
                    {
                        throw new ArgumentException( ex.Message);
                    }
                    if (num != null)
                    {
                        removeColumnsIndex.Add(num.Value);
                    }
                });
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    if (!removeColumnsIndex.Contains(i))
                    {
                        columnNamesToKeep.Add(dataTable.Columns[i].ColumnName);
                    }
                }
            }
            else if (this.SelectColumnsMode == SelectMode.Keep)
            {
                this.SelectColumns.ForEach(delegate (InArgument col)
                {
                    int? columnIndex = dataTable.GetColumnIndex(col, context, false);
                    if (columnIndex != null)
                    {
                        columnNamesToKeep.Add(dataTable.Columns[columnIndex.Value].ColumnName);
                    }
                });
            }
            return dataView.ToTable(false, columnNamesToKeep.ToArray());
        }


        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
        }
    }
}