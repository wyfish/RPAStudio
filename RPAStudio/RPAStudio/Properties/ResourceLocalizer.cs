﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Globalization;

namespace RPAStudio.Properties
{
    static class ResourceLocalizer
    {
        internal static byte[] GetConfigXML(string resourceName)
        {
            //string isoCulture = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            string cultureName = CultureInfo.CurrentCulture.Name;
            if (!cultureName.Equals("zh-CN"))
            {
                resourceName += "_" + cultureName.Replace("-", "_");
            }

            return GetResourceByName(resourceName);
        }

        internal static byte[] GetResourceByName(string resourceName)
        {
            // https://stackoverflow.com/questions/451453/how-to-get-a-static-property-with-reflection
            PropertyInfo propertyInfo;
            propertyInfo = typeof(Resources).GetProperty(resourceName, BindingFlags.NonPublic | BindingFlags.Static);
            object value = propertyInfo.GetValue(null, null);
            return (byte[])value;
        }

    }
}
