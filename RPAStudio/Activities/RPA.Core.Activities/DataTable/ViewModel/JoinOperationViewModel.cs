using RPA.Core.Activities.DataTableActivity.Operators;
using Plugins.Shared.Library.Converters;
using System;
using System.Activities.Presentation.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RPA.Core.Activities.DataTableActivity.ViewModel
{
    public class JoinOperationViewModel : INotifyPropertyChanged
    {
        private int _index;

        private JoinOperator _operator;

        private ModelItem _operand;

        private ModelItem _column1;

        private ModelItem _column2;

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

        public ModelItem Column1
        {
            get
            {
                return this._column1;
            }
            set
            {
                this._column1 = value;
                this.NotifyPropertyChanged("Column1");
            }
        }

        public ModelItem Column2
        {
            get
            {
                return this._column2;
            }
            set
            {
                this._column2 = value;
                this.NotifyPropertyChanged("Column2");
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

        public JoinOperator Operator
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
}
