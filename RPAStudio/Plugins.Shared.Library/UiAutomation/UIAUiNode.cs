using System;
using System.Collections.Generic;
using System.Drawing;
using FlaUI.UIA3;
using WindowsAccessBridgeInterop;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA2;
using FlaUI.Core;
using FlaUI.Core.Definitions;
using FlaUI.Core.Conditions;

namespace Plugins.Shared.Library.UiAutomation
{
    class UIAUiNode : UiNode
    {
        private static ITreeWalker _treeWalker;

        internal static AutomationBase uia3Automation = new UIA3Automation();
        internal static AutomationBase uia2Automation = new UIA2Automation();

        internal AutomationElement automationElement;

        //优化读取速度
        private string cachedProcessName;
        private string cachedProcessFullPath;
        private UIAUiNode cachedParent;
        private UIAUiNode automationElementParent;

        public UIAUiNode(AutomationElement element, UIAUiNode parent = null)
        {
            this.automationElement = element;
            this.cachedParent = parent;
        }

        public static AutomationBase UIAAutomation
        {
            get
            {
                //此处根据需求来确定用UIA2还是UIA3

                //目前测试来看UIA2兼容性强些，腾讯的TIM用UIA2展示相对正常
                return uia2Automation;
            }
        }

        private static ITreeWalker TreeWalker
        {
            get
            {
                if (_treeWalker == null)
                {
                    _treeWalker = UIAAutomation.TreeWalkerFactory.GetControlViewWalker();
                }

                return _treeWalker;
            }
        }

