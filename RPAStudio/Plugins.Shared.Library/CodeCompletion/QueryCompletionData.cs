using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using Microsoft.CodeAnalysis;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Plugins.Shared.Library.CodeCompletion
{
    public class QueryCompletionData : ICompletionData
    {
        private static ImageSource MethodIcon;
        private static ImageSource PropertyIcon;
        private static ImageSource FieldIcon;
        private static ImageSource EventIcon;
        private static ImageSource LocalIcon;
        private static ImageSource NamespaceIcon;
        private static ImageSource NamedTypeIcon;

        private IconType iconType;

        static QueryCompletionData()
        {
            MethodIcon = GetImageSourceFromResource("Icons.16x16.Method.png");
            PropertyIcon = GetImageSourceFromResource("Icons.16x16.Property.png");
            FieldIcon = GetImageSourceFromResource("Icons.16x16.Field.png");
            EventIcon = GetImageSourceFromResource("Icons.16x16.Event.png");
            LocalIcon = GetImageSourceFromResource("Icons.16x16.Local.png");
            NamespaceIcon = GetImageSourceFromResource("Icons.16x16.Namespace.png");
            NamedTypeIcon = GetImageSourceFromResource("Icons.16x16.NamedType.png");
        }

        static internal ImageSource GetImageSourceFromResource(string resourceName)
        {
            return BitmapFrame.Create(typeof(QueryCompletionData).Assembly.GetManifestResourceStream(typeof(QueryCompletionData).Namespace + ".ClassBrowserIcons." + resourceName));
        }

        private void pocessKind(SymbolKind Kind)
        {
            switch (Kind)
            {
                case SymbolKind.Event: iconType = IconType.Event; break;
                case SymbolKind.Field: iconType = IconType.Field; break;
                case SymbolKind.Method: iconType = IconType.Method; break;
                case SymbolKind.Property: iconType = IconType.Property; break;
                case SymbolKind.Local: iconType = IconType.Local; break;
                case SymbolKind.Namespace: iconType = IconType.Namespace; break;
                case SymbolKind.NamedType: iconType = IconType.NamedType; break;
            }
        }

        public QueryCompletionData(string name, ISymbol[] symbols)
        {
            this.Text = name;
            this.Description = symbols[0].ToDisplayString();
            pocessKind(symbols[0].Kind);
        }

        public QueryCompletionData(string name, string description, SymbolKind Kind)
        {
            this.Text = name;
            this.Description = description;
            pocessKind(Kind);
        }

        public ImageSource Image
        {
            get
            {
                switch (iconType)
                {
                    case IconType.Event: return EventIcon;
                    case IconType.Field: return FieldIcon;
                    case IconType.Property: return PropertyIcon;
                    case IconType.Method: return MethodIcon;
                    case IconType.Local: return LocalIcon;
                    case IconType.Namespace: return NamespaceIcon;
                    case IconType.NamedType: return NamedTypeIcon;
                    default: return MethodIcon;
                }
            }
        }

        public string Text { get; private set; }

        // Use this property if you want to show a fancy UIElement in the list.
        public object Content
        {
            get { return this.Text; }
        }

        public object Description { get; private set; }


        public void Complete(TextArea textArea, ISegment completionSegment,
            EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, this.Text);

            textArea.PerformTextInput("");//解决文本过长时不自动移动的问题
        }

        public double Priority
        {
            get { return 1.0; }
        }

        enum IconType
        {
            Property,
            Field,
            Method,
            Event,
            Local,
            Namespace,
            NamedType,
        }
    }
}
