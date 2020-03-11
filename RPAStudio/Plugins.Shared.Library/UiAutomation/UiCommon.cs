using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Plugins.Shared.Library.UiAutomation
{
    public class UiCommon
    {
        public const int PROCESS_QUERY_LIMITED_INFORMATION = 0x1000;

        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_LAYERED = 0x00080000;
        public const int WS_EX_TRANSPARENT = 0x00000020;

        public static class ShowWindowTypes
        {
            public const int SW_HIDE = 0;
            public const int SW_SHOWNORMAL = 1;
            public const int SW_NORMAL = 1;
            public const int SW_SHOWMINIMIZED = 2;
            public const int SW_SHOWMAXIMIZED = 3;
            public const int SW_MAXIMIZE = 3;
            public const int SW_SHOWNOACTIVATE = 4;
            public const int SW_SHOW = 5;
            public const int SW_MINIMIZE = 6;
            public const int SW_SHOWMINNOACTIVE = 7;
            public const int SW_SHOWNA = 8;
            public const int SW_RESTORE = 9;
            public const int SW_SHOWDEFAULT = 10;
            public const int SW_FORCEMINIMIZE = 11;
            public const int SW_MAX = 11;
        }



        public static WINDOWPLACEMENT GetPlacement(IntPtr hwnd)
        {
            WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
            placement.length = Marshal.SizeOf(placement);
            GetWindowPlacement(hwnd, ref placement);
            return placement;
        }


        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowPlacement(
            IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public ShowWindowCommands showCmd;
            public Point ptMinPosition;
            public Point ptMaxPosition;
            public Rectangle rcNormalPosition;
        }

        public enum ShowWindowCommands : int
        {
            Hide = 0,
            Normal = 1,
            Minimized = 2,
            Maximized = 3,
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(uint processAccess, bool bInheritHandle, int processId);

        [DllImport("psapi.dll")]
        static extern uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, [Out] StringBuilder lpBaseName, [In] [MarshalAs(UnmanagedType.U4)] int nSize);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseHandle(IntPtr hObject);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.SysInt)]
        public static extern IntPtr WindowFromPoint(System.Drawing.Point Point);

        [DllImport("User32.dll")]
        public static extern IntPtr GetParent(IntPtr hWnd);


        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);


        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int vKey);


        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = true)]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        static extern bool InvalidateRect(IntPtr hWnd, IntPtr lpRect, bool bErase);

        [DllImport("User32.dll", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern IntPtr LoadCursorFromFile(String str);


        /// <summary>
        /// 从内嵌的资源中加载光标（通过释放临时文件进行加载）
        /// </summary>
        /// <param name="cursorResource"></param>
        /// <returns></returns>
        public static Cursor GetCursor(byte[] cursorResource)
        {
            try
            {
                var tmpPath = System.IO.Path.GetTempPath();
                var guid = Guid.NewGuid().ToString("N");

                if (tmpPath.Substring(tmpPath.Length - 1, 1) != @"\")
                {
                    tmpPath = tmpPath + @"\";
                }

                var tempFile = tmpPath + guid + @".cur";

                File.WriteAllBytes(tempFile, cursorResource);

                var cursor = new Cursor(LoadCursorFromFile(tempFile));
                File.Delete(tempFile);
                return cursor;
            }
            catch (Exception)
            {

            }

            return Cursors.Default;

        }

        private static bool IsNeedRestoreWindow(IntPtr hWnd)
        {
            var state = GetPlacement(hWnd).showCmd;
            if (state == ShowWindowCommands.Hide || state == ShowWindowCommands.Minimized)
            {
                return true;
            }

            return false;
        }

        public static void ForceShow(IntPtr hWnd)
        {
            if (IsNeedRestoreWindow(hWnd))
            {
                //已经是最大化时调用SW_RESTORE会导致窗口还原，所以此处需要判断下
                ShowWindow(hWnd, ShowWindowTypes.SW_RESTORE);
                if(JavaUiNode.accessBridge.Functions.IsJavaWindow(hWnd))
                {
                    InvalidateRect(hWnd, IntPtr.Zero, true);//java窗口需要重刷下，不然黑屏
                }
                
            }
        }

        public static string GetProcessName(int pid)
        {
            return System.IO.Path.GetFileName(GetProcessFullPath(pid));
        }

        public static string GetProcessFullPath(int pid)
        {
            var processHandle = OpenProcess(PROCESS_QUERY_LIMITED_INFORMATION, false, pid);

            if (processHandle == IntPtr.Zero)
            {
                return null;
            }

            const int lengthSb = 4000;

            var sb = new StringBuilder(lengthSb);

            string result = "";

            if (GetModuleFileNameEx(processHandle, IntPtr.Zero, sb, lengthSb) > 0)
            {
                result = sb.ToString();
            }

            CloseHandle(processHandle);

            return result;
        }



        public static void EnablePassThrough(Form form)
        {
            int style = GetWindowLong(
                form.Handle, GWL_EXSTYLE);
            SetWindowLong(
             form.Handle, GWL_EXSTYLE,
                style | WS_EX_TRANSPARENT);
        }

        public static void DisablePassThrough(Form form)
        {
            int style = GetWindowLong(
                form.Handle, GWL_EXSTYLE);
            SetWindowLong(
            form.Handle, GWL_EXSTYLE,
               style & ~WS_EX_TRANSPARENT);
        }

        /// <summary>
        /// 获取当前鼠标点对应的根窗口
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static IntPtr GetRootWindow(Point point)
        {
            IntPtr hWnd = WindowFromPoint(point);

            IntPtr hParentWnd = GetParent(hWnd);

            while (hParentWnd != IntPtr.Zero)
            {
                hWnd = hParentWnd;
                hParentWnd = GetParent(hParentWnd);
            }

            return hWnd;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);

        public enum BinaryType : uint
        {
            SCS_32BIT_BINARY = 0, // A 32-bit Windows-based application
            SCS_64BIT_BINARY = 6, // A 64-bit Windows-based application.
            SCS_DOS_BINARY = 1,   // An MS-DOS – based application
            SCS_OS216_BINARY = 5, // A 16-bit OS/2-based application
            SCS_PIF_BINARY = 3,   // A PIF file that executes an MS-DOS – based application
            SCS_POSIX_BINARY = 4, // A POSIX – based application
            SCS_WOW_BINARY = 2    // A 16-bit Windows-based application 
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetBinaryType(
            string lpApplicationName,
            out BinaryType dwBinType
            );


        /// <summary>
        /// 判断EXE是否为64位程序
        /// </summary>
        /// <param name="exePath"></param>
        /// <returns></returns>
        public static bool IsExe64Bit(string exePath)
        {
            BinaryType exeType = BinaryType.SCS_32BIT_BINARY;
            if (GetBinaryType(exePath, out exeType))
            {
                if(exeType == BinaryType.SCS_64BIT_BINARY)
                {
                    return true;
                }
            }

            return false;
        }

        public static Process RunProcess(string fileName, string arguments,bool isHidden = false)
        {
            ProcessStartInfo psi = new ProcessStartInfo();

            if (isHidden)
            {
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                psi.CreateNoWindow = true;
            }

            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.UseShellExecute = false;
            psi.FileName = fileName;
            psi.Arguments = arguments;

            var p = Process.Start(psi);

            return p;
        }


        public static void CopyFileToDir(string srcPath,string dstDir,bool isOverwrite = false)
        {
            try
            {
                string dstPath = "";
                if(dstDir.Substring(dstDir.Length - 1, 1)== @"\")
                {
                    dstPath = dstDir + System.IO.Path.GetFileName(srcPath);
                }
                else
                {
                    dstPath = dstDir + @"\" + System.IO.Path.GetFileName(srcPath);
                }

                System.IO.File.Copy(srcPath, dstDir, isOverwrite);
            }
            catch (Exception)
            {

            }
        }
    }
}
