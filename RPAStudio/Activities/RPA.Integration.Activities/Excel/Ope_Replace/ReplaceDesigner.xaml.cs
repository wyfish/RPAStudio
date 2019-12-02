using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace RPA.Integration.Activities.ExcelPlugins
{
    public partial class ReplaceDesigner
    {
        public ReplaceDesigner()
        {
            InitializeComponent();
        }
        private void IcoPath_Loaded(object sender, RoutedEventArgs e)
        {
            icoPath.ImageSource = new BitmapImage(new Uri(CommonVariable.getPropertyValue("icoPath", ModelItem)));
        }
    }
}
