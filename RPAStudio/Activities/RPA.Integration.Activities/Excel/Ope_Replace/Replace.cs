using System.Activities;
using System.ComponentModel;
using System;
using Excel = Microsoft.Office.Interop.Excel;
using Plugins.Shared.Library;
using System.IO;

namespace RPA.Integration.Activities.ExcelPlugins
{

    [Designer(typeof(ReplaceDesigner))]
    public sealed class Replace : AsyncCodeActivity
    {
        public Replace()
        {
        }

        [Browsable(false)]
        public string icoPath { get { return "pack://application:,,,/RPA.Integration.Activities;Component/Resources/Excel/replace.png"; } }

        [Localize.LocalizedCategory("Category31")] //工作表选项 //Sheet Options //シートオプション
        [Localize.LocalizedDisplayName("DisplayName155")] //所有工作表 //All worksheets //すべてのワークシート
        [Browsable(true)]
        public bool AllSheets
        {
            get; set;
        }


        [Localize.LocalizedCategory("Category31")] //工作表选项 //Sheet Options //シートオプション
        [Localize.LocalizedDisplayName("DisplayName18")] //工作表名称 //Worksheet name //ワークシート名
        [Browsable(true)]
        public InArgument<string> SheetName
        {
            get;set;
        }


        [Localize.LocalizedCategory("Category35")] //区域选项 //Regional options //地域オプション
        [Localize.LocalizedDisplayName("DisplayName156")] //所有区域 //All areas //すべてのエリア
        [Browsable(true)]
        public bool AllRange
        {
            get; set;
        }

        [Localize.LocalizedCategory("Category35")] //区域选项 //Regional options //地域オプション
        [Localize.LocalizedDisplayName("DisplayName157")] //单元格起始行 //Cell start row //セル開始行
        [Browsable(true)]
        public InArgument<Int32> CellRow_Begin
        {
            get;set;
        }

        [Localize.LocalizedCategory("Category35")] //区域选项 //Regional options //地域オプション
        [Localize.LocalizedDisplayName("DisplayName158")] //单元格起始列 //Cell start column //セル開始列
        [Browsable(true)]
        public InArgument<Int32> CellColumn_Begin
        {
            get; set;
        }


        [Localize.LocalizedCategory("Category35")] //区域选项 //Regional options //地域オプション
        [Localize.LocalizedDisplayName("DisplayName159")] //单元格起始名称 //Cell start name //セル開始名
        [Localize.LocalizedDescription("Description76")] //代表单元格名称的VB表达式，如A1 //VB expression representing the cell name, such as A1 //A1などのセル名を表すVB式
        [Browsable(true)]
        public InArgument<string> CellName_Begin
        {
            get; set;
        }

        [Localize.LocalizedCategory("Category35")] //区域选项 //Regional options //地域オプション
        [Localize.LocalizedDisplayName("DisplayName160")] //单元格终点行 //Cell end row //セル終了行
        [Browsable(true)]
        public InArgument<Int32> CellRow_End
        {
            get; set;
        }

        [Localize.LocalizedCategory("Category35")] //区域选项 //Regional options //地域オプション
        [Localize.LocalizedDisplayName("DisplayName161")] //单元格终点列 //Cell end column //セル終了列
        [Browsable(true)]
        public InArgument<Int32> CellColumn_End
        {
            get; set;
        }

        [Localize.LocalizedCategory("Category35")] //区域选项 //Regional options //地域オプション
        [Localize.LocalizedDescription("Description78")] //代表单元格名称的VB表达式，如B2 //VB expression representing the cell name, such as B2 //B2などのセル名を表すVB式
        [Localize.LocalizedDisplayName("DisplayName162")] //单元格终点名称 //Cell end name //セル終了名
        [Browsable(true)]
        public InArgument<string> CellName_End
        {
            get; set;
        }


        [Localize.LocalizedCategory("Category36")] //替换选项 //Replace option //交換オプション
        [Localize.LocalizedDisplayName("DisplayName163")] //查找内容 //Find content //コンテンツを見つける
        [RequiredArgument]
        [Browsable(true)]
        public InArgument<string> FindData
        {
            get; set;
        }

        [Localize.LocalizedCategory("Category36")] //替换选项 //Replace option //交換オプション
        [Localize.LocalizedDisplayName("DisplayName164")] //替换内容 //Replace content //コンテンツを置き換える
        [RequiredArgument]
        [Browsable(true)]
        public InArgument<string> ReplaceData
        {
            get; set;
        }

