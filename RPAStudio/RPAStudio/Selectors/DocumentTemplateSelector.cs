using System.Windows.Controls;
using System.Windows;
using Xceed.Wpf.AvalonDock.Layout;
using RPAStudio.ViewModel;

namespace RPAStudio.Selectors
{

  class DocumentTemplateSelector : DataTemplateSelector
    {
        public DocumentTemplateSelector()
        {
        
        }


        public DataTemplate DocumentViewTemplate
        {
            get;
            set;
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var itemAsLayoutContent = item as LayoutContent;

            if (item is DocumentViewModel)
                return DocumentViewTemplate;

            return base.SelectTemplate(item, container);
        }
    }
}
