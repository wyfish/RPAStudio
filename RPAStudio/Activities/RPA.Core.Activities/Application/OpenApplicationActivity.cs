using RPA.Core.Activities.Properties;
using System;
using System.Activities;
using System.Activities.Statements;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;


namespace RPA.Core.Activities.ApplicationActivity
{
    [Designer(typeof(OpenApplicationDesigner))]
    public sealed class OpenApplicationActivity : NativeActivity
    {
        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "Open Application"; } }

        [Category("Common")]
        [Description("指定即使当前活动失败，也要继续执行其余的活动。只支持布尔值(True,False)。")]
        public InArgument<bool> ContinueOnError { get; set; }

        [Category("Common")]
        [Description("指定在抛出错误之前等待活动运行的时间(以毫秒为单位)。默认值为3000毫秒(3秒)。")]
        [DisplayNameAttribute("Timeout(milliseconds)")]
        public InArgument<Int32> Timeout { get; set; }

        [Category("Input")]
        [Description("可以在启动时传递给应用程序的参数。")]
        public InArgument<string> Arguments { get; set; }

        [Category("Input")]
        [Description("可以找到要打开的应用程序的可执行文件的完整文件路径。注意:所有字符串都必须放在引号之间。")]
        [DisplayNameAttribute("FileName")]
        public InArgument<string> ProcessPath { get; set; }

        [Category("Options")]
        [Description("当前工作目录的路径。这个字段只接受字符串变量。注意:所有字符串变量必须放在引号之间。")]
        public InArgument<string> WorkingDirectory { get; set; }

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);
            if (Arguments == null && ProcessPath == null)
            {
                metadata.AddValidationError("FileName和Arguments都为空，至少需要一个不为空。");
            }
        }

        public OpenApplicationActivity()
        {
            Body = new ActivityAction<object>
            {
                Handler = new Sequence()
                {
                    DisplayName = Resources.Do
                }
            };
        }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.Core.Activities;Component/Resources/Application/openApplication.png";
            }
        }

        [Browsable(false)]
        public ActivityAction<object> Body { get; set; }

        [Browsable(false)]
        public string SourceImgPath { get; set; }
        [Category("UI对象")]
        [OverloadGroup("G1")]
        [Browsable(true)]
        [DisplayName("窗口指示器")]
        [Description("用于在执行活动时查找特定UI元素的Text属性")]
        public InArgument<string> Selector { get; set; }

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

        protected override void Execute(NativeActivityContext context)
        {
            try
            {
                string _arguments = Arguments.Get(context);
                string _fileName = ProcessPath.Get(context);
                string _workingDirectory = WorkingDirectory.Get(context);
                Int32 _timeout = Timeout.Get(context);
                Process p = new System.Diagnostics.Process();
                if (_arguments != null)
                {
                    p.StartInfo.Arguments = _arguments;
                }
                if (_workingDirectory != null)
                {
                    p.StartInfo.WorkingDirectory = _workingDirectory;
                }
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.FileName = _fileName;
                p.Start();
                Thread.Sleep(_timeout);
                context.ScheduleAction(Body, "", OnCompleted, OnFaulted);
            }
            catch (Exception e)
            {
                if (ContinueOnError.Get(context))
                {
                }
                else
                {
                    throw new NotImplementedException(e.Message);
                }
            }
        }

        private void OnFaulted(NativeActivityFaultContext faultContext, Exception propagatedException, ActivityInstance propagatedFrom)
        {
          //  faultContext.CancelChildren();
           // Cleanup();
        }

        private void OnCompleted(NativeActivityContext context, ActivityInstance completedInstance)
        {
            //Cleanup();
        }
    }
}
