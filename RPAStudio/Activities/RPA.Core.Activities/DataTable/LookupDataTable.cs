using Plugins.Shared.Library;
using System;
using System.Activities;
using System.ComponentModel;
using System.Data;

namespace RPA.Core.Activities.DataTableActivity
{
    [Designer(typeof(LookupDataTableDesigner))]
    public sealed class LookupDataTable : CodeActivity
    {
        public LookupDataTable()
        {
            
        }

        public new string DisplayName
        {
            get
            {
                return "LookupDataTable";
            }
        }

        [Category("输入")]
        [RequiredArgument]
        [DisplayName("DataTable")]
        [Description("要在其中执行查找的DataTable变量")]
        public InArgument<DataTable> DataTable { get; set; }

        [Category("输入")]
        [RequiredArgument]
        [DisplayName("查找值")]
        [Description("要在指定DataTable变量中搜索的值")]
        public InArgument<string> LookupValue { get; set; }

        [Category("开始列")]
        [RequiredArgument]
        [OverloadGroup("DataColumn")]
        [DisplayName("列")]
        [Description("要从DataRow检索其值的DataColumn对象。在此属性字段中设置变量会禁用其他两个属性")]
        public InArgument<DataColumn> DataColumn { get; set; }

        [Category("开始列")]
        [RequiredArgument]
        [OverloadGroup("ColumnIndex")]
        [DisplayName("列索引")]
        [Description("要从DataRow检索其值的列的索引。在此属性字段中设置变量会禁用其他两个属性")]
        public InArgument<Int32> ColumnIndex { get; set; }

        [Category("开始列")]
        [RequiredArgument]
        [OverloadGroup("ColumnNames")]
        [DisplayName("列名称")]
        [Description("要从DataRow检索其值的列的名称。在此属性字段中设置变量会禁用其他两个属性")]
        public InArgument<string> ColumnName { get; set; }


        [Category("结束列")]
        [OverloadGroup("TargetDataColumn")]
        [DisplayName("列")]
        [Description("返回在此列与RowIndex属性中的值之间的坐标处找到的单元格")]
        public InArgument<DataColumn> TargetDataColumn { get; set; }

        [Category("结束列")]
        [OverloadGroup("TargetColumnIndex")]
        [DisplayName("列索引")]
        [Description("返回在此列与RowIndex属性值之间的坐标处找到的单元格的列索引")]
        public InArgument<Int32> TargetColumnIndex { get; set; }

        [Category("结束列")]
        [OverloadGroup("TargetColumnName")]
        [DisplayName("列名称")]
        [Description("返回在此列与RowIndex属性中的值之间的坐标处找到的单元格的列名称")]
        public InArgument<string> TargetColumnName { get; set; }



        [Category("输出")]
        [DisplayName("单元格值")]
        [Description("单元格中找到的值")]
        public OutArgument<object> CellValue { get; set; }

        [Category("输出")]
        [DisplayName("行索引")]
        [Description("单元格的Row索引")]
        public OutArgument<Int32> RowIndex { get; set; }


        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.Core.Activities;Component/Resources/DataTable/datatable.png";
            }
        }

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);
        }

        protected override void Execute(CodeActivityContext context)
        {
            DataTable dataTable = DataTable.Get(context);
            string lookupValue = LookupValue.Get(context);
            DataColumn dataColumn = DataColumn.Get(context);
            DataColumn targetDataColumn = TargetDataColumn.Get(context);
            Int32 columnIndex = ColumnIndex.Get(context);
            Int32 targetColumnIndex = ColumnIndex.Get(context);
            string columnName = ColumnName.Get(context);
            string targetColumnName = TargetColumnName.Get(context);

            object cellValue = null;
            Int32 rowIndex = 0;

            try
            {
                int beginIndex = 0, endInex = 0;

                DataColumn beginColumn = new DataColumn();
                if (dataColumn != null) beginIndex = dataTable.Columns.IndexOf(dataColumn);
                else if (columnName != null && columnName != "") beginIndex = dataTable.Columns.IndexOf(columnName);
                else beginIndex = columnIndex;
                if (targetDataColumn != null) endInex = dataTable.Columns.IndexOf(targetDataColumn);
                else if (targetColumnName != null && targetColumnName != "") endInex = dataTable.Columns.IndexOf(targetColumnName);
                else endInex = targetColumnIndex;

                if (beginIndex < 0 || endInex < 0 || beginIndex > endInex)
                {
                    SharedObject.Instance.Output(SharedObject.enOutputType.Error, "数据表列索引有误,请检查开始列与结束列");
                    return;
                }

                DataRowCollection dataRows = dataTable.Rows;
                for (int index = beginIndex; index < endInex; index++)
                {
                    foreach (DataRow datarow in dataRows)
                    {
                        object data = datarow[index];
                        string dataStr = data as string;
                        if (dataStr.Equals(lookupValue))
                        {
                            rowIndex = dataRows.IndexOf(datarow);
                            cellValue = data;
                            break;
                        }
                    }
                }
                if (CellValue != null)
                    CellValue.Set(context, cellValue);
                if (RowIndex != null)
                    RowIndex.Set(context, rowIndex);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "查找数据表失败", e.Message);
            }
        }
    }
}