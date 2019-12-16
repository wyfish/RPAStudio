using Plugins.Shared.Library;
using System;
using System.Activities;
using System.ComponentModel;
using System.Diagnostics;

namespace RPA.Core.Activities.ApplicationActivity
{
    [Designer(typeof(ApplicationDesigner))]
    public sealed class KillProcessActivity : CodeActivity
    {
        public new string DisplayName
        {
            get
            {
                return "Kill Process";
            }
        }

        [Category("Common")]
        [Description("指定即使当前活动失败，也要继续执行其余的活动。只支持布尔值(True,False)。")]
        public InArgument<bool> ContinueOnError { get; set; }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.Core.Activities;Component/Resources/Application/killprocess.png";
            }
        }
        
        [Category("Target")]
        [Description("描述要关闭的流程的流程类型对象。")]
        public InArgument<Process> Processes { get; set; }

        [Category("Target")]
        [Description("要关闭的进程的名称。")]
        public InArgument<string> ProcessName { get; set; }

        //protected override void CacheMetadata(CodeActivityMetadata metadata)
        //{
        //     base.CacheMetadata(metadata); 
        //     if (Processes == null && ProcessName == null)
        //     { 
        //         metadata.AddValidationError("Processes和ProcessName都为空，至少需要一个不为空。");
        //     }
        //}

        protected override void Execute(CodeActivityContext context)
        {
            Process ps = Processes.Get(context);
            string psName = ProcessName.Get(context);
            if (psName.ToUpper().EndsWith(".EXE"))
            {
                psName = psName.Substring(0, psName.Length - 4);
            }
            try
            {
                if (ps != null) ps.Kill();

                if (psName != null)
                {
                    foreach (Process p in Process.GetProcesses())
                    {
                        if (Equals(p.ProcessName,psName))
                        {
                            p.Kill();
                        }
                    }
                }
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
