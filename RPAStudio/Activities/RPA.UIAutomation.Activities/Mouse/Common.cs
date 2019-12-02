using Plugins.Shared.Library.UiAutomation;
using System;
using System.Activities;

namespace RPA.UIAutomation.Activities.Mouse
{
    public class Common 
    {
        public static T GetValueOrDefault<T>(ActivityContext context, InArgument<T> source, T defaultValue)
        {
            T result = defaultValue;
            if (source != null && source.Expression != null)
            {
                result = source.Get(context);
            }
            return result;
        }

        public static bool DealVirtualKeyPress(string keyType)
        {
            bool bResult = false;
            foreach (VirtualKey item in Enum.GetValues(typeof(VirtualKey)))
            {
                if (keyType==item.ToString())
                {
                    UiElement.KeyboardPress(item);
                    bResult = true;
                    break;
                }
            }
            return bResult;
        }

        public static void DealVirtualKeyRelease(string keyType)
        {
            foreach (VirtualKey item in Enum.GetValues(typeof(VirtualKey)))
            {
                if (keyType == item.ToString())
                {
                    UiElement.KeyboardRelease(item);
                    break;
                }
            }
        }

        public static void DealKeyBordPress(string keyType)
        {
            switch (keyType)
            {
                case "Alt":
                    UiElement.KeyboardPress(VirtualKey.ALT);
                    break;
                case "Ctrl":
                    UiElement.KeyboardPress(VirtualKey.CONTROL);
                    break;
                case "Shift":
                    UiElement.KeyboardPress(VirtualKey.SHIFT);
                    break;
                case "Win":
                    UiElement.KeyboardPress(VirtualKey.LWIN);
                    break;
                default:
                    break;
            }
        }

        public static void DealKeyBordRelease(string keyType)
        {
            switch (keyType)
            {
                case "Alt":
                    UiElement.KeyboardRelease(VirtualKey.ALT);
                    break;
                case "Ctrl":
                    UiElement.KeyboardRelease(VirtualKey.CONTROL);
                    break;
                case "Shift":
                    UiElement.KeyboardRelease(VirtualKey.SHIFT);
                    break;
                case "Win":
                    UiElement.KeyboardRelease(VirtualKey.LWIN);
                    break;
                default:
                    break;
            }
        }
    }
}
