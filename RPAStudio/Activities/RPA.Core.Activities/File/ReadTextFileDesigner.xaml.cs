using System.Activities;
using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.Linq;
using System.Windows;


namespace RPA.Core.Activities.FileActivity
{
    /// <summary>
    /// ReadTextFileDesigner.xaml 的交互逻辑
    /// </summary>
    public partial class ReadTextFileDesigner
    {
        public ReadTextFileDesigner()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "所有文件|*.*";
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
