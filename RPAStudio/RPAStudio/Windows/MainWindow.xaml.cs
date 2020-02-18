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
        public MainWindow()
        {
            InitializeComponent();

            this.WindowState = WindowState.Maximized;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
        }

       
        private void RibbonWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                //此处获取RPAStudio引用的所有程序集的代码必须放在RPAStudio模块中调用，不能放到其它DLL中调用，否则获取有误
                Assembly target = Assembly.GetExecutingAssembly();
                //排除掉NPinyinPro库，该库导致执行代码组件无法正常编译运行
                List<Assembly> references = (from assemblyName in target.GetReferencedAssemblies() where assemblyName.Name != "NPinyinPro"
                                             select Assembly.Load(assemblyName)).ToList();
                //编辑器初始化
                EditorUtil.init(references);
            });
        }

        private void RibbonWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
