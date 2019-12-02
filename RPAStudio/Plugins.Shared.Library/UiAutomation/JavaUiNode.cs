using FlaUI.Core.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using WindowsAccessBridgeInterop;
using FlaUI.Core.Conditions;
using FlaUI.Core.Definitions;

namespace Plugins.Shared.Library.UiAutomation
{
    class JavaUiNode : UiNode
    {
        internal static readonly AccessBridge accessBridge = new AccessBridge();
        private static List<AccessibleJvm> cachedJvms;
        internal AccessibleNode accessibleNode;

        private Dictionary<string, string> propDict = new Dictionary<string, string>();
        private Dictionary<string, object> propObjectDict = new Dictionary<string, object>();

        private UiNode cachedParent;

        static JavaUiNode()
        {
            try
            {
                accessBridge.Initialize();
            }
            catch (Exception)
            {
               
            }
        }

        public JavaUiNode(AccessibleNode accessibleNode)
        {
            this.accessibleNode = accessibleNode;

            var propertyList = accessibleNode.GetProperties(PropertyOptions.AccessibleContextInfo | PropertyOptions.ObjectDepth);
            foreach (var item in propertyList)
            {
                if (item.Name != null)
                {
                    propObjectDict[item.Name] = item.Value;
                    propDict[item.Name] = item.Value == null ? "" : item.Value.ToString();
                }
            }
        }
     

        public string AutomationId
        {
            get
            {
                return "";
            }
        }

        public string UserDefineId
        {
            get
            {
                return propDict["Object Depth"] + "-" + propDict["Index in parent"] + "-" + propDict["Children count"];
            }
        }

        public string Idx
        {
            get
            {
                return "";
            }
        }


        //JAVA因为不需要，所以暂时不实现该接口
        public UiNode GetChildByIdx(int idx)
        {
            return null;
        }

        public Rectangle BoundingRectangle
        {
            get
            {
                Rectangle rect = accessibleNode.GetScreenRectangle() ?? new Rectangle();
                return rect;
            }
        }

        public List<UiNode> Children
        {
            get
            {
                var list = new List<UiNode>();

                var children = accessibleNode.GetChildren();
                foreach(var child in children)
                {
                    list.Add(new JavaUiNode(child));
                }

                return list;
            }
        }

        public string ClassName
        {
            get
            {
                return "";
            }
        }

        public string ControlType
        {
            get
            {
                return "JavaNode";
            }
        }

        public bool IsTopLevelWindow
        {
            get
            {
                return WindowHandle != IntPtr.Zero;
            }
        }

        public string Name
        {
            get
            {
                if (propDict.ContainsKey("Name"))
                {
                    return propDict["Name"];
                }

                return "";
            }
        }

        public UiNode Parent
        {
            get
            {
                if(cachedParent == null)
                {
                    var parent = accessibleNode.GetParent();
                    if (parent != null)
                    {
                        //如果parent含有WindowHandle属性，则说明到了顶层窗口，改用UIA窗口结点
                        UiNode parentUiNode = new JavaUiNode(accessibleNode.GetParent());
                        if (parentUiNode.IsTopLevelWindow)
                        {
                            parentUiNode = new UIAUiNode(UIAUiNode.UIAAutomation.FromHandle(parentUiNode.WindowHandle));
                        }

                        cachedParent = parentUiNode;
                    }
                }

                return cachedParent;
            }
        }

        public string ProcessName
        {
            get
            {
                return "";
            }
        }

        public string ProcessFullPath
        {
            get
            {
                return "";
            }
        }

        public string Role
        {
            get
            {
                if(propDict.ContainsKey("Role"))
                {
                    return propDict["Role"];
                }

                return "";
            }
        }

        public IntPtr WindowHandle
        {
            get
            {
                if (propDict.ContainsKey("WindowHandle"))
                {
                    return (IntPtr)Convert.ToInt32(propDict["WindowHandle"]);
                }

                return IntPtr.Zero;
            }
        }

        public string Description
        {
            get
            {
                if (propDict.ContainsKey("Description"))
                {
                    return propDict["Description"];
                }

                return "";
            }
        }

        public UiNode AutomationElementParent
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        internal static List<AccessibleJvm> EnumJvms(bool bRefresh = false)
        {
            if(bRefresh)
            {
                cachedJvms = null;
            }

            if(cachedJvms == null || cachedJvms.Count == 0)
            {
                cachedJvms = accessBridge.EnumJvms(hwnd => accessBridge.CreateAccessibleWindow(hwnd));
            }
            return cachedJvms;
        }

        public void SetForeground()
        {
            //此处其实不会走进来，因为JAVA元素的直接父窗口为UIAUiNode
            if(WindowHandle != IntPtr.Zero)
            {
                UiCommon.SetForegroundWindow(WindowHandle);
            }
        }

        public void MouseClick(UiElementClickParams clickParams = null)
        {
            if (clickParams == null)
            {
                clickParams = new UiElementClickParams();
            }

            var clickablePoint = GetClickablePoint();
            if (clickParams.moveMouse)
            {
                Mouse.MoveTo(clickablePoint);
            }
            else
            {
                Mouse.Position = clickablePoint;
            }

            Mouse.LeftClick();
        }

        public void MouseDoubleClick(UiElementClickParams clickParams = null)
        {
            if (clickParams == null)
            {
                clickParams = new UiElementClickParams();
            }

            var clickablePoint = GetClickablePoint();
            if (clickParams.moveMouse)
            {
                Mouse.MoveTo(clickablePoint);
            }
            else
            {
                Mouse.Position = clickablePoint;
            }

            Mouse.LeftDoubleClick();
        }

        public void MouseRightClick(UiElementClickParams clickParams = null)
        {
            if (clickParams == null)
            {
                clickParams = new UiElementClickParams();
            }

            var clickablePoint = GetClickablePoint();
            if (clickParams.moveMouse)
            {
                Mouse.MoveTo(clickablePoint);
            }
            else
            {
                Mouse.Position = clickablePoint;
            }

            Mouse.RightClick();
        }

        public void MouseRightDoubleClick(UiElementClickParams clickParams = null)
        {
            if (clickParams == null)
            {
                clickParams = new UiElementClickParams();
            }

            var clickablePoint = GetClickablePoint();
            if (clickParams.moveMouse)
            {
                Mouse.MoveTo(clickablePoint);
            }
            else
            {
                Mouse.Position = clickablePoint;
            }

            Mouse.RightDoubleClick();
        }

        public void MouseHover(UiElementHoverParams hoverParams = null)
        {
            if (hoverParams == null)
            {
                hoverParams = new UiElementHoverParams();
            }

            var clickablePoint = GetClickablePoint();
            if (hoverParams.moveMouse)
            {
                Mouse.MoveTo(clickablePoint);
            }
            else
            {
                Mouse.Position = clickablePoint;
            }
        }

        public Point GetClickablePoint()
        {
            //TODO WJF JAVA窗口最小化时，程序恢复显示时，取坐标为负数，原因不明
            var point = new Point(BoundingRectangle.Left + BoundingRectangle.Width / 2, BoundingRectangle.Top + BoundingRectangle.Height / 2);
            return point;
        }

        public void Focus()
        {
            //TODO WJF 暂未实现
        }

        public List<UiNode> FindAll(TreeScope scope, ConditionBase condition)
        {
            throw new NotImplementedException();
        }

        public UiNode FindRelativeNode(int position, int offsetX, int offsetY)
        {
            throw new NotImplementedException();
        }
    }
}
