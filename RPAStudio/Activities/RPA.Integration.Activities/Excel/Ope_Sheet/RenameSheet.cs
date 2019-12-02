using System.Activities;
using System.ComponentModel;
using System;
using Excel = Microsoft.Office.Interop.Excel;
using Plugins.Shared.Library;

namespace RPA.Integration.Activities.ExcelPlugins
{
    [Designer(typeof(RenameSheetDesigner))]
    public sealed class RenameSheet : AsyncCodeActivity
    {
        public RenameSheet()
        {
        }


        [Browsable(false)]
        public string icoPath { get { return "pack://application:,,,/RPA.Integration.Activities;Component/Resources/Excel/renamesheet.png"; } }

        InArgument<string> _SheetName;
        [Category("选项")]
        [RequiredArgument]
        [DisplayName("工作表新名称")]
        [Browsable(true)]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public InArgument<string> SheetName
        {
            get
            {
                return _SheetName;
            }
            set
            {
                _SheetName = value;
            }
        }

        [Category("选项")]
        [RequiredArgument]
        [OverloadGroup("SheetName")]
        [DisplayName("工作表名称")]
        [Browsable(true)]
        public InArgument<string> OldSheetName
        { get; set; }

        InArgument<Int32> _SheetIndex;
        [Category("选项")]
        [OverloadGroup("SheetIndex")]
        [RequiredArgument]
        [DisplayName("工作表次序")]
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
                string oldSheetName = OldSheetName.Get(context);
                string sheetName = SheetName.Get(context);
                Excel::Sheets sheets = excelApp.ActiveWorkbook.Sheets;
                if (oldSheetName != "" && oldSheetName != null)
                    sheets.Item[oldSheetName].Name = sheetName;
                else
                    sheets.Item[sheetIndex].Name = sheetName;
                System.Runtime.InteropServices.Marshal.ReleaseComObject(sheets);
                sheets = null;
                GC.Collect();
            }
            catch (Exception e)
            {
                new CommonVariable().realaseProcessExit(excelApp);
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "EXCEL重命名工作表执行过程出错", e.Message);
            }
            m_Delegate = new runDelegate(Run);
            return m_Delegate.BeginInvoke(callback, state);
        }

        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
        }
    }
}
