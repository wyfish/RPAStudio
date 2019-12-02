using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Shapes;


namespace RPA.Core.Activities.DataTableActivity.Dialog
{
    public partial class EditableComboBoxControl
    {
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(List<string>), typeof(EditableComboBoxControl));

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(string), typeof(EditableComboBoxControl));

        public List<string> ItemsSource
        {
            get
            {
                return GetValue(ItemsSourceProperty) as List<string>;
            }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        public bool? MultiSelect
        {
            get;
            set;
        }

        public string SelectedItem
        {
            get
            {
                return base.GetValue(EditableComboBoxControl.SelectedItemProperty) as string;
            }
            set
            {
                base.SetValue(EditableComboBoxControl.SelectedItemProperty, (value != null) ? value.Trim() : null);
            }
        }

        public EditableComboBoxControl()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.PropertiesComboBox.IsDropDownOpen = true;
        }

        private void ComboboxInputBox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            TextBox arg_1D_0 = this.ComboboxInputBox;
            string expr_11 = this.ComboboxInputBox.Text;
            arg_1D_0.Text = ((expr_11 != null) ? expr_11.Trim() : null);
        }

        private void PropertiesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
            {
                return;
            }
            if (this.MultiSelect.HasValue && this.MultiSelect.Value)
            {
                if (this.SelectedItem == null)
                {
                    this.SelectedItem = string.Empty;
                }
                if (this.PropertiesComboBox.SelectedValue != null && !this.SelectedItem.Contains(this.PropertiesComboBox.SelectedValue.ToString()))
                {
                    string arg_99_0 = "{0}[{1}]";
                    string expr_7D = this.SelectedItem;
                    this.SelectedItem = string.Format(arg_99_0, (expr_7D != null) ? expr_7D.Trim() : null, this.PropertiesComboBox.SelectedValue.ToString());
                }
            }
            else
            {
                this.SelectedItem = string.Empty;
                if (this.PropertiesComboBox.SelectedValue != null)
                {
                    this.SelectedItem = string.Format("[{0}]", this.PropertiesComboBox.SelectedValue.ToString());
                }
            }
            this.PropertiesComboBox.SelectedItem = null;
        }
    }
}
