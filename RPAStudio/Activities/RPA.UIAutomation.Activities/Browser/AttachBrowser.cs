using System;
using System.Activities;
using System.ComponentModel;
using Plugins.Shared.Library;
using System.Threading;
using System.Activities.Statements;
using System.Collections;
using Plugins.Shared.Library.UiAutomation;

namespace RPA.UIAutomation.Activities.Browser
{
    [Designer(typeof(AttachBrowserDesigner))]
    public sealed class AttachBrowser : NativeActivity
    {
        public string _DisplayName { get { return "AttachBrowser"; } }

        [Browsable(false)]
        public ActivityAction<object> Body { get; set; }

        public AttachBrowser()
        {
            Body = new ActivityAction<object>
            {
                Argument = new DelegateInArgument<object>(OpenBrowsersPropertyTag),
                Handler = new Sequence()
                {
                    DisplayName = "Do"
                }
            };
        }

        public static string OpenBrowsersPropertyTag { get { return "AttachBrowser"; } }


        [Category("公共")]
        [DisplayName("错误执行")]
        [Description("指定即使活动引发错误，自动化是否仍应继续")]
        public InArgument<bool> ContinueOnError { get; set; }

        [Category("输入")]
        [OverloadGroup("G1")]
        [RequiredArgument]
        [Browsable(true)]
        [DisplayName("浏览器")]
        [Description("要附加的现有浏览器变量")]
        public InArgument<Browser> currBrowser { get; set; }

        [Category("输入")]
        [DisplayName("超时时间")]
        [Description("指定在引发错误之前等待活动运行的时间量（以毫秒为单位）。默认值为30000毫秒（30秒）")]
        public InArgument<Int32> TimeoutMS { get; set; }

        [Category("输入")]
        [OverloadGroup("G2")]
        [RequiredArgument]
        [Browsable(true)]
        [DisplayName("UI选择器")]
        [Description("用于在执行活动时查找特定UI元素的Text属性")]
        public InArgument<string> Selector { get; set; }

        [Category("输出")]
        [DisplayName("UI浏览器")]
        [Description("活动返回的Browser变量")]
        [Browsable(true)]
        public OutArgument<Browser> UiBrowser
        {
            get;
            set;
        }

        [Category("输入")]
        [Browsable(true)]
        [DisplayName("浏览器类型")]
        [Description("选择要使用的浏览器类型。可以使用以下选项：IE，Firefox，Chrome")]
        public BrowserTypes BrowserType
        {
            get;
            set;
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
        public SHDocVw.InternetExplorer SelectedIE { get; set; }

        [Browsable(false)]
        public Int32 HWND { get; set; }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Browser/OpenBrowser.png";
            }
        }

        CountdownEvent latch;
        private void refreshData(CountdownEvent latch)
        {
            latch.Signal();
        }

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);
        }

        protected override void Execute(NativeActivityContext context)
        {
            try
            {
                Int32 _timeout = TimeoutMS.Get(context);
                Thread.Sleep(_timeout);
                latch = new CountdownEvent(1);
                Thread td = new Thread(() =>
                {
                    if (Selector.Expression == null)
                    {
                        Browser getBrowser = currBrowser.Get(context);
                        if (getBrowser != null)
                        {
                            if (getBrowser.getIEBrowser() != null)
                            {
                                if (UiBrowser != null)
                                {
                                    UiBrowser.Set(context, getBrowser);
                                }
                            }
                            context.ScheduleAction(Body, getBrowser, OnCompleted, OnFaulted);
                        }
                    }
                    else
                    {
                        var allBrowsers = new SHDocVw.ShellWindows();
                        switch (BrowserType)
                        {
                            case BrowserTypes.IE:
                                {
                                    var selStr = Selector.Get(context);
                                    UiElement element = UiElement.FromSelector(selStr);
                                    IntPtr handle = IntPtr.Zero;
                                    if (element != null)
                                    {
                                        handle = element.WindowHandle;
                                    }

                                    mshtml.IHTMLDocument2 currDoc = null;
                                    SHDocVw.InternetExplorer ieBrowser = GetIEFromHWndClass.GetIEFromHWnd((int)handle, out currDoc);
                                    Browser thisBrowser = new Browser();
                                    thisBrowser.SetIEBrowser(ieBrowser);
                                    if (UiBrowser != null)
                                    {
                                        UiBrowser.Set(context, thisBrowser);
                                    }

                                    ArrayList list = new ArrayList();
                                    var allShellWindows = new SHDocVw.ShellWindows();
                                    foreach (SHDocVw.InternetExplorer browser in allShellWindows)
                                    {
                                        list.Add(browser);
                                    }
                                    CommonVariable.BrowsersList = list;

                                    context.ScheduleAction(Body, thisBrowser);
                                    break;
                                }
                            case BrowserTypes.Chrome:
                                {
                                    break;
                                }
                            case BrowserTypes.Firefox:
                                {
                                    break;
                                }
                            default:
                                break;
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

        private void OnFaulted(NativeActivityFaultContext faultContext, Exception propagatedException, ActivityInstance propagatedFrom)
        {
            //TODO
        }
        private void OnCompleted(NativeActivityContext context, ActivityInstance completedInstance)
        {
            //TODO
        }
    }
}
