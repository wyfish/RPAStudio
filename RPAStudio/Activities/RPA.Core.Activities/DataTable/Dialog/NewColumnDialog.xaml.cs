using System;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.View;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Activities.Presentation;

namespace RPA.Core.Activities.DataTableActivity.Dialog
{
    public partial class NewColumnDialog : Window,  INotifyPropertyChanged, IComponentConnector
    {
        private Type _dateType;

        private int _columnIndex;

        private bool _isEdit;

        private bool _autoIncrement;

        private DataTable _dataTable;

        internal TypePresenter TypePresenter = null;


        public EditingContext Context
        {
            set;
            get;
        }


        private string columnName = "Column1";
  
        public string ColumnName
        {
            set
            {
                if (this.columnName != value)
                {
                    this.columnName = value;
                }
            }
            get { return this.columnName; }
        }

        internal Image WarningImage = null;

        //private bool _contentLoaded;

        [method: CompilerGenerated]
        [CompilerGenerated]
        public event PropertyChangedEventHandler PropertyChanged;

        public string WindowTitle
        {
            get;
            set;
        }

        public string DataChangedWarning
        {
            get;
            set;
        }


        public Type DateType
        {
            get
            {
                return this._dateType;
            }
            set
            {
                this._dateType = value;
                this.OnNotifyPropertyChanged("DateType");
                this.OnNotifyPropertyChanged("IsStringColumn");
                this.OnNotifyPropertyChanged("CanAutoIncrement");
            }
        }

        public bool AllowDBNull
        {
            get;
            set;
        }

        public bool AutoIncrement
        {
            get
            {
                return this._autoIncrement;
            }
            set
            {
                this._autoIncrement = value;
                this.OnNotifyPropertyChanged("AutoIncrement");
            }
        }

        public bool CanAutoIncrement
        {
            get
            {
                return this.DateType.Equals(typeof(short)) || this.DateType.Equals(typeof(int)) || this.DateType.Equals(typeof(long));
            }
        }

        public bool Unique
        {
            get;
            set;
        }

        public object DefaultValue
        {
            get;
            set;
        }

        int maxLenth = 100;
        public int MaxLength
        {
            get
            {
                return maxLenth;
            }
            set
            {
                maxLenth = value;
            }
        }

        public bool IsStringColumn
        {
            get
            {
                return this.DateType.Equals(typeof(string));
            }
        }

        private void OnNotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public NewColumnDialog(ModelItem ownerActivity, DataTable dataTable)
        {
            this._isEdit = false;
            this.WindowTitle = "新增数据表列";
            this._dataTable = dataTable;
            this.MaxLength = -1;
            this.DateType = typeof(string);
            this.AllowDBNull = true;
            this.Context = ownerActivity.GetEditingContext();

            this.InitializeComponent();
            //this.TypePresenter.Context = ownerActivity.GetEditingContext();
        }

        public NewColumnDialog(ModelItem ownerActivity, DataTable dataTable, int columnIndex)
        {
            this._columnIndex = columnIndex;
            this._isEdit = true;
            this.WindowTitle = "编辑数据表列";
            this._dataTable = dataTable;
            DataColumn dataColumn = this._dataTable.Columns[this._columnIndex];
            this.AllowDBNull = dataColumn.AllowDBNull;
            this.Unique = dataColumn.Unique;
            this.AutoIncrement = dataColumn.AutoIncrement;
            this.ColumnName = dataColumn.ColumnName;
            this.DateType = dataColumn.DataType;
            this.MaxLength = dataColumn.MaxLength;
            this.DefaultValue = dataColumn.DefaultValue;
            this.Context = ownerActivity.GetEditingContext();
            this.InitializeComponent();
            //this.TypePresenter.Context = ownerActivity.GetEditingContext();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(this.ColumnName))
                {
                    throw new ArgumentException("哈哈哈");
                }
                if (this._isEdit && this._dataTable.Columns[this._columnIndex].DataType == this.DateType)
                {
                    DataColumn dataColumn = this._dataTable.Columns[this._columnIndex];
                    dataColumn.ColumnName = this.ColumnName;
                    dataColumn.AllowDBNull = this.AllowDBNull;
                    dataColumn.Unique = this.Unique;
                    if (this.CanAutoIncrement && this.AutoIncrement)
                    {
                        dataColumn.AutoIncrement = this.AutoIncrement;
                    }
                    else if (this.DefaultValue != null && !string.IsNullOrWhiteSpace(this.DefaultValue.ToString()))
                    {
                        dataColumn.DefaultValue = Convert.ChangeType(this.DefaultValue, this.DateType);
                        foreach (DataRow dataRow in this._dataTable.Rows)
                        {
                            if (dataRow[this._columnIndex] == DBNull.Value)
                            {
                                dataRow[this._columnIndex] = dataColumn.DefaultValue;
                            }
                        }
                    }
                    if (this.DateType == typeof(string))
                    {
                        dataColumn.MaxLength = this.MaxLength;
                    }
                    Close();
                }
                else
                {
                    if (this._isEdit)
                    {
                        this._dataTable.Columns.RemoveAt(this._columnIndex);
                    }
                    DataColumn dataColumn2 = new DataColumn(this.ColumnName, this.DateType);
                    dataColumn2.AllowDBNull = this.AllowDBNull;
                    dataColumn2.Unique = this.Unique;
                    if (this.CanAutoIncrement && this.AutoIncrement)
                    {
                        dataColumn2.AutoIncrement = this.AutoIncrement;
                    }
                    else if (this.DefaultValue != null && !string.IsNullOrWhiteSpace(this.DefaultValue.ToString()))
                    {
                        dataColumn2.DefaultValue = Convert.ChangeType(this.DefaultValue, this.DateType);
                    }
                    if (this.DateType == typeof(string))
                    {
                        dataColumn2.MaxLength = this.MaxLength;
                    }
                    if (dataColumn2.DataType != this.DateType)
                    {
                        throw new InvalidOperationException("哈哈哈");
                    }
                    this._dataTable.Columns.Add(dataColumn2);
                    if (this._isEdit)
                    {
                        dataColumn2.SetOrdinal(this._columnIndex);
                    }
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Hand);
            }
        }

        private void TypePresenter_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this._isEdit)
            {
                if (this._dataTable.Columns[this._columnIndex].DataType == this.DateType)
                {
                    if (this.WarningImage != null)
                    {
                        this.WarningImage.Visibility = Visibility.Hidden;
                    }
                    this.DataChangedWarning = string.Empty;
                }
                else
                {
                    if (this.WarningImage != null)
                    {
                        this.WarningImage.Visibility = Visibility.Visible;
                    }
                    this.DataChangedWarning = "哈哈哈";
                }
                this.OnNotifyPropertyChanged("DataChangedWarning");
            }
        }
    }
}
