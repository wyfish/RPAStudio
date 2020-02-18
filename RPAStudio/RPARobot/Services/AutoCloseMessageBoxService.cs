using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace RPARobot.Services
{
    /// <summary>
    /// 到时自动关闭的弹窗，以避免程序无人操作时弹窗卡住不动
    /// </summary>
    public class AutoCloseMessageBoxService
    {
        private static int Timeout = 10000;//10秒 到时自动关闭对话框

        public static MessageBoxResult Show(Window owner, string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult = MessageBoxResult.Yes)
        {
            var msgBox = System.Windows.Forms.AutoClosingMessageBox.Factory(
                            showMethod: (_caption, _buttons) =>
                                System.Windows.Forms.MessageBox.Show(new WindowWrapper(owner), messageBoxText, _caption, _buttons, (System.Windows.Forms.MessageBoxIcon)icon),
                            caption: caption
                        );

            var ret = msgBox.Show(
                timeout: Timeout,
                buttons: (System.Windows.Forms.MessageBoxButtons)button,
                defaultResult: (System.Windows.Forms.DialogResult)defaultResult);

            return (MessageBoxResult)ret;
        }

        public static MessageBoxResult Show(string messageBoxText)
        {
            var msgBox = System.Windows.Forms.AutoClosingMessageBox.Factory(
                            showMethod: (_caption, _buttons) =>
                                System.Windows.Forms.MessageBox.Show(messageBoxText)
                        );

            var ret = msgBox.Show(timeout: Timeout);

            return (MessageBoxResult)ret;
        }
    }





    public class WindowWrapper : System.Windows.Forms.IWin32Window
    {
        public WindowWrapper(IntPtr handle)
        {
            _hwnd = handle;
        }

        public WindowWrapper(Window window)
        {
            _hwnd = new WindowInteropHelper(window).Handle;
        }

        public IntPtr Handle
        {
            get { return _hwnd; }
        }

        private IntPtr _hwnd;
    }



}
