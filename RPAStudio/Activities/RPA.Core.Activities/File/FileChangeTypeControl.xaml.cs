using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace RPA.Core.Activities.FileActivity
{
    /// <summary>
    /// FileChangeTypeControl.xaml 的交互逻辑
    /// </summary>
    public partial class FileChangeTypeControl : UserControl
    {
        private bool isCreatedChecked { get; set; }
        private bool isDeletedChecked { get; set; }
        private bool isChangedChecked { get; set; }
        private bool isRenamedChecked { get; set; }
        private bool isAllChecked { get; set; }

        public FileChangeTypeControl()
        {
            InitializeComponent();
        }
        public string CheckedText
        {
            get { return (string)GetValue(CheckedTextProperty); }
            set
            {
                SetValue(CheckedTextProperty, value);
            }
        }
        public static readonly DependencyProperty CheckedTextProperty = DependencyProperty.Register("CheckedText", typeof(string), typeof(FileChangeTypeControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        string getCheckedValue()
        {
            var list = new List<string>();
            if (isCreatedChecked)
            {
                list.Add("Created");
            }

            if (isDeletedChecked)
            {
                list.Add("Deleted");
            }

            if (isChangedChecked)
            {
                list.Add("Changed");
            }

            if (isRenamedChecked)
            {
                list.Add("Renamed");
            }

            if (isAllChecked)
            {
                list.Add("All");
            }

            string ret = string.Join(",", list.ToArray());

            return ret;
        }

        private void Created_Checked(object sender, RoutedEventArgs e)
        {
            isCreatedChecked = !isCreatedChecked;
            CheckedText = getCheckedValue();
        }

        private void Deleted_Checked(object sender, RoutedEventArgs e)
        {
            isDeletedChecked = !isDeletedChecked;
            CheckedText = getCheckedValue();
        }

        private void Changed_Checked(object sender, RoutedEventArgs e)
        {
            isChangedChecked = !isChangedChecked;
            CheckedText = getCheckedValue();
        }

        private void Renamed_Checked(object sender, RoutedEventArgs e)
        {
            isRenamedChecked = !isRenamedChecked;
            CheckedText = getCheckedValue();
        }

        private void All_Checked(object sender, RoutedEventArgs e)
        {
            isAllChecked = !isAllChecked;
            CheckedText = getCheckedValue();
        }

        private void ComboBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (CheckedText == null) return;
            string[] list = CheckedText.Split(',');
            var listModel = new List<string>();
            listModel.AddRange(list);
            cbCreated.IsChecked = false;
            cbDeleted.IsChecked = false;
            cbChanged.IsChecked = false;
            cbRenamed.IsChecked = false;
            cbAll.IsChecked = false;
            foreach (string item in list)
            {
                if (item == "Created")
                {
                    cbCreated.IsChecked = true;
                    listModel.Remove(item);
                }
                else if (item == "Deleted")
                {
                    cbDeleted.IsChecked = true;
                    listModel.Remove(item);
                }
                else if (item == "Changed")
                {
                    cbChanged.IsChecked = true;
                    listModel.Remove(item);
                }
                else if (item == "Renamed")
                {
                    cbRenamed.IsChecked = true;
                    listModel.Remove(item);
                }
                else if (item == "All")
                {
                    cbAll.IsChecked = true;
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
