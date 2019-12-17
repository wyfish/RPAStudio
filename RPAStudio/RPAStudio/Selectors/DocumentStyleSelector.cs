using RPAStudio.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace RPAStudio.Selectors
{

  class DocumentStyleSelector : StyleSelector
  {
    public Style DocumentStyle
        {
      get;
      set;
    }

    public override System.Windows.Style SelectStyle(object item, System.Windows.DependencyObject container)
    {
      if (item is DocumentViewModel)
        return DocumentStyle;

      return base.SelectStyle(item, container);
    }
  }
}
