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
        [Localize.LocalizedCategory("Category32")] //单元格起始 //Start Cell //開始セル
        [Localize.LocalizedDisplayName("DisplayName16")] //行 //Row //行
        [Localize.LocalizedDescription("UsedRangeApplied")] //如果为空白，则使用UsedRange //UsedRange is applied if it is blank //空白の場合はUsedRangeが適用されます
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
        [Localize.LocalizedCategory("Category32")] //单元格起始 //Start Cell //開始セル
        [Localize.LocalizedDisplayName("DisplayName17")] //列 //Column //コラム
        [Localize.LocalizedDescription("UsedRangeApplied")] //如果为空白，则使用UsedRange //UsedRange is applied if it is blank //空白の場合はUsedRangeが適用されます
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

        [Localize.LocalizedCategory("Category32")] //单元格起始 //Start Cell //開始セル
        [Localize.LocalizedDescription("Description76")] //代表单元格名称的VB表达式，如A1 //VB expression representing the cell name, such as A1 //A1などのセル名を表すVB式
        [Localize.LocalizedDisplayName("DisplayName141")] //单元格名称 //Cell Name //セル名
        [Browsable(true)]
        public InArgument<string> CellName_Begin
        {
            get; set;
        }

        InArgument<Int32> _CellRow_End = 1;
        [Localize.LocalizedCategory("Category33")] //单元格结束 //End Cell //終了セル
        [Localize.LocalizedDisplayName("DisplayName16")] //行 //Row //行
        [Localize.LocalizedDescription("UsedRangeApplied")] //如果为空白，则使用UsedRange //UsedRange is applied if it is blank //空白の場合はUsedRangeが適用されます
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

        [Localize.LocalizedCategory("Category4")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName142")] //合并/拆分 //Merge / Split //マージ/分割
        [Browsable(true)]
        public CellMergeOrUnMerge cellMergeOrUnMerge
        {
            get;set;
        }

        InArgument<Int32> _CellColumn_End = 1;
        [Localize.LocalizedCategory("Category33")] //单元格结束 //End Cell //終了セル
        [Localize.LocalizedDisplayName("DisplayName17")] //列 //Column //コラム
        [Localize.LocalizedDescription("UsedRangeApplied")] //如果为空白，则使用UsedRange //UsedRange is applied if it is blank //空白の場合はUsedRangeが適用されます
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

        [Localize.LocalizedCategory("Category33")] //单元格结束 //End Cell //終了セル
        [Localize.LocalizedDescription("Description78")] //代表单元格名称的VB表达式，如B2 //VB expression representing the cell name, such as B2 //B2などのセル名を表すVB式
        [Localize.LocalizedDisplayName("DisplayName141")] //单元格名称 //Cell Name //セル名
        [Browsable(true)]
        public InArgument<string> CellName_End
        {
            get; set;
        }


        [Localize.LocalizedCategory("Category4")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName18")] //工作表名称 //Worksheet name //ワークシート名
        [Browsable(true)]
        [Localize.LocalizedDescription("Description77")] //为空代表当前活动工作表 //Blank represents the currently active worksheet //空白は現在アクティブなワークシートを表します
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
                //string cellName_Begin = CellName_Begin.Get(context);
                //string cellName_End = CellName_End.Get(context);
                //int cellRow_Begin = CellRow_Begin.Get(context);
                //int cellColumn_Begin = CellColumn_Begin.Get(context);
                //int cellRow_End = CellRow_End.Get(context);
                //int cellColumn_End = CellColumn_End.Get(context);

                //string sheetName = SheetName.Get(context);
                //Excel::_Worksheet sheet;
                //if (sheetName == null)
                //    sheet = excelApp.ActiveSheet;
                //else
                //    sheet = excelApp.ActiveWorkbook.Sheets[sheetName];

                //Excel::Range range1, range2;
                //range1 = cellName_Begin == null ? sheet.Cells[cellRow_Begin, cellColumn_Begin] : sheet.Range[cellName_Begin];
                //range2 = cellName_End == null ? sheet.Cells[cellRow_End, cellColumn_End] : sheet.Range[cellName_End];

                Excel::_Worksheet sheet;
                Excel::Range range1, range2;
                RangeFunction.GetRange(excelApp, context, SheetName, CellName_Begin, CellName_End,
                                       CellRow_Begin, CellColumn_Begin, CellRow_End, CellColumn_End,
                                       out sheet, out range1, out range2);
                if (cellMergeOrUnMerge == CellMergeOrUnMerge.合并单元格)
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
