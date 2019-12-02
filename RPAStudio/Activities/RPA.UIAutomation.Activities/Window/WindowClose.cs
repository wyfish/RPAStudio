using System;
using System.Activities;
using System.ComponentModel;
using Plugins.Shared.Library;
using Plugins.Shared.Library.UiAutomation;

namespace RPA.UIAutomation.Activities.Window
{
    [Designer(typeof(WindowCloseDesigner))]
    public sealed class WindowClose : AsyncCodeActivity
    {
        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "Window Close"; } }

        [Category("公共")]
        [DisplayName("错误执行")]
        [Description("指定即使活动引发错误，自动化是否仍应继续")]
        public InArgument<bool> ContinueOnError { get; set; }

        [Category("输入")]
        [Browsable(true)]
        [DisplayName("窗口指示器")]
        [Description("用于在执行活动时查找特定UI元素的Text属性")]
        public InArgument<string> Selector { get; set; }

        [Category("输入")]
        [Browsable(true)]
        [DisplayName("窗口")]
        [Description("要关闭的窗口。该字段仅接受Window变量")]
        public InArgument<Window> ActiveWindow { get; set; }

        [Category("输入")]
        [Browsable(true)]
        [DisplayName("窗口句柄")]
        [Description("要关闭的窗口句柄。")]
        public InArgument<IntPtr> handle { get; set; }

        [Category("输入")]
        [DisplayName("超时时间")]
        [Description("指定在引发错误之前等待活动运行的时间量（以毫秒为单位）。默认值为30000毫秒（30秒）")]
        public InArgument<Int32> TimeoutMS { get; set; }

        [Browsable(false)]
        public string SourceImgPath{ get;set;}

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

        [Browsable(false)]
        public string ClassName { get { return "WindowClose"; } }
        private delegate string runDelegate();
        private runDelegate m_Delegate;
        public string Run()
        {
            return ClassName;
        }

        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            Window currWindow = ActiveWindow.Get(context);
            IntPtr _handle = handle.Get(context);
            if(currWindow == null)
            {
                currWindow = new Window();
                currWindow.setWindowHwnd((int)_handle);
            }
            try
            {
                var selStr = Selector.Get(context);
                UiElement element = UiElement.FromSelector(selStr);

                if (currWindow != null)
                {
                    Win32Api.SendMessage((IntPtr)currWindow.getWindowHwnd(), Win32Api.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                }
                else if(element != null)
                {
                    Win32Api.SendMessage(element.WindowHandle, Win32Api.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                }
                else
                {
                    PropertyDescriptor property = context.DataContext.GetProperties()[WindowActive.OpenBrowsersPropertyTag];
                    if (property == null)
                        property = context.DataContext.GetProperties()[WindowAttach.OpenBrowsersPropertyTag];
                    if(property != null)
                    {
                        Window getBrowser = property.GetValue(context.DataContext) as Window;
                        Win32Api.SendMessage((IntPtr)getBrowser.getWindowHwnd(), Win32Api.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                    }
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
            m_Delegate = new runDelegate(Run);
            return m_Delegate.BeginInvoke(callback, state);
        }

        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
        }
    }
}
