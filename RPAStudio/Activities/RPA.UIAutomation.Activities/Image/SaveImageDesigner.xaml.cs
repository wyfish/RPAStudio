using System.Activities;
using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace RPA.UIAutomation.Activities.Image
{
    // SaveImageDesigner.xaml 的交互逻辑
    public partial class SaveImageDesigner
    {
        public SaveImageDesigner()
        {
            InitializeComponent();
        }
        
        private void button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.Filter = "jpeg|*.jpg|bmp|*.bmp|gif|*.gif|png|*.png|All Picture Files|*.jpg;*.bmp;*.gif;*.png";
            sfd.FileName = "Untitled.jpg";
            sfd.OverwritePrompt = true;
            if (sfd.ShowDialog() == true )
            {             
                InArgument<string> inFileName = sfd.FileName;
                List<ModelProperty> PropertyList = ModelItem.Properties.ToList();
                ModelProperty _property = PropertyList.Find((ModelProperty property) => property.Name.Equals("FileName"));
                _property.SetValue(inFileName);
            }
        }
        
    }
}
