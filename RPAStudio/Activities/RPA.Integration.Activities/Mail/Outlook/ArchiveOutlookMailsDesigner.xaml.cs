using Plugins.Shared.Library.Librarys;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RPA.Integration.Activities.Mail
{
    // ArchiveOutlookMailsDesigner.xaml 的交互逻辑
    public partial class ArchiveOutlookMailsDesigner
    {
        public ArchiveOutlookMailsDesigner()
        {
            InitializeComponent();
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            string userSelPath;
            bool ret = Common.ShowSaveAsFileDialog(out userSelPath, "archive", ".pst", "Outlook数据文件");

            if (ret == true)
            {
                base.ModelItem.Properties["PstFilePath"].SetValue(new InArgument<string>(userSelPath));
            }
        }
    }
}
