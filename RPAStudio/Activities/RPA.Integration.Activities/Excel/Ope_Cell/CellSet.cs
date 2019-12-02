using System.Activities;
using System.ComponentModel;
using System;
using Plugins.Shared.Library;
using Excel = Microsoft.Office.Interop.Excel;


namespace RPA.Integration.Activities.ExcelPlugins
{
    [Designer(typeof(CellSetDesigner))]
    public sealed class CellSet : AsyncCodeActivity
    {
        public CellSet()
        {
        }

        [Browsable(false)]
        public string icoPath { get { return "pack://application:,,,/RPA.Integration.Activities;Component/Resources/Excel/setcell.png"; } }

        InArgument<Int32> _CellRow = 1;
        [Category("单元格选项")]
        [OverloadGroup("CellColAndRow")]
        [RequiredArgument]
        [DisplayName("行")]
        [Browsable(true)]
        public InArgument<Int32> CellRow
        {
            get
            {
                return _CellRow;
            }
            set
            {
                _CellRow = value;
            }
        }

        InArgument<Int32> _CellColumn = 1;
        [Category("单元格选项")]
        [OverloadGroup("CellColAndRow")]
        [RequiredArgument]
        [DisplayName("列")]
        [Browsable(true)]
        public InArgument<Int32> CellColumn
        {
            get
            {
                return _CellColumn;
            }
            set
            {
                _CellColumn = value;
            }
        }

        [Category("单元格选项")]
        [OverloadGroup("CellName")]
        [Description("代表单元格名称的VB表达式，如A1")]
        [RequiredArgument]
        [DisplayName("单元格名称")]
        [Browsable(true)]
        public InArgument<string> CellName
        {
            get; set;
        }

        InArgument<Object> _CellContent;
        [Category("选项")]
        [RequiredArgument]
        [DisplayName("单元格内容")]
        [Browsable(true)]
        [Description("可设置公式 例\"= Sum(A1 / B1)\"")]
        public InArgument<Object> CellContent
        {
            get
            {
                return _CellContent;
            }
            set
            {
                _CellContent = value;
            }
        }


        [Category("选项")]
        [DisplayName("工作表名称")]
        [Browsable(true)]
        [Description("为空代表当前活动工作表")]
        public InArgument<string> SheetName
        {
            get;
            set;
        }


        [Browsable(false)]
        public string ClassName { get { return "CellSet"; } }
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
                Int32 cellColumn = CellColumn.Get(context);
                Int32 cellRow = CellRow.Get(context);
                Object cellContent = CellContent.Get(context);
                string cellName = CellName.Get(context);
                string sheetName = SheetName.Get(context);
                Excel::_Worksheet sheet;

                if (sheetName == null)
                    sheet = excelApp.ActiveSheet;
                else
                    sheet = excelApp.ActiveWorkbook.Sheets[sheetName];
                if (cellName == null)
                {
                    sheet.Cells[cellRow, cellColumn] = cellContent;
                }
                else
                {
                    sheet.Range[cellName].Value2 = cellContent;
                }

                System.Runtime.InteropServices.Marshal.ReleaseComObject(sheet);
                sheet = null;
                GC.Collect();
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "EXCEL设置单元格内容出错", e.Message);
                new CommonVariable().realaseProcessExit(excelApp);
            }
            m_Delegate = new runDelegate(Run);
            return m_Delegate.BeginInvoke(callback, state);
        }

        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
        }
    }
}
