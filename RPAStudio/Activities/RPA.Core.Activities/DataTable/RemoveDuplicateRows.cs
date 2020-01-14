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


        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [RequiredArgument]
        [DisplayName("DataTable")]
        [Localize.LocalizedDescription("Description69")] //要从中删除重复行的DataTable变量 //The DataTable variable from which to remove duplicate rows //重複行を削除するDataTable変数
        public InArgument<DataTable> DataTable { get; set; }

        [Localize.LocalizedCategory("Category2")] //输出 //Output //出力
        [RequiredArgument]
        [DisplayName("DataTable")]
        [Localize.LocalizedDescription("Description70")] //输出已删除重复行的DataTable，存储在DataTable变量中。放置与Input字段中的变量相同的变量会更改初始变量，而提供新变量会使初始变量不受影响。 //Outputs a DataTable with duplicate rows removed, stored in the DataTable variable.  Placing the same variable as the variable in the Input field changes the initial variable, while providing a new variable makes the initial variable unaffected. //DataTable変数に保存された重複行が削除されたDataTableを出力します。 入力フィールドの変数と同じ変数を配置すると、初期変数が変更されますが、新しい変数を指定すると、初期変数は影響を受けません。
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
