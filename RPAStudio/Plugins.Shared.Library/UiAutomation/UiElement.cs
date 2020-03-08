using System.Collections.Generic;
using System.Xml;
using System;
using System.Drawing;
using FlaUI.Core.WindowsAPI;
using FlaUI.Core.Input;
using FlaUI.Core.Overlay;
using System.Threading;
using FlaUI.Core.Definitions;
using FlaUI.Core.Conditions;
using System.Activities;

/// <summary>
/// 注意事项
/// 1.Gma.UserActivityMonitor.dll中SetWindowsHookEx的参数有修改，否则新版.NET会抛异常，进行了源码修改
/// 2.JAVA相关的WindowsAccessBridgeInterop.dll等dll的showing值在中文是“可见”，所以进行了特殊处理，进行了源码修改
/// </summary>

namespace Plugins.Shared.Library.UiAutomation
{
    public class UiElement
    {
        public delegate void UiElementSelectedEventHandler(UiElement uiElement);
        public delegate void UiElementCancelededEventHandler();

        public static bool IsRecordingWindowOpened { get; set; }//录制窗口是否已经打开

        public static UiElementSelectedEventHandler OnSelected;
        public static UiElementCancelededEventHandler OnCanceled;

        internal UiNode uiNode;

        public Point RelativeClickPos { get; set; }//鼠标点击的坐标

        private static UiElement cacheDesktop;
        private string cachedId;
        private UiElement cachedParent;
        private UiElement automationElementParent;
        private UiElement cachedDirectTopLevelWindow;
        private Rectangle boundingRectangle;

        public static OverlayForm overlayForm;
        internal Bitmap currentInformativeScreenshot;
        private Bitmap currentDesktopScreenshot;

        static UiElement()
        {
            overlayForm = new OverlayForm();
        }

        /// <summary>
        /// 初始化，由于JAVA的accessBridge.Initialize()生效有延迟，所以提前使其生效
        /// </summary>
        public static void Init()
        {
            JavaUiNode.EnumJvms(true);
        }

        internal UiElement(UiNode node, UiElement parent = null)
        {
            this.uiNode = node;
            this.Parent = parent;
            this.boundingRectangle = node.BoundingRectangle;
        }

        public UiElement Parent
        {
            get
            {
                if (cachedParent == null)
                {
                    if (uiNode.Parent != null)
                    {
                        cachedParent = new UiElement(uiNode.Parent);
                    }
                }

                return cachedParent;
            }

            private set
            {
                cachedParent = value;
            }
        }

        public UiElement AutomationElementParent
        {
            get
            {
                automationElementParent = new UiElement(uiNode.AutomationElementParent);
                return automationElementParent;
            }
        }

        public static UiElement Desktop
        {
            get
            {
                if (cacheDesktop == null)
                {
                    var _rootElement = UIAUiNode.UIAAutomation.GetDesktop();
                    cacheDesktop = new UiElement(new UIAUiNode(_rootElement));
                }

                return cacheDesktop;
            }
        }

        public List<UiElement> Children
        {
            get
            {
                var list = new List<UiElement>();
                var children = uiNode.Children;
                foreach (var item in children)
                {
                    list.Add(new UiElement(item, this));
                }

                return list;
            }
        }

        public string ControlType
        {
            get
            {
                if (string.IsNullOrEmpty(uiNode.ControlType))
                {
                    return "Node";
                }
                else
                {
                    return uiNode.ControlType;
                }

            }
        }

        public string Name
        {
            get
            {
                return uiNode.Name;
            }
        }

        public string AutomationId
        {
            get
            {
                return uiNode.AutomationId;
            }
        }

        public string UserDefineId
        {
            get
            {
                return uiNode.UserDefineId;
            }
        }

        public string ClassName
        {
            get
            {
                return uiNode.ClassName;

            }
        }


        public string ProcessName
        {
            get
            {
                return uiNode.ProcessName;
            }
        }

        public string ProcessFullPath
        {
            get
            {
                return uiNode.ProcessFullPath;
            }
        }

        public string Role
        {
            get
            {
                return uiNode.Role;
            }
        }

        public string Description
        {
            get
            {
                return uiNode.Description;
            }
        }

        public string Idx
        {
            get
            {
                return uiNode.Idx;
            }
        }


