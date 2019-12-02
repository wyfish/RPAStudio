using Plugins.Shared.Library.Editors;
using System;
using System.Activities;
using System.Activities.Expressions;
using System.Activities.Presentation;
using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace RPA.Core.Activities.Workflow
{
    // InvokeCodeDesigner.xaml 的交互逻辑
    public partial class InvokeCodeDesigner
    {
        public InvokeCodeDesigner()
        {
            InitializeComponent();
            base.Loaded += InvokeCodeDesigner_Loaded;
        }

        private void InvokeCodeDesigner_Loaded(object sender, RoutedEventArgs e)
        {
            CompileCode();
        }

        private void CompileCode()
        {
            string code = base.ModelItem.Properties["Code"].Value?.ToString();
            object currentValue = base.ModelItem.Properties["Arguments"].Dictionary.GetCurrentValue();
            CompileCode(code, currentValue);
        }

        private void CompileCode(string code, object args)
        {
            Dictionary<string, Argument> dictionary = args as Dictionary<string, Argument>;
            if (code != null && dictionary != null)
            {
                string importsHeader = GetImportsHeader(base.Context, base.ModelItem);
                List<Tuple<string, Type, ArgumentDirection>> list = new List<Tuple<string, Type, ArgumentDirection>>(dictionary.Count);
                foreach (KeyValuePair<string, Argument> item in dictionary)
                {
                    list.Add(new Tuple<string, Type, ArgumentDirection>(item.Key, item.Value.ArgumentType, item.Value.Direction));
                }
                try
                {
                    InvokeCodeActivity.CreateCompilerRunner(code, importsHeader, list);
                    (base.ModelItem.GetCurrentValue() as InvokeCodeActivity).SetSuccessfulCompilation();
                    base.ModelItem.Properties["CompilationError"].ComputedValue = "";
                }
                catch (Exception ex)
                {
                    (base.ModelItem.GetCurrentValue() as InvokeCodeActivity).SetCompilationError(ex.Message);
                    base.ModelItem.Properties["CompilationError"].ComputedValue = ex.Message;
                }
            }
        }

        public static IEnumerable<string> EnumerateImports(EditingContext context, ModelItem modelItem)
        {
            IEnumerable<string> enumerable = context?.Services.GetService(typeof(HashSet<string>)) as HashSet<string>;
            IEnumerable<string> first = enumerable ?? Enumerable.Empty<string>();
            object obj = modelItem?.Root?.GetCurrentValue();
            object enumerable2;
            if (obj == null)
            {
                enumerable2 = Enumerable.Empty<string>();
            }
            else
            {
                enumerable = TextExpression.GetNamespacesForImplementation(obj);
                enumerable2 = enumerable;
            }
            IEnumerable<string> second = (IEnumerable<string>)enumerable2;
            return first.Union(second).Distinct();
        }

        private string GetImportsHeader(EditingContext context, ModelItem modelItem)
        {
            return InvokeCodeActivity.GetImports(EnumerateImports(context, modelItem));
        }

        private void EditCodeBtn_Click(object sender, RoutedEventArgs e)
        {
            string a = base.ModelItem.Properties["Code"].Value?.ToString();
            new VBNetCodeEditor().ShowDialog(base.ModelItem);
            string text = base.ModelItem.Properties["Code"].Value?.ToString();
            if (a != text)
            {
                object currentValue = base.ModelItem.Properties["Arguments"].Dictionary.GetCurrentValue();
                CompileCode(text, currentValue);
            }
        }

        private void EditArgumentsBtn_Click(object sender, RoutedEventArgs e)
        {
            DynamicArgumentDesignerOptions options = new DynamicArgumentDesignerOptions
            {
                Title = "执行代码参数设置"
            };
            ModelItemDictionary dictionary = base.ModelItem.Properties["Arguments"].Dictionary;
            using (ModelEditingScope modelEditingScope = dictionary.BeginEdit("ChildArgumentEditing"))
            {
                if (DynamicArgumentDialog.ShowDialog(base.ModelItem, dictionary, base.Context, base.ModelItem.View, options))
                {
                    modelEditingScope.Complete();
                    CompileCode();
                }
                else
                {
                    modelEditingScope.Revert();
                }
            }
        }




    }
}
