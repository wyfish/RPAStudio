using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;


namespace Plugins.Shared.Library
{
    public class Win32Api
    {
        public const uint WM_CLOSE = 0x10;
        public const uint WM_DESTROY = 0x02;
        public const uint WM_QUIT = 0x12;
        public const uint SC_CLOSE = 0xF060;
        public const uint WM_SYSCOMMAND = 0x0112;
        public const int SC_MAXIMIZE = 0xF030;
        public const int SC_MINIMIZE = 0xF020;
        public const uint TH32CS_SNAPHEAPLIST = 0x00000001;
        public const uint TH32CS_SNAPPROCESS = 0x00000002;
        public const uint TH32CS_SNAPTHREAD = 0x00000004;
        public const uint TH32CS_SNAPMODULE = 0x00000008;
        public const int PROCESS_ALL_ACCESS = 0x1F0FFF;
        public const int PROCESS_VM_READ = 0x0010;
        public const int PROCESS_VM_WRITE = 0x0020;
        public const int PROCESS_QUERY_INFORMATION = 0x0400;
        public const int SW_RESTORE = 9;
        public const int SW_HIDE = 0;
        public const int SW_SHOWNORMAL = 1;


        [DllImport("user32", EntryPoint = "GetWindowThreadProcessId")]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out uint pid);

        [DllImport("User32.dll", EntryPoint = "PostMessage", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool IsWindowVisible(HandleRef hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetWindow(HandleRef hWnd, int uCmd);

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "MoveWindow")]
        public static extern bool MoveWindow(int hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("Kernel32.dll",EntryPoint = "QueryFullProcessImageNameW",CallingConvention = CallingConvention.StdCall,CharSet = CharSet.Unicode)]
        public static extern int QueryFullProcessImageName(IntPtr hProcess,UInt32 flags, char[] exeName,ref UInt32 nameLen);

        [DllImport("user32.dll", EntryPoint = "ShowWindow")]
        public static extern IntPtr ShowWindow(int hWnd, int _value);
        
        public static string GetProcessPath(int processID)
        {
            char[] buf = new char[65535];
            UInt32 len = 65535;
            int calcProcess;
            calcProcess = OpenProcess(Win32Api.PROCESS_QUERY_INFORMATION, false, processID);
            QueryFullProcessImageName((IntPtr)calcProcess, 0, buf, ref len);
            string exeName = new string(buf, 0, (int)len);
            return exeName;
        }

        [DllImport("kernel32.dll")]
        public static extern int OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        public bool IsWindowExist(IntPtr handle)
        {
            return (!(GetWindow(new HandleRef(this, handle), 4) != IntPtr.Zero) && IsWindowVisible(new HandleRef(this, handle)));
        }

        public const int GwlExstyle = -20;
        public const int SwShowna = 8;
        public const int WsExToolwindow = 0x00000080;
        public const int SwpNoactivate = 0x0010;
        public static readonly IntPtr HwndTopmost = new IntPtr(-1);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(
            IntPtr hWnd, IntPtr hwndAfter, int x, int y,
            int width, int height, int flags);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex,
            int dwNewLong);

        [DllImport("user32.dll")]
        public static extern bool SetProcessDPIAware();


        public static bool CloseAllProcesses(string processName)
        {
            try
            {
                Process[] psEaiNotes = Process.GetProcessesByName(processName);
                foreach (Process psEaiNote in psEaiNotes)
                {
                    psEaiNote.Kill();
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        [DllImport("user32")]
        public static extern IntPtr LoadCursorFromFile(string fileName);

        [DllImport("User32.DLL")]
        public static extern bool SetSystemCursor(IntPtr hcur, uint id);
        public const uint OCR_NORMAL = 32512;
        public const uint OCR_IBEAM = 32513;

        [DllImport("User32.DLL")]
        public static extern bool SystemParametersInfo(uint uiAction, uint uiParam, uint pvParam, uint fWinIni);

        public const uint SPI_SETCURSORS = 87;
        public const uint SPIF_SENDWININICHANGE = 2;

        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr ChildWindowFromPointEx(IntPtr pHwnd, LPPOINT pt, uint uFlgs);
        public const int CWP_SKIPDISABLED = 0x2;   
        public const int CWP_SKIPINVISIBL = 0x1;   
        public const int CWP_All = 0x0;                    

        [DllImport("user32.dll")]
        public static extern bool ScreenToClient(IntPtr hWnd, out LPPOINT lpPoint);

        public struct LPPOINT
        {
            public int X;
            public int Y;
        }
        public struct POINT
        {
            public int x;
            public int y;
        }

        public struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(POINT Point);

        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(int xPoint, int yPoint);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);

        [DllImport("user32", EntryPoint = "CreateIconFromResourceEx")]
        public static extern IntPtr CreateIconFromResourceEx(byte[] pbIconBits, uint cbIconBits, bool fIcon, uint dwVersion, int cxDesired, int cyDesired, uint uFlags);

        [DllImport("user32.dll")]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpString, int nMaxCont);

        [DllImport("user32.dll")]
        public static extern int GetWindowRect(IntPtr hwnd, out Rect lpRect);

        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public extern static IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(int hWnd);
    }
}
