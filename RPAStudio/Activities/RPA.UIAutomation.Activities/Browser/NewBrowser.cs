using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using Plugins.Shared.Library;
using System;
using System.Activities;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

namespace RPA.UIAutomation.Activities.Browser
{
    [Designer(typeof(NewBrowserDesigner))]
    public sealed class NewBrowser : AsyncCodeActivity
    {
        static NewBrowser()
        {
        }


        [Localize.LocalizedCategory("Category4")] //输出 //Output //出力
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName16")] //浏览器Browser //Browser Browser //ブラウザブラウザ
        [Localize.LocalizedDescription("Description26")] //新浏览器标签页面。该字段仅支持Browser变量 //New browser tab page.  This field only supports the Browser variable. //新しいブラウザタブページ。 このフィールドは、ブラウザ変数のみをサポートします。
        public OutArgument<Browser> newBrowser { get; set; }



        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Browser/OpenBrowser.png";
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

        public string ClassName { get { return "NewBrowser"; } }


        private delegate string runDelegate();
        private runDelegate m_Delegate;
        public string Run()
        {
            return ClassName;
        }
        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            Browser browser = new Browser();

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

            String winHandleBefore = getBrowser.getCFBrowser().CurrentWindowHandle;
            if (getBrowser.getCFBrowser() != null)
            {
                //ReadOnlyCollection<string> chromeHandles =  getBrowser.getCFBrowser().WindowHandles;
                //string chromeHandle = chromeHandles[chromeHandles.Count - 1];
                //ChromeDriverService service = ChromeDriverService.CreateDefaultService(@"packages\Selenium.Chrome.WebDriver.76.0.0\driver");
                //ChromeDriver chromeDriver = getBrowser.getCFBrowser() as ChromeDriver;
                //chromeDriver.SwitchTo().Window(chromeHandle);
                //ReadOnlyDesiredCapabilities sss;


                //browser.SetCFBrowser(chromeDriver);
                //getBrowser.getCFBrowser().SwitchTo().Window(chromeHandle);
                //getBrowser.getCFBrowser().Navigate().GoToUrl("http://www.baidu.com");
            }
            else if(getBrowser.getIEBrowser() != null)
            {
                var allShellWindows = new SHDocVw.ShellWindows();
                foreach (SHDocVw.InternetExplorer ieBrowser in allShellWindows)
                {
                    if (!CommonVariable.BrowsersList.Contains(ieBrowser))
                    {
                        browser.SetIEBrowser(ieBrowser);
                    }
                }
            }
            ArrayList list = new ArrayList();
            var ShellWindows = new SHDocVw.ShellWindows();
            foreach (SHDocVw.InternetExplorer isBrowser in ShellWindows)
            {
                list.Add(isBrowser);
            }
            CommonVariable.BrowsersList = list;
            newBrowser.Set(context, browser);

            m_Delegate = new runDelegate(Run);
            return m_Delegate.BeginInvoke(callback, state);
        }
        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
        }
    }
}
