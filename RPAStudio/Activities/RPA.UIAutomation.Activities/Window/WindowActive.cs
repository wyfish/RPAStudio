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

        [Category("公共")]
        [DisplayName("错误执行")]
        [Description("指定即使活动引发错误，自动化是否仍应继续")]
        public InArgument<bool> ContinueOnError { get; set; }

        [Category("输出")]
        [Browsable(true)]
        [DisplayName("窗口")]
        [Description("存储窗口的变量。该字段仅接受Window变量")]
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
