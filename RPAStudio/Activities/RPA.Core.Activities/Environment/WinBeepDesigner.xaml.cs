using System;
using System.Windows;
using System.Windows.Media.Imaging;


namespace RPA.Core.Activities.EnvironmentActivity
{
    // ActivityDesigner1.xaml 的交互逻辑
    public partial class WinBeepDesigner
    {
        public WinBeepDesigner()
        {
            InitializeComponent();
        }

        private void IcoPath_Loaded(object sender, RoutedEventArgs e)
        {
            icoPath.ImageSource = new BitmapImage(new Uri(EnvironmentActivity.CommonVariable.getPropertyValue("icoPath", ModelItem)));
        }

    }
}
