using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace RPA.Core.Activities.EnvironmentActivity
{
    // ActivityDesigner1.xaml 的交互逻辑
    public partial class GetPasswordDesigner
    {
        public GetPasswordDesigner()
        {
            InitializeComponent();
        }

        private void IcoPath_Loaded(object sender, RoutedEventArgs e)
        {
            icoPath.ImageSource = new BitmapImage(new Uri(EnvironmentActivity.CommonVariable.getPropertyValue("icoPath", ModelItem)));
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //bool isAllAsterisk = true;
            //string passWord = textBox.Text.ToString();
            //foreach(char c in passWord)
            //{
            //    if (c != '*')
            //        isAllAsterisk = false;
            //}
            //if (isAllAsterisk == false)
            //{
            //    string asteriskStr = "";
            //    foreach (char c in passWord)
            //        asteriskStr += '*';
            //    List<ModelProperty> PropertyList = ModelItem.Properties.ToList();
            //    ModelProperty _property = PropertyList.Find((ModelProperty property) => property.Name.Equals("EnPassword"));
            //    _property.SetValue(asteriskStr);
            //}
        }
    }
}
