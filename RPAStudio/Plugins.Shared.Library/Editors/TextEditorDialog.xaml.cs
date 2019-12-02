using System.Activities.Presentation.Model;

namespace Plugins.Shared.Library.Editors
{
    // TextEditorDialog.xaml 的交互逻辑
    public partial class TextEditorDialog
    {
        public TextEditorDialog(ModelItem ownerActivity)
        {
            base.ModelItem = ownerActivity;
            base.Context = ownerActivity.GetEditingContext();
            InitializeComponent();
        }
    }
}
