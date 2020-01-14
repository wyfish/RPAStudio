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


        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [RequiredArgument]
        [DisplayName("DataTable")]
        [Localize.LocalizedDescription("Description63")] //要从中删除列的DataTable对象 //The DataTable object from which to remove the column //列を削除するDataTableオブジェクト
        public InArgument<DataTable> DataTable { get; set; }


        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [RequiredArgument]
        [OverloadGroup("DataColumn")]
        [Localize.LocalizedDisplayName("DisplayName19")] //列 //Column //コラム
        [Localize.LocalizedDescription("Description64")] //要从DataTable的列集合中删除的DataColumn对象。如果设置了此属性，则忽略其它两个列索引选项 //The DataColumn object to be removed from the column set of the DataTable.  If this property is set, the other two column index options are ignored //DataTableの列セットから削除されるDataColumnオブジェクト。 このプロパティが設定されている場合、他の2つの列インデックスオプションは無視されます
        public InArgument<DataColumn> DataColumn { get; set; }

        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [RequiredArgument]
        [OverloadGroup("ColumnIndex")]
        [Localize.LocalizedDisplayName("DisplayName20")] //列索引 //Column index //列インデックス
        [Localize.LocalizedDescription("Description65")] //要从DataTable的列集合中删除的列的索引。如果设置了此属性，则忽略其它两个列索引选项 //The index of the column to remove from the column set of the DataTable.  If this property is set, the other two column index options are ignored //DataTableの列セットから削除する列のインデックス。 このプロパティが設定されている場合、他の2つの列インデックスオプションは無視されます
        public InArgument<Int32> ColumnIndex { get; set; }

        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [RequiredArgument]
        [OverloadGroup("ColumnName")]
        [Localize.LocalizedDisplayName("DisplayName21")] //列名称 //Column name //列名
        [Localize.LocalizedDescription("Description66")] //要从DataTable的列集合中删除的列的名称。如果设置了此属性，则忽略其它两个列索引选项 //The name of the column to remove from the column set of the DataTable.  If this property is set, the other two column index options are ignored //DataTableの列セットから削除する列の名前。 このプロパティが設定されている場合、他の2つの列インデックスオプションは無視されます
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
