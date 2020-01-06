using System.Activities;
using System.ComponentModel;
using System;
using System.Windows.Data;
using System.Globalization;
using Excel = Microsoft.Office.Interop.Excel;
using Plugins.Shared.Library;
using System.Collections.ObjectModel;

namespace RPA.Integration.Activities.ExcelPlugins
{
    [Designer(typeof(InsertRowColDesigner))]
    public sealed class InsertRowCol : AsyncCodeActivity
    {
        public InsertRowCol()
        {
        }


        [Browsable(false)]
        public string icoPath { get { return "pack://application:,,,/RPA.Integration.Activities;Component/Resources/Excel/rowcol.png"; } }



        [Localize.LocalizedCategory("Category4")] //选项 //Option //オプション
        [RequiredArgument]
        [DisplayName("行号/列号")]
        [Browsable(true)]
        public InArgument<Int32> RowColNum
        {
            get;set;
        }

        public enum InsertType
        {
            复制行,
            复制列
        }


        [Localize.LocalizedCategory("Category4")] //选项 //Option //オプション
        [RequiredArgument]
        [DisplayName("行/列")]
        [Browsable(true)]
        public InsertType CurrInsertType
        {
            get; set;
        }

        [Localize.LocalizedCategory("Category4")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName18")] //工作表名称 //Worksheet name //ワークシート名
        [Description("为空代表当前活动工作表")]
        [Browsable(true)]
        public InArgument<string> SheetName
        {
            get; set;
        }

        [Localize.LocalizedCategory("Category5")] //输入 //Enter //入力
        [RequiredArgument]
        [DisplayName("行/列数据")]
        [Browsable(true)]
        public InArgument<object> RolColData
        {
            get;set;
        }


        [Browsable(false)]
        public string ClassName { get { return "InsertRowCol"; } }
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
                m_Delegate = new runDelegate(Run);
                string sheetName = SheetName.Get(context);
                Excel::_Worksheet sheet;
                if (sheetName == null)
                    sheet = excelApp.ActiveSheet;
                else
                    sheet = excelApp.ActiveWorkbook.Sheets[sheetName];

                Int32 rowColNum = RowColNum.Get(context);
                if(CurrInsertType == InsertType.复制行)
                {
                    sheet.Rows[rowColNum].Value = RolColData.Get(context);
                }
                else
                {
                    sheet.Columns[rowColNum].Value = RolColData.Get(context);
                }

                System.Runtime.InteropServices.Marshal.ReleaseComObject(sheet);
                sheet = null;
                GC.Collect();
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "EXCEL行列复制执行过程出错", e.Message);
                new CommonVariable().realaseProcessExit(excelApp);
            }
            return m_Delegate.BeginInvoke(callback, state);
        }

        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
        }
    }
}
