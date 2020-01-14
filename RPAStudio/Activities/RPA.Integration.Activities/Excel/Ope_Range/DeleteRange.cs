using System.Activities;
using System.ComponentModel;
using System;
using Excel=Microsoft.Office.Interop.Excel;
using Plugins.Shared.Library;

namespace RPA.Integration.Activities.ExcelPlugins
{
    [Designer(typeof(DeleteRangeDesigner))]
    public sealed class DeleteRange : AsyncCodeActivity
    {
        public DeleteRange()
        {
        }

        [Browsable(false)]
        public string icoPath { get { return "pack://application:,,,/RPA.Integration.Activities;Component/Resources/Excel/rangedelete.png"; } }

        [Localize.LocalizedCategory("Category4")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName18")] //工作表名称 //Worksheet name //ワークシート名
        [Browsable(true)]
        public InArgument<string> SheetName
        {
            get;set;
        }

        [Localize.LocalizedCategory("Category32")] //单元格起始 //Start Cell //開始セル
        [Localize.LocalizedDisplayName("DisplayName16")] //行 //Row //行
        [Browsable(true)]
        public InArgument<Int32> CellRow_Begin
        {
            get;set;
        }

        [Localize.LocalizedCategory("Category32")] //单元格起始 //Start Cell //開始セル
        [Localize.LocalizedDisplayName("DisplayName17")] //列 //Column //コラム
        [Browsable(true)]
        public InArgument<Int32> CellColumn_Begin
        {
            get;set;
        }

        [Localize.LocalizedCategory("Category32")] //单元格起始 //Start Cell //開始セル
        [Localize.LocalizedDescription("Description76")] //代表单元格名称的VB表达式，如A1 //VB expression representing the cell name, such as A1 //A1などのセル名を表すVB式
        [Localize.LocalizedDisplayName("DisplayName141")] //单元格名称 //Cell Name //セル名
        [Browsable(true)]
        public InArgument<string> CellName_Begin
        {
            get; set;
        }

        [Localize.LocalizedCategory("Category34")] //单元格终点 //Cell end //セルエンド
        [Localize.LocalizedDisplayName("DisplayName16")] //行 //Row //行
        [Browsable(true)]
        public InArgument<Int32> CellRow_End
        {
            get; set;
        }

        [Localize.LocalizedCategory("Category34")] //单元格终点 //Cell end //セルエンド
        [Localize.LocalizedDisplayName("DisplayName17")] //列 //Column //コラム
        [Browsable(true)]
        public InArgument<Int32> CellColumn_End
        {
            get; set;
        }

        [Localize.LocalizedCategory("Category34")] //单元格终点 //Cell end //セルエンド
        [Localize.LocalizedDescription("Description78")] //代表单元格名称的VB表达式，如B2 //VB expression representing the cell name, such as B2 //B2などのセル名を表すVB式
        [Localize.LocalizedDisplayName("DisplayName141")] //单元格名称 //Cell Name //セル名
        [Browsable(true)]
        public InArgument<string> CellName_End
        {
            get; set;
        }

        [Localize.LocalizedCategory("Category4")] //选项 //Option //オプション
        [DisplayName("ShiftCells")]
        [Browsable(true)]
        public bool ShiftCells
        {
            get;
            set;
        }

        public enum ShiftTypes
        {
            ShiftUp,
            ShiftLeft,
            EntireRow,
            EntireColumn
        }

        [Localize.LocalizedCategory("Category4")] //选项 //Option //オプション
        [DisplayName("ShiftCells")]
        [Browsable(true)]
        public ShiftTypes ShiftType
        {
            get;
            set;
        }

        [Browsable(false)]
        public string ClassName { get { return "DeleteRange"; } }
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
                string sheetName = SheetName.Get(context);
                string cellName_Begin = CellName_Begin.Get(context);
                string cellName_End = CellName_End.Get(context);
                int cellRow_Begin = CellRow_Begin.Get(context);
                int cellColumn_Begin = CellColumn_Begin.Get(context);
                int cellRow_End = CellRow_End.Get(context);
                int cellColumn_End = CellColumn_End.Get(context);

                Excel::_Worksheet sheet;
                if (sheetName != null)
                    sheet = excelApp.ActiveWorkbook.Sheets[sheetName];
                else
                    sheet = excelApp.ActiveSheet;

                Excel::Range range1, range2;
                range1 = cellName_Begin == null ? sheet.Cells[cellRow_Begin, cellColumn_Begin] : sheet.Range[cellName_Begin];
                range2 = cellName_End == null ? sheet.Cells[cellRow_End, cellColumn_End] : sheet.Range[cellName_End];
                Excel::Range range = sheet.Range[range1, range2];

                if (!ShiftCells)
                    range.Clear();
                else
                {
                    if (ShiftType == ShiftTypes.ShiftUp)
                        range.Delete(Excel.XlDeleteShiftDirection.xlShiftUp);
                    else if (ShiftType == ShiftTypes.ShiftLeft)
                        range.Delete(Excel.XlDeleteShiftDirection.xlShiftToLeft);
                }

                System.Runtime.InteropServices.Marshal.ReleaseComObject(sheet);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(range);
                sheet = null;
                range = null;
                GC.Collect();
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "EXCEL删除区域执行过程出错", e.Message);
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
