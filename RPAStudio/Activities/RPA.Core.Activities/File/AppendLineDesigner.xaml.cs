using System.Activities;
using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.Linq;
using System.Windows;


namespace RPA.Core.Activities.FileActivity
{
    // ActivityDesigner1.xaml 的交互逻辑
    public partial class AppendLineDesigner
    {
        public AppendLineDesigner()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.Filter = "所有文件|*.*";
            sfd.FileName = "Untitled.txt";
            sfd.OverwritePrompt = false;
            if (sfd.ShowDialog() == true)
            {
                InArgument<string> inFileName = sfd.FileName;
                List<ModelProperty> PropertyList = ModelItem.Properties.ToList();
                ModelProperty _property = PropertyList.Find((ModelProperty property) => property.Name.Equals("FileName"));
                _property.SetValue(inFileName);
            }
        }
    }
}
