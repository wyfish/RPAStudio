using RPA.Core.Activities.DataTableActivity.Operators;
using Plugins.Shared.Library.Librarys;
using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace RPA.Core.Activities.DataTableActivity
{
    [Designer(typeof(JoinDataTablesDesigner))]
    public sealed class JoinDataTables : AsyncCodeActivity
    {
        public JoinDataTables()
        {
            
        }

        public new string DisplayName
        {
            get
            {
                return "JoinDataTables";
            }
        }

        [Category("输入")]
        [RequiredArgument]
        [DisplayName("DataTable1")]
        [Description("要在此操作中使用的第一个表，存储在DataTable变量中。该字段仅支持DataTable变量")]
        public InArgument<DataTable> DataTable1 { get; set; }


        [Category("输入")]
        [RequiredArgument]
        [DisplayName("DataTable2")]
        [Description("要在此操作中使用的第二个表，存储在DataTable变量中。该字段仅支持DataTable变量")]
        public InArgument<DataTable> DataTable2 { get; set; }


        [Category("选项")]
        [DisplayName("JoinType")]
        public JoinType JoinType { get; set; }


        [Category("输出")]
        [RequiredArgument]
        [DisplayName("DataTable")]
        public OutArgument<DataTable> DataTable { get; set; }

        [Browsable(false)]
        public List<JoinOperationArgument> Arguments
        {
            get;
            set;
        }


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


        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            DataTable table = this.DataTable1.Get(context);
            DataTable table2 = this.DataTable2.Get(context);
            DataTable table3 = this.JoinTables(table, table2, context);
            this.DataTable.Set(context, table3);

            m_Delegate = new runDelegate(Run);
            return m_Delegate.BeginInvoke(callback, state);
        }


        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
        }

        private bool ApplyJoinArgumentsOnRows(DataRow leftRow, DataRow rightRow, List<SimplifiedArgument> joinArguments)
        {
            foreach (SimplifiedArgument argument in joinArguments)
            {
                if ((argument.Operation == null) || !argument.Operation.Validate(leftRow, argument.Column1, rightRow, argument.Column2))
                {
                    return false;
                }
                if (!argument.Operation.ApplyOn(leftRow, argument.Column1, rightRow, argument.Column2))
                {
                    return false;
                }
            }
            return true;
        }

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);
            this.ValidateArguments(metadata);
        }

        private bool CheckColumnType(Type type)
        {
            if ((!(type == typeof(string)) && !(type == typeof(int))) && !(type == typeof(DataColumn)))
            {
                return (type == typeof(GenericValue));
            }
            return true;
        }

        private Dictionary<object, List<int>>[] CreateDictionaries(DataTable dataTable, List<SimplifiedArgument> joinOperations)
        {
            Dictionary<object, List<int>>[] array = new Dictionary<object, List<int>>[joinOperations.Count];
            for (int i = 0; i < joinOperations.Count; i++)
            {
                array[i] = new Dictionary<object, List<int>>();
            }
            int num = 0;
            foreach (DataRow dataRow in dataTable.Rows)
            {
                int num2 = 0;
                foreach (SimplifiedArgument current in joinOperations)
                {
                    object key = dataRow[current.Column1];
                    List<int> list;
                    if (!array[num2].TryGetValue(key, out list))
                    {
                        list = new List<int>();
                        array[num2].Add(key, list);
                    }
                    list.Add(num);
                    num2++;
                }
                num++;
            }
            return array;
        }

        private DataTable CreateResult(List<Tuple<DataRow, DataRow>> resultedRows, DataTable dataTable1, DataTable dataTable2)
        {
            DataTable table = new DataTable();
            foreach (DataColumn column in dataTable1.Columns)
            {
                table.Columns.Add(new DataColumn(column.ColumnName, column.DataType));
            }
            foreach (DataColumn column2 in dataTable2.Columns)
            {
                if (table.Columns.Contains(column2.ColumnName))
                {
                    int num = 1;
                    while (table.Columns.Contains(column2.ColumnName + "_" + num))
                    {
                        num++;
                    }
                    table.Columns.Add(new DataColumn(column2.ColumnName + "_" + num, column2.DataType));
                }
                else
                {
                    table.Columns.Add(new DataColumn(column2.ColumnName, column2.DataType));
                }
            }
            foreach (Tuple<DataRow, DataRow> tuple in resultedRows)
            {
                object[] destinationArray = new object[tuple.Item1.ItemArray.Length + tuple.Item2.ItemArray.Length];
                Array.Copy(tuple.Item1.ItemArray, destinationArray, tuple.Item1.ItemArray.Length);
                Array.Copy(tuple.Item2.ItemArray, 0, destinationArray, tuple.Item1.ItemArray.Length, tuple.Item2.ItemArray.Length);
                table.Rows.Add(destinationArray);
            }
            return table;
        }

        private List<Tuple<DataRow, DataRow>> JoinRows(DataTable dataTable1, DataTable dataTable2, ActivityContext context)
        {
            List<Tuple<DataRow, DataRow>> list = new List<Tuple<DataRow, DataRow>>();
            List<int> second = (from a in Arguments select dataTable1.GetColumnIndex(a.Column1, context, true).Value).ToList<int>();
            List<int> list3 = (from a in Arguments select dataTable2.GetColumnIndex(a.Column2, context, true).Value).ToList<int>();
            List<SimplifiedArgument> joinArguments = (from a in this.Arguments select a.JoinOperationFactory(context)).Zip(second, (op, i) => new {
                Op = op,
                Column1 = i
            }).Zip(list3, (prev, j) => new SimplifiedArgument
            {
                Operation = prev.Op,
                Column1 = prev.Column1,
                Column2 = j
            }).ToList<SimplifiedArgument>();
            bool[] flagArray = new bool[dataTable2.Rows.Count];
            List<DataRow> list5 = new List<DataRow>();
            foreach (DataRow row in dataTable1.Rows)
            {
                bool flag = false;
                int index = 0;
                foreach (DataRow row2 in dataTable2.Rows)
                {
                    if (this.ApplyJoinArgumentsOnRows(row, row2, joinArguments))
                    {
                        flag = true;
                        flagArray[index] = true;
                        list.Add(new Tuple<DataRow, DataRow>(row, row2));
                    }
                    index++;
                }
                if (!flag)
                {
                    list5.Add(row);
                }
            }
            if ((this.JoinType == JoinType.Left) || (this.JoinType == JoinType.Full))
            {
                DataRow row3 = dataTable2.NewRow();
                foreach (DataRow row4 in list5)
                {
                    list.Add(new Tuple<DataRow, DataRow>(row4, row3));
                }
            }
            if (this.JoinType == JoinType.Full)
            {
                DataRow row5 = dataTable1.NewRow();
                for (int k = 0; k < flagArray.Length; k++)
                {
                    if (!flagArray[k])
                    {
                        list.Add(new Tuple<DataRow, DataRow>(row5, dataTable2.Rows[k]));
                    }
                }
            }
            return list;
        }

        private List<Tuple<DataRow, DataRow>> JoinRows(DataTable dataTable1, DataTable dataTable2, ActivityContext context, bool onlyEquals)
        {
            if (onlyEquals)
            {
                return this.JoinRowsWithEquals(dataTable1, dataTable2, context);
            }
            return this.JoinRows(dataTable1, dataTable2, context);
        }

        private List<Tuple<DataRow, DataRow>> JoinRowsWithEquals(DataTable dataTable1, DataTable dataTable2, ActivityContext context)
        {
            List<Tuple<DataRow, DataRow>> list = new List<Tuple<DataRow, DataRow>>();
            var second = (from a in this.Arguments select dataTable1.GetColumnIndex(a.Column1, context, true).Value).ToList<int>().Zip((from a in this.Arguments select dataTable2.GetColumnIndex(a.Column2, context, true).Value).ToList<int>(), (c1, c2) => new {
                Column1 = c1,
                Column2 = c2
            }).ToList();
            List<SimplifiedArgument> joinOperations = (from a in this.Arguments select a.JoinOperationFactory(context)).Zip(second, (arg, columns) => new SimplifiedArgument
            {
                Operation = arg,
                Column1 = columns.Column1,
                Column2 = columns.Column2
            }).ToList<SimplifiedArgument>();
            bool[] flagArray = new bool[dataTable2.Rows.Count];
            bool[] flagArray2 = new bool[dataTable1.Rows.Count];
            Dictionary<object, List<int>>[] dictionaryArray = this.CreateDictionaries(dataTable1, joinOperations);
            int index = 0;
            foreach (DataRow row in dataTable2.Rows)
            {
                int num2 = 0;
                List<int> first = null;
                foreach (SimplifiedArgument argument in joinOperations)
                {
                    List<int> list6;
                    if (dictionaryArray[num2].TryGetValue(row[argument.Column2], out list6))
                    {
                        if (first == null)
                        {
                            first = list6;
                            goto Label_018E;
                        }
                        first = first.Intersect<int>(list6).ToList<int>();
                        if (first.Count != 0)
                        {
                            goto Label_018E;
                        }
                    }
                    else
                    {
                        first = null;
                    }
                    break;
                    Label_018E:
                    num2++;
                }
                if ((first != null) && (first.Count > 0))
                {
                    flagArray[index] = true;
                    foreach (int num3 in first)
                    {
                        list.Add(new Tuple<DataRow, DataRow>(dataTable1.Rows[num3], dataTable2.Rows[index]));
                        flagArray2[num3] = true;
                    }
                }
                index++;
            }
            if ((this.JoinType == JoinType.Left) || (this.JoinType == JoinType.Full))
            {
                DataRow row2 = dataTable2.NewRow();
                for (int i = 0; i < flagArray2.Length; i++)
                {
                    if (!flagArray2[i])
                    {
                        list.Add(new Tuple<DataRow, DataRow>(dataTable1.Rows[i], row2));
                    }
                }
            }
            if (this.JoinType == JoinType.Full)
            {
                DataRow row3 = dataTable1.NewRow();
                for (int i = 0; i < flagArray.Length; i++)
                {
                    if (!flagArray[i])
                    {
                        list.Add(new Tuple<DataRow, DataRow>(row3, dataTable2.Rows[i]));
                    }
                }
            }
            return list;
        }

        private DataTable JoinTables(DataTable dataTable1, DataTable dataTable2, ActivityContext context)
        {
            List<JoinOperationArgument> source = Arguments;
            if ((source == null) || (source.Count == 0))
            {
                throw new Exception("NoJoinArguments");
            }
            bool onlyEquals = !source.Any<JoinOperationArgument>(a => (a.Operator != JoinOperator.EQ));
            List<Tuple<DataRow, DataRow>> resultedRows = this.JoinRows(dataTable1, dataTable2, context, onlyEquals);
            return this.CreateResult(resultedRows, dataTable1, dataTable2);
        }

        private void ValidateArguments(CodeActivityMetadata metadata)
        {
            if (this.Arguments == null)
            {
                metadata.AddValidationError("NoJoinArguments");
            }
            else if ((this.Arguments.Count == 1) && this.Arguments[0].IsJoinEmpty)
            {
                metadata.AddValidationError("NoJoinArguments");
            }
            else
            {
                int num = 0;
                foreach (JoinOperationArgument argument in this.Arguments)
                {
                    Type argumentType = typeof(object);
                    Type type2 = typeof(object);
                    if (argument.Column1 == null)
                    {
                        metadata.AddValidationError("JoinInvalidColumn1");
                        break;
                    }
                    if (argument.Column2 == null)
                    {
                        metadata.AddValidationError("JoinInvalidColumn2");
                        break;
                    }
                    argumentType = argument.Column1.ArgumentType;
                    type2 = argument.Column2.ArgumentType;
                    if (!this.CheckColumnType(argumentType))
                    {
                        metadata.AddValidationError("Column1InvalidType");
                        break;
                    }
                    if (!this.CheckColumnType(type2))
                    {
                        metadata.AddValidationError("Column2InvalidType");
                        break;
                    }
                    RuntimeArgument argument2 = new RuntimeArgument("Arg" + num, argumentType, ArgumentDirection.In);
                    metadata.Bind(argument.Column1, argument2);
                    metadata.AddArgument(argument2);
                    num++;
                    RuntimeArgument argument3 = new RuntimeArgument("Arg" + num, type2, ArgumentDirection.In);
                    metadata.Bind(argument.Column2, argument3);
                    metadata.AddArgument(argument3);
                    num++;
                }
            }
        }
    }


    internal class SimplifiedArgument
    {
        public IJoinOperation Operation
        {
            get;
            set;
        }

        public int Column1
        {
            get;
            set;
        }

        public int Column2
        {
            get;
            set;
        }
    }
}