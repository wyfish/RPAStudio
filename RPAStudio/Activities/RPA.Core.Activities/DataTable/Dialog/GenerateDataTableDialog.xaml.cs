using RPA.Core.Activities.DataTableActivity.Operators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Plugins.Shared.Library.Librarys;
using System.Collections.ObjectModel;
using System.Linq;
using Plugins.Shared.Library;

namespace RPA.Core.Activities.DataTableActivity.Dialog
{
    public class ElementInfo : INotifyPropertyChanged
    {
        private int _value;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Value
        {
            get
            {
                return this._value;
            }
            set
            {
                this._value = value;
                this.OnPropertyChanged("Value");
            }
        }

        public ElementInfo()
        {
        }

        public ElementInfo(int value)
        {
            this.Value = value;
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler expr_06 = this.PropertyChanged;
            if (expr_06 == null)
            {
                return;
            }
            expr_06(this, new PropertyChangedEventArgs(name));
        }
    }

    public partial class GenerateDataTableDialog : Window
    {
        private string _inputText;
        public event PropertyChangedEventHandler PropertyChanged;

        public FormatOptions FormatOptions
        {
            get;
            set;
        }

        public TableOptions TableOptions
        {
            get;
            set;
        }

        public List<string> ItemsSource
        {
            get;set;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.PropertiesComboBox.IsDropDownOpen = true;
        }

        private void Button_Click_Newline(object sender, RoutedEventArgs e)
        {
            this.PropertiesComboBox_Newline.IsDropDownOpen = true;
        }


        private void ComboboxInputBox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            TextBox arg_1D_0 = this.ComboboxInputBox;
            string expr_11 = this.ComboboxInputBox.Text;
            arg_1D_0.Text = ((expr_11 != null) ? expr_11.Trim() : null);
        }
        private void ComboboxInputBox_Newline_OnLostFocus(object sender, RoutedEventArgs e)
        {
            TextBox arg_1D_0 = this.ComboboxInputBox_Newline;
            string expr_11 = this.ComboboxInputBox_Newline.Text;
            arg_1D_0.Text = ((expr_11 != null) ? expr_11.Trim() : null);
        }

