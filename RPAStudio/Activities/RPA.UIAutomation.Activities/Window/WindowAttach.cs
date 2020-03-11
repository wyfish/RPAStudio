using System;
using System.Activities;
using System.ComponentModel;
using System.Text;
using System.Runtime.InteropServices;
using System.Activities.Statements;
using Plugins.Shared.Library.UiAutomation;
using Plugins.Shared.Library;

namespace RPA.UIAutomation.Activities.Window
{
    [Designer(typeof(WindowAttachDesigner))]
    public sealed class WindowAttach : NativeActivity
    {
        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "Window Attach"; } }

        [Browsable(false)]
        public ActivityAction<object> Body { get; set; }

        public WindowAttach()
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

        [Localize.LocalizedCategory("Category1")] //公共 //Public //一般公開
        [Localize.LocalizedDisplayName("DisplayName1")] //错误执行 //Error execution //エラー実行
        [Localize.LocalizedDescription("Description1")] //指定即使活动引发错误，自动化是否仍应继续 //Specifies whether automation should continue even if the activity raises an error //アクティビティでエラーが発生した場合でも自動化を続行するかどうかを指定します
        public InArgument<bool> ContinueOnError { get; set; }

        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [RequiredArgument]
        [OverloadGroup("G1")]
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName2")] //窗口指示器 //Window selector //セレクター
        [Localize.LocalizedDescription("Description2")] //用于在执行活动时查找特定UI元素的Text属性 //The Text property used to find specific UI elements when performing activities //アクティビティの実行時に特定のUI要素を見つけるために使用されるTextプロパティ
        public InArgument<string> Selector { get; set; }

        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [RequiredArgument]
        [OverloadGroup("G2")]
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName62")] //窗口 //Window //窓
        [Localize.LocalizedDescription("Description43")] //要关闭的窗口。该字段仅接受Window变量 //The window to close.  This field only accepts Window variables //閉じるウィンドウ。 このフィールドはウィンドウ変数のみを受け入れます
        public InArgument<Window> ActiveWindow { get; set; }

        [Localize.LocalizedCategory("Category4")] //输出 //Output //出力
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName62")] //窗口 //Window //窓
        [Localize.LocalizedDescription("Description77")] //找到的活动窗口。该字段仅支持Window变量。指定Window变量时，将忽略SearchScope和Selector属性。 //The active window found.  This field only supports Window variables.  The SearchScope and Selector properties are ignored when the Window variable is specified. //アクティブなウィンドウが見つかりました。 このフィールドはウィンドウ変数のみをサポートします。  Window変数が指定されている場合、SearchScopeプロパティとSelectorプロパティは無視されます。
        public InArgument<Window> OutPutWindow { get; set; }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Window/WindowActive.png";
            }
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

        internal static string OpenBrowsersPropertyTag { get { return "WindowAttach"; } }

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int GetForegroundWindow();

        [DllImport("user32.dll", EntryPoint = "GetWindowText")]
        public static extern int GetWindowText(int hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", EntryPoint = "GetClassName")]
        public static extern int GetClassName(int hWnd, StringBuilder lpString, int nMaxCont);
        protected override void Execute(NativeActivityContext context)
        {
            var selStr = Selector.Get(context);
            UiElement element = UiElement.FromSelector(selStr);
            try
            {
                if (element != null)
                {
                    element.SetForeground();
                    int hwnd = (int)element.WindowHandle;
                    StringBuilder windowText = new StringBuilder(256);
                    GetWindowText(hwnd, windowText, 256);
                    StringBuilder className = new StringBuilder(256);
                    GetClassName(hwnd, className, 256);

                    Window currWindow = new Window();
                    currWindow.setWindowHwnd(hwnd);
                    if (Body != null)
                        context.ScheduleAction(Body, currWindow, OnCompleted, OnFaulted);
                }
                else
                {
                    SharedObject.Instance.Output(SharedObject.enOutputType.Error, Localize.LocalizedResources.GetString("msgErrorOccurred"), Localize.LocalizedResources.GetString("msgNoElementFound"));
                    throw new NotImplementedException(Localize.LocalizedResources.GetString("msgNoElementFound"));
                }
            }
            catch(Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "关闭窗口错误产生", e.Message);
                if (ContinueOnError.Get(context))
                {
                }
                else
                {
                    throw;
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
