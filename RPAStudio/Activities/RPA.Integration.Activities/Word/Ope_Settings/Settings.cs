using System.Activities;
using System.ComponentModel;
using System;
using Plugins.Shared.Library;
using RPA.Integration.Activities.ExcelPlugins;
using Microsoft.Office.Interop.Word;

namespace RPA.Integration.Activities.WordPlugins
{
    [Designer(typeof(SettingsDesigner))]
    public sealed class Settings : AsyncCodeActivity
    {
        public Settings()
        {
        }


        ExcelFontEnum _Font;
        [Category("字体设置")]
        [DisplayName("字体")]
        public ExcelFontEnum Font
        {
            get { return _Font; }
            set { _Font = value; }
        }

        InArgument<float> _FontSize = 10.5F;
        [Category("字体设置")]
        [DisplayName("字体大小")]
        public InArgument<float> FontSize
        {
            get { return _FontSize; }
            set { _FontSize = value; }
        }

        WdColorIndexEnum _FontColor;
        [Category("字体设置")]
        [DisplayName("字体颜色")]
        public WdColorIndexEnum FontColor
        {
            get { return _FontColor; }
            set { _FontColor = value; }
        }

        bool _FontBold = false;
        [Category("字体设置")]
        [DisplayName("粗体")]
        public bool FontBold
        {
            get { return _FontBold; }
            set { _FontBold = value; }
        }


        bool _FontItalic = false;
        [Category("字体设置")]
        [DisplayName("斜体")]
        public bool FontItalic
        {
            get { return _FontItalic; }
            set { _FontItalic = value; }
        }

        bool _FontUnderLine = false;
        [Category("字体设置")]
        [DisplayName("下划线")]
        public bool FontUnderLine
        {
            get { return _FontUnderLine; }
            set { _FontUnderLine = value; }
        }

        bool _Shadow = false;
        [Category("字体设置")]
        [DisplayName("字体阴影")]
        public bool Shadow
        {
            get { return _Shadow; }
            set { _Shadow = value; }
        }


        public enum alignStyle
        {
            左对齐 = 0,
            居中 = 1,
            右对齐 = 2
        }

        alignStyle _Align = alignStyle.左对齐;
        [Category("页面属性")]
        [DisplayName("对齐方式")]
        [Browsable(true)]
        public alignStyle Align
        {
            get { return _Align; }
            set { _Align = value; }
        }


        InArgument<Int32> _LeftMargin = 80;
        [Category("页面边距")]
        [DisplayName("左边距")]
        [Browsable(true)]
        public InArgument<Int32> LeftMargin
        {
            get { return _LeftMargin; }
            set { _LeftMargin = value; }
        }

        InArgument<Int32> _RightMargin = 80;
        [Category("页面边距")]
        [DisplayName("右边距")]
        [Browsable(true)]
        public InArgument<Int32> RightMargin
        {
            get { return _RightMargin; }
            set { _RightMargin = value; }
        }

        InArgument<Int32> _TopMargin = 64;
        [Category("页面边距")]
        [DisplayName("上边距")]
        [Browsable(true)]
        public InArgument<Int32> TopMargin
        {
            get { return _TopMargin; }
            set { _TopMargin = value; }
        }

        InArgument<Int32> _BottomMargin = 64;
        [Category("页面边距")]
        [DisplayName("下边距")]
        [Browsable(true)]
        public InArgument<Int32> BottomMargin
        {
            get { return _BottomMargin; }
            set { _BottomMargin = value; }
        }

        [Browsable(false)]
        public string icoPath { get { return "pack://application:,,,/RPA.Integration.Activities;Component/Resources/Word/property.png"; } }


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
        public string ClassName { get { return "Settings"; } }
        private delegate string runDelegate();
        private runDelegate m_Delegate;
        public string Run()
        {
            return ClassName;
        }


        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            PropertyDescriptor property = context.DataContext.GetProperties()[WordCreate.GetWordAppTag];
            Application wordApp = property.GetValue(context.DataContext) as Application;

            try
            {
                float fontSize = FontSize.Get(context);
                Int32 leftMargin = LeftMargin.Get(context);
                Int32 rightMargin = RightMargin.Get(context);
                Int32 topMargin = TopMargin.Get(context);
                Int32 bottomMargin = BottomMargin.Get(context);
                Selection sel = wordApp.Selection;
                Font font = sel.Font;

                /*字体设置*/
                font.Size = fontSize;
                if (Font != 0)
                    font.Name = ConvertFont(Font.ToString());
                font.ColorIndex = (WdColorIndex)_FontColor;
                font.Shadow = Convert.ToInt32(_Shadow);
                font.Bold = Convert.ToInt32(_FontBold);
                font.Italic = Convert.ToInt32(_FontItalic);
                if (_FontUnderLine)
                    font.Underline = WdUnderline.wdUnderlineSingle;

                /*段落对齐设置*/
                ParagraphFormat paraFmt;
                paraFmt = sel.ParagraphFormat;
                paraFmt.Alignment = (WdParagraphAlignment)_Align;
                sel.ParagraphFormat = paraFmt;

                /*页面设置*/
                PageSetup pgSet = wordApp.ActiveDocument.PageSetup;
                pgSet.LeftMargin = leftMargin;
                pgSet.RightMargin = rightMargin;
                pgSet.TopMargin = topMargin;
                pgSet.BottomMargin = bottomMargin;
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "Word执行过程出错", e.Message);
                CommonVariable.realaseProcessExit(wordApp);
            }

            m_Delegate = new runDelegate(Run);
            return m_Delegate.BeginInvoke(callback, state);
        }

        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
        }
    }
}
