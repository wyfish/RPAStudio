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
        [Localize.LocalizedDescription("Description1")] //指定即使当前活动失败，也要继续执行其余的活动。只支持布尔值(True,False)。 //Specifies that the remaining activities will continue even if the current activity fails. Only Boolean values are supported. //現在のアクティビティが失敗した場合でも、アクティビティの残りを続行するように指定します。 ブール値（True、False）のみがサポートされています。
        public InArgument<bool> ContinueOnError { get; set; }

        [Localize.LocalizedCategory("Category2")] //输出 //Output //出力
        [Localize.LocalizedDescription("Description4")] //表示当前Windows会话中正在运行的进程的进程对象的集合。 //A collection of process objects representing the processes that are running in the current Windows session. //現在のWindowsセッションで実行されているプロセスを表すプロセスオブジェクトのコレクション。
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
