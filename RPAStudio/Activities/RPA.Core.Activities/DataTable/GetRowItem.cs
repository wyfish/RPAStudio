using Plugins.Shared.Library;
using System;
using System.Activities;
using System.ComponentModel;
using System.Data;

namespace RPA.Core.Activities.DataTableActivity
{
    [Designer(typeof(GetRowItemDesigner))]
    public sealed class GetRowItem : AsyncCodeActivity
    {
        public GetRowItem()
        {
            
        }

        public new string DisplayName
        {
            get
            {
                return "GetRowItem";
            }
        }


        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [RequiredArgument]
        [Localize.LocalizedDisplayName("DisplayName18")] //行 //Row //行
        [Localize.LocalizedDescription("Description41")] //要从中检索值的DataRow对象 //The DataRow object from which to retrieve the value //値を取得するDataRowオブジェクト
        public InArgument<DataRow> DataRow { get; set; }


        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [RequiredArgument]
        [OverloadGroup("DataColumn")]
        [Localize.LocalizedDisplayName("DisplayName19")] //列 //Column //コラム
        [Localize.LocalizedDescription("Description42")] //要从DataRow检索其值的DataColumn对象。如果设置了此属性，则忽略ColumnName和ColumnIndex属性。 //The DataColumn object whose value is to be retrieved from the DataRow.  If this property is set, the ColumnName and ColumnIndex properties are ignored. //値がDataRowから取得されるDataColumnオブジェクト。 このプロパティが設定されている場合、ColumnNameプロパティとColumnIndexプロパティは無視されます。
        public InArgument<DataColumn> DataColumn { get; set; }

        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [RequiredArgument]
        [OverloadGroup("ColumnIndex")]
        [Localize.LocalizedDisplayName("DisplayName20")] //列索引 //Column index //列インデックス
        [Localize.LocalizedDescription("Description43")] //要从DataRow检索其值的列的索引 //The index of the column whose value is to be retrieved from the DataRow //値がDataRowから取得される列のインデックス
        public InArgument<Int32> ColumnIndex { get; set; }

        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [RequiredArgument]
        [OverloadGroup("ColumnNames")]
        [Localize.LocalizedDisplayName("DisplayName21")] //列名称 //Column name //列名
        [Localize.LocalizedDescription("Description44")] //要从DataRow检索其值的列的名称。如果设置了此属性，则忽略ColumnIndex属性 //The name of the column whose value is to be retrieved from the DataRow.  If this property is set, the ColumnIndex property is ignored //値がDataRowから取得される列の名前。 このプロパティが設定されている場合、ColumnIndexプロパティは無視されます
        public InArgument<string> ColumnName { get; set; }



        [Localize.LocalizedCategory("Category2")] //输出 //Output //出力
        [Localize.LocalizedDisplayName("DisplayName22")] //值 //value //価値
        [Localize.LocalizedDescription("Description45")] //指定DataRow的列值 //Specify the column value of the DataRow //DataRowの列値を指定します
        public OutArgument<object> Value { get; set; }



        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.Core.Activities;Component/Resources/DataTable/datatable.png";
            }
        }

        

        private delegate string runDelegate();
        private runDelegate m_Delegate;
        public string Run()
        {
            return DisplayName;
        }


        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            DataRow dataRow = DataRow.Get(context);
            DataColumn dataColumn = DataColumn.Get(context);
            Int32 columnIndex = ColumnIndex.Get(context);
            string columnName = ColumnName.Get(context);
            object value = null;
            try
            {
                if(dataColumn != null)
                {
                    value = dataRow[dataColumn];
                }
                else if(columnName != null && columnName != "")
                {
                    value = dataRow[columnName];
                }
                else
                {
                    value = dataRow[columnIndex];
                }
            }
            catch(Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "获取行数据失败", e.Message);
                throw e;
            }

            m_Delegate = new runDelegate(Run);
            return m_Delegate.BeginInvoke(callback, state);
        }

        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
        }
    }
}
