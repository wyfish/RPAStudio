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
    public partial class SaveMailAttachmentsDesigner
    {
        public SaveMailAttachmentsDesigner()
        {
            InitializeComponent();
        }

        private void PathSelect(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();


            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                string fName = folderDialog.SelectedPath;
                List<ModelProperty> PropertyList = ModelItem.Properties.ToList();
                ModelProperty _property = PropertyList.Find((ModelProperty property) => property.Name.Equals("PathName"));
                InArgument<string> pathValue = fName;
                _property.SetValue(pathValue);
            }
        }
    }
}
