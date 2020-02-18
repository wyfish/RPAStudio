using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Threading;
using Gma.UserActivityMonitor;
using System.Linq;
using WindowsAccessBridgeInterop;
using FlaUI.Core.AutomationElements;
using System.Collections.Generic;

namespace Plugins.Shared.Library.UiAutomation
{
    public partial class OverlayForm : Form
    {
        public const int WS_EX_TOOLWINDOW = 0x00000080;
        public const int WS_EX_NOACTIVATE = 0x08000000;
        const int WM_NCHITTEST = 0x0084;
        const int HTTRANSPARENT = -1;

        public bool IsWindowHighlight { get; internal set; }

        private bool enablePassThrough { get; set; }

        private static DispatcherTimer dispatcherTimer = new DispatcherTimer();

        private static DispatcherTimer dealySelectDispatcherTimer = new DispatcherTimer();
        private static DispatcherTimer keyStateDispatcherTimer = new DispatcherTimer();

        /// <summary>
        /// 四个边框
        /// </summary>
        private ExtendedPanel panelBorderLeft, panelBorderTop, panelBorderRight, panelBorderBottom;

        /// <summary>
        /// 内部边框
        /// </summary>
        private ExtendedPanel panelInside;

        /// <summary>
        /// 当前正处于高亮状态的元素
        /// </summary>
        internal UiNode CurrentHighlightElement;

        public OverlayForm()
        {
            InitializeComponent();

            this.panelBorderLeft = createPanel(Color.FromArgb(232, 193, 116));
            this.panelBorderTop = createPanel(Color.FromArgb(232, 193, 116));
            this.panelBorderRight = createPanel(Color.FromArgb(232, 193, 116));
            this.panelBorderBottom = createPanel(Color.FromArgb(232, 193, 116));

            this.panelInside = createPanel(Color.FromArgb(123, 159, 212));

            this.Cursor = UiCommon.GetCursor(Properties.Resources.cursor);
        }

        private void keyStateDispatcherTimer_Tick(object sender, EventArgs e)
        {
            const int MBUTTON = 0x04;
            if (UiCommon.GetAsyncKeyState(MBUTTON) != 0)
            {
                doDelaySelect();
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= WS_EX_TOOLWINDOW;//避免ATL+TAB时显示该窗体
                cp.ExStyle |= WS_EX_NOACTIVATE;//禁止激活，以便能够inspect菜单条目
                return cp;
            }
        }

        public Point CurrentHighlightElementRelativeClickPos { get; private set; }
        public bool hasDoSelect { get; private set; }

        private ExtendedPanel createPanel(Color color)
        {
            var panel = new ExtendedPanel();
            panel.Size = new Size(0, 0);
            panel.Parent = this;
            panel.BackColor = color;
            panel.Show();
            return panel;
        }



        protected override void WndProc(ref Message m)
        {
            if (enablePassThrough)
            {
                if (m.Msg == WM_NCHITTEST)
                {
                    m.Result = (IntPtr)HTTRANSPARENT;
                    return;
                }
            }

            base.WndProc(ref m);
        }

        private void OverlayForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }


        private void doCancel()
        {
            System.Threading.Tasks.Task.Run(() => {
                this.Invoke(new Action(() => {
                    UiElement.OnCanceled?.Invoke();
                }));
            });

            StopHighlight();
        }

        private void doDelaySelect()
        {
            StopHighlight(false);
            dealySelectDispatcherTimer.Tick -= new EventHandler(dealySelectDispatcherTimer_Tick);
            dealySelectDispatcherTimer.Tick += new EventHandler(dealySelectDispatcherTimer_Tick);

            dealySelectDispatcherTimer.Interval = TimeSpan.FromSeconds(5);
            dealySelectDispatcherTimer.Stop();
            dealySelectDispatcherTimer.Start();
        }

