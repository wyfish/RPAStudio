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

namespace RPA.UIAutomation.Activities.Browser
{
    public partial class SetWebAttrDesigner
    {
        public SetWebAttrDesigner()
        {
            InitializeComponent();
            Net.Surviveplus.Localization.WpfLocalization.ApplyResources(this, Properties.Resources.ResourceManager);
        }

        private void meauItemClickOne(object sender, RoutedEventArgs e)
        {
            UiElement.OnSelected = UiElement_OnSelected;//也可在程序安装始化时赋值
            UiElement.StartElementHighlight();
        }

        private void NavigateButtonClick(object sender, RoutedEventArgs e)
        {
            contextMenu.PlacementTarget = this.navigateButton;
            contextMenu.Placement = PlacementMode.Top;
            contextMenu.IsOpen = true;
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
            navigateTextBlock.Visibility = System.Windows.Visibility.Hidden;
            setPropertyValue("visibility", System.Windows.Visibility.Visible);
            string displayName = getPropertyValue("_DisplayName") + " \"" + uiElement.ProcessName + " " + uiElement.Name + "\"";
            setPropertyValue("DisplayName", displayName);
        }


        private void setPropertyValue<T>(string propertyName, T value)
        {
            //异步执行,不等待委托结束就更新
            Dispatcher.BeginInvoke(new Action(delegate
            {
                List<ModelProperty> PropertyList = ModelItem.Properties.ToList();
                ModelProperty _property = PropertyList.Find((ModelProperty property) => property.Name.Equals(propertyName));
                _property.SetValue(value);
            }));
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

        }

        private void NavigateButtonInitialized(object sender, EventArgs e)
        {
            navigateButton.ContextMenu = null;
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
