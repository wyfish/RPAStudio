using System.Activities.Presentation;
using System.Activities.Presentation.Model;
using System.Windows;

namespace RPA.Core.Activities.Execute
{
    // InvokeComMethod.xaml 的交互逻辑
    public partial class InvokeComMethod
    {
        public InvokeComMethod()
        {
            InitializeComponent();
        }

        private void EditArgumentsBtn_Click(object sender, RoutedEventArgs e)
        {
            DynamicArgumentDesignerOptions options = new DynamicArgumentDesignerOptions
            {
                Title = "方法参数"
            };
            ModelItemDictionary dictionary = base.ModelItem.Properties["Arguments"].Dictionary;
            using (ModelEditingScope modelEditingScope = dictionary.BeginEdit())
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
