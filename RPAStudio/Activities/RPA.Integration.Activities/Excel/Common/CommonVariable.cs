using System;
using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;


namespace RPA.Integration.Activities.ExcelPlugins
{
    public enum ColorIndexEnum
    {
        无设置 = 0,
        无色 = -4142,
        自动 = -4105,
        黑色 = 1,
        白色 = 2,
        红色 = 3,
        鲜绿 = 4,
        蓝色 = 5,
        黄色 = 6,
        粉红 = 7,
        青绿 = 8,
        深红 = 9,
        绿色 = 10,
        深蓝 = 11,
        深黄 = 12,
        紫罗兰 = 13,
        青色 = 14,
        灰色25 = 15,
        褐色 = 53,
        橄榄 = 52,
        深绿 = 51,
        深青 = 49,
        靛蓝 = 55,
        灰色80 = 56,
        橙色 = 46,
        蓝灰 = 47,
        灰色50 = 16,
        浅橙色 = 45,
        酸橙色 = 43,
        海绿 = 50,
        水绿色 = 42,
        浅蓝 = 41,
        灰色40 = 48,
        金色 = 44,
        天蓝 = 33,
        梅红 = 54,
        玫瑰红 = 38,
        茶色 = 40,
        浅黄 = 36,
        浅绿 = 35,
        浅青绿 = 34,
        淡蓝 = 37,
        淡紫 = 39
    }

    public enum ExcelFontEnum
    {
        无设置,
        等线,
        等线Light,
        黑体,
        楷体,
        宋体,
        新宋体,
        仿宋,
        隶书,
        幼圆,
        微软雅黑,
        方正等线,
        方正舒体,
        方正姚体,
        华文彩云,
        华文仿宋,
        华文行楷,
        华文琥珀,
        华文楷体,
        华文隶书,
        华文宋体,
        华文细黑,
        华文新魏,
        华文中宋,
        Arial,
        ArialBlack,
        ArialNarrow,
        ArialRoundedMTBold,
        ArialUnicodeMS,
        Calibri,
        CalibriLight,
        MicrosoftYaHeiUI,
        MicrosoftYaHeiUILight,
        MicrosoftJhengHei,
        MicrosoftJhengHeiLight,
        MicrosoftJhengHeiUI,
        MicrosoftJhengHeiUILight,
        MicrosoftMHei,
        MicrosoftNeoGothic,
        MalgunGothic,
        AgencyFB,
        Algerian,
        Bauhaus93,
        BellMT,
        BerlinSansFB,
        BerlinSansFBDemi,
        BernardMTCondensed,
        BlackadderITC,
        BodoniMT,
        BodoniMTBlack,
        YuGothic,
        Bahnschrift,
        Cambria,
        Century
    }

    public enum BorderType
    {
        全部 = 0,
        上 = 3,
        下 = 4,
        左 = 1,
        右 = 2
    }

    public enum BorderStyle
    {
        实线 = 1,
        虚线 = -4115,
        细虚线 = 2,
        点划线 = 4,
        双点划线 = 5,
        点式线 = -4118,
        双线 = -4119,
        双细实线 = 9,
        无线条 = -4142,
        斜划线 = 13
    }

    public enum AlignEnum
    {
        无设置 = 0,
        居中 = -4108,
        分散对齐 = -4117,
        两端对齐 = -4130,
        左对齐 = -4131,
        右对齐 = -4152
    }

    public class CommonVariable
    {
        public static string getPropertyValue(string propertyName, ModelItem currItem)
        {
            List<ModelProperty> PropertyList = currItem.Properties.ToList();
            ModelProperty _property = PropertyList.Find((ModelProperty property) => property.Name.Equals(propertyName));
            return _property.Value.ToString();
        }
        public void realaseProcessExit(Excel::Application app)
        {
            //if (CommonVariable.range != null)
            //{
            //    Marshal.ReleaseComObject(CommonVariable.range);
            //    CommonVariable.range = null;
            //}
            //if (CommonVariable._wsh != null)
            //{
            //    Marshal.ReleaseComObject(CommonVariable._wsh);
            //    CommonVariable._wsh = null;
            //}
            //if (CommonVariable.shs != null)
            //{
            //    Marshal.ReleaseComObject(CommonVariable.shs);
            //    CommonVariable.shs = null;
            //}
            //if (CommonVariable._wbk != null)
            //{
            //    Marshal.ReleaseComObject(CommonVariable._wbk);
            //    CommonVariable._wbk = null;
            //}
            //if (CommonVariable.wbks != null)
            //{
            //    Marshal.ReleaseComObject(CommonVariable.wbks);
            //    CommonVariable.wbks = null;
            //}
            if (app != null)
            {
                app.Quit();
                Marshal.ReleaseComObject(app);
                app = null;
            }
            GC.Collect();
        }
        public void realaseProcess(Excel::Application app)
        {
            //if (CommonVariable.range != null)
            //{
            //    Marshal.ReleaseComObject(CommonVariable.range);
            //    CommonVariable.range = null;
            //}
            //if (CommonVariable._wsh != null)
            //{
            //    Marshal.ReleaseComObject(CommonVariable._wsh);
            //    CommonVariable._wsh = null;
            //}
            //if (CommonVariable.shs != null)
            //{
            //    Marshal.ReleaseComObject(CommonVariable.shs);
            //    CommonVariable.shs = null;
            //}
            //if (CommonVariable._wbk != null)
            //{
            //    Marshal.ReleaseComObject(CommonVariable._wbk);
            //    CommonVariable._wbk = null;
            //}
            //if (CommonVariable.wbks != null)
            //{
            //    Marshal.ReleaseComObject(CommonVariable.wbks);
            //    CommonVariable.wbks = null;
            //}
            if (app != null)
            {
                Marshal.ReleaseComObject(app);
                app = null;
            }
            GC.Collect();
        }
    }


    /*杀死进程*/
    public class KillExcelMethod
    {
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);
        public static void Kill(Microsoft.Office.Interop.Excel.Application excel)
        {
            IntPtr t = new IntPtr(excel.Hwnd);//得到这个句柄，具体作用是得到这块内存入口 

            int k = 0;
            GetWindowThreadProcessId(t, out k);   //得到本进程唯一标志k
            System.Diagnostics.Process p = System.Diagnostics.Process.GetProcessById(k);   //得到对进程k的引用
            p.Kill();     //关闭进程k
        }
    }
}
