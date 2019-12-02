using Plugins.Shared.Library;
using System;
using System.Activities;
using System.ComponentModel;
using System.Diagnostics;


namespace RPA.Core.Activities.ApplicationActivity
{
    [Designer(typeof(StartProcessDesigner))]
    public sealed class StartProcessActivity : CodeActivity
    {
        public new string DisplayName
        {
            get
            {
                return "Start Application";
            }
        }

        [Category("Common")]
        [Description("指定即使当前活动失败，也要继续执行其余的活动。只支持布尔值(True,False)。")]
        public InArgument<bool> ContinueOnError { get; set; }

        [Category("Input")]
        [Description("可以在启动时传递给应用程序的参数。")]
        public InArgument<string> Arguments { get; set; }

        [Category("Input")]
        [Description("可以找到要打开的应用程序的executabel文件的完整路径。注意:所有字符串都必须放在引号之间。")]
        public InArgument<string> FileName { get; set; }

        [Category("Input")]
        [Description("当前工作目录的路径。这个字段只接受字符串变量。注意:所有字符串变量必须放在引号之间。")]
        public InArgument<string> WorkingDirectory { get; set; }

        [Category("Input")]
        public bool Default { get; set; }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.Core.Activities;Component/Resources/Application/openApplication.png";
            }
        }
        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);
            if (Arguments == null && FileName == null)
            {
                metadata.AddValidationError("FileName和Arguments都为空，至少需要一个不为空。");
            }
        }

        protected override void Execute(CodeActivityContext context)
        {
            string _arguments = Arguments.Get(context);
            string _fileName = FileName.Get(context);
            string _workingDirectory = WorkingDirectory.Get(context);
            try
            {
                Process p = new System.Diagnostics.Process();
                if (_arguments != null)
                {
                    p.StartInfo.Arguments = _arguments;
                }
                if (_workingDirectory != null)
                {
                    p.StartInfo.WorkingDirectory = _workingDirectory;
                }
                p.StartInfo.UseShellExecute = Default;
                p.StartInfo.Verb = "Open";
                p.StartInfo.FileName = _fileName;
                p.Start();
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", e.Message);
                if (!ContinueOnError.Get(context))
                    throw new NotImplementedException(e.Message);
            }
        }
    }
}
