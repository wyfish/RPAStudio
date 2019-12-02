using System;
using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace RPA.UIAutomation.Activities.Window
{
    public class Window
    {
        private static int WindowHind;
        private static string WindowText;
        private static string WindowClass;

        public Window()
        {
            WindowHind = 0;
            WindowText = "";
            WindowClass = "";
        }

        public void setWindowHwnd(int hwnd)
        {
            WindowHind = hwnd;
        }
        public void setWindowText(string windowText)
        {
            WindowText = windowText;
        }
        public void setWindowClass(string windowClass)
        {
            WindowClass = windowClass;
        }
        public int getWindowHwnd()
        {
            return WindowHind;
        }
        public string getWindowText()
        {
            return WindowText;
        }
        public string getWindowClass()
        {
            return WindowClass;
        }
    }

    public class CommonVariable
    {
        public static string getPropertyValue(string propertyName, ModelItem currItem)
        {
            List<ModelProperty> PropertyList = currItem.Properties.ToList();
            ModelProperty _property = PropertyList.Find((ModelProperty property) => property.Name.Equals(propertyName));
            return _property.Value.ToString();
        }
    }
}
