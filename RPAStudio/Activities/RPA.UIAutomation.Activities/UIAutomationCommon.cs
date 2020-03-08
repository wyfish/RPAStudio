using System;
using System.Activities;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Runtime.InteropServices;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Tools;
using Plugins.Shared.Library;
using Plugins.Shared.Library.UiAutomation;

namespace RPA.UIAutomation.Activities
{
    internal static class UIAutomationCommon
    {
        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpStr, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        internal static AutomationElement GetNativeElement(CodeActivityContext context,
                                                           InArgument<string> arg_WindowTitle,
                                                           InArgument<string> arg_AutomationId,
                                                           InArgument<string> arg_Name)
        {
            string title = arg_WindowTitle.Get(context) ?? "";
            string automationId = arg_AutomationId.Get(context) ?? "";
            string name = arg_Name.Get(context) ?? "";
            if (title.Length > 0 && (automationId.Length > 0 || name.Length > 0))
            {
                using (var automation = new FlaUI.UIA3.UIA3Automation())
                {
                    AutomationElement parentWindowAE = null;
                    uint lpdwProcessId;
                    var hWnd = GetWindowHandleFromTitle(title);
                    FlaUI.Core.WindowsAPI.User32.GetWindowThreadProcessId(hWnd, out lpdwProcessId);
                    var app = FlaUI.Core.Application.Attach((int)lpdwProcessId);
                    parentWindowAE = app.GetMainWindow(automation);

                    // 1.Use AutomationId property to get the element.
                    if (automationId.Length > 0)
                    {
                        return Retry.WhileNull(() =>
                            parentWindowAE.FindFirstDescendant(cf => cf.ByAutomationId(automationId)), throwOnTimeout: false, ignoreException: true).Result;
                    }
                    // 2.Use Name property to get the element.
                    if (name.Length > 0)
                    {
                        return Retry.WhileNull(() =>
                            parentWindowAE.FindFirstDescendant(cf => cf.ByName(name)), throwOnTimeout: false, ignoreException: true).Result;
                    }
                }
            }
            return null;
        }

        internal static string GetRootWindowTitle(IntPtr hwnd)
        {
            return GetWindowTextHelper(hwnd);
        }

        private static string GetWindowTextHelper(IntPtr hWnd)
        {
            string title = "";
            StringBuilder sb = new StringBuilder(0x1024);
            if (IsWindowVisible(hWnd) && GetWindowText(hWnd, sb, sb.Capacity) != 0)
            {
                title = sb.ToString();
            }
            return title;
        }

        private static IntPtr GetWindowHandleFromTitle(string wName)
        {
            IntPtr hwnd = IntPtr.Zero;
            foreach (Process pList in Process.GetProcesses())
            {
                if (IsWindowVisible(pList.MainWindowHandle))
                {
                    if (wName.StartsWith("*") && wName.EndsWith("*"))
                    {
                        if (pList.MainWindowTitle.Contains(wName))
                        {
                            return pList.MainWindowHandle;
                        }
                    }
                    else if (wName.StartsWith("*"))
                    {
                        if (pList.MainWindowTitle.EndsWith(wName))
                        {
                            return pList.MainWindowHandle;
                        }
                    }
                    else if (wName.EndsWith("*"))
                    {
                        if (pList.MainWindowTitle.StartsWith(wName))
                        {
                            return pList.MainWindowHandle;
                        }
                    }
                    else if (pList.MainWindowTitle.Equals(wName))
                    {
                        return pList.MainWindowHandle;
                    }
                }
            }
            return hwnd;
        }

        internal static Point GetPoint(CodeActivityContext context, bool usePoint, InArgument<int> x_Coordinate, InArgument<int> y_Coordinate, UiElement element, bool setForeground = true)
        {
            Int32 pointX = -1;
            Int32 pointY = -1;
            if (usePoint)
            {
                pointX = x_Coordinate.Get(context);
                pointY = y_Coordinate.Get(context);
            }
            else
            {
                if (element != null)
                {
                    if (setForeground)
                    {
                        element.SetForeground();
                    }
                    return element.GetClickablePoint();
                }
            }
            return new Point(pointX, pointY);
        }

        internal static void HandleContinueOnError(CodeActivityContext context, InArgument<bool> continueOnError, string errMsg)
        {
            SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", errMsg);
            if (continueOnError.Get(context))
            {
                return;
            }
            throw new NotImplementedException(errMsg);
        }

    }
}
