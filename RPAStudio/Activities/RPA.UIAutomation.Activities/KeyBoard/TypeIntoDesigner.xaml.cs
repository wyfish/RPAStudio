using System;
using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Activities;
using Microsoft.VisualBasic.Activities;
using System.Activities.Expressions;
using Plugins.Shared.Library.UiAutomation;
using Plugins.Shared.Library.Window;

namespace RPA.UIAutomation.Activities.Keyboard
{
    public partial class TypeIntoDesigner
    {
        private string screenshotsPath { get; set; }
        public TypeIntoDesigner()
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
            // Get the window title to identify the element from Name/AutomationId
            string title = UIAutomationCommon.GetRootWindowTitle(UiCommon.GetForegroundWindow());
            setPropertyValue("WindowTitle", new InArgument<string>(title));

            var screenshotsPath = uiElement.CaptureInformativeScreenshotToFile();
            navigateTextBlock.Visibility = System.Windows.Visibility.Hidden;
            setPropertyValue("SourceImgPath", screenshotsPath);
            setPropertyValue("Name", new InArgument<string>(uiElement.Name));
            setPropertyValue("AutomationId", new InArgument<string>(uiElement.AutomationId));
            setPropertyValue("Selector", new InArgument<string>(uiElement.Selector));
            setPropertyValue("visibility", System.Windows.Visibility.Visible);
            InArgument<Int32> _offsetX = uiElement.GetClickablePoint().X;
            InArgument<Int32> _offsetY = uiElement.GetClickablePoint().Y;
            setPropertyValue("offsetX", _offsetX);
            setPropertyValue("offsetY", _offsetY);
            string displayName = getPropertyValue("DisplayName") + " \"" + uiElement.ProcessName + " " + uiElement.Name + "\"";
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
            object selectedValue = (e.OriginalSource as ComboBox).SelectedValue;
            if (selectedValue == null)
            {
                return;
            }
            string text = selectedValue.ToString().TrimStart(' ');
            if (text == "Special Keys")
            {
                return;
            }
            InArgument<string> inArgument = base.ModelItem.Properties["Text"].ComputedValue as InArgument<string>;
            if (inArgument == null || inArgument.Expression == null)
            {
                base.ModelItem.Properties["Text"].SetValue(new InArgument<string>("[k(" + text + ")]"));
            }
            else if (inArgument.Expression.GetType().Equals(typeof(VisualBasicValue<string>)))
            {
                VisualBasicValue<string> visualBasicValue = inArgument.Expression as VisualBasicValue<string>;
                if (visualBasicValue == null || string.IsNullOrWhiteSpace(visualBasicValue.ExpressionText))
                {
                    base.ModelItem.Properties["Text"].SetValue(new InArgument<string>("[k(" + text + ")]"));
                    return;
                }
                string expressionText = visualBasicValue.ExpressionText;
                expressionText = ((expressionText[expressionText.Length - 1] != '"') ? (expressionText + "+ \"[k(" + text + ")]\"") : (expressionText.Substring(0, expressionText.Length - 1) + "[k(" + text + ")]\""));
                InArgument<string> inArgument2 = new InArgument<string>();
                inArgument2.Expression = new VisualBasicValue<string>(expressionText);
                base.ModelItem.Properties["Text"].SetValue(inArgument2);
            }
            else if (inArgument.Expression.GetType().Equals(typeof(Literal<string>)))
            {
                Literal<string> literal = inArgument.Expression as Literal<string>;
                if (literal == null || string.IsNullOrEmpty(literal.Value))
                {
                    base.ModelItem.Properties["Text"].SetValue(new InArgument<string>("[k(" + text + ")]"));
                }
                else
                {
                    base.ModelItem.Properties["Text"].SetValue(new InArgument<string>(literal.Value + "[k(" + text + ")]"));
                }
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
