using Plugins.Shared.Library;
using System;
using System.Activities;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;


namespace RPA.Core.Activities.ApplicationActivity
{
    [Designer(typeof(CloseApplicationDesigner))]
    public sealed class CloseApplicationActivity : CodeActivity
    {
        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "Close Application"; } }

        [Category("Common")]
        [Description("指定即使当前活动失败，也要继续执行其余的活动。只支持布尔值(True,False)。")]
        public InArgument<bool> ContinueOnError { get; set; }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.Core.Activities;Component/Resources/Application/close.png";
            }
        }

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

        [Browsable(false)]
        public string SourceImgPath { get; set; }

        [Category("Input")]
        [Description("进程名称")]
        [DisplayNameAttribute("ProcessName")]
        public InArgument<string> ProcessName { get; set; }

        [Category("UI对象")]
        [OverloadGroup("G1")]
        [Browsable(true)]
        [DisplayName("窗口指示器")]
        [Description("用于在执行活动时查找特定UI元素的Text属性")]
        public InArgument<string> Selector { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                string _ProcessName = ProcessName.Get(context);
                if(_ProcessName.ToUpper().EndsWith(".EXE"))
                {
                    _ProcessName = _ProcessName.Substring(0,_ProcessName.Length-4);
                }

                Process[] ps = Process.GetProcessesByName(_ProcessName);
                foreach(Process item in ps)
                {
                    item.Kill();
                }
                Thread.Sleep(1000);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", e.Message);
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
