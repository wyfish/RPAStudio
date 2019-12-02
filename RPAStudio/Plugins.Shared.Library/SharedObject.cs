using System;
using System.Windows;

namespace Plugins.Shared.Library
{
    public class SharedObject
    {
        public enum enOutputType
        {
            Trace,
            Information,
            Warning,
            Error,
        }

        public delegate void OutputDelegate(enOutputType type, string msg, string msgDetails = "");


        /// <summary>
        /// 输出日志到主程序输出窗口
        /// </summary>
        private OutputDelegate OutputFun { get; set; }

        public void SetOutputFun(OutputDelegate logToOutputWindow)
        {
            OutputFun = logToOutputWindow;
        }

        public void Output(enOutputType type, object msg, object msgDetails = null)
        {
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                var msgStr = msg == null? "": msg.ToString();
                var msgDetailsStr = msgDetails == null ? msgStr : msgDetails.ToString();
                OutputFun(type, msgStr, msgDetailsStr);
            });
        }

        /// <summary>
        /// 当前项目所在路径
        /// </summary>
        public string ProjectPath { get; set; }


        /// <summary>
        /// 是否高亮元素(调试选项中设置)
        /// </summary>
        public bool isHighlightElements { get; set; }


        private static SharedObject _instance = null;
        public static SharedObject Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new SharedObject();
                }

                if(_instance.OutputFun == null)
                {
                    _instance.OutputFun = OutputDefault;
                }

                return _instance;
            }
        }

        private static void OutputDefault(enOutputType type, string msg, string msgDetails)
        {
            //无操作
        }
    }
}
