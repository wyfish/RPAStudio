using Plugins.Shared.Library;
using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using Excel = Microsoft.Office.Interop.Excel;

namespace RPA.Integration.Activities.ExcelPlugins
{
    [Designer(typeof(GetAllSheetsDesigner))]
    public sealed class GetAllSheets : AsyncCodeActivity
    {
        public GetAllSheets()
        {
        }

        [Browsable(false)]
        public string icoPath { get { return "pack://application:,,,/RPA.Integration.Activities;Component/Resources/Excel/sheetnames.png"; } }

        [Localize.LocalizedCategory("Category2")] //输出 //Output //出力
        [RequiredArgument]
        [Localize.LocalizedDisplayName("DisplayName176")] //工作表名称列表 //List of worksheet names //ワークシート名のリスト
        [Browsable(true)]
        public OutArgument<string[]> SheetNames
        {
            get;set;
        }

        [Browsable(false)]
        public string ClassName { get { return "GetAllSheets"; } }
        private delegate string runDelegate();
        private runDelegate m_Delegate;
        public string Run()
        {
            return ClassName;
        }


        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            PropertyDescriptor property = context.DataContext.GetProperties()[ExcelCreate.GetExcelAppTag];
            Excel::Application excelApp = property.GetValue(context.DataContext) as Excel::Application;
            try
            {
                List<string> list = new List<string>();
                for(int i=1; i< excelApp.ActiveWorkbook.Worksheets.Count+1; i++)
                {
                    list.Add(excelApp.ActiveWorkbook.Worksheets[i].Name);
                }
                SheetNames.Set(context, list.ToArray());
            }   
            catch (Exception e)
            {
                new CommonVariable().realaseProcessExit(excelApp);
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "EXCEL增加新工作表执行过程出错", e.Message);
            }
            m_Delegate = new runDelegate(Run);
            return m_Delegate.BeginInvoke(callback, state);
        }

        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
        }
    }   
}
