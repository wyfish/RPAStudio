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
        [Localize.LocalizedDescription("Description1")] //指定即使当前活动失败，也要继续执行其余的活动。只支持布尔值(True,False)。 //Specifies that the remaining activities will continue even if the current activity fails. Only Boolean values are supported. //現在のアクティビティが失敗した場合でも、アクティビティの残りを続行するように指定します。 ブール値（True、False）のみがサポートされています。
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
        [Localize.LocalizedDescription("Description5")] //描述要关闭的流程的流程类型对象。 //A process type object that describes the process to be closed. //終了するプロセスを記述するプロセスタイプオブジェクト。
        public InArgument<Process> Processes { get; set; }

        [Category("Target")]
        [Localize.LocalizedDescription("Description6")] //要关闭的进程的名称。 //The name of the process to close. //クローズするプロセスの名前。
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
