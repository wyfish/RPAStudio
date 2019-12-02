using System.Activities.Presentation.Model;
using System.IO;
using System.Xml;

namespace Plugins.Shared.Library.Editors
{
    public partial class PythonScriptEditorDialog
    {
        public PythonScriptEditorDialog(ModelItem ownerActivity)
        {
            base.ModelItem = ownerActivity;
            base.Context = ownerActivity.GetEditingContext();
            InitializeComponent();
            textEditor.Text = base.ModelItem.Properties["Code"].ComputedValue as string;

            //Python语法高亮
            XmlTextReader xshd_reader = new XmlTextReader(new MemoryStream(Properties.Resources.Python_Mode));
            textEditor.SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(xshd_reader, ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance);
            xshd_reader.Close();
        }

        protected override void OnWorkflowElementDialogClosed(bool? dialogResult)
        {
            if (dialogResult.HasValue && dialogResult.Value)
            {
                base.ModelItem.Properties["Code"].ComputedValue = textEditor.Text;
            }
        }

    }
}
