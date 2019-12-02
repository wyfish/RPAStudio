using System.Activities;
using System.ComponentModel;
using System;
using Excel = Microsoft.Office.Interop.Excel;
using Plugins.Shared.Library;

namespace RPA.Integration.Activities.ExcelPlugins
{
    [Designer(typeof(AreaSettingDesigner))]
    public sealed class AreaSetting : AsyncCodeActivity
    {
        public AreaSetting()
        {
        }

        [Browsable(false)]
        public string icoPath { get { return "pack://application:,,,/RPA.Integration.Activities;Component/Resources/Excel/setting.png"; } }

        [Category("单元格起始")]
        [DisplayName("行")]
        [Browsable(true)]
        public InArgument<Int32> CellRow_Begin
        {
            get; set;
        }
        [Category("单元格起始")]
        [DisplayName("列")]
        [Browsable(true)]
        public InArgument<Int32> CellColumn_Begin
        {
            get;set;
        }

        [Category("单元格起始")]
        [Description("代表单元格名称的VB表达式，如A1")]
        [DisplayName("单元格名称")]
        [Browsable(true)]
        public InArgument<string> CellName_Begin
        {
            get; set;
        }

        [Category("单元格终点")]
        [DisplayName("行")]
        [Browsable(true)]
        public InArgument<Int32> CellRow_End
        {
            get;set;
        }

        [Category("单元格终点")]
        [DisplayName("列")]
        [Browsable(true)]
        public InArgument<Int32> CellColumn_End
        {
            get;set;
        }

        [Category("单元格终点")]
        [Description("代表单元格名称的VB表达式，如B2")]
        [DisplayName("单元格名称")]
        [Browsable(true)]
        public InArgument<string> CellName_End
        {
            get; set;
        }


        ColorIndexEnum _CellColor = (ColorIndexEnum)0;
        [Category("选项")]
        [DisplayName("单元格填充色")]
        [Browsable(true)]
        public ColorIndexEnum CellColor
        {
            get
            {
                return _CellColor;
            }
            set
            {
                _CellColor = value;
            }
        }

        AlignEnum _AlignStyle = (AlignEnum)0;
        [Category("选项")]
        [DisplayName("对齐方式")]
        [Browsable(true)]
        public AlignEnum AlignStyle
        {
            get
            {
                return _AlignStyle;
            }
            set
            {
                _AlignStyle = value;
            }
        }

        ExcelFontEnum _Font = (ExcelFontEnum)0;
        [Category("字体设置")]
        [DisplayName("字体")]
        [Browsable(true)]
        public ExcelFontEnum Font
        {
            get
            {
                return _Font;
            }
            set
            {
                _Font = value;
            }
        }

        InArgument<Int32> _FontSize = 11;
        [Category("字体设置")]
        [DisplayName("字号")]
        [Browsable(true)]
        public InArgument<Int32> FontSize
        {
            get
            {
                return _FontSize;
            }
            set
            {
                _FontSize = value;
            }
        }

        ColorIndexEnum _FontColor = (ColorIndexEnum)0;
        [Category("字体设置")]
        [DisplayName("颜色")]
        [Browsable(true)]
        public ColorIndexEnum FontColor
        {
            get
            {
                return _FontColor;
            }
            set
            {
                _FontColor = value;
            }
        }

        bool _isBold;
        [Category("字体设置")]
        [DisplayName("粗体")]
        [Browsable(true)]
        public bool isBold
        {
            get
            {
                return _isBold;
            }
            set
            {
                _isBold = value;
            }
        }


        bool _isItalic;
        [Category("字体设置")]
        [DisplayName("斜体")]
        [Browsable(true)]
        public bool isItalic
        {
            get
            {
                return _isItalic;
            }
            set
            {
                _isItalic = value;
            }
        }



        bool _isUnderLine;
        [Category("字体设置")]
        [DisplayName("底线")]
        [Browsable(true)]
        public bool isUnderLine
        {
            get
            {
                return _isUnderLine;
            }
            set
            {
                _isUnderLine = value;
            }
        }

        InArgument<double> _RowHeight = 14.25;
        [Category("行高列宽")]
        [DisplayName("行高")]
        [Browsable(true)]
        public InArgument<double> RowHeight
        {
            get
            {
                return _RowHeight;
            }
            set
            {
                _RowHeight = value;
            }
        }

        InArgument<double> _ColWidth = 8.38;
        [Category("行高列宽")]
        [DisplayName("列宽")]
        [Browsable(true)]
        public InArgument<double> ColWidth
        {
            get
            {
                return _ColWidth;
            }
            set
            {
                _ColWidth = value;
            }
        }

        BorderType _BorderType = (BorderType)0;
        [Category("边框")]
        [DisplayName("边框选择")]
        [Browsable(true)]
        public BorderType BorderType
        {
            get
            {
                return _BorderType;
            }
            set
            {
                _BorderType = value;
            }
        }