        private void PropertiesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
            {
                return;
            }
            if (ColumnSeparators != null && ColumnSeparators != string.Empty)
            {
                if (this.PropertiesComboBox.SelectedValue != null && !this.ColumnSeparators.Contains(this.PropertiesComboBox.SelectedValue.ToString()))
                {
                    string arg_99_0 = "{0}[{1}]";
                    string expr_7D = this.ColumnSeparators;
                    this.ColumnSeparators = string.Format(arg_99_0, (expr_7D != null) ? expr_7D.Trim() : null, this.PropertiesComboBox.SelectedValue.ToString());
                }
            }
            else
            {
                this.ColumnSeparators = string.Empty;
                if (this.PropertiesComboBox.SelectedValue != null)
                {
                    this.ColumnSeparators = string.Format("[{0}]", this.PropertiesComboBox.SelectedValue.ToString());
                }
            }
            ComboboxInputBox.Text = ColumnSeparators;
            this.PropertiesComboBox.SelectedItem = null;
        }
        private void PropertiesComboBox_Newline_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
            {
                return;
            }
            if (NewLineSeparator != null && NewLineSeparator != string.Empty)
            {
                if (this.PropertiesComboBox_Newline.SelectedValue != null && !this.NewLineSeparator.Contains(this.PropertiesComboBox_Newline.SelectedValue.ToString()))
                {
                    string arg_99_0 = "{0}[{1}]";
                    string expr_7D = this.NewLineSeparator;
                    this.NewLineSeparator = string.Format(arg_99_0, (expr_7D != null) ? expr_7D.Trim() : null, this.PropertiesComboBox_Newline.SelectedValue.ToString());
                }
            }
            else
            {
                this.NewLineSeparator = string.Empty;
                if (this.PropertiesComboBox_Newline.SelectedValue != null)
                {
                    this.NewLineSeparator = string.Format("[{0}]", this.PropertiesComboBox_Newline.SelectedValue.ToString());
                }
            }
            ComboboxInputBox_Newline.Text = NewLineSeparator;
            this.PropertiesComboBox_Newline.SelectedItem = null;
        }



        public ObservableCollection<ElementInfo> CollectionElements
        {
            get;
            set;
        }


        private int? _newValue;
        public int? NewValue
        {
            get
            {
                return this._newValue;
            }
            set
            {
                if (this._newValue != value)
                {
                    this._newValue = value;
                    //this.OnPropertyChanged("NewValue");
                }
            }
        }

        //public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(List<int>), typeof(IntCollectionEditor));
        //public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(string), typeof(IntCollectionEditor));
        //public string SelectedItem_Intcollection
        //{
        //    get
        //    {
        //        return GetValue(SelectedItemProperty) as string;
        //    }
        //    set
        //    {
        //        SetValue(SelectedItemProperty, value);
        //    }
        //}

        public List<int> ItemsSource_Intcollection
        {
            get; set;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.NewValue.HasValue && this.NewValue.HasValue)
            {
                this.CollectionElements.Add(new ElementInfo(this.NewValue.Value));
                this.ItemsSource_Intcollection.Add(NewValue.Value);
                this.NewValue = null;
                this.NewValueElement.Focus();
            }
        }

        private void Button_Click_Intcollection(object sender, RoutedEventArgs e)
        {
            this.PropertiesComboBox_Intcollection.IsDropDownOpen = true;
            this.CollectionElements.Clear();
            if (this.ItemsSource_Intcollection != null)
            {
                foreach (int current in ItemsSource_Intcollection)
                {
                    this.CollectionElements.Add(new ElementInfo(current));
                }
            }
        }

        
        //private void ItemsPanel_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    this.ItemsSource_Intcollection = new List<int>(from element in this.CollectionElements select element.Value);
        //}

        private void PropertiesComboBox_Intcollection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.PropertiesComboBox_Intcollection.IsDropDownOpen = true;
            e.Handled = true;
        }

        private void RemoveElementButton_Click(object sender, RoutedEventArgs e)
        {
            ElementInfo dataContext = (sender as Button).DataContext as ElementInfo;
            if (dataContext != null)
            {
                this.CollectionElements.Remove(dataContext);
                this.ItemsSource_Intcollection = new List<int>(from elem in this.CollectionElements select elem.Value);
            }
        }



        public bool AutoDetectTypes
        {
            get
            {
                return this.TableOptions.AutoDetectTypes;
            }
            set
            {
                if (this.TableOptions.AutoDetectTypes != value)
                {
                    this.TableOptions.AutoDetectTypes = value;
                    this.OnPropertyChanged("AutoDetectTypes");
                }
            }
        }

        public string ColumnSeparators
        {
            get
            {
                return this.FormatOptions.ColumnSeparators;
            }
            set
            {
                if (this.FormatOptions.ColumnSeparators != value)
                {
                    this.FormatOptions.ColumnSeparators = value;
                    this.OnPropertyChanged("ColumnSeparators");
                }
            }
        }

        public IEnumerable<int> ColumnSizes
        {
            get
            {
                return this.FormatOptions.ColumnSizes;
            }
            set
            {
                if (this.FormatOptions.ColumnSizes != value)
                {
                    this.FormatOptions.ColumnSizes = value;
                    this.OnPropertyChanged("ColumnSizes");
                }
            }
        }

        public string InputText
        {
            get
            {
                return this._inputText;
            }
            set
            {
                this._inputText = value;
                try
                {
                    this.OutputDataTable.ItemsSource = null;
                }
                catch
                {
                }
                this.OnPropertyChanged("InputText");
            }
        }

        public string NewLineSeparator
        {
            get
            {
                return this.FormatOptions.NewLineSeparator;
            }
            set
            {
                if (this.FormatOptions.NewLineSeparator != value)
                {
                    this.FormatOptions.NewLineSeparator = value;
                    this.OnPropertyChanged("NewLineSeparator");
                }
            }
        }

        public bool PreserveNewLines
        {
            get
            {
                return this.FormatOptions.PreserveNewLines;
            }
            set
            {
                if (this.FormatOptions.PreserveNewLines != value)
                {
                    this.FormatOptions.PreserveNewLines = value;
                    this.OnPropertyChanged("PreserveNewLines");
                }
            }
        }

        public bool CsvParsing
        {
            get
            {
                return this.FormatOptions.CSVParsing;
            }
            set
            {
                if (this.FormatOptions.CSVParsing != value)
                {
                    this.FormatOptions.CSVParsing = value;
                    this.OnPropertyChanged("CsvParsing");
                }
            }
        }

        public bool UseColumnHeader
        {
            get
            {
                return this.TableOptions.UseColumnHeader;
            }
            set
            {
                if (this.TableOptions.UseColumnHeader != value)
                {
                    this.TableOptions.UseColumnHeader = value;
                    this.OnPropertyChanged("UseColumnHeader");
                }
            }
        }

        public bool UseRowHeader
        {
            get
            {
                return this.TableOptions.UseRowHeader;
            }
            set
            {
                if (this.TableOptions.UseRowHeader != value)
                {
                    this.TableOptions.UseRowHeader = value;
                    this.OnPropertyChanged("UseRowHeader");
                }
            }
        }

        public static List<string> Separators
        {
            get
            {
                return GenerateDataTableDialog.Separators;
            }
        }

        public GenerateDataTableDialog()
        {
            ItemsSource = new List<string>();
            ItemsSource.Add("space");
            ItemsSource.Add("tab");
            ItemsSource.Add("newline");

            CollectionElements = new ObservableCollection<ElementInfo>();
            this.FormatOptions = new FormatOptions();
            this.TableOptions= new TableOptions();
            this.InitializeComponent();
        }

        public GenerateDataTableDialog(FormatOptions presetFormatOptions, TableOptions presetTableOptions, string inputText)
        {
            ItemsSource = new List<string>();
            ItemsSource.Add("space");
            ItemsSource.Add("tab");
            ItemsSource.Add("newline");

            this.FormatOptions = presetFormatOptions;
            this.TableOptions = presetTableOptions;
            this.InputText = inputText;
          

            this.InitializeComponent();
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler expr_06 = this.PropertyChanged;
            if (expr_06 == null)
            {
                return;
            }
            expr_06(this, new PropertyChangedEventArgs(name));
        }

        private void Format()
        {
            try
            {
                DataTable dataTable = new TableFormatter().Format(this.InputTextBox.Text, this.FormatOptions, this.TableOptions);
                //DataTable dataTable = new DataTable();
                foreach (DataColumn dataColumn in dataTable.Columns)
                {
                    DataColumn expr_3D = dataColumn;
                    expr_3D.ColumnName += string.Format("{0}({1})", Environment.NewLine, dataColumn.DataType);
                }
                this.OutputDataTable.ItemsSource = dataTable.AsDataView();
            }
            catch (Exception ex)
            {
                Window window = Window.GetWindow(this);
                if (window != null)
                {
                    SharedObject.Instance.Output(SharedObject.enOutputType.Error, "", ex);
                }
                else
                {
                    SharedObject.Instance.Output(SharedObject.enOutputType.Error, "", ex);
                }
                Trace.TraceError(ex.Message);
            }
        }

        private void Preview_Click(object sender, RoutedEventArgs e)
        {
            this.Format();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            base.DialogResult = new bool?(false);
            base.Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            base.DialogResult = new bool?(true);
            base.Close();
        }
    }
}