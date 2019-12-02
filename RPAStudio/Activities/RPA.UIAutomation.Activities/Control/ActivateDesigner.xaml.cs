using System;
using System.Activities.Presentation.Model;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Activities;
using Plugins.Shared.Library.Window;
using Plugins.Shared.Library.UiAutomation;

namespace RPA.UIAutomation.Activities.Control
{
    public partial class ActivateDesigner
    {
        public ActivateDesigner()
        {
            InitializeComponent();
        }

        private void HyperlinkClick(object sender, RoutedEventArgs e)
        {
            UiElement.OnSelected = UiElement_OnSelected;//也可在程序安装始化时赋值
            UiElement.StartElementHighlight();
        }

        private void UiElement_OnSelected(UiElement uiElement)
        {
            var screenshotsPath = uiElement.CaptureInformativeScreenshotToFile();
            setPropertyValue("SourceImgPath", screenshotsPath);
            setPropertyValue("Selector", new InArgument<string>(uiElement.Selector));
            grid1.Visibility = System.Windows.Visibility.Hidden;
            setPropertyValue("visibility", System.Windows.Visibility.Visible);
            string displayName = getPropertyValue("_DisplayName") + " \"" + uiElement.ProcessName + " " + uiElement.Name + "\"";
            setPropertyValue("DisplayName", displayName);
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
            if (src != "")
                grid1.Visibility = System.Windows.Visibility.Hidden;
        }
    }
}
