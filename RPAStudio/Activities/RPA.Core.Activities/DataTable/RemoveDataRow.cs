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


        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [RequiredArgument]
        [DisplayName("DataTable")]
        [Localize.LocalizedDescription("Description63")] //要从中删除列的DataTable对象 //The DataTable object from which to remove the column //列を削除するDataTableオブジェクト
        public InArgument<DataTable> DataTable { get; set; }


        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [RequiredArgument]
        [OverloadGroup("DataRow")]
        [Localize.LocalizedDisplayName("DisplayName18")] //行 //Row //行
        [Localize.LocalizedDescription("Description67")] //要删除的DataRow对象。如果设置了此属性，则忽略行索引选项 //The DataRow object to delete.  Ignore row indexing option if this property is set //削除するDataRowオブジェクト。 このプロパティが設定されている場合、行のインデックス付けオプションを無視する
        public InArgument<DataRow> DataRow { get; set; }

        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [RequiredArgument]
        [OverloadGroup("RowIndex ")]
        [Localize.LocalizedDisplayName("DisplayName28")] //行索引 //Row index //行インデックス
        [Localize.LocalizedDescription("Description68")] //要删除的行的索引。如果设置了此属性，则忽略DataRow索引选项 //The index of the row to delete.  Ignore DataRow indexing option if this property is set //削除する行のインデックス。 このプロパティが設定されている場合、DataRowインデックスオプションを無視します
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
