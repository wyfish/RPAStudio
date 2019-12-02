using System;
using System.Activities;
using System.ComponentModel;
using Plugins.Shared.Library;
using System.Threading;
using System.Activities.Statements;
using System.Diagnostics;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System.Collections;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;
using System.Collections.Generic;

namespace RPA.UIAutomation.Activities.Browser
{
    [Designer(typeof(OpenBrowserDesigner))]
    public class OpenBrowser : NativeActivity
    {

        [Browsable(false)]
        public ActivityAction<object> Body { get; set; }

        [Category("选项")]
        [DisplayName("错误执行")]
        [Description("指定即使活动引发错误，自动化是否仍应继续")]
        public InArgument<bool> ContinueOnError { get; set; }

        [Category("输入")]
        [Browsable(true)]
        [DisplayName("浏览器类型")]
        [Description("选择要使用的浏览器类型。可以使用以下选项：IE，Firefox，Chrome")]
        public BrowserTypes BrowserType
        {
            get;
            set;
        }


        [Category("输入")]
        [Browsable(true)]
        [RequiredArgument]
        [DisplayName("网址URL")]
        [Description("要在指定浏览器中打开的URL")]
        public InArgument<string> Url
        {
            get;
            set;
        }

        /*为等待浏览器页面显示，如果未显示无法获得对应句柄或InternetExplorer变量*/
        InArgument<Int32> _OverTime = 10 * 1000;
        [Category("选项")]
        [DisplayName("超时时间")]
        [Description("指定浏览器响应超时时间(毫秒)")]
        public InArgument<Int32> OverTime
        {
            get
            {
                return _OverTime;
            }
            set
            {
                _OverTime = value;
            }
        }


        [Category("选项")]
        [Browsable(true)]
        [DisplayName("私密/无痕")]
        public bool Private
        {
            get;
            set;
        }


        //[Category("选项")]
        //[Browsable(true)]
        //[DisplayName("新会话")]
        //public bool NewSession
        //{
        //    get;
        //    set;
        //}


        [Category("选项")]
        [Browsable(true)]
        [DisplayName("隐藏")]
        public bool Hidden
        {
            get;
            set;
        }

