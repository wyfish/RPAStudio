using RPA.Core.Activities.DataTableActivity.Operators;
using RPA.Core.Activities.DataTableActivity.ViewModel;
using Plugins.Shared.Library.Converters;
using System;
using System.Activities;
using System.Activities.Presentation;
using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace RPA.Core.Activities.DataTableActivity.Dialog
{
    public partial class JoinDataTablesWizard : WorkflowElementDialog
    {
        public ObservableCollection<JoinOperationViewModel> Arguments
        {
            get;
            set;
        }

        public JoinDataTablesWizard(ModelItem ownerActivity)
        {
            Arguments = new ObservableCollection<JoinOperationViewModel>();
            this.InitializeComponent();
            base.ModelItem = ownerActivity;
            base.Context = ownerActivity.GetEditingContext();
            this.InitalizeJoinType();
            this.InitializeArguments();
            if (this.Arguments.Count == 0)
            {
                this.Arguments.Add(new JoinOperationViewModel
                {
                    Operator = JoinOperator.LT,
                    LogicalOperator = BooleanOperator.And
                });
            }
            this.UpdateArgumentIndex(0);
        }

        private void InitalizeJoinType()
        {
            this.TypeBox.SelectedIndex = 1;
            ModelProperty modelProperty = base.ModelItem.Properties["JoinType"];
            if (modelProperty.Value.GetCurrentValue() == null)
            {
                return;
            }
            this.TypeBox.SelectedValue = modelProperty.Value.GetCurrentValue().ToString();
        }

        private void InitializeArguments()
        {
            ModelProperty expr_15 = base.ModelItem.Properties["Arguments"];
            object arg_2D_0;
            if (expr_15 == null)
            {
                arg_2D_0 = null;
            }
            else
            {
                ModelItem expr_21 = expr_15.Value;
                arg_2D_0 = ((expr_21 != null) ? expr_21.GetCurrentValue() : null);
            }
            List<JoinOperationArgument> list = arg_2D_0 as List<JoinOperationArgument>;
            if (list == null)
            {
                return;
            }
            foreach (JoinOperationArgument current in list)
            {
                JoinOperationViewModel item = new JoinOperationViewModel
                {
                    Column1 = this.ArgumentToModelItem(current.Column1, base.Context),
                    Column2 = this.ArgumentToModelItem(current.Column2, base.Context),
                    Operand = this.ArgumentToModelItem(current.Operand, base.Context),
                    LogicalOperator = current.BooleanOperator,
                    Operator = current.Operator
                };
                this.Arguments.Add(item);
            }
        }

        private void SaveArguments()
        {
            List<JoinOperationArgument> list = new List<JoinOperationArgument>();
            foreach (JoinOperationViewModel current in this.Arguments)
            {
                JoinOperationArgument item = new JoinOperationArgument
                {
                    Operand = this.ModelItemToArgument(current.Operand),
                    Column1 = this.ModelItemToArgument(current.Column1),
                    Column2 = this.ModelItemToArgument(current.Column2),
                    BooleanOperator = current.LogicalOperator,
                    Operator = current.Operator
                };
                list.Add(item);
            }
            base.ModelItem.Properties["Arguments"].SetValue(list);
        }

        protected override void OnWorkflowElementDialogClosed(bool? dialogResult)
        {
            if (dialogResult.HasValue && dialogResult.Value)
            {
                this.SaveArguments();
            }
            JoinType joinType = (JoinType)Enum.Parse(typeof(JoinType), this.TypeBox.SelectedValue.ToString());
            base.ModelItem.Properties["JoinType"].SetValue(joinType);
            base.OnWorkflowElementDialogClosed(dialogResult);
        }

        private void AddArgumentClick(object sender, RoutedEventArgs e)
        {
            int num = this.GetCurrentItemIndex(sender) + 1;
            this.Arguments.Insert(num, new JoinOperationViewModel
            {
                LogicalOperator = BooleanOperator.And,
                Operator = JoinOperator.LT,
                Index = num
            });
            this.UpdateArgumentIndex(num + 1);
        }

        private void RemoveArgumentClick(object sender, RoutedEventArgs e)
        {
            if (this.Arguments.Count == 1)
            {
                return;
            }
            int currentItemIndex = this.GetCurrentItemIndex(sender);
            this.Arguments.RemoveAt(currentItemIndex);
            this.UpdateArgumentIndex(currentItemIndex);
        }

        private void UpdateArgumentIndex(int index)
        {
            for (int i = index; i < this.Arguments.Count; i++)
            {
                this.Arguments[i].Index = i;
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
