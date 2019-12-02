using System;
using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Activities;
using Plugins.Shared.Library.UiAutomation;
using Plugins.Shared.Library.Window;

namespace RPA.UIAutomation.Activities.Keyboard
{
    public partial class HotKeyDesigner
    {
        private string screenshotsPath { get; set; }
        public HotKeyDesigner()
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
            navigateTextBlock.Visibility = System.Windows.Visibility.Hidden;
            setPropertyValue("SourceImgPath", screenshotsPath);
            setPropertyValue("Selector", new InArgument<string>(uiElement.Selector));
            setPropertyValue("visibility", System.Windows.Visibility.Visible);
            InArgument<Int32> _offsetX = uiElement.GetClickablePoint().X;
            InArgument<Int32> _offsetY = uiElement.GetClickablePoint().Y;
            setPropertyValue("offsetX", _offsetX);
            setPropertyValue("offsetY", _offsetY);
            string displayName = getPropertyValue("_DisplayName") + " \"" + uiElement.ProcessName + " " + uiElement.Name + "\"";
            setPropertyValue("DisplayName", displayName);
        }

        private void NavigateButtonClick(object sender, RoutedEventArgs e)
        {
            contextMenu.PlacementTarget = this.navigateButton;
            contextMenu.Placement = PlacementMode.Top;
            contextMenu.IsOpen = true;
        }

        private void NavigateButtonInitialized(object sender, EventArgs e)
        {
            navigateButton.ContextMenu = null;
        }

        private void meauItemClickOne(object sender, RoutedEventArgs e)
        {
            UiElement.OnSelected = UiElement_OnSelected;//也可在程序安装始化时赋值
            UiElement.StartElementHighlight();
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

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (comboBox.SelectedItem.ToString().Equals("Special Keys"))
                    comboBox.SelectedIndex = -1;
            }
            catch (Exception)
            {
                return;
            }
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
                navigateTextBlock.Visibility = System.Windows.Visibility.Hidden;
        }
    }
}
