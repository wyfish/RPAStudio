using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RPA.Core.Activities.DialogActivity.Windows
{
    /// <summary>
    /// InputDialogWindow.xaml 的交互逻辑
    /// </summary>
    /// 
    public class ResultValue : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _value;
        public string RValue
        {
            get { return _value; }
            set
            {
                _value = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("RValue"));
                }
            }
        }
    }

    public partial class InputDialogWindow : Window
    {
        private ResultValue _rValue;
        public InputDialogWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            _rValue = new ResultValue();
        }

        public void setTextContent(string _value)
        {
            TextContent.Text = _value;
        }

        public void CreateCombobox(string[] listItem)
        {
            cbBox.ItemsSource = listItem;
            cbBox.DataContext = _rValue;
            cbBox.Visibility = System.Windows.Visibility.Visible;
            cbBox.Focus();
        }

        public void CreateEditBox(bool bIsPassword)
        {
            if(bIsPassword)
            {
                CreatePassWordBox();
            }
            else
            {
                CreateTextBox();
            }
        }

        private void CreateTextBox()
        {
            textBox.DataContext = _rValue;
            textBox.Visibility = System.Windows.Visibility.Visible;
            textBox.Focus();
        }

        private void CreatePassWordBox()
        {
            passWordBox.DataContext = _rValue;
            passWordBox.Visibility = System.Windows.Visibility.Visible;
            passWordBox.Focus();
        }

        private List<CheckBox> listCheckBox = new List<CheckBox>();

        public void CreateCheckBox(string[] listItem)
        {
            for(int i = 0; i<listItem.Length;i++)
            {
                ChangeControlGrid.RowDefinitions.Add(new RowDefinition());
                CheckBox ckbox = new CheckBox();
                ckbox.Content = listItem[i];
                ckbox.Checked += new RoutedEventHandler(CheckBox_Click);
                ChangeControlGrid.Children.Add(ckbox);
                ckbox.SetValue(Grid.RowProperty,i);
                if (i == 0) ckbox.IsChecked = true;
                listCheckBox.Add(ckbox);
            }
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            var current = (CheckBox)sender;
            _rValue.RValue = current.Content.ToString();
            foreach (var checkBox in listCheckBox)
            {
                if (checkBox != current)
                {
                    checkBox.IsChecked = !current.IsChecked;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {  
           this.Hide();
        }

        public string getValue()
        {
            return _rValue.RValue;
        }
    }
}
 