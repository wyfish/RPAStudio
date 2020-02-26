using System;
using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Runtime.InteropServices;


namespace RPA.UIAutomation.Activities.Browser
{
    // MouseDesigner.xaml 的交互逻辑
    public partial class OpenBrowserDesigner
    {
        public OpenBrowserDesigner()
        {
            InitializeComponent();
            Net.Surviveplus.Localization.WpfLocalization.ApplyResources(this, Properties.Resources.ResourceManager);
            //SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", "详细错误信息说明，可省略");
        }

        private void HyperlinkClick(object sender, RoutedEventArgs e)
        {
            PickScreen();
        }

        private void PickScreen()
        {
            //Application.Current.MainWindow.WindowState = WindowState.Minimized;

            //IntPtr hwnd = new WindowInteropHelper(Application.Current.MainWindow).Handle;

            //string ID = getPropertyValue("guid");
            //var screenshotsPath = SharedObject.Instance.ProjectPath + @"\.screenshots";
            //JsonObject jsonObject = new JsonObject()
            //{ cmd = "grab", mainHwnd = (int)hwnd, classID = getPropertyValue("guid"),
            //    className = getPropertyValue("ClassName"), savePath = screenshotsPath.ToString(), bIsAccessible=false};
            //string json = JsonMapper.ToJson(jsonObject);
            //Console.WriteLine(json);
            //try
            //{
            //    ProcessStartInfo startInfo = new ProcessStartInfo();
            //    string exePath = getCurrentPath() + "\\ExecutableFiles\\UIElementSelector.exe";
            //    startInfo.FileName = exePath;
            //    startInfo.Arguments = EncodeBase64("utf-8", json);
            //    startInfo.WindowStyle = ProcessWindowStyle.Normal;
            //    startInfo.UseShellExecute = false;
            //    Process proc = Process.Start(startInfo);
            //    if (proc != null)
            //    {
            //        proc.EnableRaisingEvents = true;
            //        //指定退出事件方法
            //        proc.Exited += new EventHandler(procExited);
            //    }
            //    //proc.WaitForExit();
            //}
            //catch (ArgumentException ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}
            ////System.Threading.Thread.Sleep(5000);
            ////GetWindowMsg();
        }


        /*WINDOW窗口使用，同步属性框*/
        [DllImport("user32.dll", EntryPoint = "GetWindowText")]
        public static extern int GetWindowText(int hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", EntryPoint = "GetClassName")]
        public static extern int GetClassName(int hWnd, StringBuilder lpString, int nMaxCont);

        [DllImport("user32.dll", EntryPoint = "WindowFromPoint")]
        public static extern int WindowFromPoint(int xPoint, int yPoint);

        [StructLayout(LayoutKind.Sequential)]//定义与API相兼容结构体，实际上是一种内存转换
        public struct POINTAPI { public int X; public int Y; }

        [DllImport("user32.dll", EntryPoint = "GetCursorPos")]//获取鼠标坐标
        public static extern int GetCursorPos(ref POINTAPI lpPoint);
        private void procExited(object sender, EventArgs e)
        {
            Console.WriteLine("外部程序已经退出！");
        }

        //private void GetWindowMsg()
        //{
        //    string XStr = getPropertyValue("offsetX");
        //    string YStr = getPropertyValue("offsetX");

        //    int IntSetX = Convert.ToInt32(XStr);
        //    int IntSetY = Convert.ToInt32(YStr);
        //    POINTAPI point = new POINTAPI();
        //    point.X = IntSetX;
        //    point.Y = IntSetY;

        //    int hwnd = WindowFromPoint(point.X, point.Y);
        //    StringBuilder windowText = new StringBuilder(256);
        //    GetWindowText(hwnd, windowText, 256);
        //    StringBuilder className = new StringBuilder(256);
        //    GetClassName(hwnd, className, 256);

        //    System.Diagnostics.Debug.WriteLine("formHandle 窗口句柄 : " + hwnd);
        //    System.Diagnostics.Debug.WriteLine("windowText 窗口的标题: " + windowText);
        //    System.Diagnostics.Debug.WriteLine("className: " + className);

        //    List<ModelProperty> PropertyList = ModelItem.Properties.ToList();
        //    ModelProperty SelectorProperty = PropertyList.Find((ModelProperty property) => property.Name.Equals("Selector"));
        //    InArgument<string> Selector = "<" + "Handle:" + hwnd
        //        + " WindowText:" + windowText + " ClassName:" + className + ">";
        //    SelectorProperty.SetValue(Selector);
        //    ModelProperty hwndProperty = PropertyList.Find((ModelProperty property) => property.Name.Equals("hwnd"));
        //    hwndProperty.SetValue(hwnd);
        //}

        private string getPropertyValue(string propertyName)
        {
            List<ModelProperty> PropertyList = ModelItem.Properties.ToList();
            ModelProperty _property = PropertyList.Find((ModelProperty property) => property.Name.Equals(propertyName));
            return _property.Value.ToString();
        }

        private string getCurrentPath()
        {
            return System.IO.Directory.GetCurrentDirectory();
        }

        public static string EncodeBase64(string code_type, string code)
        {
            string encode = "";
            byte[] bytes = Encoding.GetEncoding(code_type).GetBytes(code);
            try
            {
                encode = Convert.ToBase64String(bytes);
            }
            catch
            {
                encode = code;
            }
            return encode;
        }

        public static string DecodeBase64(string code_type, string code)
        {
            string decode = "";
            byte[] bytes = Convert.FromBase64String(code);
            try
            {
                decode = Encoding.GetEncoding(code_type).GetString(bytes);
            }
            catch
            {
                decode = code;
            }
            return decode;
        }

        //private void HiddenNavigateTextBlock()
        //{
        //    navigateTextBlock.Visibility = System.Windows.Visibility.Hidden;
        //}

        ////菜单按钮点击
        //private void NavigateButtonClick(object sender, RoutedEventArgs e)
        //{
        //    contextMenu.PlacementTarget = this.navigateButton;
        //    contextMenu.Placement = PlacementMode.Top;
        //    contextMenu.IsOpen = true;
        //}

        ////菜单按钮初始化
        //private void NavigateButtonInitialized(object sender, EventArgs e)
        //{
        //    navigateButton.ContextMenu = null;
        //}

        ////菜单项点击测试
        //private void meauItemClickOne(object sender, RoutedEventArgs e)
        //{
        //    PickScreen();
        //}
    }
}
