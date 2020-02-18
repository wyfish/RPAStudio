using System;
using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using OpenQA.Selenium;
using System.Diagnostics;
using System.Collections;


namespace RPA.UIAutomation.Activities.Browser
{
    public enum BrowserTypes
    {
        IE,
        Chrome,
        Firefox
    }

    public class GetIEFromHWndClass
    {
        [DllImport("user32.dll", EntryPoint = "SendMessageTimeoutA")]
        public static extern int SendMessageTimeout(int hwnd, int msg, int wParam, int lParam, int fuFlags, int uTimeout, out int lpdwResult);
        [DllImport("user32.dll", EntryPoint = "RegisterWindowMessageA")]
        public static extern int RegisterWindowMessage(string lpString);
        [DllImport("oleacc.dll", PreserveSig = false)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public static extern object ObjectFromLresult(UIntPtr lResult, [MarshalAs(UnmanagedType.LPStruct)]Guid refiid, IntPtr wParam);
        [DllImport("oleacc.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern Int32 ObjectFromLresult(Int32 lResult, ref Guid riid, Int32 wParam, out mshtml.IHTMLDocument2 ppvObject);
        [DllImport("oleacc.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern bool ObjectFromLresult(Int32 lResult, ref Guid riid, Int32 wParam, [Out, MarshalAs(UnmanagedType.IUnknown)]out object ppvObject);

        [ComImport, Guid("6d5140c1-7436-11ce-8034-00aa006009fa"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IServiceProvider
        {
            void QueryService(ref Guid guidService,
                ref Guid riid,
                [MarshalAs(UnmanagedType.Interface)] out object ppvObject);
        }
        public const int SMTO_ABORTIFHUNG = 0x2;
        public static Guid IID_IHTMLDocument = new Guid("626FC520-A41E-11CF-A731-00A0C9082637");
        public Guid IID_IServiceProvider = new Guid("6d5140c1-7436-11ce-8034-00aa006009fa");

        public static SHDocVw.InternetExplorer GetIEFromHWnd(int hIEWindow, out mshtml.IHTMLDocument2 currDoc)
        {
            Guid IID_IWebBrowser2 = typeof(SHDocVw.InternetExplorer).GUID;
            Guid IID_IServiceProvider = typeof(Microsoft.VisualStudio.OLE.Interop.IServiceProvider).GUID;
            Guid IID_IHTMLDocument2 = typeof(mshtml.IHTMLDocument2).GUID;
            Guid IID_IHTMLWindow2 = typeof(mshtml.IHTMLWindow2).GUID;
            Guid SID_SWebBrowserApp = typeof(SHDocVw.IWebBrowserApp).GUID;

            int hWnd = hIEWindow;
            int lRes = 0;
            mshtml.IHTMLDocument2 spDoc = null;
            int lngMsg = RegisterWindowMessage("WM_HTML_GETOBJECT");

            SendMessageTimeout(hWnd, lngMsg, 0, 0, SMTO_ABORTIFHUNG, 1000, out lRes);
            int flag = ObjectFromLresult((int)lRes, ref IID_IHTMLDocument2, 0, out spDoc);
            if (spDoc == null)
            {
                currDoc = null;
                return null;
            }
            IntPtr puk = Marshal.GetIUnknownForObject(spDoc);
            object objIWebBrowser2 = new object();
            IServiceProvider pro = spDoc as IServiceProvider;
            pro.QueryService(ref SID_SWebBrowserApp, ref IID_IWebBrowser2, out objIWebBrowser2);
            SHDocVw.InternetExplorer web = objIWebBrowser2 as SHDocVw.InternetExplorer;

            Debug.WriteLine("spDoc.title : " + spDoc.title);
            Debug.WriteLine("点击选取句柄为 : " + hWnd);
            Debug.WriteLine("查询到的服务接口句柄为 : " + web.HWND);
            currDoc = spDoc;
            return web;
        }


        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool ScreenToClient(int hWnd, ref System.Drawing.Point lpPoint);

        public static mshtml.IHTMLElement GetEleFromDoc(System.Drawing.Point windowPos, int hwnd, mshtml.IHTMLDocument2 document)
        {
            int hWnd = hwnd;
            ScreenToClient(hWnd, ref windowPos);
            dynamic element = document.elementFromPoint(windowPos.X, windowPos.Y);
            mshtml.IHTMLElement ele = (mshtml.IHTMLElement)element;
            return ele;
        }


        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.SysInt)]
        public static extern IntPtr WindowFromPoint(System.Drawing.Point Point);

        public static void GetIEFromPos(System.Drawing.Point Point)
        {
            IntPtr hWnd = WindowFromPoint(Point);
        }
    }

    public class Browser
    {
        SHDocVw.InternetExplorer IEBrowser = null;
        IWebDriver ICFBrowser = null;

        public Browser()
        {
        }

        public void SetIEBrowser(SHDocVw.InternetExplorer ie)
        {
            this.IEBrowser = ie;
        }
        public void SetICFBrowser(IWebDriver icf)
        {
            this.ICFBrowser = icf;
        }

        public SHDocVw.InternetExplorer getIEBrowser()
        {
            return this.IEBrowser;
        }

        public IWebDriver getICFBrowser()
        {
            return this.ICFBrowser;
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
        public static ArrayList BrowsersList;
    }
}