        [Localize.LocalizedCategory("Category36")] //替换选项 //Replace option //交換オプション
        [Localize.LocalizedDisplayName("DisplayName165")] //区分大小写 //Case sensitive //大文字と小文字を区別
        [Browsable(true)]
        public bool IsTextTransform { get; set; }

        [Localize.LocalizedCategory("Category36")] //替换选项 //Replace option //交換オプション
        [Localize.LocalizedDisplayName("DisplayName166")] //单元格匹配 //Cell match //セル一致
        [Browsable(true)]
        public bool IsCellMatch { get; set; }

        [Localize.LocalizedCategory("Category36")] //替换选项 //Replace option //交換オプション
        [Localize.LocalizedDisplayName("DisplayName167")] //区分半全角 //Distinguish half-full angle //ハーフフルアングルを区別する
        [Browsable(true)]
        public bool isSemiFull { get; set; }


        [Browsable(false)]
        public string ClassName { get { return "Replace"; } }
        private delegate string runDelegate();
        private runDelegate m_Delegate;
        public string Run()
        {
            return ClassName;
        }

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);
        }

        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            PropertyDescriptor property = context.DataContext.GetProperties()[ExcelCreate.GetExcelAppTag];
            Excel::Application excelApp = property.GetValue(context.DataContext) as Excel::Application;
            string cellName_Begin = CellName_Begin.Get(context);
            string cellName_End = CellName_End.Get(context);
            int cellRow_Begin = CellRow_Begin.Get(context);
            int cellColumn_Begin = CellColumn_Begin.Get(context);
            int cellRow_End = CellRow_End.Get(context);
            int cellColumn_End = CellColumn_End.Get(context);
            string findData = FindData.Get(context);
            string replaceData = ReplaceData.Get(context);
            Excel::Range range1, range2;
            Excel::_Worksheet sheet = null;
            Excel::_Worksheet currSheet = null;
            Excel.XlLookAt lookAt = IsCellMatch ? Excel.XlLookAt.xlWhole : Excel.XlLookAt.xlPart;
            if (SheetName.Get(context) != null)
                sheet = excelApp.ActiveWorkbook.Sheets[SheetName];
            else
                sheet = excelApp.ActiveSheet;

            try
            {
                if (AllSheets)  //全部工作表
                {
                    int sheetCount = excelApp.ActiveWorkbook.Worksheets.Count;
                    for(int i=1; i< sheetCount+1; i++)
                    {
                        if(AllRange)
                        {
                            excelApp.ActiveWorkbook.Worksheets[i].Range["A1", "IV65535"].Replace(findData, replaceData, lookAt, Type.Missing, IsTextTransform, isSemiFull);
                        }
                        else
                        {
                            currSheet = excelApp.ActiveWorkbook.Worksheets[i];
                            range1 = cellName_Begin == null ? currSheet.Cells[cellRow_Begin, cellColumn_Begin] : currSheet.Range[cellName_Begin];
                            range2 = cellName_End == null ? currSheet.Cells[cellRow_End, cellColumn_End] : currSheet.Range[cellName_End];
                            currSheet.Range[range1, range2].Replace(findData, replaceData, lookAt, Type.Missing, IsTextTransform, isSemiFull);
                        }
                    }
                }
                else
                {
                    if (AllRange)    //全部区域
                    {
                        sheet.Range["A1", "IV65535"].Replace(findData, replaceData, lookAt, Type.Missing, IsTextTransform, isSemiFull);
                    }
                    else
                    {
                        range1 = cellName_Begin == null ? sheet.Cells[cellRow_Begin, cellColumn_Begin] : sheet.Range[cellName_Begin];
                        range2 = cellName_End == null ? sheet.Cells[cellRow_End, cellColumn_End] : sheet.Range[cellName_End];
                        sheet.Range[range1, range2].Replace(findData, replaceData, lookAt, Type.Missing, IsTextTransform, isSemiFull);
                    }
                }

                /* 资源回收释放 */
                System.Runtime.InteropServices.Marshal.ReleaseComObject(sheet);
                sheet = null;
                if (currSheet != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(currSheet);
                    currSheet = null;
                }
                GC.Collect();
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "EXCEL查找替换过程出错", e.Message);
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
