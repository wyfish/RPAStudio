using Plugins.Shared.Library;
using System.Activities;
using System.ComponentModel;
using System.IO;


namespace RPA.Core.Activities.FileActivity
{
    [Designer(typeof(CreateDirectoryDesigner))]
    public sealed class CreateDirectoryActivity : CodeActivity
    {
        public new string DisplayName
        {
            get
            {
                return "Create Directory";
            }
        }

        [Category("Common")]
        [Description("指定即使当前活动失败，也要继续执行其余的活动。只支持布尔值(True,False)。")]
        public InArgument<bool> ContinueOnError { get; set; }

        [RequiredArgument]
        [Category("Directory")]
        [Description("要创建的目录的完整路径。")]
        public InArgument<string> Path { get; set; }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.Core.Activities;Component/Resources/File/createdir.png";
            }
        }

        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                string sPath = Path.Get(context);
                if (!Directory.Exists(sPath))
                {
                    Directory.CreateDirectory(sPath);
                }
                else
                {
                    SharedObject.Instance.Output(SharedObject.enOutputType.Warning, "有一个警告产生", "文件夹已存在!");
                }
            }
            catch
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", "创建文件夹失败!");
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
