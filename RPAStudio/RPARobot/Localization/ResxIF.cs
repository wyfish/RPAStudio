using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPARobot.Localization
{
    public static class ResxIF
    {
        public static string GetString(string key)
        {
            return Strings.ResourceManager.GetString(key);
        }

    }
}
