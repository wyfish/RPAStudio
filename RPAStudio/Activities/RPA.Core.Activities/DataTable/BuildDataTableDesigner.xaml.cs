using RPA.Core.Activities.DataTableActivity.Dialog;
using System;
using System.Activities;
using System.Activities.Presentation;
using System.Data;
using System.Windows;

namespace RPA.Core.Activities.DataTableActivity
{
    public partial class BuildDataTableDesigner
    {
        public BuildDataTableDesigner()
        {
            InitializeComponent();
        }

        private void DataTableBuild(object sender, System.Windows.RoutedEventArgs e)
        {
            string text = base.ModelItem.Properties["TableInfo"].ComputedValue as string;
            DataTable dataTable = new DataTable();
            try
            {
                BuildDataTable.ReadDataTableFromXML(text, dataTable);
            }
            catch (Exception ex)
            {
                throw new Exception("DataTableBuild Exception", ex);
            }
            BuildDataTableDialog buildDataTableDialog = new BuildDataTableDialog(dataTable, base.ModelItem);
            buildDataTableDialog.ShowDialog();
            if (buildDataTableDialog.SaveTable)
            {
                base.ModelItem.Properties["TableInfo"].SetValue(buildDataTableDialog.DataTableXmlSchema);
            }
        }
    }
}