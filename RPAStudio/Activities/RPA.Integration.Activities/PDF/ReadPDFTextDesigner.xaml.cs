using System.Activities;
using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace RPA.Integration.Activities.PDF
{
    public partial class ReadPDFTextDesigner
    {
        public ReadPDFTextDesigner()
        {
            InitializeComponent();
        }


        private void PathSelect(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All files (*.*)|*.*|PDF files (*.pdf)|*.pdf";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fName = openFileDialog.FileName;
                List<ModelProperty> PropertyList = ModelItem.Properties.ToList();
                ModelProperty _property = PropertyList.Find((ModelProperty property) => property.Name.Equals("PathUrl"));
                InArgument<string> pathValue = fName;
                _property.SetValue(pathValue);
            }
        }
    }
}
