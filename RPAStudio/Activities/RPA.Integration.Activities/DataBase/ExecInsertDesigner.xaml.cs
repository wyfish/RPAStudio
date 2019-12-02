using System.Activities.Presentation.Model;

namespace RPA.Integration.Activities.Database
{
    public partial class ExecInsertDesigner
    {
        public ExecInsertDesigner()
        {
            InitializeComponent();
        }

        private void SqlButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            using (ModelEditingScope editingScope = ModelItem.BeginEdit())
            {
                SqlEditDialog sqlDialog = new SqlEditDialog(ModelItem);
                if (sqlDialog.ShowOkCancel())
                {
                    editingScope.Complete();
                }
                else
                {
                    editingScope.Revert();
                }
            }
        }
        private void ConnSet(object sender, System.Windows.RoutedEventArgs e)
        {
            ConnectionDialog connDialog = new ConnectionDialog(this.ModelItem);
            connDialog.Show();
        }
    }
}