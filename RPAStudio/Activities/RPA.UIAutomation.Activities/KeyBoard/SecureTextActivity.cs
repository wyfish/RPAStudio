using System;
using System.Activities;
using System.Activities.Presentation.Metadata;
using System.Collections.Generic;
using System.ComponentModel;
using System.Activities.Presentation.PropertyEditing;
using System.Security;
using System.Windows;
using RPA.UIAutomation.Activities.Mouse;
using System.Runtime.InteropServices;
using Plugins.Shared.Library.UiAutomation;
using Plugins.Shared.Library;
using System.Threading;
using System.Windows.Forms;

namespace RPA.UIAutomation.Activities.Keyboard
{
    [Designer(typeof(SecureTextDesigner))]
    public sealed class SecureTextActivity : CodeActivity
    {
        static SecureTextActivity()
        {
            AttributeTableBuilder builder = new AttributeTableBuilder();
            builder.AddCustomAttributes(typeof(SecureTextActivity), "ClickType", new EditorAttribute(typeof(MouseClickTypeEditor), typeof(PropertyValueEditor)));
            builder.AddCustomAttributes(typeof(SecureTextActivity), "MouseButton", new EditorAttribute(typeof(MouseButtonTypeEditor), typeof(PropertyValueEditor)));
            //builder.AddCustomAttributes(typeof(SecureTextActivity), "KeyModifiers", new EditorAttribute(typeof(KeyModifiersEditor), typeof(PropertyValueEditor)));
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }

        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "Secure Text"; } }

        [Category("UI对象")]
        [OverloadGroup("G1")]
        [Browsable(true)]
        [DisplayName("窗口指示器")]
        [Description("用于在执行活动时查找特定UI元素的Text属性")]
        public InArgument<string> Selector { get; set; }

        [Category("UI对象")]
        [OverloadGroup("G1")]
        [Browsable(true)]
        [DisplayName("UI元素")]
        [Description("输入UIElement")]
        public InArgument<UiElement> Element { get; set; }

        //  [RequiredArgument]
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
        public string icoPath { get { return "pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/KeyBoard/text.png"; } }

        [Browsable(false)]
        public List<string> KeyTypes
        {
            get
            {
                KeyboardTypes key = new KeyboardTypes();
                return key.getKeyTypes;
            }
        }


        [RequiredArgument]
        [Category("输入项")]
        [Browsable(true)]
        [DisplayName("Secure Text")]
        public InArgument<SecureString> SecureText
        {
            get;
            set;
        }

        [Category("鼠标选项")]
        public bool isRunClick { get; set; }

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


        //[Category("鼠标选项")]
        //public string KeyModifiers { get; set; }

        [Browsable(false)]
        public string SourceImgPath { get; set; }

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
            Int32 _delayAfter = Common.GetValueOrDefault(context, this.DelayAfter, 300);
            Int32 _delayBefore = Common.GetValueOrDefault(context, this.DelayBefore, 300);
            try
            {
                Thread.Sleep(_delayBefore);
                SecureString secureText = SecureText.Get(context);
                IntPtr inP = Marshal.SecureStringToBSTR(secureText);//inP为secureStr的句柄
                string text = Marshal.PtrToStringBSTR(inP);
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
                    else
                    {
                        SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", "查找不到元素");
                        if (ContinueOnError.Get(context))
                        {
                            return;
                        }
                        else
                        {
                            throw new NotImplementedException("查找不到元素");
                        }
                    }
                }
                /*执行鼠标点击事件*/
                if (isRunClick)
                {
                    UiElement.MouseMoveTo(pointX, pointY);
                    UiElement.MouseAction((Plugins.Shared.Library.UiAutomation.ClickType)ClickType, (Plugins.Shared.Library.UiAutomation.MouseButton)MouseButton);
                }
                else if(true)
                {

                }
                else
                {
                    SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", "找不到键值");
                    if (ContinueOnError.Get(context))
                    {
                        return;
                    }
                    else
                    {
                        throw new NotImplementedException("找不到键值");
                    }
                }
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "发送安全文本执行过程出错", e.Message);
            }
        }
    }
}
