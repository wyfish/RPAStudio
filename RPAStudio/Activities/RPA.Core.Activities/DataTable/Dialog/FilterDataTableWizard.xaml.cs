using System.Activities.Presentation;
using System.Activities.Presentation.Model;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using RPA.Core.Activities.DataTableActivity.Operators;
using Plugins.Shared.Library.Converters;
using RPA.Core.Activities.DataTableActivity.Dialog;

namespace RPA.Core.Activities.DataTableActivity.Dialog
{
    // FilterDataTableWizard.xaml 的交互逻辑
    public partial class FilterDataTableWizard : WorkflowElementDialog
    {
        private const string SelectColumnsModeProperty = "SelectColumnsMode";

        private const string FilterRowsModeProperty = "FilterRowsMode";

        private const string FiltersProperty = "Filters";

        public const string SelectColumnsProperty = "SelectColumns";

        public ObservableCollection<FilterOperationViewModel> Filters
        {
            get;
            set;
        }

        public ObservableCollection<SelectColumnViewModel> SelectColumns
        {
            get;
            set;
        }

        public FilterDataTableWizard(ModelItem ownerActivity)
        {
            Filters = new ObservableCollection<FilterOperationViewModel>();
            SelectColumns = new ObservableCollection<SelectColumnViewModel>();
            this.InitializeComponent();
            base.ModelItem = ownerActivity;
            base.Context = ownerActivity.GetEditingContext();
            this.InitializeRadioButton("SelectColumnsMode",RemoveColumnsRadio);
            this.InitializeRadioButton("FilterRowsMode", RemoveRowsRadio);
            this.InitializeFilters();
            if (this.Filters.Count == 0)
            {
                FilterOperationViewModel item = new FilterOperationViewModel
                {
                    Operator = FilterOperator.LT,
                    LogicalOperator = BooleanOperator.And
                };
                this.Filters.Add(item);
            }
            this.UpdateFilterIndex(0);
            this.InitializeSelectColumns();
            if (this.SelectColumns.Count == 0)
            {
                this.SelectColumns.Add(new SelectColumnViewModel());
            }
            this.UpdateSelectColumnIndex(0);
        }

        private void InitializeSelectColumns()
        {
            List<InArgument> list = ((base.ModelItem.Properties["SelectColumns"]?.Value == null) ? null : ((List<InArgument>)base.ModelItem.Properties["SelectColumns"]?.Value.GetCurrentValue())) as List<InArgument>;
            if (list != null)
            {
                list.ForEach(delegate (InArgument column) {
                    SelectColumnViewModel item = new SelectColumnViewModel
                    {
                        Column = this.ArgumentToModelItem(column, base.Context)
                    };
                    this.SelectColumns.Add(item);
                });
            }

        }

        private void InitializeFilters()
        {
            List<FilterOperationArgument> list = ((base.ModelItem.Properties["Filters"]?.Value == null) ? null : ((List<FilterOperationArgument>)base.ModelItem.Properties["Filters"]?.Value.GetCurrentValue())) as List<FilterOperationArgument>;
            if (list != null)
            {
                foreach (FilterOperationArgument argument in list)
                {
                    FilterOperationViewModel item = new FilterOperationViewModel
                    {
                        Column = this.ArgumentToModelItem(argument.Column, base.Context),
                        Operand = this.ArgumentToModelItem(argument.Operand, base.Context),
                        LogicalOperator = argument.BooleanOperator,
                        Operator = argument.Operator
                    };
                    this.Filters.Add(item);
                }
            }
        }

        private void InitializeRadioButton(string property, RadioButton radioButton)
        {
            ModelProperty property2 = base.ModelItem.Properties[property];
            if ((property2 != null) && (property2.Value?.GetCurrentValue() != null))
            {
                SelectMode currentValue = (SelectMode)property2.Value.GetCurrentValue();
                radioButton.IsChecked = new bool?(currentValue > SelectMode.Keep);
            }
        }

