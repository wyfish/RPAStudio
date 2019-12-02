using RPA.Core.Activities.DataTableActivity.Dialog;
using Microsoft.VisualBasic.Activities;
using Plugins.Shared.Library.Converters;
using Plugins.Shared.Library.Librarys;
using System;
using System.Activities;
using System.Activities.Expressions;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace RPA.Core.Activities.DataTableActivity
{
    public partial class GenerateDataTableDesigner
    {
        private string sampleInput;

        public GenerateDataTableDesigner()
        {
            InitializeComponent();
        }

        private void GenerateDataTableButton_Click(object sender, RoutedEventArgs e)
        {
            FormatOptions presetFormatOptions = new FormatOptions
            {
                PreserveNewLines = this.TryGetArgumentValue<bool>("PreserveNewLines"),
                CSVParsing = this.TryGetArgumentValue<bool>("CSVParse"),
                PreserveStrings = this.TryGetArgumentValue<bool>("PreserveStrings"),
                ColumnSeparators = this.GetSeparatorsFromArgumentValue("ColumnSeparators"),
                NewLineSeparator = this.GetSeparatorsFromArgumentValue("NewLineSeparator")
            };
            try
            {
                char[] trimChars = new char[((!((base.ModelItem.Properties["ColumnSizes"].ComputedValue as InArgument<IEnumerable<int>>).Expression is VisualBasicValue<IEnumerable<int>>) ?
                    null : ((base.ModelItem.Properties["ColumnSizes"].ComputedValue as InArgument<IEnumerable<int>>).Expression as VisualBasicValue<IEnumerable<int>>).ExpressionText) == null) ? 0 : 1];
                trimChars[0] = '{';
                char[] chArray2 = new char[] { '}' };
                string str2 = (!((base.ModelItem.Properties["ColumnSizes"].ComputedValue as InArgument<IEnumerable<int>>).Expression is VisualBasicValue<IEnumerable<int>>) ? 
                    null : ((base.ModelItem.Properties["ColumnSizes"].ComputedValue as InArgument<IEnumerable<int>>).Expression as VisualBasicValue<IEnumerable<int>>).ExpressionText).TrimStart(trimChars).TrimEnd(chArray2);
                presetFormatOptions.ColumnSizes = new IntCollectionToStringConverter().ConvertBack(str2, null, ",", null) as IEnumerable<int>;
            }
            catch (Exception exception1)
            {
                Trace.TraceError(exception1.Message);
            }
            TableOptions presetTableOptions = new TableOptions
            {
                UseColumnHeader = (bool)base.ModelItem.Properties["UseColumnHeader"].Value.GetCurrentValue(),
                UseRowHeader = (bool)base.ModelItem.Properties["UseRowHeader"].Value.GetCurrentValue(),
                AutoDetectTypes = (bool)base.ModelItem.Properties["AutoDetectTypes"].Value.GetCurrentValue()
            };
            string inputText = this.TryGetArgumentValue<string>("InputString") ?? this.sampleInput;
            GenerateDataTableDialog dialog = new GenerateDataTableDialog(presetFormatOptions, presetTableOptions, inputText);
            bool? nullable = dialog.ShowDialog();
            bool flag = true;
            if ((nullable.GetValueOrDefault() == flag) ? nullable.HasValue : false)
            {
                Func<string, string> func = delegate (string s)
                {
                    if (s != Environment.NewLine)
                    {
                        return "string.Format(\"" + s.Replace(Environment.NewLine, "{0}") + "\", Environment.NewLine)";
                    }
                    return "Environment.NewLine";
                };
                if (!string.IsNullOrEmpty(dialog.FormatOptions.ColumnSeparators))
                {
                    if (dialog.FormatOptions.ColumnSeparators.Contains(Environment.NewLine))
                    {
                        base.ModelItem.Properties["ColumnSeparators"].SetValue(new InArgument<string>(new VisualBasicValue<string>(func(dialog.FormatOptions.ColumnSeparators))));
                    }
                    else
                    {
                        base.ModelItem.Properties["ColumnSeparators"].SetValue(new InArgument<string>(dialog.FormatOptions.ColumnSeparators));
                    }
                }
                else
                {
                    base.ModelItem.Properties["ColumnSeparators"].SetValue(null);
                }
                if (!string.IsNullOrEmpty(dialog.FormatOptions.NewLineSeparator))
                {
                    if (dialog.FormatOptions.NewLineSeparator.Contains(Environment.NewLine))
                    {
                        base.ModelItem.Properties["NewLineSeparator"].SetValue(new InArgument<string>(new VisualBasicValue<string>(func(dialog.FormatOptions.NewLineSeparator))));
                    }
                    else
                    {
                        base.ModelItem.Properties["NewLineSeparator"].SetValue(new InArgument<string>(dialog.FormatOptions.NewLineSeparator));
                    }
                }
                else
                {
                    base.ModelItem.Properties["NewLineSeparator"].SetValue(null);
                }
                if ((dialog.FormatOptions.ColumnSizes != null) && dialog.FormatOptions.ColumnSizes.Any<int>())
                {
                    string expressionText = $"{{ {new IntCollectionToStringConverter().Convert(dialog.FormatOptions.ColumnSizes, null, ",", null) as string} }}";
                    InArgument<IEnumerable<int>> argument = new InArgument<IEnumerable<int>>
                    {
                        Expression = new VisualBasicValue<IEnumerable<int>>(expressionText)
                    };
                    base.ModelItem.Properties["ColumnSizes"].SetValue(argument);
                }
                else
                {
                    base.ModelItem.Properties["ColumnSizes"].SetValue(null);
                }
                base.ModelItem.Properties["PreserveNewLines"].SetValue(dialog.FormatOptions.PreserveNewLines);
                base.ModelItem.Properties["CSVParse"].SetValue(new InArgument<bool>(dialog.FormatOptions.CSVParsing));
                base.ModelItem.Properties["PreserveStrings"].SetValue(dialog.FormatOptions.PreserveStrings);
                base.ModelItem.Properties["UseColumnHeader"].SetValue(dialog.TableOptions.UseColumnHeader);
                base.ModelItem.Properties["UseRowHeader"].SetValue(dialog.TableOptions.UseRowHeader);
                base.ModelItem.Properties["AutoDetectTypes"].SetValue(dialog.TableOptions.AutoDetectTypes);
                this.sampleInput = dialog.InputText;
            }
        }

        private string GetSeparatorsFromArgumentValue(string propertyName)
        {
            string str = this.TryGetArgumentValue<string>(propertyName);
            if (string.IsNullOrEmpty(str))
            {
                try
                {
                    if ((!((base.ModelItem.Properties[propertyName].ComputedValue as InArgument<string>).Expression is VisualBasicValue<string>) ? null : ((base.ModelItem.Properties[propertyName].ComputedValue as InArgument<string>).Expression as VisualBasicValue<string>).ExpressionText) == "Environment.NewLine")
                    {
                        return Environment.NewLine;
                    }
                }
                catch (Exception exception1)
                {
                    Trace.TraceError(exception1.Message);
                }
            }
            return str;
        }


        private T TryGetArgumentValue<T>(string propertyName)
        {
            try
            {
                InArgument<T> expr_1B = base.ModelItem.Properties[propertyName].ComputedValue as InArgument<T>;
                Literal<T> literal;
                if ((literal = (((expr_1B != null) ? expr_1B.Expression : null) as Literal<T>)) != null)
                {
                    return literal.Value;
                }
            }
            catch (Exception arg_3B_0)
            {
                Trace.TraceError(arg_3B_0.Message);
            }
            return default(T);
        }
    }
}