using System;
using System.Activities;
using System.ComponentModel;
using System.Text;
using System.Runtime.InteropServices;
using System.Activities.Statements;
using Plugins.Shared.Library;

namespace RPA.UIAutomation.Activities.Window
{
    [Designer(typeof(WindowActiveDesigner))]
    public class WindowActive : NativeActivity
    {
        public new string DisplayName
        {
            get
            {
                return "WindowActive";
            }
        }

        [Browsable(false)]
        public ActivityAction<object> Body { get; set; }

        public WindowActive()
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

        [Localize.LocalizedCategory("Category4")] //输出 //Output //出力
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName62")] //窗口 //Window //窓
        [Localize.LocalizedDescription("Description76")] //存储窗口的变量。该字段仅接受Window变量 //The variable that stores the window.  This field only accepts Window variables //ウィンドウを格納する変数。 このフィールドはウィンドウ変数のみを受け入れます
        public OutArgument<Window> thisWindow
        {
            get;
            set;
        }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Window/WindowActive.png";
            }
        }

        internal static string OpenBrowsersPropertyTag { get { return "WindowActive"; } }


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
            try
            {
                int hwnd = GetForegroundWindow();
                StringBuilder windowText = new StringBuilder(256);
                GetWindowText(hwnd, windowText, 256);
                StringBuilder className = new StringBuilder(256);
                GetClassName(hwnd, className, 256);

                Window currWindow = new Window();
                currWindow.setWindowHwnd(hwnd);
                currWindow.setWindowText("" + windowText);
                currWindow.setWindowClass("" + className);
                context.SetValue(thisWindow, currWindow);

                if (Body != null)
                    context.ScheduleAction(Body, currWindow, OnCompleted, OnFaulted);
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