        BorderStyle _BorderStyle = (BorderStyle)(-4142);
        [Category("边框")]
        [DisplayName("边框选择")]
        [Browsable(true)]
        public BorderStyle BorderStyle
        {
            get
            {
                return _BorderStyle;
            }
            set
            {
                _BorderStyle = value;
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

        public string ConvertFont(string fontName)
        {
            if (fontName == "等线Light")
                return "等线 Light";
            else if (fontName == "ArialBlack")
                return "Arial Black";
            else if (fontName == "ArialNarrow")
                return "Arial Narrow";
            else if (fontName == "ArialRoundedMTBold")
                return "Arial Rounded MT Bold";
            else if (fontName == "ArialUnicodeMS")
                return "Arial Unicode MS";
            else if (fontName == "CalibriLight")
                return "Calibri Light";
            else if (fontName == "MicrosoftYaHeiUI")
                return "Microsoft YaHei UI";
            else if (fontName == "MicrosoftYaHeiUILight")
                return "Microsoft YaHei UI Light";
            else if (fontName == "MicrosoftJhengHei")
                return "Microsoft JhengHei";
            else if (fontName == "MicrosoftJhengHeiLight")
                return "Microsoft JhengHei Light";
            else if (fontName == "MicrosoftJhengHeiUI")
                return "Microsoft JhengHei UI";
            else if (fontName == "MicrosoftJhengHeiUILight")
                return "Microsoft JhengHei UI Light";
            else if (fontName == "MicrosoftMHei")
                return "Microsoft MHei";
            else if (fontName == "MicrosoftNeoGothic")
                return "Microsoft NeoGothic";
            else if (fontName == "MalgunGothic")
                return "Malgun Gothic";
            else if (fontName == "AgencyFB")
                return "Agency FB";
            else if (fontName == "Bauhaus93")
                return "Bauhaus 93";
            else if (fontName == "BellMT")
                return "Bell MT";
            else if (fontName == "BerlinSansFB")
                return "Berlin Sans FB";
            else if (fontName == "BerlinSansFBDemi")
                return "Berlin Sans FB Demi";
            else if (fontName == "BernardMTCondensed")
                return "Bernard MT Condensed";
            else if (fontName == "BlackadderITC")
                return "Blackadder ITC";
            else if (fontName == "BodoniMT")
                return "Bodoni MT";
            else if (fontName == "BodoniMTBlack")
                return "Bodoni MT Black";
            else if (fontName == "YuGothic")
                return "Yu Gothic";
            else
                return fontName;
        }

        [Browsable(false)]
        public string ClassName { get { return "AreaSetting"; } }
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
                double rowHeight = RowHeight.Get(context);
                double colWidth = ColWidth.Get(context);
                Int32 fontSize = FontSize.Get(context);
                string sheetName = SheetName.Get(context);

                Excel::_Worksheet sheet = null;
                if (sheetName == null)
                    sheet = excelApp.ActiveSheet;
                else
                    sheet = excelApp.ActiveWorkbook.Sheets[sheetName];

                Excel::Range range1, range2;
                range1 = cellName_Begin == null ? sheet.Cells[cellRow_Begin, cellColumn_Begin] : sheet.Range[cellName_Begin];
                range2 = cellName_End == null ? sheet.Cells[cellRow_End, cellColumn_End] : sheet.Range[cellName_End];
                Excel::Range range = sheet.Range[range1, range2];

                /*对齐设置*/
                if ((int)_AlignStyle != 0)
                    range.HorizontalAlignment = (AlignEnum)_AlignStyle;

                /*字体*/
                range.Font.Bold = isBold;
                range.Font.Italic = isItalic;
                range.Font.Underline = isUnderLine;
                if (Font != 0)
                    range.Font.Name = ConvertFont(Font.ToString());
                range.Font.Size = fontSize;

                if ((int)_FontColor != 0)
                    range.Font.ColorIndex = (int)_FontColor;

                /*填充色*/
                if ((int)_CellColor != 0)
                    range.Interior.ColorIndex = (int)_CellColor;

                /*行列宽度*/
                range.RowHeight = rowHeight;
                range.ColumnWidth = colWidth;

                /*边框*/
                if ((int)_BorderStyle != 0)
                {
                    switch ((int)_BorderType)
                    {
                        case 0:
                            {
                                range.Borders.LineStyle = (int)_BorderStyle;
                                break;
                            }
                        case 1:
                            {
                                range.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = (int)_BorderStyle;
                                break;
                            }
                        case 2:
                            {
                                range.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = (int)_BorderStyle;
                                break;
                            }
                        case 3:
                            {
                                range.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = (int)_BorderStyle;
                                break;
                            }
                        case 4:
                            {
                                range.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = (int)_BorderStyle;
                                break;
                            }
                        default:
                            break;
                    }
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(sheet);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(range);
                sheet = null;
                range = null;
                GC.Collect();
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "EXCEL区域设置执行过程出错", e.Message);
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
