using Plugins.Shared.Library;
using System.Activities;
using System.ComponentModel;
using System.IO;

namespace RPA.Core.Activities.FileActivity
{
    [Designer(typeof(CreateDirectoryDesigner))]
    public sealed class MoveFileActivity : CodeActivity
    {
        public new string DisplayName
        {
            get
            {
                return "Move File";
            }
        }

        [Category("Common")]
        [Description("指定即使当前活动失败，也要继续执行其余的活动。只支持布尔值(True,False)。")]
        public InArgument<bool> ContinueOnError { get; set; }

        [RequiredArgument]
        [Category("From")]
        [Description("要移动的文件的路径。")]
        public InArgument<string> Path { get; set; }

        [Category("Options")]
        public bool Overwrite { get; set; }

        [RequiredArgument]
        [Category("To")]
        [Description("要移动文件的目标路径。")]
        public InArgument<string> Destination { get; set; }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.Core.Activities;Component/Resources/File/movefile.png";
            }
        }

        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                string filePath = Path.Get(context);
                FileInfo file = new FileInfo(filePath);
                string desFilePath = Destination.Get(context);
                string DestinationPath = desFilePath;
                FileInfo desFile = new FileInfo(desFilePath);
                if(desFile.Name=="")
                {
                    DestinationPath = desFilePath+ file.Name;
                }
                if (file.Exists)
                {
                    if(File.Exists(DestinationPath))
                    {
                        if(Overwrite)
                        {
                            File.Delete(DestinationPath);
                        }
                        else
                        {
                            SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", "有重复文件!");
                            return;
                        }
                    }
                    file.MoveTo(DestinationPath);    
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
