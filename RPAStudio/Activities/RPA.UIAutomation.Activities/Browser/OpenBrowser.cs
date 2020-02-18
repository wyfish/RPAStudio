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
using Microsoft.Win32;

namespace RPA.UIAutomation.Activities.Browser
{
    [Designer(typeof(OpenBrowserDesigner))]
    public class OpenBrowser : NativeActivity
    {

        [Browsable(false)]
        public ActivityAction<object> Body { get; set; }

        [Localize.LocalizedCategory("Category5")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName1")] //错误执行 //Error execution //エラー実行
        [Localize.LocalizedDescription("Description1")] //指定即使活动引发错误，自动化是否仍应继续 //Specifies whether automation should continue even if the activity raises an error //アクティビティでエラーが発生した場合でも自動化を続行するかどうかを指定します
        public InArgument<bool> ContinueOnError { get; set; }

        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName15")] //浏览器类型 //Browser type //ブラウザの種類
        [Localize.LocalizedDescription("Description17")] //选择要使用的浏览器类型。可以使用以下选项：IE，Firefox，Chrome //Choose the type of browser you want to use.  The following options are available: IE, Firefox, Chrome //使用するブラウザの種類を選択します。 次のオプションが利用可能です：IE、Firefox、Chrome
        public BrowserTypes BrowserType
        {
            get;
            set;
        }


        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [Browsable(true)]
        [RequiredArgument]
        [Localize.LocalizedDisplayName("DisplayName23")] //网址URL //URL //URL
        [Localize.LocalizedDescription("Description27")] //要在指定浏览器中打开的URL //The URL to open in the specified browser //指定されたブラウザーで開くURL
        public InArgument<string> Url
        {
            get;
            set;
        }

        /*为等待浏览器页面显示，如果未显示无法获得对应句柄或InternetExplorer变量*/
        InArgument<Int32> _OverTime = 10 * 1000;
        [Localize.LocalizedCategory("Category5")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName12")] //超时时间 //overtime time //タイムアウト
        [Localize.LocalizedDescription("Description28")] //指定浏览器响应超时时间(毫秒) //Specify browser response timeout (ms) //ブラウザーの応答タイムアウト（ミリ秒）を指定する
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


        [Localize.LocalizedCategory("Category5")] //选项 //Option //オプション
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName24")] //私密/无痕 //Private/no trace //プライベート/トレースなし
        public bool Private
        {
            get;
            set;
        }


        //[Localize.LocalizedCategory("Category5")] //选项 //Option //オプション
        //[Browsable(true)]
        //[DisplayName("新会话")]
        //public bool NewSession
        //{
        //    get;
        //    set;
        //}


        [Localize.LocalizedCategory("Category5")] //选项 //Option //オプション
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName25")] //隐藏 //Hide //隠す
        public bool Hidden
        {
            get;
            set;
        }

        [Localize.LocalizedCategory("Category4")] //输出 //Output //出力
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName11")] //浏览器 //Browser //ブラウザ
        [Localize.LocalizedDescription("Description29")] //存储浏览器的变量。该字段仅接受Browser变量 //Store the variables of the browser.  This field only accepts the Browser variable //ブラウザの変数を保存します。 このフィールドはブラウザ変数のみを受け入れます
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
            IWebDriver iCFBrowser = null;
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
                                           "请检查已安装Chrome版本(支持Chrome Version >78.0.0  或  请先关闭RPA已打开的chrome窗口)", e);
                                return;
                            }
                            iCFBrowser = chromeDriver;
                            break;
                        }
                    case BrowserTypes.Firefox:
                        {
                            try
                            {
                                FirefoxDriverService service = FirefoxDriverService.CreateDefaultService(@"Selenium.WebDriver");
                                service.SuppressInitialDiagnosticInformation = true;
                                service.HideCommandPromptWindow = true;

                                FirefoxOptions option = new FirefoxOptions();
                                if (Hidden)
                                    option.AddArgument("-headless");
                                if (Private)
                                    option.AddArgument("-private");
                                iCFBrowser = new FirefoxDriver(service, option);
                                iCFBrowser.Navigate().GoToUrl(url);
                            }
                            catch(InvalidOperationException e)
                            {
                                SharedObject.Instance.Output(SharedObject.enOutputType.Error,
                                    "请检查是否已安装FireFox浏览器并在环境变量中配置正确安装路径", e);
                                return;
                            }
                            break;
                        }
                    case BrowserTypes.IE:
                        {
                            InternetExplorerDriverService service = InternetExplorerDriverService.CreateDefaultService(@"Selenium.WebDriver");
                            service.HideCommandPromptWindow = true;
                            service.SuppressInitialDiagnosticInformation = true;
                            /**IE暂不支持headless模式**/
                            try
                            {
                                InternetExplorerOptions op = new InternetExplorerOptions();
                                op.ForceCreateProcessApi = true;
                                op.PageLoadStrategy = PageLoadStrategy.Normal;
                                op.IgnoreZoomLevel = true;
                                op.RequireWindowFocus = false;
                                op.InitialBrowserUrl = url;
                                if (Private)
                                    op.BrowserCommandLineArguments = "-private";

                                iCFBrowser = new InternetExplorerDriver(service, op);
                            }
                            catch(System.InvalidOperationException e)
                            {
                                RegeditForIETabProcGrowth();
                                InternetExplorerOptions op = new InternetExplorerOptions();
                                op.ForceCreateProcessApi = true;
                                op.PageLoadStrategy = PageLoadStrategy.Normal;
                                op.RequireWindowFocus = false;
                                op.IgnoreZoomLevel = true;
                                op.InitialBrowserUrl = url;
                                if (Private)
                                    op.BrowserCommandLineArguments = "-private";
                                iCFBrowser = new InternetExplorerDriver(service, op);
                            }
                            //iCFBrowser.Close();
                            //iCFBrowser.Quit();
                            //service.Dispose();
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
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "打开浏览器出现错误: ", e.Message);
                if (ContinueOnError.Get(context))
                {
                }
                else
                {
                    throw;
                }
            }

            Browser thisBrowser = new Browser();
            thisBrowser.SetICFBrowser(iCFBrowser);
            if (Browser != null)
                Browser.Set(context, thisBrowser);


            //ArrayList list = new ArrayList();
            //var allShellWindows = new SHDocVw.ShellWindows();
            //foreach (SHDocVw.InternetExplorer browser in allShellWindows)
            //{
            //    list.Add(browser);
            //}
            //CommonVariable.BrowsersList = list;

            if (Body != null)
            {
                context.ScheduleAction(Body, thisBrowser, OnCompleted, OnFaulted);
            }
            else
            {
                GC.Collect();
            }
        }


        //protected override void Execute(NativeActivityContext context)
        //{
        //    string url = Url.Get(context);
        //    SHDocVw.InternetExplorer IEBrowser = null;
        //    IWebDriver CFBrowser = null;
        //    try
        //    {
        //        switch (BrowserType)
        //        {
        //            case BrowserTypes.Chrome:
        //                {
        //                    ChromeOptions option = new ChromeOptions();
        //                    ChromeDriverService service = ChromeDriverService.CreateDefaultService(@"Selenium.WebDriver");
        //                    service.SuppressInitialDiagnosticInformation = true;
        //                    service.HideCommandPromptWindow = true;
        //                    if (Hidden)
        //                        option.AddArgument("--headless");
        //                    if (Private)
        //                        option.AddArgument("--incognito");
        //                    try
        //                    {
        //                        option.AddArgument("--no-sandbox");
        //                        option.AddArgument("--disable-gpu");
        //                        option.AddArgument("--log-level=3");
        //                        option.AddArgument("--start-maximized");
        //                        //option.AddArguments("allow-running-insecure-content", "--test-type");

        //                        option.AddArgument("--disable-plugins");
        //                        option.AddArgument("--remote-debugging-port=9222");
        //                        //option.AddArgument("--user-data-dir=" + Environment.GetEnvironmentVariable("USERPROFILE") + "/AppData/Local/Google/Chrome/User Data/Default");
        //                        chromeDriver = new ChromeDriver(service, option);
        //                        chromeDriver.Navigate().GoToUrl(url);
        //                    }
        //                    catch (InvalidOperationException e)
        //                    {
        //                        SharedObject.Instance.Output(SharedObject.enOutputType.Error,
        //                                   "请检查已安装Chrome版本(支持Chrome Version >76.0.0  或  请先关闭RPA已打开的chrome窗口)", e);
        //                        return;
        //                    }
        //                    catch (Exception e)
        //                    {
        //                        SharedObject.Instance.Output(SharedObject.enOutputType.Error,
        //                                 "打开Chrome浏览器出错: ", e.Message);
        //                        return;
        //                    }
        //                    CFBrowser = chromeDriver;
        //                    break;
        //                }
        //            case BrowserTypes.Firefox:
        //                {
        //                    FirefoxDriverService service = FirefoxDriverService.CreateDefaultService(@"Selenium.WebDriver");
        //                    service.SuppressInitialDiagnosticInformation = true;
        //                    service.HideCommandPromptWindow = true;

        //                    FirefoxOptions option = new FirefoxOptions();
        //                    if (Hidden)
        //                        option.AddArgument("-headless");
        //                    if (Private)
        //                        option.AddArgument("-private");
        //                    CFBrowser = new FirefoxDriver(service, option);
        //                    CFBrowser.Navigate().GoToUrl(url);
        //                    break;
        //                }
        //            case BrowserTypes.IE:
        //                {
        //                    /******/
        //                    /*首次启动IE 为主进程可获取句柄，而后打开的IE则为子进程，需分别判断*/
        //                    /******/
        //                    var Browsers = new SHDocVw.ShellWindows();
        //                    int ieCounts = Browsers.Count;
        //                    ArrayList preHwnds = new ArrayList();
        //                    foreach (SHDocVw.InternetExplorer browser in Browsers)
        //                    {
        //                        preHwnds.Add(browser.HWND);
        //                    }


        //                    Process proc;
        //                    ProcessStartInfo procInfo = new ProcessStartInfo();
        //                    //procInfo.WindowStyle = ProcessWindowStyle.Hidden;
        //                    //procInfo.CreateNoWindow = true;
        //                    procInfo.UseShellExecute = true;
        //                    procInfo.FileName = "iexplore.exe";
        //                    if (Private)
        //                        procInfo.Arguments = "-private " + url;
        //                    else
        //                        procInfo.Arguments = url;
        //                    proc = Process.Start(procInfo);

        //                    //首次打开IE
        //                    if (ieCounts == 0)
        //                    {
        //                        try
        //                        {
        //                            int handle;
        //                            int overTime = OverTime.Get(context);
        //                            int Times = overTime / 1000;

        //                            handle = proc.MainWindowHandle.ToInt32();
        //                            if (handle == 0)
        //                            {
        //                                for (int i = 0; i < Times; i++)
        //                                {
        //                                    Thread.Sleep(1000);
        //                                    handle = proc.MainWindowHandle.ToInt32();
        //                                    if (handle != 0)
        //                                        break;
        //                                }
        //                            }
        //                            if (handle == 0)
        //                            {
        //                                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "浏览器等待响应时间过长");
        //                                return;
        //                            }

        //                            while (true)
        //                            {
        //                                var allBrowsers = new SHDocVw.ShellWindows();
        //                                if (allBrowsers.Count != 0)
        //                                {
        //                                    foreach (SHDocVw.InternetExplorer browser in allBrowsers)
        //                                    {
        //                                        if (browser.HWND == handle)
        //                                        {
        //                                            IEBrowser = browser;
        //                                            break;
        //                                        }
        //                                    }
        //                                    break;
        //                                }
        //                                Thread.Sleep(200);
        //                            }
        //                        }
        //                        catch (Exception e)
        //                        {
        //                            SharedObject.Instance.Output(SharedObject.enOutputType.Error, "首次打开IE浏览器失败:" + e.Message);
        //                            return;
        //                        }
        //                    }
        //                    //已存在IE浏览器的情况下打开IE
        //                    else
        //                    {
        //                        int overTime = OverTime.Get(context);
        //                        int Times = overTime / 1000;
        //                        ArrayList afterHwnds = new ArrayList();
        //                        int childHwnd = 0;
        //                        for (int i = 0; i < Times; i++)
        //                        {
        //                            var getAllBrowsers = new SHDocVw.ShellWindows();
        //                            foreach (SHDocVw.InternetExplorer browser in getAllBrowsers)
        //                            {
        //                                afterHwnds.Add(browser.HWND);
        //                            }
        //                            for (int j = 0; j < afterHwnds.Count; j++)
        //                            {
        //                                if (!preHwnds.Contains(afterHwnds[j]))
        //                                {
        //                                    childHwnd = Convert.ToInt32(afterHwnds[j]);
        //                                }
        //                            }
        //                            if (childHwnd != 0)
        //                                break;
        //                            Thread.Sleep(1000);
        //                        }

        //                        while (true)
        //                        {
        //                            var allBrowsers = new SHDocVw.ShellWindows();
        //                            if (allBrowsers.Count != 0)
        //                            {
        //                                foreach (SHDocVw.InternetExplorer browser in allBrowsers)
        //                                {
        //                                    if (browser.HWND == childHwnd)
        //                                    {
        //                                        IEBrowser = browser;
        //                                        break;
        //                                    }
        //                                }
        //                                break;
        //                            }
        //                            Thread.Sleep(200);
        //                        }
        //                    }

        //                    //确保页面是否已加载完成
        //                    while (IEBrowser.ReadyState != SHDocVw.tagREADYSTATE.READYSTATE_COMPLETE)
        //                    {
        //                        Thread.Sleep(500);
        //                    }

        //                    if (Hidden)
        //                        IEBrowser.Visible = false;
        //                    break;
        //                }
        //            default:
        //                {
        //                    break;
        //                }
        //        }
        //    }
        //    catch (System.ComponentModel.Win32Exception)
        //    {
        //        SharedObject.Instance.Output(SharedObject.enOutputType.Error, "未安装相应浏览器程序:" + BrowserType);
        //        if (ContinueOnError.Get(context))
        //        {
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        SharedObject.Instance.Output(SharedObject.enOutputType.Error, "运行浏览器出现错误: ", e.Message);
        //        if (ContinueOnError.Get(context))
        //        {
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    Browser thisBrowser = new Browser();
        //    thisBrowser.SetCFBrowser(CFBrowser);
        //    thisBrowser.SetIEBrowser(IEBrowser);
        //    if (Browser != null)
        //        Browser.Set(context, thisBrowser);


        //    ArrayList list = new ArrayList();
        //    var allShellWindows = new SHDocVw.ShellWindows();
        //    foreach (SHDocVw.InternetExplorer browser in allShellWindows)
        //    {
        //        list.Add(browser);
        //    }
        //    CommonVariable.BrowsersList = list;

        //    if (Body != null)
        //    {
        //        context.ScheduleAction(Body, thisBrowser, OnCompleted, OnFaulted);
        //    }
        //    else
        //    {
        //        if (Hidden)
        //            chromeDriver.Quit();
        //        GC.Collect();
        //    }
        //}


        //活动异常结束后续处理
        private void OnFaulted(NativeActivityFaultContext faultContext, Exception propagatedException, ActivityInstance propagatedFrom)
        {
            GC.Collect();
        }
        private void OnCompleted(NativeActivityContext context, ActivityInstance completedInstance)
        {
            GC.Collect();
        }

        //IE出现异常需进行注册表处理
        private void RegeditForIETabProcGrowth()
        {
            RegistryKey hklm = Registry.CurrentUser;
            RegistryKey ieMain = hklm.OpenSubKey("Software\\Microsoft\\Internet Explorer\\Main", true);
            ieMain.SetValue("TabProcGrowth", 0);
        }
    }
}