        private void doSelect()
        {
            lock(this)
            {
                if (hasDoSelect)
                {
                    return;
                }

                hasDoSelect = true;

                if (CurrentHighlightElement != null)
                {
                    if (enableJavaUiNode(CurrentHighlightElement))
                    {
                        return;
                    }

                    var element = new UiElement(CurrentHighlightElement);
                    element.RelativeClickPos = CurrentHighlightElementRelativeClickPos;

                    //隐藏窗体并截全屏，以便后期自己绘制红色标记矩形
                    this.Hide();
                    element.currentInformativeScreenshot = element.CaptureInformativeScreenshot();

                    //System.Diagnostics.Debug.WriteLine(string.Format("CurrentHighlightElementRelativeClickPos = ({0},{1})", CurrentHighlightElementRelativeClickPos.X, CurrentHighlightElementRelativeClickPos.Y));
                    System.Threading.Tasks.Task.Run(()=> {
                        this.Invoke(new Action(()=> {
                            UiElement.OnSelected?.Invoke(element);
                        }));
                    });

                    StopHighlight();
                }
            }
            
        }

        

        private void dealySelectDispatcherTimer_Tick(object sender, EventArgs e)
        {
            dealySelectDispatcherTimer.Tick -= new EventHandler(dealySelectDispatcherTimer_Tick);
            dealySelectDispatcherTimer.Stop();


            StartHighlight();
        }


        public void MoveRect(Rectangle rect)
        {
            int margin = 5;

            panelBorderLeft.Location = new Point(rect.Left, rect.Top);
            panelBorderLeft.Size = new Size(margin, rect.Height);

            panelBorderTop.Location = new Point(rect.Left, rect.Top);
            panelBorderTop.Size = new Size(rect.Width, margin);

            panelBorderRight.Location = new Point((rect.Left + rect.Width - margin), rect.Top);
            panelBorderRight.Size = new Size(margin, rect.Height);

            panelBorderBottom.Location = new Point(rect.Left, (rect.Top + rect.Height - margin));
            panelBorderBottom.Size = new Size(rect.Width, margin);

            panelInside.Location = new Point(rect.Left + 5, rect.Top + 5);
            panelInside.Size = new Size(rect.Width - 10, rect.Height - 10);

        }

        internal void installHook()
        {
            //使用 https://www.codeproject.com/Articles/7294/Processing-Global-Mouse-and-Keyboard-Hooks-in-C?msg=5642094#xx5642094xx
            uninstallHook();
            HookManager.MouseClickExt += HookManager_MouseClickExt;
            HookManager.KeyUp += HookManager_KeyUp;
        }

        internal void uninstallHook()
        {
            HookManager.MouseClickExt -= HookManager_MouseClickExt;
            HookManager.KeyUp -= HookManager_KeyUp;
        }


