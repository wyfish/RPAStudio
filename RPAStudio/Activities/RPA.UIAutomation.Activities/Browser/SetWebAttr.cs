using System;
using System.Activities;
using System.ComponentModel;
using Plugins.Shared.Library;
using System.Threading;
using System.Windows;
using Plugins.Shared.Library.UiAutomation;

namespace RPA.UIAutomation.Activities.Browser
{
    [Designer(typeof(SetWebAttrDesigner))]
    public sealed class SetWebAttr : CodeActivity
    {
        [Category("公共")]
        [DisplayName("错误执行")]
        [Description("指定即使活动引发错误，自动化是否仍应继续")]
        public InArgument<bool> ContinueOnError { get; set; }

        [Category("公共")]
        [DisplayName("延迟时间(结束)")]
        [Description("执行活动后的延迟时间(以毫秒为单位),默认时间为300毫秒")]
        public InArgument<Int32> DelayAfter { get; set; }

        [Category("公共")]
        [DisplayName("延迟时间(开始)")]
        [Description("活动开始执行任何操作之前的延迟时间(以毫秒为单位),默认的时间量是200毫秒。")]
        public InArgument<Int32> DelayBefore { get; set; }


        [Category("UI对象")]
        [OverloadGroup("G1")]
        [RequiredArgument]
        [Browsable(true)]
        [DisplayName("窗口指示器")]
        [Description("用于在执行活动时查找特定UI元素的Text属性")]
        public InArgument<string> Selector { get; set; }

        [Category("UI对象")]
        [DisplayName("超时时间")]
        [Description("指定在引发错误之前等待活动运行的时间量（以毫秒为单位）。默认值为30000毫秒（30秒）")]
        public InArgument<Int32> TimeoutMS { get; set; }

        [Category("UI对象")]
        [OverloadGroup("G1")]
        [Browsable(true)]
        [DisplayName("UI元素")]
        [Description("输入UIElement")]
        public InArgument<UiElement> Element { get; set; }

        InArgument<string> _Attribute;
        [Category("输入")]
        [DisplayName("HTML属性")]
        [Description("要更改的HTML属性的名称")]
        [RequiredArgument]
        [Browsable(true)]
        public InArgument<string> Attribute
        {
            get
            {
                return _Attribute;
            }
            set
            {
                _Attribute = value;
            }
        }

        InArgument<string> _Value;
        [Category("输入")]
        [DisplayName("值")]
        [RequiredArgument]
        [Description("要设置为指定属性的值。仅支持字符串变量")]
        [Browsable(true)]
        public InArgument<string> Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
            }
        }

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

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Browser/SetWebAttr.png";
            }
        }

        CountdownEvent latch;
        private void refreshData(CountdownEvent latch)
        {
            latch.Signal();
        }

        public static T GetValueOrDefault<T>(ActivityContext context, InArgument<T> source, T defaultValue)
        {
            T result = defaultValue;
            if (source != null && source.Expression != null)
            {
                result = source.Get(context);
            }
            return result;
        }

        protected override void Execute(CodeActivityContext context)
        {
            string attribute_str = Attribute.Get(context);
            string attribute_value = Value.Get(context);
            try
            {
                Int32 _timeout = TimeoutMS.Get(context);
                Thread.Sleep(_timeout);
                latch = new CountdownEvent(1);
                Thread td = new Thread(() =>
                {
                    if (Selector.Expression == null)
                    {
                        //ActiveElement处理
                    }
                    else
                    {
                        var selStr = Selector.Get(context);
                        UiElement element = GetValueOrDefault(context, this.Element, null);
                        if (element == null && selStr != null)
                        {
                            element = UiElement.FromSelector(selStr);
                        }
                        if(element != null)
                        {
                            element.SetForeground();
                            mshtml.IHTMLDocument2 currDoc = null;
                            SHDocVw.InternetExplorer ieBrowser = GetIEFromHWndClass.GetIEFromHWnd((int)element.WindowHandle, out currDoc);
                            mshtml.IHTMLElement currEle = GetIEFromHWndClass.GetEleFromDoc(
                                element.GetClickablePoint(), (int)element.WindowHandle, currDoc);
                            currEle.setAttribute(attribute_str, attribute_value);
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
                    refreshData(latch);
                });
                td.TrySetApartmentState(ApartmentState.STA);
                td.IsBackground = true;
                td.Start();
                latch.Wait();
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "出现异常", e.Message);
                if (ContinueOnError.Get(context))
                {

                }
            }
        }
    }
}
