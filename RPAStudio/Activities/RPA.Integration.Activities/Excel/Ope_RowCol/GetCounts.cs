using System.Activities;
using System.ComponentModel;
using System;
using System.Windows.Data;
using System.Globalization;
using Excel = Microsoft.Office.Interop.Excel;
using Plugins.Shared.Library;

namespace RPA.Integration.Activities.ExcelPlugins
{

    [Designer(typeof(GetCountsDesigner))]
    public sealed class GetCounts : AsyncCodeActivity
    {
        public GetCounts()
        {
        }


        [Browsable(false)]
        public string icoPath { get { return "pack://application:,,,/RPA.Integration.Activities;Component/Resources/Excel/rowcol.png"; } }



        public enum RowCols
        {
            行数 = 1,
            列数 = 2
        }

        [Category("行列数选项")]
        [DisplayName("有效数据")]
        [Browsable(true)]
        public bool isValid
        { get; set; }


        [Category("行列数选项")]
        [DisplayName("活动区域")]
        [Browsable(true)]
        public bool isActive
        { get; set; }


        private bool _isSingleLine = true;
        [Category("行列数选项")]
        [DisplayName("A1单元格列/行截止")]
        [Browsable(true)]
        public bool isSingleLine
        {
            get
            {
                return _isSingleLine;
            }
            set
            {
                _isSingleLine = value;
            }
        }



        //InArgument<Int32> _CellRow = 1;
        //[Category("行列数选项")]
        //[DisplayName("单元格列")]
        //[Browsable(true)]
        //public InArgument<Int32> CellRow
        //{
        //    get
        //    {
        //        return _CellRow;
        //    }
        //    set
        //    {
        //        _CellRow = value;
        //    }
        //}

        //InArgument<Int32> _CellColumn = 1;
        //[Category("行列数选项")]
        //[DisplayName("单元格行")]
        //[Browsable(true)]
        //public InArgument<Int32> CellColumn
        //{
        //    get
        //    {
        //        return _CellColumn;
        //    }
        //    set
        //    {
        //        _CellColumn = value;
        //    }
        //}


        //[Category("行列数选项")]
        //[Description("代表单元格名称的VB表达式，如A1")]
        //[DisplayName("单元格名称")]
        //[Browsable(true)]
        //public InArgument<string> CellName
        //{
        //    get; set;
        //}

        [Category("工作表选项")]
        [DisplayName("工作表名称")]
        [Browsable(true)]
        [Description("为空代表当前活动工作表")]
        public InArgument<string> SheetName
        {
            get;
            set;
        }


        [Category("输出")]
        [DisplayName("行数")]
        [Browsable(true)]
        [Description("工作表中有效行数")]
        public OutArgument<int> RowCounts
        {
            get;
            set;
        }


        [Category("输出")]
        [DisplayName("列数")]
        [Browsable(true)]
        [Description("工作表中有效行数")]
        public OutArgument<int> ColCounts
        {
            get;
            set;
        }

        [Browsable(false)]
        public string ClassName { get { return "RowCol"; } }
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

                int rowCounts = 0, colCounts = 0;
                //有效行列数 不包含中间的空行
                if(isValid)
                {
                    rowCounts = sheet.UsedRange.Rows.Count;
                    colCounts = sheet.UsedRange.Columns.Count;
                }
                //空行/列截止
                else if (isActive)
                {
                    rowCounts = sheet.UsedRange.CurrentRegion.Rows.Count;
                    colCounts = sheet.UsedRange.CurrentRegion.Columns.Count;
                }
                else if(isSingleLine)
                {
                    rowCounts = sheet.get_Range("A65535").get_End(Excel.XlDirection.xlUp).Row;
                    colCounts = sheet.get_Range("IV1").get_End(Excel.XlDirection.xlToLeft).Column;
                }

                RowCounts.Set(context, rowCounts);
                ColCounts.Set(context, colCounts);

                System.Runtime.InteropServices.Marshal.ReleaseComObject(sheet);
                sheet = null;
                GC.Collect();
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "EXCEL获取行列总数过程出错", e.Message);
                new CommonVariable().realaseProcessExit(excelApp);
            }
            return m_Delegate.BeginInvoke(callback, state);
        }

        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
        }
    }
}