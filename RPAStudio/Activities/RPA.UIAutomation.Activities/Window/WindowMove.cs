using System;
using System.Activities;
using System.ComponentModel;
using Plugins.Shared.Library;

namespace RPA.UIAutomation.Activities.Window
{
    [Designer(typeof(WindowMoveDesigner))]
    public sealed class WindowMove : AsyncCodeActivity
    {
        public new string DisplayName
        {
            get
            {
                return "Window Move";
            }
        }

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);
        }

        [Browsable(false)]
        public Window currWindow { get; set; }

        [Category("公共")]
        [DisplayName("错误执行")]
        [Description("指定即使活动引发错误，自动化是否仍应继续")]
        public InArgument<bool> ContinueOnError { get; set; }

        [Category("选项")]
        [Browsable(true)]
        [DisplayName("窗口")]
        [Description("存储窗口的变量。该字段仅接受Window变量")]
        public InArgument<Window> ActiveWindow { get; set; }

        [Category("选项")]
        [Browsable(true)]
        [DisplayName("高度")]
        [Description(" 窗口的新高度，支持正负整数")]
        public InArgument<Int32> Height { get; set; }

        [Category("选项")]
        [Browsable(true)]
        [DisplayName("宽度")]
        [Description("窗口的新宽度，支持正负整数")]
        public InArgument<Int32> Width { get; set; }

        [Category("选项")]
        [Browsable(true)]
        [DisplayName("坐标X")]
        [Description("窗口的新位置坐标X轴，支持正负整数")]
        public InArgument<Int32> PosX { get; set; }


        [Category("选项")]
        [Browsable(true)]
        [DisplayName("坐标Y")]
        [Description("窗口的新坐标位置Y轴，支持正负整数")]
        public InArgument<Int32> PosY { get; set; }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Window/WindowMove.png";
            }
        }


        [Browsable(false)]
        public string ClassName { get { return "WindowMove"; } }

        private delegate string runDelegate();
        private runDelegate m_Delegate;
        public string Run()
        {
            return ClassName;
        }


        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            Window currWindow = ActiveWindow.Get(context);
            try
            {
                if(currWindow == null)
                {
                    PropertyDescriptor property = context.DataContext.GetProperties()[WindowActive.OpenBrowsersPropertyTag];
                    if (property == null)
                        property = context.DataContext.GetProperties()[WindowAttach.OpenBrowsersPropertyTag];
                    if(property != null)
                        currWindow = property.GetValue(context.DataContext) as Window;
                }
                Win32Api.Rect rect = new Win32Api.Rect();
                Win32Api.GetWindowRect((IntPtr)currWindow.getWindowHwnd(), out rect);
                int oldWidth = rect.Right - rect.Left;
                int oldHeight = rect.Bottom - rect.Top;
                int oldPosX = rect.Left;
                int oldPosY = rect.Top;

                int newPosX = PosX.Get(context);
                int newPosY = PosY.Get(context);
                int newWidth = Width.Get(context);
                int newHeight = Height.Get(context);

                int defPosX = newPosX == 0 ? oldPosX : newPosX;
                int defPosY = newPosY == 0 ? oldPosY : newPosY;
                int defWidth = newWidth == 0 ? oldWidth : newWidth;
                int defHeight = newHeight == 0 ? oldHeight : newHeight;

                Win32Api.MoveWindow(currWindow.getWindowHwnd(), defPosX, defPosY, defWidth, defHeight, true);
            }
            catch (Exception e)
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
