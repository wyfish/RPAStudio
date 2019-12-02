using Plugins.Shared.Library;
using System.Activities;
using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace RPA.Core.Activities.FileActivity
{
    public partial class CopyFileDesigner
    {
        public CopyFileDesigner()
        {
            InitializeComponent();
        }

        private void button_Click_fileFrom(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "所有文件|*.*";
            if (ofd.ShowDialog() == true)
            {
                InArgument<string> inFileName = ofd.FileName;
                List<ModelProperty> PropertyList = ModelItem.Properties.ToList();
                ModelProperty _property = PropertyList.Find((ModelProperty property) => property.Name.Equals("Path"));
                _property.SetValue(inFileName);
            }
        }

        private void button_Click_fileTo(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = "请选择文件复制目标文件夹";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (string.IsNullOrEmpty(dialog.SelectedPath))
                {
                    SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", "文件夹路径不能为空!");
                    return;
                }
                InArgument<string> inFileName = dialog.SelectedPath;
                List<ModelProperty> PropertyList = ModelItem.Properties.ToList();
                ModelProperty _property = PropertyList.Find((ModelProperty property) => property.Name.Equals("Destination"));
                _property.SetValue(inFileName);
            }
        }
    }
}
