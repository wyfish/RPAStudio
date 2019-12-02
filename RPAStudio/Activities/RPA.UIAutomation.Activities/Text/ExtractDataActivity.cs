using System;
using System.Activities;
using System.ComponentModel;
using System.Windows;

namespace RPA.UIAutomation.Activities.Text
{
    [Designer(typeof(ExtractDataDesigner))]
    public sealed class ExtractDataActivity : CodeActivity
    {
        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "Extract Data"; } }

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
        [DisplayName("文本")]
        [Description("要单击的文本")]
        public InArgument<String> Text { get; set; }


        [Category("Common")]
        [Description("指定即使当前活动失败，也要继续执行其余的活动。只支持布尔值(True,False)。")]
        public InArgument<bool> ContinueOnError { get; set; }

        private InArgument<Int32> _DelayPage = 300;
        [Category("选项")]
        [DisplayName("等待时间")]
        [Description("等待加载到下一页的时间量")]
        public InArgument<Int32> DelayPage
        {
            get
            {
                return _DelayPage;
            }
            set
            {
                if (_DelayPage == value) return;
                _DelayPage = value;
            }
        }

        private InArgument<Int32> _MaxNumber = 100;
        [Category("选项")]
        [DisplayName("抽取最大值")]
        [Description("可以抽取数据的最大值")]
        public InArgument<Int32> MaxNumber
        {
            get
            {
                return _MaxNumber;
            }
            set
            {
                if (_MaxNumber == value) return;
                _MaxNumber = value;
            }
        }

        private InArgument<string> _NextSelector;
        [Category("选项")]
        [DisplayName("下一个连接器")]
        [Description("选择器标识用于导航到下一页的链接/按钮。应该相对于现有的uielement属性")]
        public InArgument<string> NextSelector
        {
            get
            {
                return _NextSelector;
            }
            set
            {
                _NextSelector = value;
            }
        }

        [Category("选项")]
        [DisplayName("发送窗体消息")]
        [Description("如果选中，单击用于导航到下一页的next link/按钮将通过向othe目标应用程序发送特定消息来执行。这种输入方法可以在后台工作，与大多数桌面应用程序兼容，但它不是最快的方法")]
        public bool SendMessage { get; set; }

        private bool _SimulateClick = true;
        [Category("选项")]
        [DisplayName("模拟点击")]
        [Description("如果选中，它将使用目标应用程序的技术模拟单击用于导航下一页的next链接/按钮。这种输入法是三种输入法中速度最快的一种，可以在后台工作")]
        public bool SimulateClick
        {
            get
            {
                return _SimulateClick;
            }
            set
            {
                _SimulateClick = value;
            }
        }

        [Category("选项")]
        [DisplayName("抽取目标数据")]
        [Description("允许您定义要从指定的web页面提取哪些数据的xml字符串")]
        public InArgument<string> ExtractMetaData { get; set; }

        [Browsable(false)]
        public string SourceImgPath { get; set; }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Text/extract.png";
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
