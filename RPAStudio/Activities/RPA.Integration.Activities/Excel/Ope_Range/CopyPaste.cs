using System.Activities;
using System.ComponentModel;
using System;
using Excel = Microsoft.Office.Interop.Excel;
using Plugins.Shared.Library;
using System.IO;

namespace RPA.Integration.Activities.ExcelPlugins
{

    [Designer(typeof(CopyPasteDesigner))]
    public sealed class CopyPaste : AsyncCodeActivity
    {
        public CopyPaste()
        {
        }

        [Browsable(false)]
        public string icoPath { get { return "pack://application:,,,/RPA.Integration.Activities;Component/Resources/Excel/copy.png"; } }

        [Localize.LocalizedCategory("Category8")] //粘贴选项 //Paste option //貼り付けオプション
        [Localize.LocalizedDisplayName("DisplayName143")] //目标文件路径 //Destination File Path //宛先ファイルのパス
        [Browsable(true)]
        public InArgument<string> DestFilePath { get; set;}


        [Localize.LocalizedCategory("Category8")] //粘贴选项 //Paste option //貼り付けオプション
        [Localize.LocalizedDisplayName("DisplayName144")] //目标单元格行 //Target Cell row //ターゲットセル行
        [Browsable(true)]
        public InArgument<Int32> DestCellRow
        {
            get;set;
        }

        [Localize.LocalizedCategory("Category8")] //粘贴选项 //Paste option //貼り付けオプション
        [Localize.LocalizedDisplayName("DisplayName145")] //目标单元格列 //Target Cell column //ターゲットセル列
        [Browsable(true)]
        public InArgument<Int32> DestCellColumn
        {
            get;set;
        }

        InArgument<string> _DestCell;
        [Localize.LocalizedCategory("Category8")] //粘贴选项 //Paste option //貼り付けオプション
        [Localize.LocalizedDisplayName("DisplayName146")] //目标单元格名称 //Target cell name //ターゲットセル名
        [Browsable(true)]
        [Localize.LocalizedDescription("Description17")] //粘贴单元格区域的起始单元格 //Paste the starting cell of the cell range //セル範囲の開始セルを貼り付けます
        public InArgument<string> DestCell
        {
            get
            {
                return _DestCell;
            }
            set
            {
                _DestCell = value;
            }
        }

        InArgument<string> _DestSheet;
        [Localize.LocalizedCategory("Category8")] //粘贴选项 //Paste option //貼り付けオプション
        [Localize.LocalizedDisplayName("DisplayName147")] //目标工作表 //Target sheet //対象シート
        [Browsable(true)]
        [Localize.LocalizedDescription("Description18")] //粘贴单元格区域的工作薄名称 //Paste the work area name of the cell area //セル領域の作業領域名を貼り付けます
        public InArgument<string> DestSheet
        {
            get
            {
                return _DestSheet;
            }
            set
            {
                _DestSheet = value;
            }
        }

        [Localize.LocalizedCategory("Category9")] //复制选项 //Copy option //コピーオプション
        [Localize.LocalizedDisplayName("DisplayName18")] //工作表名称 //Worksheet name //ワークシート名
        [Browsable(true)]
        public InArgument<string> CopySheet
        {
            get;set;
        }

        [Localize.LocalizedCategory("Category9")] //复制选项 //Copy option //コピーオプション
        [Localize.LocalizedDisplayName("DisplayName148")] //起始单元格行 //Starting cell row //開始セル行
        [Browsable(true)]
        public InArgument<Int32> CellRow_Begin
        {
            get;set;
        }

        [Localize.LocalizedCategory("Category9")] //复制选项 //Copy option //コピーオプション
        [Localize.LocalizedDisplayName("DisplayName149")] //起始单元格列 //Starting cell column //開始セル列
        [Browsable(true)]
        public InArgument<Int32> CellColumn_Begin
        {
            get;set;
        }

