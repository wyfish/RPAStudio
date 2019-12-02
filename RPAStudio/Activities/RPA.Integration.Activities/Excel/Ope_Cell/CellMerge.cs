using Plugins.Shared.Library;
using System;
using System.Activities;
using System.ComponentModel;
using Excel = Microsoft.Office.Interop.Excel;


namespace RPA.Integration.Activities.ExcelPlugins
{
    [Designer(typeof(CellMergeDesigner))]
    public sealed class CellMerge : AsyncCodeActivity
    {
        InArgument<Int32> _CellRow_Begin = 1;
        [Category("单元格起始")]
        [DisplayName("行")]
        [Browsable(true)]
        public InArgument<Int32> CellRow_Begin
        {
            get
            {
                return _CellRow_Begin;
            }
            set
            {
                _CellRow_Begin = value;
            }
        }

        InArgument<Int32> _CellColumn_Begin = 1;
        [Category("单元格起始")]
        [DisplayName("列")]
        [Browsable(true)]
        public InArgument<Int32> CellColumn_Begin
        {
            get
            {
                return _CellColumn_Begin;
            }
            set
            {
                _CellColumn_Begin = value;
            }
        }

        [Category("单元格起始")]
        [Description("代表单元格名称的VB表达式，如A1")]
        [DisplayName("单元格名称")]
        [Browsable(true)]
        public InArgument<string> CellName_Begin
        {
            get; set;
        }

        InArgument<Int32> _CellRow_End = 1;
        [Category("单元格结束")]
        [DisplayName("行")]
        [Browsable(true)]
        public InArgument<Int32> CellRow_End
        {
            get
            {
                return _CellRow_End;
            }
            set
            {
                _CellRow_End = value;
            }
        }

        public enum CellMergeOrUnMerge
        {
            合并单元格,
            拆分单元格
        }

        [Category("选项")]
        [DisplayName("合并/拆分")]
        [Browsable(true)]
        public CellMergeOrUnMerge cellMergeOrUnMerge
        {
            get;set;
        }

        InArgument<Int32> _CellColumn_End = 1;
        [Category("单元格结束")]
        [DisplayName("列")]
        [Browsable(true)]
        public InArgument<Int32> CellColumn_End
        {
            get
            {
                return _CellColumn_End;
            }
            set
            {
                _CellColumn_End = value;
            }
        }

        [Category("单元格结束")]
        [Description("代表单元格名称的VB表达式，如B2")]
        [DisplayName("单元格名称")]
        [Browsable(true)]
        public InArgument<string> CellName_End
        {
            get; set;
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
        public string icoPath { get { return "pack://application:,,,/RPA.Integration.Activities;Component/Resources/Excel/cellmerge.png"; } }


        [Browsable(false)]
        public string ClassName { get { return "CellMerge"; } }
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
                string cellName_Begin = CellName_Begin.Get(context);
                string cellName_End = CellName_End.Get(context);
                int cellRow_Begin = CellRow_Begin.Get(context);
                int cellColumn_Begin = CellColumn_Begin.Get(context);
                int cellRow_End = CellRow_End.Get(context);
                int cellColumn_End = CellColumn_End.Get(context);
                
                string sheetName = SheetName.Get(context);
                Excel::_Worksheet sheet;
                if (sheetName == null)
                    sheet = excelApp.ActiveSheet;
                else
                    sheet = excelApp.ActiveWorkbook.Sheets[sheetName];

                Excel::Range range1, range2;
                range1 = cellName_Begin == null ? sheet.Cells[cellRow_Begin, cellColumn_Begin] : sheet.Range[cellName_Begin];
                range2 = cellName_End == null ? sheet.Cells[cellRow_End, cellColumn_End] : sheet.Range[cellName_End];
                if(cellMergeOrUnMerge == CellMergeOrUnMerge.合并单元格)
                    sheet.Range[range1, range2].Merge();
                else
                    sheet.Range[range1, range2].UnMerge();

                System.Runtime.InteropServices.Marshal.ReleaseComObject(sheet);
                sheet = null;
                GC.Collect();
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "EXCEL合并单元格过程出错", e.Message);
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
