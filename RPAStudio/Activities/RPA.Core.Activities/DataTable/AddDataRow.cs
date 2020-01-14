using Plugins.Shared.Library;
using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

namespace RPA.Core.Activities.DataTableActivity
{
    [Designer(typeof(AddDataRowDesigner))]
    public sealed class AddDataRow : AsyncCodeActivity
    {
        public AddDataRow()
        {
            
        }

        public new string DisplayName
        {
            get
            {
                return "AddDataRow";
            }
        }


        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [RequiredArgument]
        [OverloadGroup("DataRow")]
        [DisplayName("DataRow")]
        [Localize.LocalizedDescription("Description22")] //要添加到DataTable的DataRow对象，如果设置了此属性，则忽略ArrayRow属性。 //The DataRow object to be added to the DataTable. If this property is set, the ArrayRow property is ignored. //DataTableに追加されるDataRowオブジェクトこのプロパティが設定されている場合、ArrayRowプロパティは無視されます。
        public InArgument<DataRow> DataRow { get; set; }

        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [RequiredArgument]
        [OverloadGroup("ArrayRow")]
        [DisplayName("ArrayRow")]
        [Localize.LocalizedDescription("Description23")] //要添加到DataTable的对象数组。每个对象的类型应映射到DataTable中其对应列的类型。 //An array of objects to add to the DataTable.  The type of each object should be mapped to the type of its corresponding column in the DataTable. //DataTableに追加するオブジェクトの配列。 各オブジェクトのタイプは、DataTable内の対応する列のタイプにマップする必要があります。
        public InArgument<object[]> ArrayRow { get; set; }

        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [RequiredArgument]
        [DisplayName("DataTable")]
        [Localize.LocalizedDescription("Description24")] //要添加行数据的DataTable对象 //The DataTable object to add row data to //行データを追加するDataTableオブジェクト
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
            DataRow dataRow = DataRow.Get(context);
            object[] objs = ArrayRow.Get(context);
            try
            {
                if(dataRow != null)
                {
                    dataTable.Rows.Add(dataRow);
                }
                else
                {
                    object[] newObjs = null;
                    if (objs.Length > dataTable.Columns.Count)
                    {
                        List<object> list = new List<object>();
                        for(int i=0;i< dataTable.Columns.Count;i++)
                        {
                            list.Add(objs[i]);
                        }
                        newObjs = list.ToArray();
                        dataTable.Rows.Add(newObjs);
                    }
                    else
                    {
                        dataTable.Rows.Add(objs);
                    }
                }
            }
            catch(Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "增加数据库行失败", e.Message);
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
