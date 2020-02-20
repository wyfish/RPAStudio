using System;
using System.IO;
using System.Runtime.InteropServices;
using Plugins.Shared.Library;

namespace RPA.OpenCV.Activities.Mouse
{
    internal static class Common
    {
        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        private static int _width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
        private static int _height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
        private static FlaxCV _cv;

        static Common()
        {
            using (FileStream fs = new FileStream(FlaxCV._FlaxCV_exe, FileMode.Create))
            {
                fs.Write(Properties.Resources.Flax_CV, 0, Properties.Resources.Flax_CV.Length);
            }
            _cv = new FlaxCV();
        }

        internal static FlaxCV.CvResult CVMouseAction(string templateImagePath, FlaxCV.CvActionType cvActionType, string windowTitle, int matchingThreshold, int matchingInterval, int retry)
        {
            int x = 0, y = 0;

            if (windowTitle != null && windowTitle.Length > 0)
            {
                IntPtr hWnd = IntPtr.Zero;
                hWnd = Win32Api.FindWindow(null, windowTitle);
                RECT rect;
                bool flag = GetWindowRect(hWnd, out rect);
                x = rect.left;
                y = rect.top;
                _width = rect.right - rect.left;
                _height = rect.bottom - rect.top;
            }
            var cvRet = _cv.DoCVAction(cvActionType, templateImagePath, matchingThreshold, matchingInterval, retry, new System.Drawing.Rectangle(x, y, _width, _height));
            string resultIfo = string.Format(Properties.Resources.ResultInformation, x, y, _width, _height, cvRet.IsMatched, cvRet.MatchedLevel);
            SharedObject.Instance.Output(SharedObject.enOutputType.Information, Properties.Resources.ImageMatchingResult, resultIfo);
            return cvRet;
        }


    }
}
