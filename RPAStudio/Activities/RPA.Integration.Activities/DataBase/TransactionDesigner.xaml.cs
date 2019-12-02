using System;
using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Runtime.InteropServices;

namespace RPA.Integration.Activities.Database
{
    public partial class TransactionDesigner
    {
        public TransactionDesigner()
        {
            InitializeComponent();
        }

        private void ConnSet(object sender, RoutedEventArgs e)
        {
            ConnectionDialog connDialog = new ConnectionDialog(this.ModelItem);
            connDialog.Show();
        }
    }
}
