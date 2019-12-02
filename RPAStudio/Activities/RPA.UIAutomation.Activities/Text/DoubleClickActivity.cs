using System;
using System.Activities;
using System.ComponentModel;
using System.Windows;
using System.Activities.Presentation.Metadata;
using System.Activities.Presentation.PropertyEditing;
using RPA.UIAutomation.Activities.Mouse;

namespace RPA.UIAutomation.Activities.Text
{
    [Designer(typeof(DoubleClickDesigner))]
    public sealed class DoubleClickActivity : CodeActivity
    {
        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "Double Click"; } }
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
        private MouseClickType _ClickType = MouseClickType.CLICK_DOUBLE;
        [Category("Input")]
        public MouseClickType ClickType
        {
            get
            {
                return _ClickType;
            }
            set
            {
                _ClickType = value;
            }
        }

        private MouseButtonType _ButtonType = MouseButtonType.BTN_LEFT;
        [Category("Input")]
        public MouseButtonType MouseButton
        {
            get
            {
                return _ButtonType;
            }
            set
            {
                _ButtonType = value;
            }
        }

        private InArgument<Int32> _Occurrence = 1;
        [Category("Input")]
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

        [Category("Input")]
        public InArgument<Int32> offsetX { get; set; }
        [Category("Input")]
        public InArgument<Int32> offsetY { get; set; }
        [Category("Input")]
        public string KeyModifiers { get; set; }

        private PositionType _Position = PositionType.Center;
        [Category("Input")]
        public PositionType Position
        {
            get
            {
                return _Position;
            }
            set
            {
                _Position = value;
            }
        }

        [Category("Input")]
        [Description("如果此选项被选中,所选文本的屏幕布局将保持不变")]
        public bool Formatted { get; set; }

        [Browsable(false)]
        public string SourceImgPath { get; set; }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Text/click.png";
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

        static DoubleClickActivity()
        {
            AttributeTableBuilder builder = new AttributeTableBuilder();
            builder.AddCustomAttributes(typeof(DoubleClickActivity), "KeyModifiers", new EditorAttribute(typeof(KeyModifiersEditor), typeof(PropertyValueEditor)));
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }

        protected override void Execute(CodeActivityContext context)
        {
        }
    }
}
