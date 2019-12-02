using RPA.Core.Activities.DialogActivity.TypeEditor;
using Plugins.Shared.Library;
using System;
using System.Activities;
using System.Activities.Presentation.Metadata;
using System.Activities.Presentation.PropertyEditing;
using System.ComponentModel;
using System.Windows;

namespace RPA.Core.Activities.DialogActivity
{
    [Designer(typeof(MessageBoxDesigner))]
    public sealed class MessageBoxActivity : CodeActivity
    {
        public new string DisplayName
        {
            get
            {
                return "Message Box";
            }
        }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.Core.Activities;Component/Resources/Dialog/messagebox.png";
            }
        }

        [Category("Input")]
        public Int32 Buttons { get; set; }
        static MessageBoxActivity()
        {
            AttributeTableBuilder builder = new AttributeTableBuilder();
            builder.AddCustomAttributes(typeof(MessageBoxActivity), "Buttons", new EditorAttribute(typeof(ButtonsClickTypeEditor), typeof(PropertyValueEditor)));
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }

        [Category("Input")]
        [Description("消息框对话框的标题。")]
        [DisplayNameAttribute("Caption")]
        public InArgument<string> Captions { get; set; }

        [RequiredArgument]
        [Category("Input")]
        [Description("要显示在消息框中的文本。")]
        [DisplayNameAttribute("Text")]
        public InArgument<string> Text { get; set; }

        [Category("Input")]
        [DisplayNameAttribute("TopMost")]
        public bool TopMost { get; set; }

        [Category("输出")]
        [Description("表示消息框对话框中按下的按钮的字符串。它可以是:Ok,Yes,No或Cancel")]
        public OutArgument<string> ChosenButton { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                string _Captions = Captions.Get(context);
                string _Text = Text.Get(context);

                if (_Captions == "") _Captions = "RPAStudio";

                if (Buttons > 1) Buttons++;

                MessageBoxOptions mo;
                if (TopMost)
                {
                    mo = MessageBoxOptions.DefaultDesktopOnly;
                }  
                else
                {
                    mo = MessageBoxOptions.None;
                }
                var result = MessageBox.Show(_Text, _Captions, (MessageBoxButton)Buttons, MessageBoxImage.None, MessageBoxResult.None, mo);

                ChosenButton.Set(context,result.ToString());
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", e.Message);
            }
        }
    }
}
