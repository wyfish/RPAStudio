using Plugins.Shared.Library;
using System;
using System.Activities;
using System.ComponentModel;
using System.Threading;

namespace RPA.UIAutomation.Activities.Window
{
    [Designer(typeof(FindWindowDesigner))]
    public sealed class FindWindowActivity : CodeActivity
    {
        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "Find Window"; } }

        [Browsable(false)]
        public string SourceImgPath { get; set; }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Window/findwindow.png";
            }
        }

        [Category("公共")]
        [DisplayName("错误执行")]
        [Description("指定即使活动引发错误，自动化是否仍应继续")]
        public InArgument<bool> ContinueOnError { get; set; }

        [Category("输入")]
        [Browsable(true)]
        [DisplayName("窗口标题")]
        [Description("输入窗口标题")]
        public InArgument<string> Title
        {
            get;
            set;
        }

        [Category("输入")]
        [Browsable(true)]
        [DisplayName("窗口类名")]
        [Description("输入窗口类名")]
        public InArgument<string> ClassName
        {
            get;
            set;
        }

        [Category("输出")]
        [Browsable(true)]
        public OutArgument<IntPtr> Result{ get; set; }

        private System.Windows.Visibility visi = System.Windows.Visibility.Hidden;
        [Browsable(false)]
        public System.Windows.Visibility visibility
        {
            get
            {
                return visi;
            }
            set
            {
                visi = value;
            }
        }

        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                string _Title = Title.Get(context);
                string _ClassName = ClassName.Get(context);
                IntPtr _result = IntPtr.Zero;

                _result = Win32Api.FindWindow(null, _Title);
                Result.Set(context,_result);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "关闭窗口错误产生", e.Message);
                if (ContinueOnError.Get(context))
                {
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