        private void HookManager_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    doCancel();
                    break;
                case Keys.F2:
                    doDelaySelect();
                    break;
                default:
                    break;
            }
        }

        private void HookManager_MouseClickExt(object sender, MouseEventExtArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    doSelect();
                    e.Handled = true;
                    break;
                case MouseButtons.Right:
                    doCancel();
                    e.Handled = true;
                    break;
                default:
                    break;
            }
        }


        internal void StopHighlight(bool isNeedShowMainWindow = true)
        {
            uninstallHook();

            dispatcherTimer.Stop();
            dealySelectDispatcherTimer.Stop();
            keyStateDispatcherTimer.Stop();

            this.Hide();
            panelBorderLeft.Size = panelBorderTop.Size = panelBorderRight.Size = panelBorderBottom.Size = panelInside.Size = new Size(0, 0);

            if (!UiElement.IsRecordingWindowOpened)
            {
                if (isNeedShowMainWindow)
                {
                    System.Windows.Application.Current.MainWindow.WindowState = System.Windows.WindowState.Normal;
                }
            }
        }

        internal void StartHighlight()
        {
            hasDoSelect = false;

            installHook();

            JavaUiNode.EnumJvms(true);//重刷JVM列表

            this.Show();

            dispatcherTimer.Tick -= new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);

            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(100);
            dispatcherTimer.Start();

            keyStateDispatcherTimer.Tick -= new EventHandler(keyStateDispatcherTimer_Tick);
            keyStateDispatcherTimer.Tick += new EventHandler(keyStateDispatcherTimer_Tick);

            keyStateDispatcherTimer.Interval = TimeSpan.FromMilliseconds(30);
            keyStateDispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                enablePassThrough = true;

                var screenPoint = System.Windows.Forms.Cursor.Position;

                AutomationElement element = null;

                if (IsWindowHighlight)
                {
                    IntPtr hWnd = UiCommon.GetRootWindow(screenPoint);

                    if (hWnd != IntPtr.Zero)
                    {
                        element = UIAUiNode.UIAAutomation.FromHandle(hWnd);
                    }
                }
                else
                {
                    try
                    {
                        element = UIAUiNode.UIAAutomation.FromPoint(screenPoint);
                    }
                    catch (Exception)
                    {
                        if (element == null)
                        {
                            IntPtr hWnd = UiCommon.WindowFromPoint(screenPoint);

                            if (hWnd != IntPtr.Zero)
                            {
                                element = UIAUiNode.UIAAutomation.FromHandle(hWnd);
                            }
                        }
                    }

                    if (inspectJavaUiNode(element, screenPoint))
                    {
                        return;
                    }

                }

                CurrentHighlightElement = new UIAUiNode(element);

                if (element == null)
                {
                    return;
                }

                var rect = element.BoundingRectangle;

                //计算鼠标点击时的相对元素的偏移,以供后期有必要时使用
                CurrentHighlightElementRelativeClickPos = new Point(screenPoint.X - rect.Left, screenPoint.Y - rect.Top);
                
                this.MoveRect(rect);
            }
            catch (Exception)
            {

            }
            finally
            {
                enablePassThrough = false;

                this.TopMost = true;
            }
        }


        private bool enableJavaUiNode(UiNode node)
        {

            if (node.WindowHandle != IntPtr.Zero && node.ProcessName == "javaw.exe" && !JavaUiNode.accessBridge.Functions.IsJavaWindow(node.WindowHandle))
            {
                StopHighlight();
                //提示用户是否启用JAVA自动化
                var ret = System.Windows.MessageBox.Show("是否启用Java Access Bridge？", "询问", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question, System.Windows.MessageBoxResult.Yes);
                if (ret == System.Windows.MessageBoxResult.Yes)
                {
                    //java8及以后的版本直接调用命令行，之前的版本需要手动安装
                    var jabswitchExe = System.IO.Path.GetDirectoryName(node.ProcessFullPath) + @"\jabswitch.exe";
                    if (System.IO.File.Exists(jabswitchExe))
                    {
                        //存在jabswitch.exe，则可直接调用jabswitch.exe -enable来启用
                        UiCommon.RunProcess(jabswitchExe, "-enable", true);
                    }
                    else
                    {
                        //需要主动安装accessbridge相关文件(根据32位或64位的JRE进行拷贝)
                        var javaExe = System.IO.Path.GetDirectoryName(node.ProcessFullPath) + @"\java.exe";

                        string windowsHome = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
                        string javaHome = System.IO.Directory.GetParent(System.IO.Path.GetDirectoryName(node.ProcessFullPath)).FullName;

                        installJavaAccessBridge(Environment.Is64BitOperatingSystem, UiCommon.IsExe64Bit(javaExe), windowsHome, javaHome);
                    }

                    System.Windows.MessageBox.Show("操作完成，请重新运行Java程序以便操作生效", "提示", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                }

                return true;
            }

            return false;
        }

        private void installJavaAccessBridge(bool isOperatingSystem64Bit, bool isJava64Bit, string windowsHome, string javaHome)
        {
            var jabPath = System.Environment.CurrentDirectory + @"\JAB";
            if (isOperatingSystem64Bit)
            {
                //Windows64位系统安装JAB
                UiCommon.CopyFileToDir(jabPath+ @"\WindowsAccessBridge-64.dll", windowsHome + @"\SYSTEM32");
                UiCommon.CopyFileToDir(jabPath + @"\WindowsAccessBridge-32.dll", windowsHome + @"\SYSWOW64");

                if (isJava64Bit)
                {
                    UiCommon.CopyFileToDir(jabPath + @"\JavaAccessBridge-64.dll", javaHome + @"\bin");
                    UiCommon.CopyFileToDir(jabPath + @"\JAWTAccessBridge-64.dll", javaHome + @"\bin");
                    UiCommon.CopyFileToDir(jabPath + @"\access-bridge-64.jar", javaHome + @"\lib\ext");
                }
                else
                {
                    UiCommon.CopyFileToDir(jabPath + @"\JavaAccessBridge-32.dll", javaHome + @"\bin");
                    UiCommon.CopyFileToDir(jabPath + @"\JAWTAccessBridge-32.dll", javaHome + @"\bin");
                    UiCommon.CopyFileToDir(jabPath + @"\access-bridge-32.jar", javaHome + @"\lib\ext");
                }

                UiCommon.CopyFileToDir(jabPath + @"\accessibility.properties", javaHome + @"\lib");
                UiCommon.CopyFileToDir(jabPath + @"\jaccess.jar", javaHome + @"\lib\ext");
            }
            else
            {
                //Windows32位系统安装JAB
                UiCommon.CopyFileToDir(jabPath + @"\WindowsAccessBridge.dll", windowsHome + @"\SYSTEM32");

                UiCommon.CopyFileToDir(jabPath + @"\JavaAccessBridge.dll", javaHome + @"\bin");
                UiCommon.CopyFileToDir(jabPath + @"\JAWTAccessBridge.dll", javaHome + @"\bin");
                UiCommon.CopyFileToDir(jabPath + @"\access-bridge.jar", javaHome + @"\lib\ext");

                UiCommon.CopyFileToDir(jabPath + @"\accessibility.properties", javaHome + @"\lib");
                UiCommon.CopyFileToDir(jabPath + @"\jaccess.jar", javaHome + @"\lib\ext");
            }

        }



        /// <summary>
        /// 若element是JAVA窗口，则里面的元素要按照JAVA节点方式获取
        /// </summary>
        /// <param name="element"></param>
        /// <param name="screenPoint"></param>
        /// <returns></returns>
        private bool inspectJavaUiNode(AutomationElement element,Point screenPoint)
        {
            if (element != null)
            {
                //判断是否是JAVA窗口
                var node = new UIAUiNode(element);
                if (JavaUiNode.accessBridge.Functions.IsJavaWindow(node.WindowHandle))
                {
                    //是JAVA窗口，内部节点按JAVA方式选择
                    Path<AccessibleNode> javaNodePath = JavaUiNode.EnumJvms().Select(javaNode => javaNode.GetNodePathAt(screenPoint)).Where(x => x != null).FirstOrDefault(); 
                    var currentJavaNode = javaNodePath == null ? null : javaNodePath.Leaf;
                    if (currentJavaNode == null)
                    {
                        return false;
                    }

                    CurrentHighlightElement = new JavaUiNode(currentJavaNode);

                    var rect = currentJavaNode.GetScreenRectangle();
                    //计算鼠标点击时的相对元素的偏移,以供后期有必要时使用
                    CurrentHighlightElementRelativeClickPos = new Point(screenPoint.X - (int)rect?.Left, screenPoint.Y - (int)rect?.Top);

                    this.MoveRect((Rectangle)currentJavaNode.GetScreenRectangle());
                    return true;
                }
            }
            //Console.WriteLine("return false ");
            return false;
        }
    }



    /// <summary>
    /// 设置可穿透的Panel，以便标记框上的鼠标能选择下方窗体
    /// </summary>
        public class ExtendedPanel : Panel
    {
        private const int WM_NCHITTEST = 0x84;
        private const int HTTRANSPARENT = -1;

        protected override void WndProc(ref Message message)
        {
            if (message.Msg == (int)WM_NCHITTEST)
                message.Result = (IntPtr)HTTRANSPARENT;
            else
                base.WndProc(ref message);
        }
    }


}
