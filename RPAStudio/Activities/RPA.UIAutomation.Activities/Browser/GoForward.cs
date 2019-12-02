using Plugins.Shared.Library;
using System;
using System.Activities;
using System.ComponentModel;
using System.Threading;

namespace RPA.UIAutomation.Activities.Browser
{
    [Designer(typeof(GoForwardDesigner))]
    public sealed class GoForward : AsyncCodeActivity
    {
        static GoForward()
        {
        }

        private string classID = Guid.NewGuid().ToString("N");
        [Category("选项")]
        [DisplayName("错误执行")]
        [Description("指定即使活动引发错误，自动化是否仍应继续")]
        public InArgument<bool> ContinueOnError { get; set; }


        [Category("输入")]
        [Browsable(true)]
        [DisplayName("浏览器Browser")]
        [Description("要关闭的浏览器页面。该字段仅支持Browser变量")]
        public InArgument<Browser> currBrowser { get; set; }


        [Browsable(false)]
        public string guid { get { return classID; } }
        [Browsable(false)]
        public string SourceImgPath { get; set; }
        [Browsable(false)]
        public string ClassName { get { return "GoForward"; } }


        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Browser/GoForward.png";
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

        private delegate string runDelegate();
        private runDelegate m_Delegate;
        public string Run()
        {
            return ClassName;
        }
        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            Browser thisBrowser = currBrowser.Get(context);
            //先走 其次走流程
            if (thisBrowser == null)
            {
                PropertyDescriptor property = context.DataContext.GetProperties()[OpenBrowser.OpenBrowsersPropertyTag];
                if (property == null)
                    property = context.DataContext.GetProperties()[AttachBrowser.OpenBrowsersPropertyTag];
                if (property == null)
                {
                    SharedObject.Instance.Output(SharedObject.enOutputType.Error, "活动流程传递的浏览器变量为空，请检查！");
                    m_Delegate = new runDelegate(Run);
                    return m_Delegate.BeginInvoke(callback, state);
                }
                Browser getBrowser = property.GetValue(context.DataContext) as Browser;
                if (getBrowser.getCFBrowser() != null)
                {
                    getBrowser.getCFBrowser().Navigate().Forward();
                }
                else if (getBrowser.getIEBrowser() != null)
                {
                    getBrowser.getIEBrowser().GoForward();
                }
                //确保页面是否已加载完成
                while (getBrowser.getIEBrowser().ReadyState != SHDocVw.tagREADYSTATE.READYSTATE_COMPLETE)
                {
                    Thread.Sleep(500);
                }
            }
            else
            {
                if (thisBrowser.getCFBrowser() != null)
                {
                    thisBrowser.getCFBrowser().Navigate().Forward();
                }
                else if (thisBrowser.getIEBrowser() != null)
                {
                    thisBrowser.getIEBrowser().GoForward();
                }
                //确保页面是否已加载完成
                while (thisBrowser.getIEBrowser().ReadyState != SHDocVw.tagREADYSTATE.READYSTATE_COMPLETE)
                {
                    Thread.Sleep(500);
                }
            }


            m_Delegate = new runDelegate(Run);
            return m_Delegate.BeginInvoke(callback, state);
        }
        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
        }
    }
}
