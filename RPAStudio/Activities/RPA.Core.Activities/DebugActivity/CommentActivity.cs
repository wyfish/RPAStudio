using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.ComponentModel;
using System.Activities.Presentation.Metadata;
using System.Activities.Presentation.PropertyEditing;
using Plugins.Shared.Library.Editors;

namespace RPA.Core.Activities.DebugActivity
{
    [Designer(typeof(CommentDesigner))]
    public sealed class CommentActivity : CodeActivity
    {

        [DisplayName("Text")]
        public string Text
        {
            get;
            set;
        }

        public CommentActivity()
        {
            Text = $"// {Localize.LocalizedResources.GetString("WriteCommentInTextProperty")}"; // 请在Text属性中写下注释内容

            var builder = new AttributeTableBuilder();
            builder.AddCustomAttributes(typeof(CommentActivity), "Text", new EditorAttribute(typeof(TextEditor), typeof(DialogPropertyValueEditor)));
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }

        protected override void Execute(CodeActivityContext context)
        {
            
        }
    }
}
