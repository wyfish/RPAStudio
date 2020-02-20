using System.Activities;
using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace RPA.UIAutomation.Activities.Image
{
    // LoadImageDesigner.xaml 的交互逻辑
    public partial class LoadImageDesigner
    {
        public LoadImageDesigner()
        {
            InitializeComponent();
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter= "jpeg|*.jpg|bmp|*.bmp|gif|*.gif|png|*.png|All Picture Files|*.jpg;*.bmp;*.gif;*.png";
            ofd.FileName = "Untitled.jpg";
            if (ofd.ShowDialog() == true)
            {
                InArgument<string> inFileName = ofd.FileName;
                List<ModelProperty> PropertyList = ModelItem.Properties.ToList();
                ModelProperty _property = PropertyList.Find((ModelProperty property) => property.Name.Equals("FileName"));
                _property.SetValue(inFileName);
            }
        }
    }
}
