using Plugins.Shared.Library;
using System.Activities;
using System.ComponentModel;
using System.IO;

namespace RPA.Core.Activities.FileActivity
{
    [Designer(typeof(CreateDirectoryDesigner))]
    public sealed class CopyFileActivity : CodeActivity
    {
        public new string DisplayName
        {
            get
            {
                return "Copy File";
            }
        }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.Core.Activities;Component/Resources/File/copyfile.png";
            }
        }

        [Category("Common")]
        [Description("指定即使当前活动失败，也要继续执行其余的活动。只支持布尔值(True,False)。")]
        public InArgument<bool> ContinueOnError { get; set; }

        [RequiredArgument]
        [Category("From")]
        [Description("要复制的文件的路径。")]
        public InArgument<string> Path { get; set; }

        [Category("Options")]
        public bool Overwrite { get; set; }

        [RequiredArgument]
        [Category("To")]
        [Description("要复制文件的目标路径。")]
        public InArgument<string> Destination { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                string filePath = Path.Get(context);
                FileInfo file = new FileInfo(filePath);
                string DestinationPath = Destination.Get(context) + file.Name;
                if (file.Exists)
                {
                    System.IO.File.Copy(filePath, DestinationPath, Overwrite);
                }
                else
                {
                    SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", "文件复制失败");
                }  
            }
            catch
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", "文件复制失败");
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
