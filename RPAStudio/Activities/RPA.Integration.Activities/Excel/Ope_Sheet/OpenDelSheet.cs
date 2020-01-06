using System.Activities;
using System.ComponentModel;
using System;
using Plugins.Shared.Library;
using Excel = Microsoft.Office.Interop.Excel;

namespace RPA.Integration.Activities.ExcelPlugins
{
    [Designer(typeof(OpenDelSheetDesigner))]
    public sealed class OpenDelSheet : AsyncCodeActivity
    {
        public OpenDelSheet()
        {
        }


        [Browsable(false)]
        public string icoPath { get { return "pack://application:,,,/RPA.Integration.Activities;Component/Resources/Word/create.png"; } }


        [Localize.LocalizedCategory("Category4")] //选项 //Option //オプション
        [RequiredArgument]
        [OverloadGroup("SheetName")]
        [Localize.LocalizedDisplayName("DisplayName18")] //工作表名称 //Worksheet name //ワークシート名
        [Browsable(true)]
        public InArgument<string> SheetName
        { get; set; }


        InArgument<Int32> _SheetIndex;
        [Localize.LocalizedCategory("Category4")] //选项 //Option //オプション
        [OverloadGroup("SheetIndex")]
        [RequiredArgument]
        [Localize.LocalizedDisplayName("DisplayName40")] //工作表次序 //Worksheet order //ワークシートの順序
        [Browsable(true)]
        public InArgument<Int32> SheetIndex
        {
            get
            {
                return _SheetIndex;
            }
            set
            {
                _SheetIndex = value;
            }
        }

        public enum FuncOptions
        {
            打开 = 1,
            删除 = 2
        }

        FuncOptions _FuncOption = (FuncOptions)1;
        [Localize.LocalizedCategory("Category4")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName41")] //功能选项 //Function option //機能オプション
        [Browsable(true)]
        public FuncOptions FuncOption
        {
            get
            {
                return _FuncOption;
            }
            set
            {
                _FuncOption = value;
            }
        }

        [Browsable(false)]
        public string ClassName { get { return "OpenDelSheet"; } }
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
                Int32 sheetIndex = SheetIndex.Get(context);
                string sheetName = SheetName.Get(context);

                Excel::Sheets sheets = excelApp.ActiveWorkbook.Sheets;
                Excel::_Worksheet sheet;
                if (sheetName != null && sheetName != "")
                    sheet = sheets.Item[sheetName];
                else
                    sheet = sheets.Item[sheetIndex];

                if (_FuncOption == (FuncOptions)1)
                    sheet.Activate();
                if (_FuncOption == (FuncOptions)2)
                    sheet.Delete();

                System.Runtime.InteropServices.Marshal.ReleaseComObject(sheets);
                sheets = null;
                System.Runtime.InteropServices.Marshal.ReleaseComObject(sheet);
                sheet = null;
                GC.Collect();
            }
            catch (Exception e)
            {
                new CommonVariable().realaseProcessExit(excelApp);
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "EXCEL工作表打开删除执行过程出错", e.Message);
            }
            m_Delegate = new runDelegate(Run);
            return m_Delegate.BeginInvoke(callback, state);
        }

        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
        }
    }
}
