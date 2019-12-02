using System;
using System.Activities;
using System.ComponentModel;
using System.Data;

namespace RPA.Core.Activities.DataTableActivity
{
    [Designer(typeof(RemoveDuplicateRowsDesigner))]
    public sealed class RemoveDuplicateRows : CodeActivity
    {
        public RemoveDuplicateRows()
        {
            
        }

        public new string DisplayName
        {
            get
            {
                return "RemoveDuplicateRows";
            }
        }


        [Category("输入")]
        [RequiredArgument]
        [DisplayName("DataTable")]
        [Description("要从中删除重复行的DataTable变量")]
        public InArgument<DataTable> DataTable { get; set; }

        [Category("输出")]
        [RequiredArgument]
        [DisplayName("DataTable")]
        [Description("输出已删除重复行的DataTable，存储在DataTable变量中。放置与Input字段中的变量相同的变量会更改初始变量，而提供新变量会使初始变量不受影响。")]
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
            DataView dataView = dataTable.AsDataView();
            DataTable outTable =  dataView.ToTable(true);
            OutDataTable.Set(context, outTable);
        }
    }
}