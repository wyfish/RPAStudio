using Fluent;
using GalaSoft.MvvmLight.Messaging;
using Plugins.Shared.Library.CodeCompletion;
using RPAStudio.Librarys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace RPAStudio.Windows
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct CopyDataStruct
        {
            public IntPtr dwData;//用户定义数据  
            public int cbData;//字符串长度
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;//字符串
        }

        public MainWindow()
        {
            InitializeComponent();

            this.WindowState = WindowState.Maximized;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            //var hwndSource = PresentationSource.FromVisual(Application.Current.MainWindow) as HwndSource;
            //if (hwndSource != null)
            //{
            //    hwndSource.AddHook(WndProc);
            //}
        }

        //private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        //{
        //    var message = (WindowMessage)msg;
        //    var subCode = (WindowMessageParameter)wParam.ToInt32();

        //    if (message == WindowMessage.WM_COPYDATA)
        //    {
        //        CopyDataStruct cds = (CopyDataStruct)Marshal.PtrToStructure(lParam, typeof(CopyDataStruct));//从发送方接收到的数据结构
        //        string param = cds.lpData;//获取发送方传过来的消息

        //        Messenger.Default.Send(new MessengerObjects.CopyData(param));//广播消息 //Messenger.Default.Register<对象的类型>(对象, TOKEN字符串, (trans) => { });//注册
        //        Application.Current.MainWindow.WindowState = WindowState.Maximized;
        //    }
        //    return IntPtr.Zero;
        //}

        private void RibbonWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                //TODO WJF 动态加载组件后，此处运行是否还正常？

                //此处获取RPAStudio引用的所有程序集的代码必须放在RPAStudio模块中调用，不能放到其它DLL中调用，否则获取有误
                Assembly target = Assembly.GetExecutingAssembly();
                //排除掉NPinyinPro库，该库导致执行代码组件无法正常编译运行，原因不明
                List<Assembly> references = (from assemblyName in target.GetReferencedAssemblies() where assemblyName.Name != "NPinyinPro"
                                             select Assembly.Load(assemblyName)).ToList();

                EditorUtil.init(references);
            });
        }

        private void RibbonWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
