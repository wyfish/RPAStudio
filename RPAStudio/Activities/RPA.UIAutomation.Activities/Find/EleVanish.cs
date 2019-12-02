using FlaUI.Core.AutomationElements;
using RPA.UIAutomation.Activities.Mouse;
using Plugins.Shared.Library;
using Plugins.Shared.Library.UiAutomation;
using System;
using System.Activities;
using System.ComponentModel;
using System.Threading;

namespace RPA.UIAutomation.Activities.Find
{
    [Designer(typeof(EleVanishDesigner))]
    public sealed class EleVanish : AsyncCodeActivity
    {
        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "Element Vanish"; } }

        [Category("公共")]
        [DisplayName("错误执行")]
        [Description("指定即使活动引发错误，自动化是否仍应继续,取值为（True或False）")]
        public InArgument<bool> ContinueOnError { get; set; }

        [Category("选项")]
        [DisplayName("等待元素停止")]
        [Description("当此选项被选中，该活动将在指定活动结束前一直等待")]
        public bool WaitNotActivity { get; set; }

        [Category("选项")]
        [DisplayName("等待元素消失")]
        [Description("当此选项被选中，即使ui元素仍然是活动的，该活动也只会等到ui元素从屏幕上消失")]
        public bool WaitNotVisible { get; set; }

        [Category("UI对象")]
        [OverloadGroup("Selector")]
        [RequiredArgument]
        [Browsable(true)]
        [DisplayName("窗口指示器")]
        [Description("用于在执行活动时查找特定UI元素的Text属性")]
        public InArgument<string> Selector { get; set; }


        [Category("UI对象")]
        [OverloadGroup("Element")]
        [RequiredArgument]
        [Browsable(true)]
        [DisplayName("UI元素")]
        [Description("要关闭的窗口。该字段仅接受Window变量")]
        public InArgument<UiElement> Element { get; set; }


        [Browsable(false)]
        public string SourceImgPath { get; set; }
       

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Find/EleVanish.png";
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


        [Browsable(false)]
        public string ClassName { get { return "FindChildren"; } }
        private delegate string runDelegate();
        private runDelegate m_Delegate;
        public string Run()
        {
            return ClassName;
        }


        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            try
            {
                m_Delegate = new runDelegate(Run);

                UiElement element = null;
                var selStr = Selector.Get(context);

                element = Common.GetValueOrDefault(context, this.Element, null);
                if (element == null && selStr != null)
                {
                    element = UiElement.FromSelector(selStr);
                }
                else
                {
                    PropertyDescriptor property = context.DataContext.GetProperties()[EleScope.GetEleScope];
                    element = property.GetValue(context.DataContext) as UiElement;
                }

                AutomationElement autoEle = element.NativeObject as AutomationElement;
                while(true)
                {
                    if(WaitNotActivity && WaitNotVisible)
                    {
                        //不可见 不活动 逻辑
                    }
                    else if(WaitNotActivity)
                    {
                        //不活动逻辑
                    }
                    else if(WaitNotVisible)
                    {
                        //不可见逻辑
                    }
                    else
                    {
                        if (!autoEle.IsEnabled)
                            break;
                    }
                    Thread.Sleep(500);
                }

                return m_Delegate.BeginInvoke(callback, state);

            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "等待元素消失出错", e.Message);
                if (ContinueOnError.Get(context))
                {
                    return m_Delegate.BeginInvoke(callback, state);
                }
                else
                {
                    throw e;
                }
            }
        }

        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
            throw new NotImplementedException();
        }
    }
}
