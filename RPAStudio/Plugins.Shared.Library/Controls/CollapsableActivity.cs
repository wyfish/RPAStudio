using System.Windows;
using System.Windows.Controls;

namespace Plugins.Shared.Library.Controls
{
    public class CollapsableActivity : ContentControl
    {
        static CollapsableActivity()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CollapsableActivity), new FrameworkPropertyMetadata(typeof(CollapsableActivity)));
        }
    }
}
