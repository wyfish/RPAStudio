using System.Activities;
using System.ComponentModel;
using System;
using Plugins.Shared.Library;
using Excel = Microsoft.Office.Interop.Excel;

namespace RPA.Integration.Activities.ExcelPlugins
{
    /// <summary>
    /// 区域数据填充
    /// </summary>
    [Designer(typeof(DataFillDesigner))]
    public sealed class DataFill : AsyncCodeActivity
    {
        public DataFill()
        {
        }

        //插件图标路径
        [Browsable(false)]
        public string icoPath { get { return "pack://application:,,,/RPA.Integration.Activities;Component/Resources/Excel/rangewrite.png"; } }


        [Localize.LocalizedCategory("Category4")]//选项//Option//オプション
        [Localize.LocalizedDisplayName("DisplayName18")]//工作表名称 //Worksheet name //ワークシート名
        [Localize.LocalizedDescription("Description77")] //为空代表当前活动工作表 //Blank represents the currently active worksheet //空白の場合はアクティブシートとなります
        [Browsable(true)]
        public InArgument<string> SheetName
        {
            get;set;
        }


        [Localize.LocalizedCategory("Category32")]//单元格起始 //Cell start //セルの開始
        [Localize.LocalizedDisplayName("DisplayName16")]//行 //Row //行
        [Localize.LocalizedDescription("UsedRangeApplied")] //如果为空白，则使用UsedRange //UsedRange is applied if it is blank //空白の場合はUsedRangeが適用されます
        [Browsable(true)]
        public InArgument<Int32> CellRow_Begin
        {
            get;set;
        }

        [Localize.LocalizedCategory("Category32")]//单元格起始 //Cell start //セルの開始
        [Localize.LocalizedDisplayName("DisplayName17")]//列 //Column //列
        [Localize.LocalizedDescription("UsedRangeApplied")] //如果为空白，则使用UsedRange //UsedRange is applied if it is blank //空白の場合はUsedRangeが適用されます
        [Browsable(true)]
        public InArgument<Int32> CellColumn_Begin
        {
            get; set;
        }


        [Localize.LocalizedCategory("Category32")]//单元格起始 //Cell start //セルの開始
        [Localize.LocalizedDescription("Description76")]//代表单元格名称的VB表达式，如A1 //VB expression for cell name, such as A1 //A1などのセル名を表すVB式
        [Localize.LocalizedDisplayName("DisplayName141")]//单元格名称 //Cell Name //セル名
        [Browsable(true)]
        public InArgument<string> CellName_Begin
        {
            get; set;
        }


        [Localize.LocalizedCategory("Category32")]//单元格起始 //Cell start //セルの開始
        [Localize.LocalizedDisplayName("DisplayName16")]//行 //Row //行
        [Localize.LocalizedDescription("UsedRangeApplied")] //如果为空白，则使用UsedRange //UsedRange is applied if it is blank //空白の場合はUsedRangeが適用されます
        [Browsable(true)]
        public InArgument<Int32> CellRow_End
        {
            get; set;
        }

        [Localize.LocalizedCategory("Category32")]//单元格起始 //Cell start //セルの開始
        [Localize.LocalizedDisplayName("DisplayName17")]//列 //Column //列
        [Localize.LocalizedDescription("UsedRangeApplied")] //如果为空白，则使用UsedRange //UsedRange is applied if it is blank //空白の場合はUsedRangeが適用されます
        [Browsable(true)]
        public InArgument<Int32> CellColumn_End
        {
            get; set;
        }

        [Localize.LocalizedCategory("Category32")]//单元格起始 //Cell start //セルの開始
        [Localize.LocalizedDescription("Description78")]//代表单元格名称的VB表达式，如B2 //VB expression for cell name, such as B2 //B2などのセル名を表すVB式
        [Localize.LocalizedDisplayName("DisplayName141")]//单元格名称 //Cell Name //セル名
        [Browsable(true)]
        public InArgument<string> CellName_End
        {
            get; set;
        }


        [Localize.LocalizedCategory("Category5")] //输入 //Enter //入力
        [Localize.LocalizedDisplayName("DisplayName177")]//单元格数据 //Cell data //セルのデータ
        [RequiredArgument]
        [Browsable(true)]
        public InArgument<object> FillData
        {
            get; set;
        }

        //异步执行流程中标志类名
        [Browsable(false)]
        public string ClassName { get { return "DataFill"; } }
        //异步流程返回值
        private delegate string runDelegate();
        private runDelegate m_Delegate;
        public string Run()
        {
            return ClassName;
        }

        //异步执行开始函数
        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            //根据EXCEL主控件标志获取其抽象化属性
            PropertyDescriptor property = context.DataContext.GetProperties()[ExcelCreate.GetExcelAppTag];
            //抽象化属性转换为可用ExcelApplication属性
            Excel::Application excelApp = property.GetValue(context.DataContext) as Excel::Application;
            try
            {
                //string sheetName = SheetName.Get(context);
                //string cellName_Begin = CellName_Begin.Get(context);
                //string cellName_End = CellName_End.Get(context);
                //int cellRow_Begin = CellRow_Begin.Get(context);
                //int cellColumn_Begin = CellColumn_Begin.Get(context);
                //int cellRow_End = CellRow_End.Get(context);
                //int cellColumn_End = CellColumn_End.Get(context);
                //Excel::_Worksheet sheet;
                //if (sheetName != null)
                //    sheet = excelApp.ActiveWorkbook.Sheets[sheetName];
                //else
                //    sheet = excelApp.ActiveSheet;

                //Excel::Range range1, range2;
                //range1 = cellName_Begin == null ? sheet.Cells[cellRow_Begin, cellColumn_Begin] : sheet.Range[cellName_Begin];
                //range2 = cellName_End == null ? sheet.Cells[cellRow_End, cellColumn_End] : sheet.Range[cellName_End];
                Excel::_Worksheet sheet;
                Excel::Range range1, range2;
                RangeFunction.GetRange(excelApp, context, SheetName, CellName_Begin, CellName_End,
                                       CellRow_Begin, CellColumn_Begin, CellRow_End, CellColumn_End,
                                       out sheet, out range1, out range2);
                Excel::Range range3 = sheet.Range[range1, range2];
                range3.Value2 = FillData.Get(context);

                //释放资源
                System.Runtime.InteropServices.Marshal.ReleaseComObject(sheet);
                sheet = null;
                //资源回收
                GC.Collect();
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "EXCEL区域填充过程出错", e.Message);
                new CommonVariable().realaseProcessExit(excelApp);
            }

            m_Delegate = new runDelegate(Run);
            return m_Delegate.BeginInvoke(callback, state);
        }

        //异步执行结束函数
        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
        }
    }
}
