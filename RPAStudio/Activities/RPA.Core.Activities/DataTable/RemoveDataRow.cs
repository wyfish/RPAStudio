using System;
using System.Activities;
using System.ComponentModel;
using System.Data;

namespace RPA.Core.Activities.DataTableActivity
{
    [Designer(typeof(RemoveDataRowDesigner))]
    public sealed class RemoveDataRow : CodeActivity
    {
        public RemoveDataRow()
        {
            
        }

        public new string DisplayName
        {
            get
            {
                return "RemoveDataRow";
            }
        }


        [Category("输入")]
        [RequiredArgument]
        [DisplayName("DataTable")]
        [Description("要从中删除列的DataTable对象")]
        public InArgument<DataTable> DataTable { get; set; }


        [Category("输入")]
        [RequiredArgument]
        [OverloadGroup("DataRow")]
        [DisplayName("行")]
        [Description("要删除的DataRow对象。如果设置了此属性，则忽略行索引选项")]
        public InArgument<DataRow> DataRow { get; set; }

        [Category("输入")]
        [RequiredArgument]
        [OverloadGroup("RowIndex ")]
        [DisplayName("行索引")]
        [Description("要删除的行的索引。如果设置了此属性，则忽略DataRow索引选项")]
        public InArgument<Int32> RowIndex { get; set; }


        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.Core.Activities;Component/Resources/DataTable/datatable.png";
            }
        }
        

        protected override void Execute(CodeActivityContext context)
        {
            DataRow dataRow = DataRow.Get(context);
            Int32 rowIndex = RowIndex.Get(context);
            if (dataRow == null)
                DataTable.Get(context).Rows.RemoveAt(rowIndex);
            else
                DataTable.Get(context).Rows.Remove(dataRow);
        }
    }
}