using RPA.Core.Activities.DataTableActivity.Operators;
using Plugins.Shared.Library.Converters;
using System;
using System.Activities.Presentation.Model;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace RPA.Core.Activities.DataTableActivity
{
    public class FilterOperationViewModel : INotifyPropertyChanged
    {
        private int _index;

        private FilterOperator _operator;

        private ModelItem _operand;

        private ModelItem _column;

        private BooleanOperator _booleanOperator;

        [method: CompilerGenerated]
        [CompilerGenerated]
        public event PropertyChangedEventHandler PropertyChanged;

        public ModelItem Operand
        {
            get
            {
                return this._operand;
            }
            set
            {
                this._operand = value;
                this.NotifyPropertyChanged("Operand");
            }
        }

        public ModelItem Column
        {
            get
            {
                return this._column;
            }
            set
            {
                this._column = value;
                this.NotifyPropertyChanged("Column");
            }
        }

        public BooleanOperator LogicalOperator
        {
            get
            {
                return this._booleanOperator;
            }
            set
            {
                this._booleanOperator = value;
                this.NotifyPropertyChanged("LogicalOperator");
            }
        }

        public FilterOperator Operator
        {
            get
            {
                return this._operator;
            }
            set
            {
                this._operator = value;
                this.NotifyPropertyChanged("Operator");
            }
        }

        public int Index
        {
            get
            {
                return this._index;
            }
            set
            {
                this._index = value;
                this.NotifyPropertyChanged("Index");
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChangedEventHandler expr_06 = this.PropertyChanged;
            if (expr_06 == null)
            {
                return;
            }
            expr_06(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class SelectColumnViewModel : INotifyPropertyChanged
    {
        private ModelItem _column;

        private int _index;

        [method: CompilerGenerated]
        [CompilerGenerated]
        public event PropertyChangedEventHandler PropertyChanged;

        public ModelItem Column
        {
            get
            {
                return this._column;
            }
            set
            {
                this._column = value;
                this.NotifyPropertyChanged("Column");
            }
        }

        public int Index
        {
            get
            {
                return this._index;
            }
            set
            {
                this._index = value;
                this.NotifyPropertyChanged("Index");
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChangedEventHandler expr_06 = this.PropertyChanged;
            if (expr_06 == null)
            {
                return;
            }
            expr_06(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
