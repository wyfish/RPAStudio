using System;
using System.Activities.Presentation.Converters;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.PropertyEditing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Plugins.Shared.Library.Editors
{
    public class TextEditor : DialogPropertyValueEditor
    {
        public TextEditor()
        {
            base.InlineEditorTemplate = (DataTemplate)new EditorTemplates()["TextEditorTemplate"];
        }

        public override void ShowDialog(PropertyValue propertyValue, IInputElement commandSource)
        {
            try
            {
                ModelItem modelItem = new ModelPropertyEntryToOwnerActivityConverter().Convert(propertyValue.ParentProperty, typeof(ModelItem), false, null) as ModelItem;
                using (ModelEditingScope modelEditingScope = modelItem.BeginEdit())
                {
                    if (new TextEditorDialog(modelItem).ShowOkCancel())
                    {
                        modelEditingScope.Complete();
                    }
                    else
                    {
                        modelEditingScope.Revert();
                    }
                }
            }
            catch
            {
            }
        }
    }

}
