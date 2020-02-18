using Plugins.Shared.Library;
using System;
using System.Activities;
using System.ComponentModel;
using System.Threading;

namespace RPA.UIAutomation.Activities.Browser
{
    [Designer(typeof(NavigateToDesigner))]
    public sealed class NavigateTo : AsyncCodeActivity
    {
        static NavigateTo()
        {
        }

        private string classID = Guid.NewGuid().ToString("N");
        [Localize.LocalizedCategory("Category5")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName1")] //错误执行 //Error execution //エラー実行
        [Localize.LocalizedDescription("Description1")] //指定即使活动引发错误，自动化是否仍应继续 //Specifies whether automation should continue even if the activity raises an error //アクティビティでエラーが発生した場合でも自動化を続行するかどうかを指定します
        public InArgument<bool> ContinueOnError { get; set; }


        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName16")] //浏览器Browser //Browser Browser //ブラウザブラウザ
        [Localize.LocalizedDescription("Description24")] //要导航的浏览器页面。该字段仅支持Browser变量 //The browser page to navigate.  This field only supports the Browser variable. //ナビゲートするブラウザーページ。 このフィールドは、ブラウザ変数のみをサポートします。
        public InArgument<Browser> currBrowser { get; set; }

        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [Browsable(true)]
        [RequiredArgument]
        [Localize.LocalizedDisplayName("DisplayName22")] //导航网址 //Navigation URL //ナビゲーションURL
        [Localize.LocalizedDescription("Description25")] //要显示的网页的位置 //The location of the page to display //表示するページの場所
        public InArgument<string> NevaUrl { get; set; }


        [Browsable(false)]
        public string guid { get { return classID; } }
        [Browsable(false)]
        public string SourceImgPath { get; set; }
        [Browsable(false)]
        public string ClassName { get { return "NavigateTo"; } }


        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Browser/NavigateTo.png";
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
            string url = NevaUrl.Get(context);
            Browser thisBrowser = currBrowser.Get(context);
            //先走 其次走流程
            if (thisBrowser == null)
            {
                PropertyDescriptor property = context.DataContext.GetProperties()[OpenBrowser.OpenBrowsersPropertyTag];
                if (property == null)
                    property = context.DataContext.GetProperties()[AttachBrowser.OpenBrowsersPropertyTag];
                if(property ==null)
                {
                    SharedObject.Instance.Output(SharedObject.enOutputType.Error, "活动流程传递的浏览器变量为空，请检查！");
                    m_Delegate = new runDelegate(Run);
                    return m_Delegate.BeginInvoke(callback, state);
                }
                Browser getBrowser = property.GetValue(context.DataContext) as Browser;
                if (getBrowser.getICFBrowser() != null)
                {
                    getBrowser.getICFBrowser().Navigate().GoToUrl(url);
                }
                //else if (getBrowser.getIEBrowser() != null)
                //{
                //    getBrowser.getIEBrowser().Navigate(url);
                //}
                ////确保页面是否已加载完成
                //while (getBrowser.getIEBrowser().ReadyState != SHDocVw.tagREADYSTATE.READYSTATE_COMPLETE)
                //{
                //    Thread.Sleep(500);
                //}
            }
            else
            {
                if (thisBrowser.getICFBrowser() != null)
                {
                    thisBrowser.getICFBrowser().Navigate().GoToUrl(url);
                }
                //else if (thisBrowser.getIEBrowser() != null)
                //{
                //    thisBrowser.getIEBrowser().Navigate(url);
                //}
                ////确保页面是否已加载完成
                //while (thisBrowser.getIEBrowser().ReadyState != SHDocVw.tagREADYSTATE.READYSTATE_COMPLETE)
                //{
                //    Thread.Sleep(500);
                //}
            }


            m_Delegate = new runDelegate(Run);
            return m_Delegate.BeginInvoke(callback, state);
        }
        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
            
        }
    }
}
