using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Activities.Presentation.Model;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using Plugins.Shared.Library;

namespace RPA.Core.Activities.DataTableActivity.Dialog
{
    public partial class BuildDataTableDialog : Window, IComponentConnector
    {
        private DataSet _dataSet;

        private ModelItem _activityModel;

        internal BuildDataTableDialog BuildDataTableWnd=null;

        public DataTable DataTable
        {
            get;
            set;
        }

        internal string DataTableXmlSchema
        {
            get;
            set;
        }

        public bool SaveTable
        {
            get;
            set;
        }

        public BuildDataTableDialog(DataTable dataTable, ModelItem ownerActivity)
        {
            this.InitializeComponent();
            this.DataTable = dataTable;
            this._activityModel = ownerActivity;
            this._dataSet = new DataSet();
            this._dataSet.EnforceConstraints = false;
            this._dataSet.Tables.Add(this.DataTable);
            this.TableDataGrid.DataContext = this._dataSet.Tables[0];
            this.TableDataGrid.ItemsSource = this.DataTable.DefaultView;
        }

        private void RemoveColumnButton_Click(object sender, RoutedEventArgs e)
        {
            string text = (sender as Button).DataContext as string;
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }
            DataColumn dataColumn = this.DataTable.Columns[text];
            if (dataColumn == null)
            {
                return;
            }
            bool unique = dataColumn.Unique;
            try
            {
                if (unique)
                {
                    dataColumn.Unique = false;
                }
                this.DataTable.Columns.Remove(text);
                dataColumn.Unique = unique;
                this.UpdateItemsSource();
            }
            catch (Exception ex)
            {
                dataColumn.Unique = unique;
                this.ShowError(ex.Message);
            }
        }

        private void AddColumnButtonClick(object sender, RoutedEventArgs e)
        {
            new NewColumnDialog(this._activityModel, this.DataTable)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            }.ShowDialog();
            this.UpdateItemsSource();
        }

        private void RemoveRowButton_Click(object sender, RoutedEventArgs e)
        {
            this.OKButton.Focus();
            DataRowView dataRowView = (sender as Button).DataContext as DataRowView;
            if (dataRowView != null)
            {
                try
                {
                    this.DataTable.Rows.Remove(dataRowView.Row);
                    this.UpdateItemsSource();
                }
                catch (Exception ex)
                {
                    this.ShowError(ex.Message);
                }
            }
        }

        private void UpdateItemsSource()
        {
            this.TableDataGrid.ItemsSource = null;
            this.TableDataGrid.ItemsSource = this.DataTable.DefaultView;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.SaveTable = false;
            base.Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this._dataSet.EnforceConstraints = true;
                using (StringWriter stringWriter = new StringWriter())
                {
                    this.DataTable.WriteXml(stringWriter, XmlWriteMode.WriteSchema);
                    this.DataTableXmlSchema = stringWriter.ToString();
                }

                this.SaveTable = true;
                base.Close();
            }
            catch (Exception arg_46_0)
            {
                Trace.TraceError(arg_46_0.Message);
            }
        }

        private void ShowError(string message)
        {
            SharedObject.Instance.Output(SharedObject.enOutputType.Error, "", message);
        }

        private void TableDataGrid_ColumnReordered(object sender, DataGridColumnEventArgs e)
        {
            Mouse.OverrideCursor = null;
            string str = e?.Column?.Header?.ToString();
            if (!string.IsNullOrWhiteSpace(str) && (this.DataTable.Columns.IndexOf(str) != -1))
            {
                DataColumn column = this.DataTable.Columns[str];
                if (column == null)
                {
                    this.ShowError("ColumnNotFoundError");
                }
                else
                {
                    column.SetOrdinal(e.Column.DisplayIndex);
                }
            }
        }

        private void TableDataGrid_ColumnHeaderDragStarted(object sender, DragStartedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void TableDataGrid_ColumnHeaderDragCompleted(object sender, DragCompletedEventArgs e)
        {
            Mouse.OverrideCursor = null;
        }

        private void DataGridColumnHeader_MouseEnter(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void DataGridColumnHeader_MouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = null;
        }

        private void EditColumnButton_Click(object sender, RoutedEventArgs e)
        {
            Button expr_06 = sender as Button;
            string text = ((expr_06 != null) ? expr_06.DataContext : null) as string;
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }
            DataColumn dataColumn = this.DataTable.Columns[text];
            if (dataColumn == null)
            {
                this.ShowError("ColumnNotFoundError");
                return;
            }
            new NewColumnDialog(this._activityModel, this.DataTable, dataColumn.Ordinal)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            }.ShowDialog();
            this.UpdateItemsSource();
        }

        private void TableDataGrid_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {

        }
    }
}
