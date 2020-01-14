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

        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [RequiredArgument]
        [DisplayName("DataTable")]
        [Localize.LocalizedDescription("Description48")] //要在其中执行查找的DataTable变量 //The DataTable variable in which to perform the lookup //ルックアップを実行するDataTable変数
        public InArgument<DataTable> DataTable { get; set; }

        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [RequiredArgument]
        [Localize.LocalizedDisplayName("DisplayName26")] //查找值 //Find value //価値を見つける
        [Localize.LocalizedDescription("Description49")] //要在指定DataTable变量中搜索的值 //The value to search for in the specified DataTable variable //指定されたDataTable変数で検索する値
        public InArgument<string> LookupValue { get; set; }

        [Localize.LocalizedCategory("Category7")] //开始列 //Start column //開始列
        [RequiredArgument]
        [OverloadGroup("DataColumn")]
        [Localize.LocalizedDisplayName("DisplayName19")] //列 //Column //コラム
        [Localize.LocalizedDescription("Description50")] //要从DataRow检索其值的DataColumn对象。在此属性字段中设置变量会禁用其他两个属性 //The DataColumn object whose value is to be retrieved from the DataRow.  Setting a variable in this property field disables the other two properties //値がDataRowから取得されるDataColumnオブジェクト。 このプロパティフィールドで変数を設定すると、他の2つのプロパティが無効になります
        public InArgument<DataColumn> DataColumn { get; set; }

        [Localize.LocalizedCategory("Category7")] //开始列 //Start column //開始列
        [RequiredArgument]
        [OverloadGroup("ColumnIndex")]
        [Localize.LocalizedDisplayName("DisplayName20")] //列索引 //Column index //列インデックス
        [Localize.LocalizedDescription("Description51")] //要从DataRow检索其值的列的索引。在此属性字段中设置变量会禁用其他两个属性 //The index of the column whose value is to be retrieved from the DataRow.  Setting a variable in this property field disables the other two properties //値がDataRowから取得される列のインデックス。 このプロパティフィールドで変数を設定すると、他の2つのプロパティが無効になります
        public InArgument<Int32> ColumnIndex { get; set; }

        [Localize.LocalizedCategory("Category7")] //开始列 //Start column //開始列
        [RequiredArgument]
        [OverloadGroup("ColumnNames")]
        [Localize.LocalizedDisplayName("DisplayName21")] //列名称 //Column name //列名
        [Localize.LocalizedDescription("Description52")] //要从DataRow检索其值的列的名称。在此属性字段中设置变量会禁用其他两个属性 //The name of the column whose value is to be retrieved from the DataRow.  Setting a variable in this property field disables the other two properties //値がDataRowから取得される列の名前。 このプロパティフィールドで変数を設定すると、他の2つのプロパティが無効になります
        public InArgument<string> ColumnName { get; set; }


        [Localize.LocalizedCategory("Category8")] //结束列 //End column //終了列
        [OverloadGroup("TargetDataColumn")]
        [Localize.LocalizedDisplayName("DisplayName19")] //列 //Column //コラム
        [Localize.LocalizedDescription("Description53")] //返回在此列与RowIndex属性中的值之间的坐标处找到的单元格 //Returns the cell found at the coordinates between this column and the value in the RowIndex property //この列とRowIndexプロパティの値の間の座標にあるセルを返します
        public InArgument<DataColumn> TargetDataColumn { get; set; }

        [Localize.LocalizedCategory("Category8")] //结束列 //End column //終了列
        [OverloadGroup("TargetColumnIndex")]
        [Localize.LocalizedDisplayName("DisplayName20")] //列索引 //Column index //列インデックス
        [Localize.LocalizedDescription("Description54")] //返回在此列与RowIndex属性值之间的坐标处找到的单元格的列索引 //Returns the column index of the cell found at the coordinates between this column and the value of the RowIndex property //この列とRowIndexプロパティの値の間の座標にあるセルの列インデックスを返します
        public InArgument<Int32> TargetColumnIndex { get; set; }

        [Localize.LocalizedCategory("Category8")] //结束列 //End column //終了列
        [OverloadGroup("TargetColumnName")]
        [Localize.LocalizedDisplayName("DisplayName21")] //列名称 //Column name //列名
        [Localize.LocalizedDescription("Description55")] //返回在此列与RowIndex属性中的值之间的坐标处找到的单元格的列名称 //Returns the column name of the cell found at the coordinates between this column and the value in the RowIndex property //この列とRowIndexプロパティの値の間の座標にあるセルの列名を返します
        public InArgument<string> TargetColumnName { get; set; }



        [Localize.LocalizedCategory("Category2")] //输出 //Output //出力
        [Localize.LocalizedDisplayName("DisplayName27")] //单元格值 //Cell value //セル値
        [Localize.LocalizedDescription("Description56")] //单元格中找到的值 //The value found in the cell //セルで見つかった値
        public OutArgument<object> CellValue { get; set; }

        [Localize.LocalizedCategory("Category2")] //输出 //Output //出力
        [Localize.LocalizedDisplayName("DisplayName28")] //行索引 //Row index //行インデックス
        [Localize.LocalizedDescription("Description57")] //单元格的Row索引 //Row index of the cell //セルの行インデックス
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
