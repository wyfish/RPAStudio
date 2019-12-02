using System.Activities.Presentation.PropertyEditing;
using System.Windows;

namespace RPA.Core.Activities.FileActivity
{
    public class NotifyFiltersEditor : PropertyValueEditor
    {
        public NotifyFiltersEditor()
        {
            this.InlineEditorTemplate = PropertyEditorResources.GetResources()["NotifyFiltersEditor"] as DataTemplate;
        }
    }
}
