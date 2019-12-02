using System;
using System.Activities;
using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace RPA.Core.Activities.EnvironmentActivity
{
    // ActivityDesigner1.xaml 的交互逻辑
    public partial class GetEnvVarDesigner
    {
        public GetEnvVarDesigner()
        {
            InitializeComponent();
        }

        private void IcoPath_Loaded(object sender, RoutedEventArgs e)
        {
            icoPath.ImageSource = new BitmapImage(new Uri(EnvironmentActivity.CommonVariable.getPropertyValue("icoPath", ModelItem)));
        }
        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string comboBoxValue = comboBox.SelectedItem.ToString();
            if (comboBoxValue != null)
            {
                List<ModelProperty> PropertyList = ModelItem.Properties.ToList();
                ModelProperty _property = PropertyList.Find((ModelProperty property) => property.Name.Equals("EnvVarName"));
                InArgument<string> newValue = comboBoxValue;
                _property.SetValue(newValue);
            }
        }
    }
}
