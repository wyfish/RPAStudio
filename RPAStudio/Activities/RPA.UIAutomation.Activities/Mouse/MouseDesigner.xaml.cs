using Plugins.Shared.Library;
using Plugins.Shared.Library.UiAutomation;
using Plugins.Shared.Library.Window;
using System;
using System.Activities;
using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;


namespace RPA.UIAutomation.Activities.Mouse
{
    // MouseDesigner.xaml 的交互逻辑
    public partial class MouseDesigner
    {
        public MouseDesigner()
        {
            InitializeComponent();
        }

        private void HyperlinkClick(object sender, RoutedEventArgs e)
        {
            UiElement.OnSelected = UiElement_OnSelected;
            UiElement.StartElementHighlight();
        }

        private void UiElement_OnSelected(UiElement uiElement)
        {
            // Get the window title to identify the element from Name/AutomationId
            string title = UIAutomationCommon.GetRootWindowTitle(UiCommon.GetForegroundWindow());
            setPropertyValue("WindowTitle", new InArgument<string>(title));

            var screenshotsPath = uiElement.CaptureInformativeScreenshotToFile();
            setPropertyValue("SourceImgPath", screenshotsPath);
            setPropertyValue("Name", new InArgument<string>(uiElement.Name));
            setPropertyValue("AutomationId", new InArgument<string>(uiElement.AutomationId));
            setPropertyValue("Selector", new InArgument<string>(uiElement.Selector));
            grid1.Visibility = System.Windows.Visibility.Hidden;
            setPropertyValue("visibility", System.Windows.Visibility.Visible);
            InArgument<Int32> _offsetX = uiElement.GetClickablePoint().X;
            InArgument<Int32> _offsetY = uiElement.GetClickablePoint().Y;
            setPropertyValue("offsetX", _offsetX);
            setPropertyValue("offsetY", _offsetY);
            string displayName = getPropertyValue("_DisplayName") + " \"" + uiElement.ProcessName + " " + uiElement.Name + "\"";
            setPropertyValue("DisplayName", displayName);
            InArgument<Int32> _Left = uiElement.BoundingRectangle.Left;
            setPropertyValue("Left", _Left);
            InArgument<Int32> _Right = uiElement.BoundingRectangle.Right;
            setPropertyValue("Right", _Right);
            InArgument<Int32> _Top = uiElement.BoundingRectangle.Top;
            setPropertyValue("Top", _Top);
            InArgument<Int32> _Bottom = uiElement.BoundingRectangle.Bottom;
            setPropertyValue("Bottom", _Bottom);
        }

        private void setPropertyValue<T>(string propertyName, T value)
        {
            base.ModelItem.Properties[propertyName].SetValue(value);
        }

        private string getPropertyValue(string propertyName)
        {
            ModelProperty _property = base.ModelItem.Properties[propertyName];
            if (_property.Value == null)
                return "";
            return _property.Value.ToString();
        }

        private void HiddenNavigateTextBlock()
        {
            navigateTextBlock.Visibility = System.Windows.Visibility.Hidden;
        }

        //菜单按钮点击
        private void NavigateButtonClick(object sender, RoutedEventArgs e)
        {
            contextMenu.PlacementTarget = this.navigateButton;
            contextMenu.Placement = PlacementMode.Top;
            contextMenu.IsOpen = true;
        }

        //菜单按钮初始化
        private void NavigateButtonInitialized(object sender, EventArgs e)
        {
            navigateButton.ContextMenu = null;
        }

        //菜单项点击测试
        private void meauItemClickOne(object sender, RoutedEventArgs e)
        {
            UiElement.OnSelected = UiElement_OnSelected;//也可在程序安装始化时赋值
            UiElement.StartElementHighlight();
        }

        private void Button_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            string src = getPropertyValue("SourceImgPath");
            ShowImageWindow imgShow = new ShowImageWindow();
            imgShow.ShowImage(src);
        }

        private void ActivityDesigner_Loaded(object sender, RoutedEventArgs e)
        {
            string src = getPropertyValue("SourceImgPath");
            if(src != "")
                grid1.Visibility = System.Windows.Visibility.Hidden;
        }
    }
}
