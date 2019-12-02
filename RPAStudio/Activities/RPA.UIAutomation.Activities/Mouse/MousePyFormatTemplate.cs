using System;

namespace RPA.UIAutomation.Activities.Mouse
{
    public class MousePyFormatTemplate
    {
        public string setMousePoint(Int32 pointX, Int32 pointY)
        {
            return $"PyMousePlugins.PyMousePlugins().setMousePoint({pointX},{pointY})";
        }

        public string MouseAction(Int32 clickType,Int32 buttonType)
        {
            return $"PyMousePlugins.PyMousePlugins().MouseAction({clickType},{buttonType})";
        }

        public string DelayTime(Int32 millisecond)
        {
            return $"PyMousePlugins.PyMousePlugins().DelayTime({millisecond})";
        }
    }
}
