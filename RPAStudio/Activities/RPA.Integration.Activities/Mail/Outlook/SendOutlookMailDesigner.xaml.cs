using Plugins.Shared.Library.Editors;
using System;
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
    // SendOutlookMailDesigner.xaml 的交互逻辑
    public partial class SendOutlookMailDesigner
    {
        public SendOutlookMailDesigner()
        {
            InitializeComponent();
        }

        private void AttachFiles_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ArgumentCollectionEditor.ShowDialog("附件", base.ModelItem);
        }
    }
}
