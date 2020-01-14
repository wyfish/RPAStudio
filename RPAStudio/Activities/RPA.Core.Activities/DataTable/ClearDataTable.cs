using Plugins.Shared.Library;
using System;
using System.Activities;
using System.ComponentModel;
using System.Data;

namespace RPA.Core.Activities.DataTableActivity
{
    [Designer(typeof(ClearDataTableDesigner))]
    public sealed class ClearDataTable : AsyncCodeActivity
    {
        public ClearDataTable()
        {

        }

        public new string DisplayName
        {
            get
            {
                return "ClearDataTable";
            }
        }


        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [RequiredArgument]
        [DisplayName("DataTable")]
        [Localize.LocalizedDescription("Description26")] //要清除所有数据的DataTable对象 //The DataTable object to clear all data //すべてのデータをクリアするDataTableオブジェクト
        public InArgument<DataTable> DataTable { get; set; }


        [Browsable(false)]
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
            DataTable dataTable = DataTable.Get(context);
            try
            {
                dataTable.Clear();
            }
            catch(Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "清空数据表失败", e.Message);
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
