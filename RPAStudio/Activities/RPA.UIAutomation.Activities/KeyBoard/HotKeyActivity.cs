using System;
using System.Activities;
using System.Activities.Presentation.Metadata;
using System.Collections.Generic;
using System.ComponentModel;
using System.Activities.Presentation.PropertyEditing;
using Plugins.Shared.Library;
using Plugins.Shared.Library.UiAutomation;
using RPA.UIAutomation.Activities.Mouse;
using System.Threading;

namespace RPA.UIAutomation.Activities.Keyboard
{
    [Designer(typeof(HotKeyDesigner))]
    public sealed class HotKeyActivity : CodeActivity
    {
        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "Hot Key"; } }

        [Category("UI对象")]
        [OverloadGroup("G1")]
        [Browsable(true)]
        [DisplayName("窗口指示器")]
        [Description("用于在执行活动时查找特定UI元素的Text属性")]
        public InArgument<string> Selector { get; set; }

        [Category("Common")]
        [Description("指定即使当前活动失败，也要继续执行其余的活动。只支持布尔值(True,False)。")]
        public InArgument<bool> ContinueOnError { get; set; }

        [Category("Common")]
        [Description("执行活动后的延迟时间(以毫秒为单位)。默认时间为300毫秒。")]
        public InArgument<Int32> DelayAfter { get; set; }

        [Category("Common")]
        [Description("延迟活动开始执行任何操作之前的时间(以毫秒为单位)。默认时间为300毫秒。")]
        public InArgument<Int32> DelayBefore { get; set; }

        [Browsable(false)]
        public string icoPath { get { return "pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/KeyBoard/hotkey.png"; } }

        [Browsable(false)]
        public List<string> KeyTypes
        {
            get
            {
                KeyboardTypes key = new KeyboardTypes();
                return key.getKeyTypes;
            }
            set
            {

            }
        }

        [Category("按键选项")]
        [Browsable(true)]
        [DisplayName("其它按键")]
        public string SelectedKey { get; set; }

        [Category("按键选项")]
        [Browsable(true)]
        [DisplayName("Alt")]
        public bool Alt { get; set; }

        [Category("按键选项")]
        [Browsable(true)]
        [DisplayName("Ctrl")]
        public bool Ctrl { get; set; }

        [Category("按键选项")]
        [Browsable(true)]
        [DisplayName("Shift")]
        public bool Shift { get; set; }

        [Category("按键选项")]
        [Browsable(true)]
        [DisplayName("Win")]
        public bool Win { get; set; }

        [Category("鼠标选项")]
        public Int32 ClickType { get; set; }
        [Category("鼠标选项")]
        public Int32 MouseButton { get; set; }

        [Category("鼠标选项")]
        public InArgument<Int32> offsetX { get; set; }
        [Category("鼠标选项")]
        public InArgument<Int32> offsetY { get; set; }

        [Category("鼠标选项")]
        [DisplayName("使用坐标点")]
        public bool usePoint { get; set; }
        [Category("鼠标选项")]
        public bool isRunClick { get; set; }

        [Browsable(false)]
        public string SourceImgPath { get; set; }

        [Category("UI对象")]
        [OverloadGroup("G1")]
        [Browsable(true)]
        [DisplayName("UI元素")]
        [Description("输入UIElement")]
        public InArgument<UiElement> Element { get; set; }

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

        static HotKeyActivity()
        {
            AttributeTableBuilder builder = new AttributeTableBuilder();
            builder.AddCustomAttributes(typeof(HotKeyActivity), "ClickType", new EditorAttribute(typeof(MouseClickTypeEditor), typeof(PropertyValueEditor)));
            builder.AddCustomAttributes(typeof(HotKeyActivity), "MouseButton", new EditorAttribute(typeof(MouseButtonTypeEditor), typeof(PropertyValueEditor)));
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }

        private void DealBaseKeyBordPress()
        {
            if (Alt)
                Common.DealKeyBordPress("Alt");
            if(Ctrl)
                Common.DealKeyBordPress("Ctrl");
            if(Shift)
                Common.DealKeyBordPress("Shift");
            if(Win)
                Common.DealKeyBordPress("Win");
        }

        private void DealBaseKeyBordRelease()
        {
            if (Alt)
                Common.DealKeyBordRelease("Alt");
            if (Ctrl)
                Common.DealKeyBordRelease("Ctrl");
            if (Shift)
                Common.DealKeyBordRelease("Shift");
            if (Win)
                Common.DealKeyBordRelease("Win");
        }

        protected override void Execute(CodeActivityContext context)
        {
            Int32 _delayAfter = Common.GetValueOrDefault(context, this.DelayAfter, 300);
            Int32 _delayBefore = Common.GetValueOrDefault(context, this.DelayBefore, 300);
            try
            {
                Thread.Sleep(_delayBefore);
                var selStr = Selector.Get(context);
                UiElement element = Common.GetValueOrDefault(context, this.Element, null);
                if (element == null && selStr != null)
                {
                    element = UiElement.FromSelector(selStr);
                }

                Int32 pointX = 0;
                Int32 pointY = 0;
                if (usePoint)
                {
                    pointX = offsetX.Get(context);
                    pointY = offsetY.Get(context);
                }
                else
                {
                    if (element != null)
                    {
                        pointX = element.GetClickablePoint().X;
                        pointY = element.GetClickablePoint().Y;
                        element.SetForeground();
                    }
                }
                if (isRunClick)
                {
                    UiElement.MouseMoveTo(pointX, pointY);
                    UiElement.MouseAction((Plugins.Shared.Library.UiAutomation.ClickType)ClickType, (Plugins.Shared.Library.UiAutomation.MouseButton)MouseButton);
                }
                DealBaseKeyBordPress();
                if (Common.DealVirtualKeyPress(SelectedKey.ToUpper()))
                {
                    Common.DealVirtualKeyRelease(SelectedKey.ToUpper());
                }
                DealBaseKeyBordRelease();
                Thread.Sleep(_delayAfter);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", e.Message);
                if (ContinueOnError.Get(context))
                {
                    return;
                }
                else
                {
                    throw new NotImplementedException(e.Message);
                }
            }
        } 
    }
}
