using Plugins.Shared.Library;
using System.Activities;
using System.ComponentModel;
using System.IO;


namespace RPA.Core.Activities.FileActivity
{
    [Designer(typeof(CreateDirectoryDesigner))]
    public sealed class DeleteActivity : CodeActivity
    {
        public new string DisplayName
        {
            get
            {
                return "Delete";
            }
        }

        [Category("Common")]
        [Description("指定即使当前活动失败，也要继续执行其余的活动。只支持布尔值(True,False)。")]
        public InArgument<bool> ContinueOnError { get; set; }

        [RequiredArgument]
        [Category("File")]
        [Description("要永久删除的文件或目录的路径。")]
        public InArgument<string> Path { get; set; }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.Core.Activities;Component/Resources/File/delete.png";
            }
        }

        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                string srcPath = Path.Get(context);
                if (File.Exists(srcPath))
                {
                    File.Delete(srcPath);
                }
                else
                {
                    Directory.Delete(srcPath, true);
                }
            }
            catch 
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", "删除文件或文件夹失败!");
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
