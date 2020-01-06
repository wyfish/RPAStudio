using System.ComponentModel;
using System.Resources;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace RPA.UIAutomation.Activities
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

        internal static class LocalText
        {
            private static bool _iszh = System.Globalization.CultureInfo.CurrentCulture.Name.StartsWith("zh");
            private static bool _isja = System.Globalization.CultureInfo.CurrentCulture.Name.StartsWith("jp");

            internal static string ArgumentError {
                get {
                    if (_iszh) return "FileName和Arguments都为空，至少需要一个不为空。";
                    if (_isja) return "FileNameとArgumentsは両方とも空です。少なくとも1つは必要ありません。";
                    return "FileName and Arguments are both empty, at least one is not required.";
                }
            }
        }
    }
}
