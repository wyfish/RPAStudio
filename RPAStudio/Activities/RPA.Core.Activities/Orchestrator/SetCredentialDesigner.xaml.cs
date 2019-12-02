using System;
using System.Windows;
using System.Windows.Media.Imaging;


namespace RPA.Core.Activities.OrchestratorActivity
{
    // ActivityDesigner1.xaml 的交互逻辑
    public partial class SetCredentialDesigner
    {
        public SetCredentialDesigner()
        {
            InitializeComponent();
        }

        private void IcoPath_Loaded(object sender, RoutedEventArgs e)
        {
            icoPath.ImageSource = new BitmapImage(new Uri(CommonVariable.getPropertyValue("icoPath", ModelItem)));
        }
    }
}