        [Category("输出")]
        [Browsable(true)]
        [DisplayName("浏览器")]
        [Description("存储浏览器的变量。该字段仅接受Browser变量")]
        public OutArgument<Browser> Browser
        {
            get;
            set;
        }


        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Browser/OpenBrowser.png";
            }
        }

        public static string OpenBrowsersPropertyTag { get { return "OpenBrowser"; } }


        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);
        }

        public OpenBrowser()
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

        //protected override void Execute(NativeActivityContext context)
        //{
        //    InternetExplorerOptions op = new InternetExplorerOptions();
        //    op.PageLoadStrategy = PageLoadStrategy.Normal;
        //    op.IgnoreZoomLevel = true;
        //    op.InitialBrowserUrl = "http://www.baidu.com";
        //    //op.ForceCreateProcessApi = true;
        //    op.BrowserCommandLineArguments = "-private";
        //    IWebDriver driver = new InternetExplorerDriver(@"E:\GitSource\RPAPlatform\RPAStudio\packages\Selenium.WebDriver.IEDriver.3.150.1\driver", op);
        //}
        
        ChromeDriver chromeDriver;
        protected override void Execute(NativeActivityContext context)
        {
            string url = Url.Get(context);
            SHDocVw.InternetExplorer IEBrowser = null;
            IWebDriver CFBrowser = null;
            try
            {
                switch (BrowserType)
                {
                    case BrowserTypes.Chrome:
                        {
                            ChromeOptions option = new ChromeOptions();
                            ChromeDriverService service = ChromeDriverService.CreateDefaultService(@"Selenium.WebDriver");
                            service.SuppressInitialDiagnosticInformation = true;
                            service.HideCommandPromptWindow = true;
                            if (Hidden)
                                option.AddArgument("--headless");
                            if (Private)
                                option.AddArgument("--incognito");
                            try
                            {
                                option.AddArgument("--no-sandbox");
                                option.AddArgument("--disable-gpu");
                                option.AddArgument("--log-level=3");
                                option.AddArgument("--start-maximized");
                                //option.AddArguments("allow-running-insecure-content", "--test-type");

                                option.AddArgument("--disable-plugins");
                                option.AddArgument("--remote-debugging-port=9222");
                                //option.AddArgument("--user-data-dir=" + Environment.GetEnvironmentVariable("USERPROFILE") + "/AppData/Local/Google/Chrome/User Data/Default");
                                chromeDriver = new ChromeDriver(service, option);
                                chromeDriver.Navigate().GoToUrl(url);
                            }
                            catch (InvalidOperationException e)
                            {
                                SharedObject.Instance.Output(SharedObject.enOutputType.Error,
                                           "请检查已安装Chrome版本(支持Chrome Version >76.0.0  或  请先关闭RPA已打开的chrome窗口)", e);
                                return;
                            }
                            catch (Exception e)
                            {
                                SharedObject.Instance.Output(SharedObject.enOutputType.Error,
                                         "打开Chrome浏览器出错: ", e.Message);
                                return;
                            }
                            CFBrowser = chromeDriver;
                            break;
                        }
                    case BrowserTypes.Firefox:
                        {
                            FirefoxDriverService service = FirefoxDriverService.CreateDefaultService(@"Selenium.WebDriver");
                            service.SuppressInitialDiagnosticInformation = true;
                            service.HideCommandPromptWindow = true;

                            FirefoxOptions option = new FirefoxOptions();
                            if (Hidden)
                                option.AddArgument("-headless");
                            if (Private)
                                option.AddArgument("-private");
                            CFBrowser = new FirefoxDriver(service, option);
                            CFBrowser.Navigate().GoToUrl(url);
                            break;
                        }
                    case BrowserTypes.IE:
                        {
                            /******/
                            /*首次启动IE 为主进程可获取句柄，而后打开的IE则为子进程，需分别判断*/
                            /******/
                            var Browsers = new SHDocVw.ShellWindows();
                            int ieCounts = Browsers.Count;
                            ArrayList preHwnds = new ArrayList();
                            foreach (SHDocVw.InternetExplorer browser in Browsers)
                            {
                                preHwnds.Add(browser.HWND);
                            }


                            Process proc;
                            ProcessStartInfo procInfo = new ProcessStartInfo();
                            //procInfo.WindowStyle = ProcessWindowStyle.Hidden;
                            //procInfo.CreateNoWindow = true;
                            procInfo.UseShellExecute = true;
                            procInfo.FileName = "iexplore.exe";
                            if (Private)
                                procInfo.Arguments = "-private " + url;
                            else
                                procInfo.Arguments = url;
                            proc = Process.Start(procInfo);

                            //首次打开IE
                            if (ieCounts == 0)
                            {
                                try
                                {
                                    int handle;
                                    int overTime = OverTime.Get(context);
                                    int Times = overTime / 1000;

                                    handle = proc.MainWindowHandle.ToInt32();
                                    if (handle == 0)
                                    {
                                        for (int i = 0; i < Times; i++)
                                        {
                                            Thread.Sleep(1000);
                                            handle = proc.MainWindowHandle.ToInt32();
                                            if (handle != 0)
                                                break;
                                        }
                                    }
                                    if (handle == 0)
                                    {
                                        SharedObject.Instance.Output(SharedObject.enOutputType.Error, "浏览器等待响应时间过长");
                                        return;
                                    }

                                    while (true)
                                    {
                                        var allBrowsers = new SHDocVw.ShellWindows();
                                        if (allBrowsers.Count != 0)
                                        {
                                            foreach (SHDocVw.InternetExplorer browser in allBrowsers)
                                            {
                                                if (browser.HWND == handle)
                                                {
                                                    IEBrowser = browser;
                                                    break;
                                                }
                                            }
                                            break;
                                        }
                                        Thread.Sleep(200);
                                    }
                                }
                                catch (Exception e)
                                {
                                    SharedObject.Instance.Output(SharedObject.enOutputType.Error, "首次打开IE浏览器失败:" + e.Message);
                                    return;
                                }
                            }
                            //已存在IE浏览器的情况下打开IE
                            else
                            {
                                int overTime = OverTime.Get(context);
                                int Times = overTime / 1000;
                                ArrayList afterHwnds = new ArrayList();
                                int childHwnd = 0;
                                for (int i = 0; i < Times; i++)
                                {
                                    var getAllBrowsers = new SHDocVw.ShellWindows();
                                    foreach (SHDocVw.InternetExplorer browser in getAllBrowsers)
                                    {
                                        afterHwnds.Add(browser.HWND);
                                    }
                                    for (int j = 0; j < afterHwnds.Count; j++)
                                    {
                                        if (!preHwnds.Contains(afterHwnds[j]))
                                        {
                                            childHwnd = Convert.ToInt32(afterHwnds[j]);
                                        }
                                    }
                                    if (childHwnd != 0)
                                        break;
                                    Thread.Sleep(1000);
                                }

                                while (true)
                                {
                                    var allBrowsers = new SHDocVw.ShellWindows();
                                    if (allBrowsers.Count != 0)
                                    {
                                        foreach (SHDocVw.InternetExplorer browser in allBrowsers)
                                        {
                                            if (browser.HWND == childHwnd)
                                            {
                                                IEBrowser = browser;
                                                break;
                                            }
                                        }
                                        break;
                                    }
                                    Thread.Sleep(200);
                                }
                            }

                            //确保页面是否已加载完成
                            while (IEBrowser.ReadyState != SHDocVw.tagREADYSTATE.READYSTATE_COMPLETE)
                            {
                                Thread.Sleep(500);
                            }

                            if (Hidden)
                                IEBrowser.Visible = false;
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
            catch (System.ComponentModel.Win32Exception)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "未安装相应浏览器程序:" + BrowserType);
                if (ContinueOnError.Get(context))
                {
                }
                else
                {
                    throw;
                }
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "运行浏览器出现错误: ", e.Message);
                if (ContinueOnError.Get(context))
                {
                }
                else
                {
                    throw;
                }
            }

            Browser thisBrowser = new Browser();
            thisBrowser.SetCFBrowser(CFBrowser);
            thisBrowser.SetIEBrowser(IEBrowser);
            if (Browser != null)
                Browser.Set(context, thisBrowser);


            ArrayList list = new ArrayList();
            var allShellWindows = new SHDocVw.ShellWindows();
            foreach (SHDocVw.InternetExplorer browser in allShellWindows)
            {
                list.Add(browser);
            }
            CommonVariable.BrowsersList = list;

            if (Body != null)
            {
                context.ScheduleAction(Body, thisBrowser, OnCompleted, OnFaulted);
            }
            else
            {
                if (Hidden)
                    chromeDriver.Quit();
                GC.Collect();
            }
            //GC.Collect();
        }

        private void OnFaulted(NativeActivityFaultContext faultContext, Exception propagatedException, ActivityInstance propagatedFrom)
        {
            //chromeDriver.Quit();
            GC.Collect();
        }
        private void OnCompleted(NativeActivityContext context, ActivityInstance completedInstance)
        {
            if (Hidden)
                chromeDriver.Quit();
            GC.Collect();
        }
    }
}
