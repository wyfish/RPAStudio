using System;
using System.Activities;
using System.ComponentModel;
using System.Data;

namespace RPA.Core.Activities.DataTableActivity
{
    [Designer(typeof(SortDataTableDesigner))]
    public sealed class SortDataTable : CodeActivity
    {
        public SortDataTable()
        {
            
        }

        public new string DisplayName
        {
            get
            {
                return "SortDataTable";
            }
        }


        [Category("输入")]
        [RequiredArgument]
        [DisplayName("DataTable")]
        [Description("要排序的DataTable变量")]
        public InArgument<DataTable> DataTable { get; set; }


        [Category("列排序")]
        [RequiredArgument]
        [OverloadGroup("DataColumn")]
        [DisplayName("列")]
        [Description("按其排序的列的变量。该字段仅支持DataColumn变量。在此属性字段中设置变量会禁用其他两个属性")]
        public InArgument<DataColumn> DataColumn { get; set; }

        [Category("列排序")]
        [OverloadGroup("ColumnIndex")]
        [RequiredArgument]
        [DisplayName("索引")]
        [Description("按其排序的列的变量。该字段仅支持DataColumn变量。在此属性字段中设置变量会禁用其他两个属性")]
        public InArgument<Int32> ColumnIndex { get; set; }

        [Category("列排序")]
        [RequiredArgument]
        [OverloadGroup("ColumnName")]
        [DisplayName("名称")]
        [Description("按其排序的列的变量。该字段仅支持DataColumn变量。在此属性字段中设置变量会禁用其他两个属性")]
        public InArgument<string> ColumnName { get; set; }

        public enum SortTypes
        {
            增序,
            降序
        }

        [Category("列排序")]
        [RequiredArgument]
        [DisplayName("排序")]
        public SortTypes SortType { get; set; }

        [Category("输出")]
        [RequiredArgument]
        [DisplayName("DataTable")]
        [Description("排序后的DataTable变量。放置与Input字段中的变量相同的变量会更改初始变量，而添加新变量会使初始变量不受影响。")]
        public OutArgument<DataTable> OutDataTable { get; set; }

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
            string SortColName = null;
            string SortText = null;

            if (dataColumn != null)
                SortColName = dataColumn.ColumnName;
            else if (columnName != null && columnName != "")
                SortColName = columnName;
            else
                SortColName = dataTable.Columns[columnIndex].ColumnName;

            SortText = SortType == SortTypes.增序 ? SortColName + "ASC" : SortColName + "DESC";
            dataTable.DefaultView.Sort = SortText;
            DataTable dtNew = dataTable.DefaultView.ToTable();
            OutDataTable.Set(context, dtNew);
        }
    }
}