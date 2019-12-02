using System.Activities.Presentation.PropertyEditing;
using System.Windows;

namespace RPA.Core.Activities.FileActivity
{
    public class ChangeTypeEditor : PropertyValueEditor
    {
        public ChangeTypeEditor()
        {
            this.InlineEditorTemplate = PropertyEditorResources.GetResources()["ChangeTypeEditor"] as DataTemplate;
        }
    }
}
