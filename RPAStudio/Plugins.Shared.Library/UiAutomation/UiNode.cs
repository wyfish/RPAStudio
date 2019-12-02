using FlaUI.Core.Conditions;
using FlaUI.Core.Definitions;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Plugins.Shared.Library.UiAutomation
{
    interface UiNode
    {
        //用户自定义Id，提供额外的标识信息，以辅助元素精确定位，目前主要为Java节点使用
        string UserDefineId { get; }

        //当节点无任何属性信息时，通过当前元素位置Idx(从0开始)来辅助快速定位，比如<Pane />这样的无属性节点
        string Idx { get; }

        string AutomationId { get; }
        string ClassName { get;}
        string ControlType { get; }
        string Name { get; }
        string Role { get; }

        string Description { get; }

        string ProcessName { get; }
        string ProcessFullPath { get; }

        IntPtr WindowHandle { get; }
        UiNode Parent { get; }
        UiNode AutomationElementParent { get; }
        Rectangle BoundingRectangle { get;}

        List<UiNode> Children { get; }

        bool IsTopLevelWindow { get;}

        UiNode FindRelativeNode(int position, int offsetX, int offsetY);
        List<UiNode> FindAll(TreeScope scope, ConditionBase condition);
        void MouseClick(UiElementClickParams clickParams = null);
        void MouseDoubleClick(UiElementClickParams clickParams = null);
        void MouseRightClick(UiElementClickParams clickParams = null);
        void MouseRightDoubleClick(UiElementClickParams clickParams = null);
        void MouseHover(UiElementHoverParams hoverParams = null);

        void Focus();

        void SetForeground();
        Point GetClickablePoint();
        UiNode GetChildByIdx(int idx);
    }
}