        private void SaveFilters()
        {
            List<FilterOperationArgument> filters = new List<FilterOperationArgument>();
            foreach (FilterOperationViewModel model in this.Filters)
            {
                if (FilterOperationArgument.IsEmptyOperator(model.Operator))
                {
                    model.Operand = null;
                }
                FilterOperationArgument item = new FilterOperationArgument
                {
                    Operand = this.ModelItemToArgument(model.Operand),
                    Column = this.ModelItemToArgument(model.Column),
                    BooleanOperator = model.LogicalOperator,
                    Operator = model.Operator
                };
                filters.Add(item);
            }
            SetFirstFilterOperator(filters, BooleanOperator.And);
            base.ModelItem.Properties["Filters"].SetValue(filters);
        }

        private void SetFirstFilterOperator(List<FilterOperationArgument> filters, BooleanOperator booleanOperator)
        {
            FilterOperationArgument argument = (filters != null) ? filters.FirstOrDefault<FilterOperationArgument>() : null;
            if (argument != null)
            {
                argument.BooleanOperator = booleanOperator;
            }
        }


        private void SaveSelectColumns()
        {
            new List<InArgument>();
            base.ModelItem.Properties["SelectColumns"].SetValue((from selectColumn in this.SelectColumns
                select this.ModelItemToArgument(selectColumn.Column)).ToList<InArgument>());
        }

        protected override void OnWorkflowElementDialogClosed(bool? dialogResult)
        {
            if (dialogResult.HasValue && dialogResult.Value)
            {
                this.SaveFilters();
                this.SaveSelectColumns();
                bool flag = !this.KeepColumnsRadio.IsChecked.HasValue ? false : this.KeepColumnsRadio.IsChecked.Value;
                base.ModelItem.Properties["SelectColumnsMode"].SetValue(flag ? SelectMode.Keep : SelectMode.Remove);
                bool flag2 = !this.KeepRowsRadio.IsChecked.HasValue ? false : this.KeepRowsRadio.IsChecked.Value;
                base.ModelItem.Properties["FilterRowsMode"].SetValue(flag2 ? SelectMode.Keep : SelectMode.Remove);
            }
            base.OnWorkflowElementDialogClosed(dialogResult);
        }

        private void AddFilterClick(object sender, RoutedEventArgs e)
        {
            int num = this.GetCurrentItemIndex(sender) + 1;
            this.Filters.Insert(num, new FilterOperationViewModel
            {
                LogicalOperator = BooleanOperator.And,
                Operator = FilterOperator.LT,
                Index = num
            });
            this.UpdateFilterIndex(num + 1);
        }

        private void RemoveFilterClick(object sender, RoutedEventArgs e)
        {
            if (this.Filters.Count == 1)
            {
                return;
            }
            int currentItemIndex = this.GetCurrentItemIndex(sender);
            this.Filters.RemoveAt(currentItemIndex);
            this.UpdateFilterIndex(currentItemIndex);
        }

        private void AddSelectClick(object sender, RoutedEventArgs e)
        {
            int num = this.GetCurrentItemIndex(sender) + 1;
            this.SelectColumns.Insert(num, new SelectColumnViewModel
            {
                Index = num
            });
            this.UpdateSelectColumnIndex(num + 1);
        }

        private void RemoveSelectClick(object sender, RoutedEventArgs e)
        {
            if (this.SelectColumns.Count == 1)
            {
                return;
            }
            int currentItemIndex = this.GetCurrentItemIndex(sender);
            this.SelectColumns.RemoveAt(currentItemIndex);
            this.UpdateSelectColumnIndex(currentItemIndex);
        }

        private void UpdateSelectColumnIndex(int index)
        {
            for (int i = index; i < this.SelectColumns.Count; i++)
            {
                this.SelectColumns[i].Index = i;
            }
        }

        private void UpdateFilterIndex(int index)
        {
            for (int i = index; i < this.Filters.Count; i++)
            {
                this.Filters[i].Index = i;
            }
        }

        protected InArgument ModelItemToArgument(ModelItem value)
        {
            return ((value != null) ? value.GetCurrentValue() : null) as InArgument;
        }

        protected ModelItem ArgumentToModelItem(InArgument arg, EditingContext editingContext)
        {
            if (arg == null)
            {
                return null;
            }
            return ModelFactory.CreateItem(editingContext, arg);
        }

        private int GetCurrentItemIndex(object sender)
        {
            Button button = sender as Button;
            if (button == null)
            {
                return -1;
            }
            return (int)button.Tag;
        }
    }
}