        private static bool isUiElementMatch(UiElement uiElement, XmlElement xmlElement)
        {
            //System.Diagnostics.Debug.WriteLine(uiElement.Id+"$$$$$"+ xmlElement.OuterXml);
            //有可能ControlType也会改变，可用Node匹配任何的ControlType类型
            if (xmlElement.Name == "Node" || xmlElement.Name == uiElement.ControlType)
            {
                bool isMatch = true;

                foreach (XmlAttribute attr in xmlElement.Attributes)
                {
                    if (attr.Name == "Name")
                    {
                        if (attr.Value != uiElement.Name)
                        {
                            isMatch = false;
                            break;
                        }
                    }
                    else if (attr.Name == "AutomationId")
                    {
                        if (attr.Value != uiElement.AutomationId)
                        {
                            isMatch = false;
                            break;
                        }
                    }
                    else if (attr.Name == "UserDefineId")
                    {
                        if (attr.Value != uiElement.UserDefineId)
                        {
                            isMatch = false;
                            break;
                        }
                    }
                    else if (attr.Name == "ClassName")
                    {
                        if (attr.Value != uiElement.ClassName)
                        {
                            isMatch = false;
                            break;
                        }
                    }
                    else if (attr.Name == "Role")
                    {
                        if (attr.Value != uiElement.Role)
                        {
                            isMatch = false;
                            break;
                        }
                    }
                    else if (attr.Name == "Description")
                    {
                        if (attr.Value != uiElement.Description)
                        {
                            isMatch = false;
                            break;
                        }
                    }
                    else if (attr.Name == "ProcessName")
                    {
                        if (attr.Value != uiElement.ProcessName)
                        {
                            isMatch = false;
                            break;
                        }
                    }
                    else
                    {

                    }
                }

                return isMatch;
            }


            return false;
        }


        public string Id
        {
            get
            {
                if (cachedId == null)
                {
                    XmlDocument xmlDoc = new XmlDocument();

                    //此处有修改，判断逻辑isUiElementMatch也要对应修改
                    //{{
                    var itemName = ControlType;
                    var itemElement = xmlDoc.CreateElement(itemName);

                    if (!string.IsNullOrEmpty(Name))
                    {
                        itemElement.SetAttribute("Name", Name);
                    }

                    if (!string.IsNullOrEmpty(AutomationId))
                    {
                        itemElement.SetAttribute("AutomationId", AutomationId);
                    }

                    if (!string.IsNullOrEmpty(UserDefineId))
                    {
                        itemElement.SetAttribute("UserDefineId", UserDefineId);
                    }

                    if (!string.IsNullOrEmpty(ClassName))
                    {
                        itemElement.SetAttribute("ClassName", ClassName);
                    }

                    if (!string.IsNullOrEmpty(Role))
                    {
                        itemElement.SetAttribute("Role", Role);
                    }


                    if (!string.IsNullOrEmpty(Description))
                    {
                        itemElement.SetAttribute("Description", Description);
                    }

                    
                    if (uiNode.IsTopLevelWindow)
                    {
                        if (!string.IsNullOrEmpty(ProcessName))
                        {
                            itemElement.SetAttribute("ProcessName", ProcessName);
                        }
                    }


                    //没有任何属性时，Idx
                    if (!itemElement.HasAttributes)
                    {
                        if (!string.IsNullOrEmpty(Idx))
                        {
                            itemElement.SetAttribute("Idx", Idx);
                        }
                    }

                    //}}

                    cachedId = itemElement.OuterXml;
                }


                return cachedId;
            }
        }

        public bool IsTopLevelWindow
        {
            get
            {
                return uiNode.IsTopLevelWindow;
            }
        }

