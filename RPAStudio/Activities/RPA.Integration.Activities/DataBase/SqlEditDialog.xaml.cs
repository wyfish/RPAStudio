using System.Activities;
using System.Activities.Presentation;
using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Windows;

namespace RPA.Integration.Activities.Database
{
    public partial class SqlEditDialog : WorkflowElementDialog
    {
        public SqlEditDialog(ModelItem modelItem)
        {
            this.ModelItem = modelItem;
            this.Context = modelItem.GetEditingContext();
            InitializeComponent();
        }

        private void OnParametersButtonClicked(object sender, RoutedEventArgs e)
        {
            DynamicArgumentDesignerOptions dadOptions = new DynamicArgumentDesignerOptions
            {
                Title = "参数"
            };
            DynamicArgumentDialog.ShowDialog(ModelItem, ModelItem.Properties["Parameters"].Value, ModelItem.GetEditingContext(), this, dadOptions);
        }
    }
}
