using Plugins.Shared.Library;
using System;
using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Activities;
using Microsoft.VisualBasic.Activities;
using System.Activities.Expressions;
using System.Windows.Forms;
using System.Diagnostics;
using System.Activities.Presentation.View;

namespace RPA.Integration.Activities.Mail
{
    public partial class SaveMailDesigner
    {
        public SaveMailDesigner()
        {
            InitializeComponent();
        }

        private void PathSelect(object sender, RoutedEventArgs e)
        {
            SaveFileDialog openFileDialog = new SaveFileDialog();
            openFileDialog.Filter = "EML files (*.eml)|*.eml";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fName = openFileDialog.FileName;
                List<ModelProperty> PropertyList = ModelItem.Properties.ToList();
                ModelProperty _property = PropertyList.Find((ModelProperty property) => property.Name.Equals("PathName"));
                InArgument<string> pathValue = fName;
                _property.SetValue(pathValue);
            }
        }
    }
}
