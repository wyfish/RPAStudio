using Plugins.Shared.Library.Editors;
using System.Activities.Presentation;
using System.Activities.Presentation.Model;
using System.Windows;

namespace RPA.Script.Activities.Python
{
    // InvokePythonScriptDesigner.xaml 的交互逻辑
    public partial class InvokePythonScriptDesigner
    {
        public InvokePythonScriptDesigner()
        {
            InitializeComponent();
        }

        private void EditCodeBtn_Click(object sender, RoutedEventArgs e)
        {
            string a = base.ModelItem.Properties["Code"].Value?.ToString();
            new PythonScriptEditor().ShowDialog(base.ModelItem);
        }

        private void EditArgumentsBtn_Click(object sender, RoutedEventArgs e)
        {
            DynamicArgumentDesignerOptions options = new DynamicArgumentDesignerOptions
            {
                Title = "执行Python脚本参数设置"
            };
            ModelItemDictionary dictionary = base.ModelItem.Properties["Arguments"].Dictionary;
            using (ModelEditingScope modelEditingScope = dictionary.BeginEdit("PythonScriptArgumentEditing"))
            {
                if (DynamicArgumentDialog.ShowDialog(base.ModelItem, dictionary, base.Context, base.ModelItem.View, options))
                {
                    modelEditingScope.Complete();
                }
                else
                {
                    modelEditingScope.Revert();
                }
            }
        }
    }
}