        public string AutomationId
        {
            get
            {
                try
                {
                    return automationElement.AutomationId;
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }

        public Rectangle BoundingRectangle
        {
            get
            {
                return automationElement.BoundingRectangle;
            }
        }

        public string ClassName
        {
            get
            {
                try
                {
                    return automationElement.ClassName;
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }

        public string ControlType
        {
            get
            {
                try
                {
                    return automationElement.ControlType.ToString();
                }
                catch (Exception)
                {

                    return "";
                }
            }
        }


        public string Name
        {
            get
            {
                try
                {
                    return automationElement.Name;
                }
                catch (Exception)
                {

                    return "";
                }
            }
        }

        public IntPtr WindowHandle
        {
            get
            {
                try
                {
                    if (automationElement.Properties.NativeWindowHandle.IsSupported)
                    {
                        var windowHandle = automationElement.Properties.NativeWindowHandle.ValueOrDefault;
                        return windowHandle;
                    }
                    else
                    {
                        return IntPtr.Zero;
                    }
                }
                catch (Exception)
                {
                    return IntPtr.Zero;
                }
                
            }
        }

        //此处可能会出现从鼠标点获取的元素和从Desktop往下获取的元素不一致的情况，根据实际情况有可能需要修正
        private AutomationElement getCorrectParent(AutomationElement element)
        {
            return TreeWalker.GetParent(element);
        }

        public UiNode Parent
        {
            get
            {
                if (cachedParent == null)
                {
                    var realParent = getCorrectParent(automationElement);

                    if (realParent != null)
                    {
                        cachedParent = new UIAUiNode(realParent);
                    }
                }

                return cachedParent;
            }
        }

        public UiNode AutomationElementParent
        {
            get
            {
                automationElementParent = new UIAUiNode(automationElement.Parent);
                return automationElementParent;
            }
        }




        public string Role
        {
            get
            {
                return "";
            }
        }

        public string ProcessName
        {
            get
            {
                if (cachedProcessName == null)
                {
                    try
                    {
                        var processId = automationElement.Properties.ProcessId.Value;
                        var name = UiCommon.GetProcessName(processId);

                        cachedProcessName = name;
                    }
                    catch (Exception)
                    {
                        cachedProcessName = "";
                    }
                }

                return cachedProcessName;
            }
        }

        public string ProcessFullPath
        {
            get
            {
                if (cachedProcessFullPath == null)
                {
                    try
                    {
                        var processId = automationElement.Properties.ProcessId.Value;
                        var path = UiCommon.GetProcessFullPath(processId);

                        cachedProcessFullPath = path;
                    }
                    catch (Exception)
                    {
                        cachedProcessFullPath = "";
                    }
                }

                return cachedProcessFullPath;
            }
        }

        public List<UiNode> Children
        {
            get
            {
                var list = new List<UiNode>();
                var children = automationElement.FindAllChildren();
                foreach (var item in children)
                {
                    list.Add(new UIAUiNode(item, this));
                }


                if (JavaUiNode.accessBridge.Functions.IsJavaWindow(this.WindowHandle))
                {
                    int vmid;
                    JavaObjectHandle ac;
                    JavaUiNode.accessBridge.Functions.GetAccessibleContextFromHWND(this.WindowHandle, out vmid, out ac);

                    if (!ac.IsNull)
                    {
                        var acNode = new AccessibleContextNode(JavaUiNode.accessBridge, ac);
                        var rootPanelNode = acNode.FetchChildNode(0);
                        list.Add(new JavaUiNode(rootPanelNode));
                    }
                }
                return list;
            }
        }


        public bool IsTopLevelWindow
        {
            get
            {
                try
                {
                    return automationElement.Parent != null && automationElement.Parent.Parent == null;
                    
                }
                catch (Exception)
                {
                    return false;
                }
               
            }
        }

        public string UserDefineId
        {
            get
            {
                return "";
            }
        }

        
        public string Description
        {
            get
            {

                try
                {
                    if (automationElement.Patterns.LegacyIAccessible.Pattern.Description.IsSupported)
                    {
                        return automationElement.Patterns.LegacyIAccessible.Pattern.Description.ValueOrDefault;
                    }
                    else
                    {
                        return "";
                    }
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }

        public string Idx
        {
            get
            {
                var realParent = getCorrectParent(automationElement);
                var children = realParent.FindAllChildren();

                int index = 0;

                foreach(var item in children)
                {
                    if(item.Equals(automationElement))
                    {
                        return index.ToString();
                    }

                    index++;
                }

                return "";
            }
        }

        public List<UiNode> FindAll(TreeScope scope, ConditionBase condition)
        {
            var list = new List<UiNode>();
            var elements = automationElement.FindAll(scope, condition);
            foreach (var item in elements)
            {
                list.Add(new UIAUiNode(item, this));
            }
            return list;
        }

        public UiNode FindRelativeNode(int position, int offsetX, int offsetY)
        {
            UIAUiNode relativeNode = null ;
            Rectangle rect = automationElement.BoundingRectangle;
            int posX, posY;
            Point point;
            switch (position)
            {
                case 0:     //中心位置偏移
                    posX = rect.Location.X + rect.Width / 2;
                    posY = rect.Location.Y + rect.Height / 2;
                    point = new Point(posX + offsetX, posY + offsetY);
                    break;
                case 1:     //左上角偏移
                    posX = rect.Location.X;
                    posY = rect.Location.Y;
                    point = new Point(posX + offsetX, posY + offsetY);
                    break;
                case 2:     //右上角偏移
                    posX = rect.Location.X + rect.Width;
                    posY = rect.Location.Y;
                    point = new Point(posX + offsetX, posY + offsetY);
                    break;
                case 3:     //左下角偏移
                    posX = rect.Location.X;
                    posY = rect.Location.Y + rect.Height;
                    point = new Point(posX + offsetX, posY + offsetY);
                    break;
                case 4:     //右下角偏移
                    posX = rect.Location.X + rect.Width;
                    posY = rect.Location.Y + rect.Height;
                    point = new Point(posX + offsetX, posY + offsetY);
                    break;
                default:    //默认中心偏移
                    posX = rect.Location.X + rect.Width / 2;
                    posY = rect.Location.Y + rect.Height / 2;
                    point = new Point(posX + offsetX, posY + offsetY);
                    break;
            }


            try
            {
                this.automationElement = UIAUiNode.UIAAutomation.FromPoint(point);
            }
            catch (Exception)
            {
                if (this.automationElement == null)
                {
                    IntPtr hWnd = UiCommon.WindowFromPoint(point);
                    if (hWnd != IntPtr.Zero)
                    {
                        this.automationElement = UIAUiNode.UIAAutomation.FromHandle(hWnd);
                    }
                }
            }
            relativeNode = new UIAUiNode(automationElement);
            return relativeNode;
        }


        public UiNode GetChildByIdx(int idx)
        {
            var item = automationElement.FindChildAt(idx);
            return new UIAUiNode(item, this);
        }

        public void MouseClick(UiElementClickParams clickParams = null)
        {
            if(clickParams == null)
            {
                clickParams = new UiElementClickParams();
            }

            try
            {
                automationElement.Click(clickParams.moveMouse);
            }
            catch (Exception)
            {
            }
        }

        public void MouseDoubleClick(UiElementClickParams clickParams = null)
        {
            if (clickParams == null)
            {
                clickParams = new UiElementClickParams();
            }

            try
            {
                automationElement.DoubleClick(clickParams.moveMouse);
            }
            catch (Exception)
            {
            }
        }

        public void MouseRightClick(UiElementClickParams clickParams = null)
        {
            if (clickParams == null)
            {
                clickParams = new UiElementClickParams();
            }

            try
            {
                automationElement.RightClick(clickParams.moveMouse);
            }
            catch (Exception)
            {
            }
        }

        public void MouseRightDoubleClick(UiElementClickParams clickParams = null)
        {
            if (clickParams == null)
            {
                clickParams = new UiElementClickParams();
            }

            try
            {
                automationElement.RightDoubleClick(clickParams.moveMouse);
            }
            catch (Exception)
            {
            }
        }

        public void SetForeground()
        {
            this.automationElement.SetForeground();
        }

        public void MouseHover(UiElementHoverParams hoverParams = null)
        {
            if (hoverParams == null)
            {
                hoverParams = new UiElementHoverParams();
            }

            try
            {
                var clickablePoint = GetClickablePoint();
                if(hoverParams.moveMouse)
                {
                    FlaUI.Core.Input.Mouse.MoveTo(clickablePoint);
                }
                
                FlaUI.Core.Input.Mouse.Position = clickablePoint;
            }
            catch (Exception)
            {

            }
            
        }

        public Point GetClickablePoint()
        {
            try
            {
                return automationElement.GetClickablePoint();
            }
            catch (Exception)
            {
                //取矩形中心点
                return new Point(BoundingRectangle.Left + BoundingRectangle.Width / 2,
                                    BoundingRectangle.Top + BoundingRectangle.Height / 2);
            }
        }

        public void Focus()
        {
            automationElement.Focus();
        }

       
    }
}
