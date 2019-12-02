using System;
using System.Activities.Presentation.PropertyEditing;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace RPA.UIAutomation.Activities.Mouse
{
    public class MouseButtonTypeEditor : PropertyValueEditor
    {
        public MouseButtonTypeEditor()
        {
            this.InlineEditorTemplate = new DataTemplate();
            FrameworkElementFactory stack = new FrameworkElementFactory(typeof(StackPanel));
            FrameworkElementFactory comBox = new FrameworkElementFactory(typeof(ComboBox));
            Binding bindEnum = new Binding("Value");
            comBox.SetValue(ComboBox.ItemsSourceProperty, MouseClickTypes);
            comBox.SetValue(ComboBox.SelectedIndexProperty, bindEnum);
            stack.AppendChild(comBox);
            this.InlineEditorTemplate.VisualTree = stack;
        }

        public IList<MouseButtonType> MouseClickTypes
        {
            get
            {
                return Enum.GetValues(typeof(MouseButtonType)).Cast<MouseButtonType>().ToList<MouseButtonType>();
            }
        }
    }
}
