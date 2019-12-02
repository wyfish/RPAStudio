using System;
using System.Activities;
using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace RPA.Core.Activities.ClipboardActivity
{
    // ActivityDesigner1.xaml 的交互逻辑
    public partial class CopySelectTextDesigner
    {
        public CopySelectTextDesigner()
        {
            InitializeComponent();
        }

        private void ActivityDesigner_Loaded(object sender, RoutedEventArgs e)
        {
            InArgument<Int32> _timeout = 5000;
            List<ModelProperty> PropertyList = ModelItem.Properties.ToList();
            ModelProperty _property = PropertyList.Find((ModelProperty property) => property.Name.Equals("Timeout"));
            _property.SetValue(_timeout);
        }
    }
}
