using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;
using Plugins.Shared.Library;
using System;
using System.Activities;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;

namespace RPA.UIAutomation.Activities.Browser
{
    /// <summary>
    /// 新建标签页
    /// </summary>
    [Designer(typeof(NewTabDesigner))]
    public sealed class NewTab : AsyncCodeActivity
    {
        static NewTab()
        {
        }

        //插件图标路径
        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Browser/NewTab.png";
            }
        }

        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName16")] //浏览器Browser //Browser Browser //ブラウザブラウザ
        [Localize.LocalizedDescription("Description128")]//该字段仅支持Browser变量 //This field only supports browser variables //このフィールドはBrowser変数のみをサポートします。
        public InArgument<Browser> currBrowser { get; set; }

        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName80")]//新标签地址 //New label address //新しいラベルの住所
        [RequiredArgument]
        public InArgument<string> UrlString { get; set; }
        
        public string ClassName { get { return "NewTab"; } }
        //异步流程返回值
        private delegate string runDelegate();
        private runDelegate m_Delegate;
        public string Run()
        {
            return ClassName;
        }

        //异步流程执行开始函数
        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            Browser thisBrowser = currBrowser.Get(context);
            try
            {
                //先走本活动配置逻辑 其次走流程
                if (thisBrowser == null)
                {
                    //根据OpenBrowser标志获取其抽象化属性
                    PropertyDescriptor property = context.DataContext.GetProperties()[OpenBrowser.OpenBrowsersPropertyTag];
                    if (property == null)
                        //根据AttachBrowser标志获取其抽象化属性
                        property = context.DataContext.GetProperties()[AttachBrowser.OpenBrowsersPropertyTag];
                    if (property == null)
                    {
                        SharedObject.Instance.Output(SharedObject.enOutputType.Error, "活动流程传递的浏览器变量为空，请检查!");
                        m_Delegate = new runDelegate(Run);
                        return m_Delegate.BeginInvoke(callback, state);
                    }
                    //抽象化属性转换为可用Browser属性
                    Browser getBrowser = property.GetValue(context.DataContext) as Browser;
                    IWebDriver driver = getBrowser.getICFBrowser();
                    NewTabFunc(context, driver);
                }
                else
                {
                    if (thisBrowser.getICFBrowser() != null)
                    {
                        IWebDriver driver = thisBrowser.getICFBrowser() as ChromeDriver;
                        NewTabFunc(context, driver);
                    }
                }
            }
            catch(Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "新建标签页失败!", e);
                throw e;
            }


            m_Delegate = new runDelegate(Run);
            return m_Delegate.BeginInvoke(callback, state);
        }

        //异步流程执行结束函数
        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
        }

        //通过运行JS方式新建标签页面
        private void NewTabFunc(AsyncCodeActivityContext context, IWebDriver driver)
        {
            ChromeDriver chromeDriver = driver as ChromeDriver;
            InternetExplorerDriver ieDriver = driver as InternetExplorerDriver;
            FirefoxDriver fxDriver = driver as FirefoxDriver;
            string jsCode = "window.open('" + UrlString.Get(context) + "');";
            string buff = null;
            if (chromeDriver != null)
                chromeDriver.ExecuteScript(jsCode, buff);
            else if (ieDriver != null)
                ieDriver.ExecuteScript(jsCode, buff);
            else if (fxDriver != null)
                fxDriver.ExecuteScript(jsCode, buff);

            bool flag = false;
            string currHandle = driver.CurrentWindowHandle;
            //切换到新标签页
            foreach (String handle in driver.WindowHandles)
            {
                driver.SwitchTo().Window(handle);
                string[] str1 = Regex.Split(driver.Url, "//", RegexOptions.IgnoreCase);
                string[] str2 = Regex.Split(UrlString.Get(context), "//", RegexOptions.IgnoreCase);
                try
                {
                    if (string.Equals(str1[1].Trim('/'), str2[1].Trim('/')))
                    {
                        flag = true;
                        break;
                    }
                }catch
                {
                    return;
                }
            }
            if(!flag)
                driver.SwitchTo().Window(currHandle);
        }
    }
}
