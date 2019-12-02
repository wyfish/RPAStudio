using System;
using System.Activities;
using System.ComponentModel;
using System.Data;

namespace RPA.Core.Activities.DataTableActivity
{
    [Designer(typeof(RemoveDataColumnDesigner))]
    public sealed class RemoveDataColumn : CodeActivity
    {
        public RemoveDataColumn()
        {
            
        }

        public new string DisplayName
        {
            get
            {
                return "RemoveDataColumn";
            }
        }


        [Category("输入")]
        [RequiredArgument]
        [DisplayName("DataTable")]
        [Description("要从中删除列的DataTable对象")]
        public InArgument<DataTable> DataTable { get; set; }


        [Category("输入")]
        [RequiredArgument]
        [OverloadGroup("DataColumn")]
        [DisplayName("列")]
        [Description("要从DataTable的列集合中删除的DataColumn对象。如果设置了此属性，则忽略其它两个列索引选项")]
        public InArgument<DataColumn> DataColumn { get; set; }

        [Category("输入")]
        [RequiredArgument]
        [OverloadGroup("ColumnIndex")]
        [DisplayName("列索引")]
        [Description("要从DataTable的列集合中删除的列的索引。如果设置了此属性，则忽略其它两个列索引选项")]
        public InArgument<Int32> ColumnIndex { get; set; }

        [Category("输入")]
        [RequiredArgument]
        [OverloadGroup("ColumnName")]
        [DisplayName("列名称")]
        [Description("要从DataTable的列集合中删除的列的名称。如果设置了此属性，则忽略其它两个列索引选项")]
        public InArgument<string> ColumnName { get; set; }


        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.Core.Activities;Component/Resources/DataTable/datatable.png";
            }
        }
        
        protected override void Execute(CodeActivityContext context)
        {
            DataTable dataTable = DataTable.Get(context);
            DataColumn dataColumn = DataColumn.Get(context);
            string columnName = ColumnName.Get(context);
            Int32 columnIndex = ColumnIndex.Get(context);

            if (dataColumn != null)
                dataTable.Columns.Remove(dataColumn);
            else if (columnName == null || columnName == "")
                dataTable.Columns.RemoveAt(columnIndex);
            else
                dataTable.Columns.Remove(columnName);
        }
    }
}