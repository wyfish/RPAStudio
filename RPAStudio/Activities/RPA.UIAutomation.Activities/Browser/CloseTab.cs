using Plugins.Shared.Library;
using System;
using System.Activities;
using System.ComponentModel;

namespace RPA.UIAutomation.Activities.Browser
{
    [Designer(typeof(CloseTabDesigner))]
    public sealed class CloseTab : AsyncCodeActivity
    {
        private string classID = Guid.NewGuid().ToString("N");
        [Localize.LocalizedCategory("Category5")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName1")] //错误执行 //Error execution //エラー実行
        [Localize.LocalizedDescription("Description1")] //指定即使活动引发错误，自动化是否仍应继续 //Specifies whether automation should continue even if the activity raises an error //アクティビティでエラーが発生した場合でも自動化を続行するかどうかを指定します
        public InArgument<bool> ContinueOnError { get; set; }

        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName16")] //浏览器Browser //Browser Browser //ブラウザブラウザ
        [Localize.LocalizedDescription("Description18")] //要关闭的浏览器页面。该字段仅支持Browser变量 //The browser page to close.  This field only supports the Browser variable. //閉じるブラウザーページ。 このフィールドは、ブラウザ変数のみをサポートします。
        public InArgument<Browser> currBrowser { get; set; }

        [Browsable(false)]
        public string guid { get { return classID; } }
        [Browsable(false)]
        public string SourceImgPath { get; set; }
        [Browsable(false)]
        public string ClassName { get { return "CloseTab"; } }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Window/WindowClose.png";
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

        static CloseTab()
        {
        }

        private delegate string runDelegate();
        private runDelegate m_Delegate;
        public string Run()
        {
            return ClassName;
        }

        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            //先走本地Browser逻辑 其次走流程
            if (currBrowser.Get(context) == null)
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
                if (getBrowser.getICFBrowser() != null)
                {
                    getBrowser.getICFBrowser().Close();
                }
            }
            else                                                                                        
            {
                Browser thisBrowser = currBrowser.Get(context);
                if (thisBrowser.getICFBrowser() != null)
                {
                    thisBrowser.getICFBrowser().Close();
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
