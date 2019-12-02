using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.Script.Activities.Python
{
    /// <summary>
    ///  重定向Python脚本中的print到控制台输出
    /// </summary>
    public class PythonPrintRedirectObject
    {
        public static PythonPrintRedirectObject Instance = null;

        static PythonPrintRedirectObject()
        {
            Instance = new PythonPrintRedirectObject();
        }

        public void write(string str)
        {
            if (str != "\n")
            {
                Console.WriteLine(str);
            }
        }
    }
}
