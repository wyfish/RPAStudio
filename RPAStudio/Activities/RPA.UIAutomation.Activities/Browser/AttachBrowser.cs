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


        [Localize.LocalizedCategory("Category1")] //公共 //Public //一般公開
        [Localize.LocalizedDisplayName("DisplayName1")] //错误执行 //Error execution //エラー実行
        [Localize.LocalizedDescription("Description1")] //指定即使活动引发错误，自动化是否仍应继续 //Specifies whether automation should continue even if the activity raises an error //アクティビティでエラーが発生した場合でも自動化を続行するかどうかを指定します
        public InArgument<bool> ContinueOnError { get; set; }

        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [OverloadGroup("G1")]
        [RequiredArgument]
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName11")] //浏览器 //Browser //ブラウザ
        [Localize.LocalizedDescription("Description14")] //要附加的现有浏览器变量 //Existing browser variables to attach //添付する既存のブラウザ変数
        public InArgument<Browser> currBrowser { get; set; }

        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [Localize.LocalizedDisplayName("DisplayName12")] //超时时间 //overtime time //タイムアウト
        [Localize.LocalizedDescription("Description15")] //指定在引发错误之前等待活动运行的时间量（以毫秒为单位）。默认值为30000毫秒（30秒） //Specifies the amount of time, in milliseconds, to wait for an activity to run before an error is raised.  The default is 30000 milliseconds (30 seconds) //エラーが発生する前にアクティビティの実行を待機する時間をミリ秒単位で指定します。 デフォルトは30000ミリ秒（30秒）です
        public InArgument<Int32> TimeoutMS { get; set; }

        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [OverloadGroup("G2")]
        [RequiredArgument]
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName13")] //UI选择器 //UI selector //UIセレクター
        [Localize.LocalizedDescription("Description2")] //用于在执行活动时查找特定UI元素的Text属性 //The Text property used to find specific UI elements when performing activities //アクティビティの実行時に特定のUI要素を見つけるために使用されるTextプロパティ
        public InArgument<string> Selector { get; set; }

        [Localize.LocalizedCategory("Category4")] //输出 //Output //出力
        [Localize.LocalizedDisplayName("DisplayName14")] //UI浏览器 //UI browser //UIブラウザー
        [Localize.LocalizedDescription("Description16")] //活动返回的Browser变量 //The Browser variable returned by the activity //アクティビティによって返されるブラウザ変数
        [Browsable(true)]
        public OutArgument<Browser> UiBrowser
        {
            get;
            set;
        }

        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName15")] //浏览器类型 //Browser type //ブラウザの種類
        [Localize.LocalizedDescription("Description17")] //选择要使用的浏览器类型。可以使用以下选项：IE，Firefox，Chrome //Choose the type of browser you want to use.  The following options are available: IE, Firefox, Chrome //使用するブラウザの種類を選択します。 次のオプションが利用可能です：IE、Firefox、Chrome
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
