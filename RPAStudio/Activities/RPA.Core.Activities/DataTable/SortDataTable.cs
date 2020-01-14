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


        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [RequiredArgument]
        [DisplayName("DataTable")]
        [Localize.LocalizedDescription("Description71")] //要排序的DataTable变量 //The DataTable variable to be sorted //ソートされるDataTable変数
        public InArgument<DataTable> DataTable { get; set; }


        [Localize.LocalizedCategory("Category9")] //列排序 //Column sort //列ソート
        [RequiredArgument]
        [OverloadGroup("DataColumn")]
        [Localize.LocalizedDisplayName("DisplayName19")] //列 //Column //コラム
        [Localize.LocalizedDescription("Description72")] //按其排序的列的变量。该字段仅支持DataColumn变量。在此属性字段中设置变量会禁用其他两个属性 //The variable of the column sorted by it.  This field only supports DataColumn variables.  Setting a variable in this property field disables the other two properties //ソートされた列の変数。 このフィールドは、DataColumn変数のみをサポートします。 このプロパティフィールドで変数を設定すると、他の2つのプロパティが無効になります
        public InArgument<DataColumn> DataColumn { get; set; }

        [Localize.LocalizedCategory("Category9")] //列排序 //Column sort //列ソート
        [OverloadGroup("ColumnIndex")]
        [RequiredArgument]
        [Localize.LocalizedDisplayName("DisplayName9")] //索引 //index //索引
        [Localize.LocalizedDescription("Description72")] //按其排序的列的变量。该字段仅支持DataColumn变量。在此属性字段中设置变量会禁用其他两个属性 //The variable of the column sorted by it.  This field only supports DataColumn variables.  Setting a variable in this property field disables the other two properties //ソートされた列の変数。 このフィールドは、DataColumn変数のみをサポートします。 このプロパティフィールドで変数を設定すると、他の2つのプロパティが無効になります
        public InArgument<Int32> ColumnIndex { get; set; }

        [Localize.LocalizedCategory("Category9")] //列排序 //Column sort //列ソート
        [RequiredArgument]
        [OverloadGroup("ColumnName")]
        [Localize.LocalizedDisplayName("DisplayName33")] //名称 //Name //お名前
        [Localize.LocalizedDescription("Description72")] //按其排序的列的变量。该字段仅支持DataColumn变量。在此属性字段中设置变量会禁用其他两个属性 //The variable of the column sorted by it.  This field only supports DataColumn variables.  Setting a variable in this property field disables the other two properties //ソートされた列の変数。 このフィールドは、DataColumn変数のみをサポートします。 このプロパティフィールドで変数を設定すると、他の2つのプロパティが無効になります
        public InArgument<string> ColumnName { get; set; }

        public enum SortTypes
        {
            增序,
            降序
        }

        [Localize.LocalizedCategory("Category9")] //列排序 //Column sort //列ソート
        [RequiredArgument]
        [Localize.LocalizedDisplayName("DisplayName34")] //排序 //Sort //並べ替え
        public SortTypes SortType { get; set; }

        [Localize.LocalizedCategory("Category2")] //输出 //Output //出力
        [RequiredArgument]
        [DisplayName("DataTable")]
        [Localize.LocalizedDescription("Description73")] //排序后的DataTable变量。放置与Input字段中的变量相同的变量会更改初始变量，而添加新变量会使初始变量不受影响。 //The sorted DataTable variable.  Placing the same variable as the variable in the Input field changes the initial variable, and adding a new variable leaves the initial variable unaffected. //ソートされたDataTable変数。 入力フィールドの変数と同じ変数を配置すると、初期変数が変更され、新しい変数を追加しても初期変数は影響を受けません。
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
