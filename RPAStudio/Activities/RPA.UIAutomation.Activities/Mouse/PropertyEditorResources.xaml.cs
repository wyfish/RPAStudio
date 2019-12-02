//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Windows;

namespace RPA.UIAutomation.Activities.Mouse
{

    public partial class PropertyEditorResources
    {
        static ResourceDictionary resources;

        internal static ResourceDictionary GetResources()
        {
            if (resources == null)
            {
                Uri resourceLocator = new Uri(string.Concat(typeof(PropertyEditorResources).Assembly.GetName().Name, @";component/PropertyEditorResources.xaml"),
                    UriKind.RelativeOrAbsolute);
                resources = (ResourceDictionary)Application.LoadComponent(resourceLocator);
            }
            return resources;
        }
    }
}
