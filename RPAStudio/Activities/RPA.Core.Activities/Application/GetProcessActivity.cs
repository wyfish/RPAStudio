using System;
using System.Activities;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

namespace RPA.Core.Activities.ApplicationActivity
{
    [Designer(typeof(ApplicationDesigner))]
    public sealed class GetProcessActivity : CodeActivity
    {
        public new string DisplayName
        {
            get
            {
                return "Get Process";
            }
        }

        [Category("Common")]
        [Description("指定即使当前活动失败，也要继续执行其余的活动。只支持布尔值(True,False)。")]
        public InArgument<bool> ContinueOnError { get; set; }

        [Category("输出")]
        [Description("表示当前Windows会话中正在运行的进程的进程对象的集合。")]
        public OutArgument<Collection<Process>> Processes { get; set; }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.Core.Activities;Component/Resources/Application/process.png";
            }
        }

        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                Process[] ps = Process.GetProcesses();
                Collection<Process> cps = new Collection<Process>();
                foreach (Process p in ps)
                {
                    cps.Add(p);
                }
                Processes.Set(context, cps);
            }
            catch (Exception )
            {
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
