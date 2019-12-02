using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace RPA.Integration.Activities.WordPlugins
{
    /// <summary>
    /// SettingsDesigner.xaml 的交互逻辑
    /// </summary>
    public partial class SettingsDesigner
    {
        public SettingsDesigner()
        {
            InitializeComponent();
        }
        private void IcoPath_Loaded(object sender, RoutedEventArgs e)
        {
            icoPath.ImageSource = new BitmapImage(new Uri(CommonVariable.getPropertyValue("icoPath", ModelItem)));
        }

    }
}
