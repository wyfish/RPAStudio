using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace RPA.Core.Activities.FileActivity
{
    /// <summary>
    /// NotifyFiltersControl.xaml 的交互逻辑
    /// </summary>
    public partial class NotifyFiltersControl : UserControl
    {
        public bool isFileNameChecked { get; set; }
        public bool isDirectoryNameChecked { get; set; }
        public bool isAttributesChecked { get; set; }
        public bool isSizeChecked { get; set; }
        public bool isLastWriteChecked { get; set; }
        public bool isLastAccessChecked { get; set; }
        public bool isCreationTimeChecked { get; set; }
        public bool isSecurityChecked { get; set; }

        public string CheckedText
        {
            get { return (string)GetValue(CheckedTextProperty); }
            set { SetValue(CheckedTextProperty, value); }
        }

        public static readonly DependencyProperty CheckedTextProperty =
            DependencyProperty.Register("CheckedText", typeof(string), typeof(NotifyFiltersControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public NotifyFiltersControl()
        {
            InitializeComponent();
        }

        string getCheckedValue()
        {
            var list = new List<string>();
            if (isFileNameChecked)
            {
                list.Add("FileName");
            }

            if (isDirectoryNameChecked)
            {
                list.Add("DirectoryName");
            }

            if (isAttributesChecked)
            {
                list.Add("Attributes");
            }

            if (isSizeChecked)
            {
                list.Add("Size");
            }

            if (isLastWriteChecked)
            {
                list.Add("LastWrite");
            }

            if (isLastAccessChecked)
            {
                list.Add("LastAccess");
            }

            if (isCreationTimeChecked)
            {
                list.Add("CreationTime");
            }

            if (isSecurityChecked)
            {
                list.Add("Security");
            }
            string ret = string.Join(",", list.ToArray());
            return ret;
        }

        private void FileName_Checked(object sender, RoutedEventArgs e)
        {
            isFileNameChecked = !isFileNameChecked;
            CheckedText = getCheckedValue();
        }

        private void DirectoryName_Checked(object sender, RoutedEventArgs e)
        {
            isDirectoryNameChecked = !isDirectoryNameChecked;
            CheckedText = getCheckedValue();
        }

        private void Attributes_Checked(object sender, RoutedEventArgs e)
        {
            isAttributesChecked = !isAttributesChecked;
            CheckedText = getCheckedValue();
        }

        private void Size_Checked(object sender, RoutedEventArgs e)
        {
            isSizeChecked = !isSizeChecked;
            CheckedText = getCheckedValue();
        }

        private void LastWrite_Checked(object sender, RoutedEventArgs e)
        {
            isLastWriteChecked = !isLastWriteChecked;
            CheckedText = getCheckedValue();
        }
        private void LastAccess_Checked(object sender, RoutedEventArgs e)
        {
            isLastAccessChecked = !isLastAccessChecked;
            CheckedText = getCheckedValue();
        }
        private void CreationTime_Checked(object sender, RoutedEventArgs e)
        {
            isCreationTimeChecked = !isCreationTimeChecked;
            CheckedText = getCheckedValue();
        }
        private void Security_Checked(object sender, RoutedEventArgs e)
        {
            isSecurityChecked = !isSecurityChecked;
            CheckedText = getCheckedValue();
        }

        private void ComboBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (CheckedText == null) return;
            string[] list = CheckedText.Split(',');
            var listModel = new List<string>();
            listModel.AddRange(list);
            cbFileName.IsChecked = false;
            cbDirectoryName.IsChecked = false;
            cbAttributes.IsChecked = false;
            cbSize.IsChecked = false;
            cbLastWrite.IsChecked = false;
            cbLastAccess.IsChecked = false;
            cbCreationTime.IsChecked = false;
            cbSecurity.IsChecked = false;
            foreach (string item in list)
            {
                if (item == "FileName")
                {
                    cbFileName.IsChecked = true;
                    listModel.Remove(item);
                }
                else if (item == "DirectoryName")
                {
                    cbDirectoryName.IsChecked = true;
                    listModel.Remove(item);
                }
                else if (item == "Attributes")
                {
                    cbAttributes.IsChecked = true;
                    listModel.Remove(item);
                }
                else if (item == "Size")
                {
                    cbSize.IsChecked = true;
                    listModel.Remove(item);
                }
                else if (item == "LastWrite")
                {
                    cbLastWrite.IsChecked = true;
                    listModel.Remove(item);
                }
                else if (item == "LastAccess")
                {
                    cbLastAccess.IsChecked = true;
                    listModel.Remove(item);
                }
                else if (item == "CreationTime")
                {
                    cbCreationTime.IsChecked = true;
                    listModel.Remove(item);
                }
                else if (item == "Security")
                {
                    cbSecurity.IsChecked = true;
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
