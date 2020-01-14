using System.Activities;
using System.ComponentModel;
using System;
using System.Windows.Data;
using System.Globalization;
using Excel = Microsoft.Office.Interop.Excel;
using Plugins.Shared.Library;

namespace RPA.Integration.Activities.ExcelPlugins
{
    public enum Operations
    {
        删除 = 1,
        隐藏 = 2,
        添加 = 3,
        //获取 = 4
    }

    /*RadioButton到Enum转换器*/
    public class EnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //return value == null ? false : value.Equals(parameter);
            if (value == null)
                return false;
            else
                return value.Equals(parameter);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //return value != null && value.Equals(true) ? parameter : Binding.DoNothing;
            if (value != null && value.Equals(true))
                return parameter;
            else
                return Binding.DoNothing;
        }
    }



    [Designer(typeof(RowColDesigner))]
    public sealed class RowCol : AsyncCodeActivity
    {
        public RowCol()
        {
        }


        [Browsable(false)]
        public string icoPath { get { return "pack://application:,,,/RPA.Integration.Activities;Component/Resources/Excel/rowcol.png"; } }


        Operations _Operation = (Operations)1;
        [Localize.LocalizedCategory("Category4")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName25")] //行列操作 //Row operation //行操作
        [Browsable(true)]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Operations Operation
        {
            get
            {
                return _Operation;
            }
            set
            {
                _Operation = value;
            }
        }

        public enum RowCols
        {
            行 = 1,
            列 = 2
        }

        RowCols _RowColSel = (RowCols)1;
        [Localize.LocalizedCategory("Category4")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName26")] //行列选择 //Row selection //行選択
        [Browsable(true)]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public RowCols RowColSel
        {
            get
            {
                return _RowColSel;
            }
            set
            {
                _RowColSel = value;
            }
        }

        InArgument<Int32>  _RowColBegin;
        [Localize.LocalizedCategory("Category4")] //选项 //Option //オプション
        [RequiredArgument]
        [Localize.LocalizedDisplayName("DisplayName27")] //行号/列号(开始) //Line number / column number (start) //行番号/列番号（開始）
        [Browsable(true)]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public InArgument<Int32> RowColBegin
        {
            get
            {
                return _RowColBegin;
            }
            set
            {
                _RowColBegin = value;
            }
        }


        InArgument<Int32> _RowColEnd;
        [Localize.LocalizedCategory("Category4")] //选项 //Option //オプション
        [RequiredArgument]
        [Localize.LocalizedDisplayName("DisplayName28")] //行号/列号(结束) //Line number / column number (end) //行番号/列番号（終了）
        [Browsable(true)]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public InArgument<Int32> RowColEnd
        {
            get
            {
                return _RowColEnd;
            }
            set
            {
                _RowColEnd = value;
            }
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

                Int32 rowColBegin = RowColBegin.Get(context);
                Int32 rowColEnd = RowColEnd.Get(context);
                if (rowColBegin > rowColEnd)
                {
                    SharedObject.Instance.Output(SharedObject.enOutputType.Error, "EXCEL行列操作开始值不能大于结束值");
                }

                switch ((int)Operation)
                {
                    case 1:
                        {
                            if (RowColSel == (RowCols)1)
                            {
                                sheet.Range[
                                   sheet.Cells[rowColBegin, 1],
                                   sheet.Cells[rowColEnd, sheet.Columns.Count]].
                                   Delete(Excel.XlDeleteShiftDirection.xlShiftUp);
                            }
                            else
                            {
                                sheet.Range[
                                    sheet.Cells[1, rowColBegin],
                                    sheet.Cells[sheet.Rows.Count, rowColEnd]]
                                    .Delete(Excel.XlDeleteShiftDirection.xlShiftToLeft);
                            }
                            break;
                        }
                    case 2:
                        {
                            if (RowColSel == (RowCols)1)
                            {
                                sheet.Range[
                                    sheet.Cells[rowColBegin, 1],
                                    sheet.Cells[rowColEnd, 1]].
                                    EntireRow.Hidden = true;
                            }
                            else
                            {
                                sheet.Range[
                                    sheet.Cells[1, rowColBegin],
                                    sheet.Cells[sheet.Rows.Count, rowColEnd]].
                                    EntireColumn.Hidden = true;
                            }
                            break;
                        }
                    case 3:
                        {
                            if (RowColSel == (RowCols)1)
                            {
                                sheet.Range[
                                   sheet.Cells[rowColBegin, 1],
                                   sheet.Cells[rowColEnd, sheet.Columns.Count]].
                                   Insert(Excel.XlInsertFormatOrigin.xlFormatFromLeftOrAbove);
                            }
                            else
                            {
                                sheet.Range[
                                    sheet.Cells[1, rowColBegin],
                                    sheet.Cells[sheet.Rows.Count, rowColEnd]].
                                    Insert(Excel.XlInsertShiftDirection.xlShiftToRight);
                            }
                            break;
                        }
                    default:
                        return m_Delegate.BeginInvoke(callback, state);
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(sheet);
                sheet = null;
                GC.Collect();

            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "EXCEL行列操作执行过程出错", e.Message);
                new CommonVariable().realaseProcessExit(excelApp);
            }
            return m_Delegate.BeginInvoke(callback, state);
        }

        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
        }
    }
}
