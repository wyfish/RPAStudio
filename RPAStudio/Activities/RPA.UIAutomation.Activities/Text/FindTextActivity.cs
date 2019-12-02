using System;
using System.Activities;
using System.ComponentModel;
using System.Windows;

namespace RPA.UIAutomation.Activities.Text
{
    [Designer(typeof(FindTextDesigner))]
    public sealed class FindTextActivity : CodeActivity
    {
        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "Find Text"; } }

        [Category("UI对象")]
        [OverloadGroup("G1")]
        [RequiredArgument]
        [Browsable(true)]
        [DisplayName("窗口指示器")]
        [Description("用于在执行活动时查找特定UI元素的Text属性")]
        public InArgument<string> Selector { get; set; }


        [Category("UI对象")]
        [OverloadGroup("G2")]
        [RequiredArgument]
        [Browsable(true)]
        [DisplayName("UI元素")]
        [Description("要关闭的窗口。该字段仅接受Window变量")]
        public InArgument<UIElement> ActiveWindow { get; set; }


        [RequiredArgument]
        [Category("UI对象")]
        [Browsable(true)]
        [DisplayName("字符串")]
        [Description("要单击的字符串")]
        public InArgument<String> Text { get; set; }

        private InArgument<Int32> _Occurrence = 1;
        [Category("UI对象")]
        [Browsable(true)]
        [DisplayName("指定次数")]
        [Description("如果文本字段中的字符串在指定的ui元素中出现多次，请在这里指定出现次数，而不是单击次数")]
        public InArgument<Int32> Occurrence
        {
            get
            {
                return _Occurrence;
            }
            set
            {
                _Occurrence = value;
            }
        }


        //  [RequiredArgument]
        [Category("Common")]
        [Description("指定即使当前活动失败，也要继续执行其余的活动。只支持布尔值(True,False)。")]
        public InArgument<bool> ContinueOnError { get; set; }

        private InArgument<Int32> _DelayAfter = 300;
        [Category("Common")]
        [Description("执行活动后的延迟时间(以毫秒为单位)。默认时间为300毫秒。")]
        public InArgument<Int32> DelayAfter
        {
            get
            {
                return _DelayAfter;
            }
            set
            {
                if (_DelayAfter == value) return;
                _DelayAfter = value;
            }
        }

        private InArgument<Int32> _DelayBefore = 200;
        [Category("Common")]
        [Description("延迟活动开始执行任何操作之前的时间(以毫秒为单位)。默认时间为200毫秒。")]
        public InArgument<Int32> DelayBefore
        {
            get
            {
                return _DelayBefore;
            }
            set
            {
                if (_DelayBefore == value) return;
                _DelayBefore = value;
            }
        }

        [Category("选项")]
        [Description("如果此选项被选中,所选文本的屏幕布局将保持不变")]
        public InArgument<UIElement> Formatted { get; set; }

        [Category("输出")]
        [DisplayName("UI元素")]
        [Description("您要查找的字符串所在的ui元素。")]
        public bool UIElement { get; set; }

        [Browsable(false)]
        public string SourceImgPath { get; set; }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Text/exist.png";
            }
        }

        private System.Windows.Visibility visi = System.Windows.Visibility.Hidden;
        [Browsable(false)]
        public System.Windows.Visibility visibility
        {
            get
            {
                return visi;
            }
            set
            {
                visi = value;
            }
        }

        protected override void Execute(CodeActivityContext context)
        {
        }
    }
}