        public UiElement DirectTopLevelWindow
        {
            get
            {
                if (cachedDirectTopLevelWindow == null)
                {
                    if (this.IsTopLevelWindow)
                    {
                        cachedDirectTopLevelWindow = this;
                    }
                    else
                    {
                        UiElement topLevelWindowToFind = this.Parent;
                        while (true)
                        {
                            if (topLevelWindowToFind != null)
                            {
                                if (topLevelWindowToFind.IsTopLevelWindow)
                                {
                                    cachedDirectTopLevelWindow = topLevelWindowToFind;
                                    break;
                                }

                                topLevelWindowToFind = topLevelWindowToFind.Parent;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }


                return cachedDirectTopLevelWindow;
            }
        }

        internal static Bitmap CaptureDesktop()
        {
            return UIAUiNode.UIAAutomation.GetDesktop().Capture();
        }


        /// <summary>
        /// 当前元素对应的Handle句柄
        /// </summary>
        public IntPtr WindowHandle
        {
            get
            {
                return uiNode.WindowHandle;
            }
        }


        public string GlobalId
        {
            get
            {
                //递归获取父节点的Id和自己的Id结合起来以组成全局ID
                return Parent == null ? this.Id : Parent.GlobalId + this.Id;
            }
        }

        public string Selector
        {
            get
            {
                return GlobalId.Replace("\"", "\'");//双引号变单引号，以便被""引用
            }
        }

        public string GlobalIdStyled
        {
            get
            {
                //递归获取父节点的Id和自己的Id结合起来以组成全局ID
                return Parent == null ? this.Id : Parent.GlobalIdStyled + Environment.NewLine + this.Id;
            }
        }

        public Rectangle BoundingRectangle
        {
            get
            {
                return this.boundingRectangle;
            }
            set
            {
                this.boundingRectangle = value;
            }
        }


        /// <summary>
        /// 返回封装的Native对象
        /// </summary>
        public object NativeObject
        {
            get
            {
                if(uiNode is UIAUiNode)
                {
                    return (uiNode as UIAUiNode).automationElement;
                }

                if (uiNode is JavaUiNode)
                {
                    return (uiNode as JavaUiNode).accessibleNode;
                }

                return null;
            }
            
        }

        /// <summary>
        /// 是否NativeObject是UIA的AutomationElement元素
        /// </summary>
        public bool IsNativeObjectAutomationElement
        {
            get
            {
                return uiNode is UIAUiNode;
            }
        }

        /// <summary>
        /// 是否NativeObject是JAVA的AccessibleNode元素
        /// </summary>
        public bool IsNativeObjectAccessibleNode
        {
            get
            {
                return uiNode is JavaUiNode;
            }
        }

        public static UiElement FromSelector(string selector)
        {
            if (!string.IsNullOrEmpty(selector))
            {
                UiElement ret = null;
                var globalId = selector.Replace("\'", "\"");

                //多次尝试
                for (int nRetry = 0; nRetry < 10; nRetry++)
                {
                    ret = FromGlobalId(globalId);

                    if (ret != null)
                    {
                        break;
                    }

                    Thread.Sleep(500);
                }

                return ret;
            }

            return null;
        }

        public static UiElement FromGlobalId(string globalId)
        {
            //根据GlobalId返回UiElement元素
            //解析XML，按顺序搜索，直到找到
            globalId = globalId.Replace(Environment.NewLine, "");
            XmlDocument xmlDoc = new XmlDocument();

            XmlElement rootXmlElement = xmlDoc.CreateElement("Root");
            xmlDoc.AppendChild(rootXmlElement);
            rootXmlElement.InnerXml = globalId;

            UiElement elementToFind = null;
            findUiElement(Desktop, rootXmlElement, ref elementToFind);

            return elementToFind;
        }


        // uiElement和子节点中去查找xmlElement对应的匹配的元素
        private static bool findUiElement(UiElement uiElement, XmlElement _xmlElement, ref UiElement elementToFind)
        {
            var xmlElement = _xmlElement.CloneNode(true) as XmlElement;

            if (isUiElementMatch(uiElement, xmlElement.FirstChild as XmlElement))
            {
                //xmlElement减去第一个xml节点
                xmlElement.RemoveChild(xmlElement.FirstChild);

                if (!xmlElement.HasChildNodes)
                {
                    //XML所有节点已经搜索完了
                    elementToFind = uiElement;
                    return true;
                }

                //xmlElement如果有idx属性，则优先跳到Idx对应的节点进行搜索
                if ((xmlElement.FirstChild as XmlElement).HasAttribute("Idx"))
                {
                    var idxStr = (xmlElement.FirstChild as XmlElement).Attributes["Idx"].Value;
                    var idx = Convert.ToInt32(idxStr);

                    var element = uiElement.GetUiElementByIdx(idx);
                    if (element != null)
                    {
                        if (findUiElement(element, xmlElement, ref elementToFind))
                        {
                            return elementToFind != null;
                        }
                    }
                }

                foreach (var item in uiElement.Children)
                {
                    if (findUiElement(item, xmlElement, ref elementToFind))
                    {
                        break;
                    }
                }
            }

            return elementToFind != null;
        }

        private UiElement GetUiElementByIdx(int idx)
        {
            var item = uiNode.GetChildByIdx(idx);
            return new UiElement(item, this);
        }


        public Bitmap CaptureInformativeScreenshot()
        {
            currentDesktopScreenshot = CaptureDesktop();

            if (uiNode.BoundingRectangle.IsEmpty)
            {
                //QQ截取时会出现为空的情况
                return null;
            }

            Bitmap target = drawInformativeOnUiNode(currentDesktopScreenshot);

            return target;
        }

        private Bitmap drawInformativeOnUiNode(Bitmap currentDesktopScreenshot)
        {
            using (Graphics g = Graphics.FromImage(currentDesktopScreenshot))
            {
                //最终截图的红色标记边框
                Pen pen = new Pen(Color.Red, 2);
                g.DrawRectangle(pen, uiNode.BoundingRectangle);
            }

            Rectangle cropRect = uiNode.BoundingRectangle;
            this.boundingRectangle = uiNode.BoundingRectangle;
            cropRect.Inflate(100, 50);
            Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);

            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(currentDesktopScreenshot, new Rectangle(0, 0, target.Width, target.Height),
                                 cropRect,
                                 GraphicsUnit.Pixel);
            }

            return target;
        }

        /// <summary>
        /// 截取提示性的信息，会扩大截取范围，并带有指示框，以方便观察
        /// </summary>
        /// <param name="filePath">不提供的话程序自动生成全局唯一名字，并在项目当前目录下的.screenshots目录下生成</param>
        public string CaptureInformativeScreenshotToFile(string filePath = null)
        {
            var ret = "";
            if (filePath == null)
            {
                var guid = Guid.NewGuid().ToString("N");

                var screenshotsPath = SharedObject.Instance.ProjectPath + @"\.screenshots";
                if (!System.IO.Directory.Exists(screenshotsPath))
                {
                    System.IO.Directory.CreateDirectory(screenshotsPath);
                }

                var fileName = guid + @".png";
                ret = fileName;
                filePath = screenshotsPath + @"\" + fileName;
            }

            if(currentInformativeScreenshot == null)
            {
                //之前生成框选元素图时出错，需要重新生成
                currentInformativeScreenshot = drawInformativeOnUiNode(currentDesktopScreenshot);
            }

            currentInformativeScreenshot.Save(filePath);

            return ret;
        }

        /// <summary>
        /// 元素方式高亮
        /// </summary>
        public static void StartElementHighlight()
        {
            StartHighlight(false);
        }

        /// <summary>
        /// 只高亮顶层WINDOW窗口
        /// </summary>
        public static void StartWindowHighlight()
        {
            StartHighlight(true);
        }

        private static void StartHighlight(bool isWindowHighlight)
        {
            if(!UiElement.IsRecordingWindowOpened)
            {
                System.Windows.Application.Current.MainWindow.WindowState = System.Windows.WindowState.Minimized;
            }
            

            overlayForm.IsWindowHighlight = isWindowHighlight;
            overlayForm.StartHighlight();
        }

        #region 元素操作事件

        /// <summary>
        /// 高亮一个元素
        /// </summary>
        /// <param name="color">高亮的颜色</param>
        /// <param name="duration">延迟时间</param>
        /// <param name="blocking">是否堵塞</param>
        public void DrawHighlight(Color? color = null, TimeSpan? duration = null, bool blocking = false)
        {
            var colorName = color ?? Color.Red;
            var rectangle = uiNode.BoundingRectangle;
            this.BoundingRectangle = uiNode.BoundingRectangle;
            if (!rectangle.IsEmpty)
            {
                var durationInMs = (int)(duration ?? TimeSpan.FromSeconds(2)).TotalMilliseconds;

                var overlayManager = new WinFormsOverlayManager();
                if (blocking)
                {
                    overlayManager.ShowBlocking(rectangle, colorName, durationInMs);
                }
                else
                {
                    overlayManager.Show(rectangle, colorName, durationInMs);
                }
            }
        }

        public void SetForeground()
        {
            var directWindow = DirectTopLevelWindow;
            if (directWindow != null)
            {
                UiCommon.ForceShow(directWindow.WindowHandle);
                DirectTopLevelWindow.uiNode.SetForeground();
            }

        }


        //根据相对坐标偏移量获取元素
        public UiElement FindRelativeElement(int position, int offsetX, int offsetY)
        {
            UiNode relativeNode = uiNode.FindRelativeNode(position, offsetX, offsetY);
            UiElement relativeElement = new UiElement(relativeNode);
            return relativeElement;
        }

        //根据过滤字符串来查找元素结点
        public List<UiElement> FindAllByFilter(TreeScope scope, ConditionBase condition, string filterStr)
        {
            List<UiElement> uiList = new List<UiElement>();
            List<UiElement> foundUiList = new List<UiElement>();

            filterStr = filterStr.Replace(Environment.NewLine, "");
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement rootXmlElement = xmlDoc.CreateElement("Root");
            xmlDoc.AppendChild(rootXmlElement);
            rootXmlElement.InnerXml = filterStr;

            var filterElement = rootXmlElement.CloneNode(true) as XmlElement;
            foundUiList = FindAll(scope, condition);

            foreach(var element in foundUiList)
            {
                if (isUiElementMatch(element as UiElement, filterElement))
                {
                    uiList.Add(element);
                }
            }         
            return uiList;
        }


        public List<UiElement> FindAll(TreeScope scope, ConditionBase condition)
        {
            List<UiElement> uiList = new List<UiElement>();

            var list = new List<UiElement>();
            var children = uiNode.FindAll(scope, condition);
            foreach (var item in children)
            {
                list.Add(new UiElement(item, this));
            }
            return list;
        }

        public Point GetClickablePoint()
        {
            return uiNode.GetClickablePoint();
        }

        public void Focus()
        {
            uiNode.Focus();
        }

        #endregion


        #region 键盘操作事件
        public static void KeyboardPress(VirtualKey virtualKey)
        {
            Keyboard.Press((VirtualKeyShort)virtualKey);
        }

        public static void KeyboardRelease(VirtualKey virtualKey)
        {
            Keyboard.Release((VirtualKeyShort)virtualKey);
        }

        public static void KeyboardType(string text)
        {
            Keyboard.Type(text);
        }
        #endregion


        #region 鼠标操作事件
        public void MouseClick(UiElementClickParams clickParams = null)
        {
            SetForeground();
            uiNode.MouseClick(clickParams);
        }

        public void MouseDoubleClick(UiElementClickParams clickParams = null)
        {
            SetForeground();
            uiNode.MouseDoubleClick(clickParams);
        }

        public void MouseRightClick(UiElementClickParams clickParams = null)
        {
            SetForeground();
            uiNode.MouseRightClick(clickParams);
        }

        public void MouseRightDoubleClick(UiElementClickParams clickParams = null)
        {
            SetForeground();
            uiNode.MouseRightDoubleClick(clickParams);
        }

        public void MouseHover(UiElementHoverParams hoverParams = null)
        {
            SetForeground();
            uiNode.MouseHover(hoverParams);
        }

        public static void MouseDrag(MouseButton mouseButton, Point startingPoint, Point endingPoint)
        {
            Mouse.Drag(startingPoint, endingPoint.X-startingPoint.X,endingPoint.Y-startingPoint.Y, (FlaUI.Core.Input.MouseButton)mouseButton);
        }

        public static void MouseUp(MouseButton mouseButton)
        {
            Mouse.Up((FlaUI.Core.Input.MouseButton)mouseButton);
        }

        public static void MouseDown(MouseButton mouseButton)
        {
            Mouse.Down((FlaUI.Core.Input.MouseButton)mouseButton);
        }

        public static void MouseMoveTo(Point newPosition)
        {
            Mouse.MoveTo(newPosition);
        }

        public static void MouseMoveTo(int newX, int newY)
        {
            Mouse.MoveTo(newX, newY);
        }

        public static void MouseSetPostion(Point newPosition)
        {
            Mouse.Position = newPosition;
        }
        public static void MouseSetPostion(int newX, int newY)
        {
            Mouse.Position = new Point(newX,newY);
        }


        public static void MouseAction(ClickType clickType, MouseButton mouseButton)
        {
            switch (clickType)
            {
                case ClickType.Single:
                    MouseClick(mouseButton);
                    break;
                case ClickType.Double: 
                    MouseDoubleClick(mouseButton);
                    break;
                case ClickType.Down:
                    MouseDown(mouseButton);
                    break;
                case ClickType.Up:
                    MouseUp(mouseButton);
                    break;
                default:
                    break;
            }
        }

        public static void MouseVerticalScroll(double lines)
        {
            Mouse.Scroll(lines);
        }

        public static void MouseHorizontalScroll(double lines)
        {
            Mouse.HorizontalScroll(lines);
        }

        public static void MouseClick(MouseButton mouseButton)
        {
            Mouse.Click((FlaUI.Core.Input.MouseButton)mouseButton);
        }

        public static void MouseDoubleClick(MouseButton mouseButton)
        {
            Mouse.DoubleClick((FlaUI.Core.Input.MouseButton)mouseButton);
        }

        public static void MouseLeftClick()
        {
            Mouse.LeftClick();
        }

        public static void MouseRightClick()
        {
            Mouse.RightClick();
        }

        public static void MouseLeftDoubleClick()
        {
            Mouse.LeftDoubleClick();
        }

        public static void MouseRightDoubleClick()
        {
            Mouse.RightDoubleClick();
        }


        #endregion


    }





}

