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

        [Localize.LocalizedCategory("Category1")] //公共 //Public //一般公開
        [Localize.LocalizedDisplayName("DisplayName1")] //错误执行 //Error execution //エラー実行
        [Localize.LocalizedDescription("Description1")] //指定即使活动引发错误，自动化是否仍应继续 //Specifies whether automation should continue even if the activity raises an error //アクティビティでエラーが発生した場合でも自動化を続行するかどうかを指定します
        public InArgument<bool> ContinueOnError { get; set; }

        [Localize.LocalizedCategory("Category5")] //选项 //Option //オプション
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName62")] //窗口 //Window //窓
        [Localize.LocalizedDescription("Description76")] //存储窗口的变量。该字段仅接受Window变量 //The variable that stores the window.  This field only accepts Window variables //ウィンドウを格納する変数。 このフィールドはウィンドウ変数のみを受け入れます
        public InArgument<Window> ActiveWindow { get; set; }

        [Localize.LocalizedCategory("Category5")] //选项 //Option //オプション
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName64")] //高度 //Height //身長
        [Localize.LocalizedDescription("Description79")] // 窗口的新高度，支持正负整数 //The new height of the window, supporting positive and negative integers //正および負の整数をサポートするウィンドウの新しい高さ
        public InArgument<Int32> Height { get; set; }

        [Localize.LocalizedCategory("Category5")] //选项 //Option //オプション
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName65")] //宽度 //Width //幅
        [Localize.LocalizedDescription("Description80")] //窗口的新宽度，支持正负整数 //The new width of the window, supporting positive and negative integers //正および負の整数をサポートするウィンドウの新しい幅
        public InArgument<Int32> Width { get; set; }

        [Localize.LocalizedCategory("Category5")] //选项 //Option //オプション
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName66")] //坐标X //Coordinate X //座標X
        [Localize.LocalizedDescription("Description81")] //窗口的新位置坐标X轴，支持正负整数 //The new position coordinate of the window is X-axis, supporting positive and negative integers //ウィンドウの新しい位置座標はX軸であり、正および負の整数をサポートします
        public InArgument<Int32> PosX { get; set; }


        [Localize.LocalizedCategory("Category5")] //选项 //Option //オプション
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName67")] //坐标Y //Coordinate Y //Y座標
        [Localize.LocalizedDescription("Description82")] //窗口的新坐标位置Y轴，支持正负整数 //The new coordinate position of the window is Y-axis, supporting positive and negative integers //ウィンドウの新しい座標位置はY軸で、正および負の整数をサポートします
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
