using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace RPA.UIAutomation.Activities.Mouse
{
    /// <summary>
    /// ComboBoxWithCheckControl.xaml 的交互逻辑
    /// </summary>
    public partial class ComboBoxWithCheckControl : UserControl
    {
        public bool isAltChecked { get; set; }
        public bool isConotrolChecked { get; set; }
        public bool isShiftChecked { get; set; }
        public bool isWinChecked { get; set; }

        public string CheckedText
        {
            get { return (string)GetValue(CheckedTextProperty); }
            set { SetValue(CheckedTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CheckedText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CheckedTextProperty =
            DependencyProperty.Register("CheckedText", typeof(string), typeof(ComboBoxWithCheckControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public ComboBoxWithCheckControl()
        {
            InitializeComponent();
        }

        string getCheckedValue()
        {
            var list = new List<string>();
            if(isAltChecked)
            {
                list.Add("Alt");
            }

            if (isConotrolChecked)
            {
                list.Add("Ctrl");
            }

            if (isShiftChecked)
            {
                list.Add("Shift");
            }

            if (isWinChecked)
            {
                list.Add("Win");
            }
            string ret = string.Join(",", list.ToArray());
            return ret;
        }

        private void Alt_Checked(object sender, RoutedEventArgs e)
        {
            isAltChecked = !isAltChecked;
            CheckedText = getCheckedValue();
        }

        private void Ctrl_Checked(object sender, RoutedEventArgs e)
        {
            isConotrolChecked = !isConotrolChecked;
            CheckedText = getCheckedValue();
        }

        private void Shift_Checked(object sender, RoutedEventArgs e)
        {
            isShiftChecked = !isShiftChecked;
            CheckedText = getCheckedValue();
        }

        private void Win_Checked(object sender, RoutedEventArgs e)
        {
            isWinChecked = !isWinChecked;
            CheckedText = getCheckedValue();
        }

        private void ComboBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (CheckedText == null) return;
            string[] list = CheckedText.Split(',');
            var listModel = new List<string>();
            listModel.AddRange(list);
            cbAlt.IsChecked = false;
            cbCtrl.IsChecked = false;
            cbShift.IsChecked = false;
            cbWin.IsChecked = false;
            foreach (string item in list)
            {
                if (item == "Alt")
                {
                    cbAlt.IsChecked = true;
                    listModel.Remove(item);
                }
                else if (item == "Ctrl")
                {
                    cbCtrl.IsChecked = true;
                    listModel.Remove(item);
                }
                else if (item == "Shift")
                {
                    cbShift.IsChecked = true;
                    listModel.Remove(item);
                }
                else if (item == "Win")
                {
                    cbWin.IsChecked = true;
                    listModel.Remove(item);
                }
            }
            if (listModel.Count != 0 && CheckedText != "")
            {
                MessageBox.Show(CheckedText + "不是有效值", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            CheckedText = getCheckedValue();
        }
    }
}
