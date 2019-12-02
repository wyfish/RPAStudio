using System.Activities.Presentation;
using System.Activities.Presentation.Converters;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.PropertyEditing;
using System.Windows;

namespace Plugins.Shared.Library.Editors
{
    public partial class ArgumentCollectionEditor : DialogPropertyValueEditor
    {
        private static DataTemplate EditorTemplate = (DataTemplate)new EditorTemplates()["ArgumentDictionaryEditor"];

        public ArgumentCollectionEditor()
        {
            this.InlineEditorTemplate = EditorTemplate;
        }

        public override void ShowDialog(PropertyValue propertyValue, IInputElement commandSource)
        {
            string displayName = propertyValue.ParentProperty.DisplayName;
            ModelItem ownerActivity = new ModelPropertyEntryToOwnerActivityConverter().Convert(propertyValue.ParentProperty, typeof(ModelItem), false, null) as ModelItem;
            ShowDialog(displayName, ownerActivity);
        }


        public static void ShowDialog(string propertyName, ModelItem ownerActivity)
        {
            DynamicArgumentDesignerOptions options = new DynamicArgumentDesignerOptions
            {
                Title = propertyName
            };
            ModelItem collection = ownerActivity.Properties["Files"].Collection;
            using (ModelEditingScope modelEditingScope = collection.BeginEdit(propertyName + "Editing"))
            {
                if (DynamicArgumentDialog.ShowDialog(ownerActivity, collection, ownerActivity.GetEditingContext(), ownerActivity.View, options))
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
