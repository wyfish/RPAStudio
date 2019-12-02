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

        [Category("公共")]
        [DisplayName("错误执行")]
        [Description("指定即使活动引发错误，自动化是否仍应继续")]
        public InArgument<bool> ContinueOnError { get; set; }

        [Category("输入")]
        [RequiredArgument]
        [OverloadGroup("G1")]
        [Browsable(true)]
        [DisplayName("窗口指示器")]
        [Description("用于在执行活动时查找特定UI元素的Text属性")]
        public InArgument<string> Selector { get; set; }

        [Category("输入")]
        [RequiredArgument]
        [OverloadGroup("G2")]
        [Browsable(true)]
        [DisplayName("窗口")]
        [Description("要关闭的窗口。该字段仅接受Window变量")]
        public InArgument<Window> ActiveWindow { get; set; }

        [Category("输出")]
        [Browsable(true)]
        [DisplayName("窗口")]
        [Description("找到的活动窗口。该字段仅支持Window变量。指定Window变量时，将忽略SearchScope和Selector属性。")]
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
                    SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", "查找不到元素");
                    throw new NotImplementedException("查找不到元素");
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
