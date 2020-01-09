using System.ComponentModel;
using System.Resources;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace RPA.Integration.Activities
{
    internal class Localize
    {
        internal class LocalizedCategoryAttribute : CategoryAttribute
        {
            public LocalizedCategoryAttribute(string resourceKey)
                : base(LocalizedResources.GetString(resourceKey)) { }
        }

        internal class LocalizedDisplayNameAttribute : DisplayNameAttribute
        {
            public LocalizedDisplayNameAttribute(string resourceKey)
                : base(LocalizedResources.GetString(resourceKey)) { }
        }

        internal class LocalizedDescriptionAttribute : DescriptionAttribute
        {
            public LocalizedDescriptionAttribute(string resourceKey)
                : base(LocalizedResources.GetString(resourceKey)) { }
        }

        internal class LocalizedCategoryOrderAttribute : CategoryOrderAttribute
        {
            public LocalizedCategoryOrderAttribute(string resourceKey, int order)
                : base(LocalizedResources.GetString(resourceKey), order) { }
        }

        internal static class LocalizedResources
        {
            readonly static ResourceManager _ResourceManager = new ResourceManager(typeof(Properties.Resources));

            public static string GetString(string resourceKey)
            {
                return _ResourceManager.GetString(resourceKey) ?? resourceKey;
            }
        }

    }
}
