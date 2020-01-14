using System;
using System.Activities;
using System.ComponentModel;
using Plugins.Shared.Library;

namespace RPA.UIAutomation.Activities.Window
{
    [Designer(typeof(WindowMinDesigner))]
    public sealed class WindowMin : AsyncCodeActivity
    {
        public new string DisplayName
        {
            get
            {
                return "Window Min";
            }
        }

        [Localize.LocalizedCategory("Category1")] //公共 //Public //一般公開
        [Localize.LocalizedDisplayName("DisplayName1")] //错误执行 //Error execution //エラー実行
        [Localize.LocalizedDescription("Description1")] //指定即使活动引发错误，自动化是否仍应继续 //Specifies whether automation should continue even if the activity raises an error //アクティビティでエラーが発生した場合でも自動化を続行するかどうかを指定します
        public InArgument<bool> ContinueOnError { get; set; }

        [Localize.LocalizedCategory("Category4")] //输出 //Output //出力
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName62")] //窗口 //Window //窓
        [Localize.LocalizedDescription("Description76")] //存储窗口的变量。该字段仅接受Window变量 //The variable that stores the window.  This field only accepts Window variables //ウィンドウを格納する変数。 このフィールドはウィンドウ変数のみを受け入れます
        public InArgument<Window> ActiveWindow { get; set; }

        [Browsable(false)]
        public string icoPath {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Window/WindowMin.png";
            }
        }


        [Browsable(false)]
        public string ClassName { get { return "WindowMin"; } }

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
                if (currWindow != null)
                {
                    Win32Api.SendMessage((IntPtr)currWindow.getWindowHwnd(), Win32Api.WM_SYSCOMMAND, (IntPtr)Win32Api.SC_MINIMIZE, IntPtr.Zero);
                }
                else
                {
                    PropertyDescriptor property = context.DataContext.GetProperties()[WindowActive.OpenBrowsersPropertyTag];
                    if (property == null)
                        property = context.DataContext.GetProperties()[WindowAttach.OpenBrowsersPropertyTag];
                    if(property != null)
                    {
                        Window getBrowser = property.GetValue(context.DataContext) as Window;
                        Win32Api.SendMessage((IntPtr)getBrowser.getWindowHwnd(), Win32Api.WM_SYSCOMMAND, (IntPtr)Win32Api.SC_MINIMIZE, IntPtr.Zero);
                    }
                }
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "窗口最小化错误产生", e.Message);
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
