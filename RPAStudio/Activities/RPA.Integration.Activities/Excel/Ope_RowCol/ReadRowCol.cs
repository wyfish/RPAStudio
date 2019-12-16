using System.Activities;
using System.ComponentModel;
using System;
using System.Windows.Data;
using System.Globalization;
using Excel = Microsoft.Office.Interop.Excel;
using Plugins.Shared.Library;
using System.Collections.ObjectModel;

namespace RPA.Integration.Activities.ExcelPlugins
{
    [Designer(typeof(ReadRowColDesigner))]
    public sealed class ReadRowCol : AsyncCodeActivity
    {
        public ReadRowCol()
        {
        }


        [Browsable(false)]
        public string icoPath { get { return "pack://application:,,,/RPA.Integration.Activities;Component/Resources/Excel/rowcol.png"; } }



        [Category("选项")]
        [RequiredArgument]
        [DisplayName("行号/列号")]
        [Browsable(true)]
        public InArgument<Int32> RowColNum
        {
            get;set;
        }

        public enum ReadType
        {
            读取行,
            读取列
        }


        [Category("选项")]
        [RequiredArgument]
        [DisplayName("行/列")]
        [Browsable(true)]
        public ReadType CurrReadType
        {
            get; set;
        }

        [Category("选项")]
        [DisplayName("工作表名称")]
        [Description("为空代表当前活动工作表")]
        [Browsable(true)]
        public InArgument<string> SheetName
        {
            get; set;
        }

        [Category("输出")]
        [RequiredArgument]
        [DisplayName("行/列数据")]
        [Browsable(true)]
        public OutArgument<object> RolColData
        {
            get;set;
        }


        [Browsable(false)]
        public string ClassName { get { return "ReadRowCol"; } }
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

                object data = new object();

                Int32 rowColNum = RowColNum.Get(context);
                if(CurrReadType == ReadType.读取行)
                {
                    data = sheet.Rows[rowColNum].Value;
                }
                else
                {
                    data = sheet.Columns[rowColNum].Value;
                }

                //Collection<object> _data = new Collection<object>();

                //foreach (object cell in ((object[,])data))
                //{
                //    if (cell != null)
                //    {
                //        _data.Add(cell);
                //    }
                //    else
                //        break;
                //}

                RolColData.Set(context, data);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(sheet);
                sheet = null;
                GC.Collect();
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "EXCEL行列读取执行过程出错", e.Message);
                new CommonVariable().realaseProcessExit(excelApp);
            }
            return m_Delegate.BeginInvoke(callback, state);
        }

        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
        }
    }
}