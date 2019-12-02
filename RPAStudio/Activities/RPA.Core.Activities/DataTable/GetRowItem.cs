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


        [Category("输入")]
        [RequiredArgument]
        [DisplayName("行")]
        [Description("要从中检索值的DataRow对象")]
        public InArgument<DataRow> DataRow { get; set; }


        [Category("输入")]
        [RequiredArgument]
        [OverloadGroup("DataColumn")]
        [DisplayName("列")]
        [Description("要从DataRow检索其值的DataColumn对象。如果设置了此属性，则忽略ColumnName和ColumnIndex属性。")]
        public InArgument<DataColumn> DataColumn { get; set; }

        [Category("输入")]
        [RequiredArgument]
        [OverloadGroup("ColumnIndex")]
        [DisplayName("列索引")]
        [Description("要从DataRow检索其值的列的索引")]
        public InArgument<Int32> ColumnIndex { get; set; }

        [Category("输入")]
        [RequiredArgument]
        [OverloadGroup("ColumnNames")]
        [DisplayName("列名称")]
        [Description("要从DataRow检索其值的列的名称。如果设置了此属性，则忽略ColumnIndex属性")]
        public InArgument<string> ColumnName { get; set; }



        [Category("输出")]
        [DisplayName("值")]
        [Description("指定DataRow的列值")]
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