        [Localize.LocalizedCategory("Category9")] //复制选项 //Copy option //コピーオプション
        [Localize.LocalizedDisplayName("DisplayName150")] //起始单元格名称 //Starting cell name //開始セル名
        [Localize.LocalizedDescription("Description76")] //代表单元格名称的VB表达式，如A1 //VB expression representing the cell name, such as A1 //A1などのセル名を表すVB式
        [Browsable(true)]
        public InArgument<string> CellName_Begin
        {
            get; set;
        }

        [Localize.LocalizedCategory("Category9")] //复制选项 //Copy option //コピーオプション
        [Localize.LocalizedDisplayName("DisplayName151")] //终点单元格行 //End cell row //終了セル行
        [Browsable(true)]
        public InArgument<Int32> CellRow_End
        {
            get;set;
        }

        [Localize.LocalizedCategory("Category9")] //复制选项 //Copy option //コピーオプション
        [Localize.LocalizedDisplayName("DisplayName152")] //终点单元格列 //End cell column //終了セル列
        [Browsable(true)]
        public InArgument<Int32> CellColumn_End
        {
            get;set;
        }

        [Localize.LocalizedCategory("Category9")] //复制选项 //Copy option //コピーオプション
        [Localize.LocalizedDescription("Description78")] //代表单元格名称的VB表达式，如B2 //VB expression representing the cell name, such as B2 //B2などのセル名を表すVB式
        [Localize.LocalizedDisplayName("DisplayName153")] //终点单元格名称 //End cell name //終了セル名
        [Browsable(true)]
        public InArgument<string> CellName_End
        {
            get; set;
        }


        [Browsable(false)]
        public string ClassName { get { return "CopyPaste"; } }
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
                int destCellRow = DestCellRow.Get(context);
                int destCellColumn = DestCellColumn.Get(context);
                string copySheet = CopySheet.Get(context);
                string destSheet = DestSheet.Get(context);
                string destCell = DestCell.Get(context);
                string destDestFilePath = DestFilePath.Get(context);
                Excel::Range range1, range2;

                Excel::_Worksheet sheet;
                if (copySheet != null)
                    sheet = excelApp.ActiveWorkbook.Sheets[copySheet];
                else
                    sheet = excelApp.ActiveSheet;
                range1 = cellName_Begin == null ? sheet.Cells[cellRow_Begin, cellColumn_Begin] : sheet.Range[cellName_Begin];
                range2 = cellName_End == null ? sheet.Cells[cellRow_End, cellColumn_End] : sheet.Range[cellName_End];
                sheet.Range[range1, range2].Copy(Type.Missing);

                Excel::_Worksheet pasteSheet;
                if(destDestFilePath != null && destDestFilePath!="")
                {
                    if (!File.Exists(destDestFilePath))
                    {
                        SharedObject.Instance.Output(SharedObject.enOutputType.Error, "文件不存在，请检查路径有效性");
                        new CommonVariable().realaseProcessExit(excelApp);
                    }
                    else
                    {
                        Excel::Workbook workbook2 = excelApp.Workbooks._Open(destDestFilePath);
                        if (destSheet != null)
                            pasteSheet = workbook2.Sheets[destSheet];
                        else
                            pasteSheet = workbook2.ActiveSheet;

                        Excel::Range pasteRange = destCell == null ? sheet.Cells[destCellRow, destCellColumn] : sheet.Range[destCell];
                        pasteSheet.Paste(pasteRange);
                        workbook2.Save();
                        workbook2.Close();
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(pasteSheet);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(pasteRange);
                        pasteRange = null;
                    }
                }
                else
                {
                    if (destSheet != null)
                        pasteSheet = excelApp.ActiveWorkbook.Sheets[destSheet];
                    else
                        pasteSheet = excelApp.ActiveSheet;

                    Excel::Range pasteRange = destCell == null ? sheet.Cells[destCellRow, destCellColumn] : sheet.Range[destCell];
                    pasteSheet.Paste(pasteRange);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(pasteSheet);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(pasteRange);
                    pasteRange = null;
                }

                System.Runtime.InteropServices.Marshal.ReleaseComObject(sheet);
                sheet = null; pasteSheet = null; 
                GC.Collect();
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "EXCEL复制粘贴过程出错", e.Message);
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
