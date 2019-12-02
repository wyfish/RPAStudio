using System;
using System.Windows;
using System.Windows.Media.Imaging;


namespace RPA.Integration.Activities.WordPlugins
{
    /// <summary>
    /// WriteText.xaml 的交互逻辑
    /// </summary>
    public partial class CopyPasteDesigner
    {
        public CopyPasteDesigner()
        {
            InitializeComponent();
        }
        private void IcoPath_Loaded(object sender, RoutedEventArgs e)
        {
            icoPath.ImageSource = new BitmapImage(new Uri(CommonVariable.getPropertyValue("icoPath", ModelItem)));
        }
    }
}
