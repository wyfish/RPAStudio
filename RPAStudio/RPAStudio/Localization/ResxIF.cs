using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPAStudio.Localization
{
    public static class ResxIF
    {
        public static string GetString(string key)
        {
            string isoCulture = System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            // Only 'zh' and 'ja' localizations are available. Others will be shown in English.
            if (isoCulture.StartsWith("zh") || isoCulture.Equals("ja")) {
                return Strings.ResourceManager.GetString(key, System.Globalization.CultureInfo.CurrentCulture);
            }
            var en = new System.Globalization.CultureInfo("en");
            return Strings.ResourceManager.GetString(key, en);
        }

    }
}
