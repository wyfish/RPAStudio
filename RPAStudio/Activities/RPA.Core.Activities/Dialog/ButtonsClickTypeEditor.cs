using System;
using System.Activities.Presentation.PropertyEditing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace RPA.Core.Activities.DialogActivity.TypeEditor
{
    class ButtonsClickTypeEditor : PropertyValueEditor
    {

        public ButtonsClickTypeEditor()
        {
            this.InlineEditorTemplate = new DataTemplate();
            FrameworkElementFactory stack = new FrameworkElementFactory(typeof(StackPanel));
            FrameworkElementFactory comBox = new FrameworkElementFactory(typeof(ComboBox));
            Binding bindEnum = new Binding("Value");
            comBox.SetValue(ComboBox.ItemsSourceProperty, ClickTypes);
            comBox.SetValue(ComboBox.SelectedIndexProperty, bindEnum);
            stack.AppendChild(comBox);
            this.InlineEditorTemplate.VisualTree = stack;
        }

        public IList<MessageBoxButton> ClickTypes
        {
            get
            {
                return Enum.GetValues(typeof(MessageBoxButton)).Cast<MessageBoxButton>().ToList<MessageBoxButton>();
            }
        }
    }
}
