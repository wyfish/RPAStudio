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
        [Localize.LocalizedDescription("Description1")] //指定即使当前活动失败，也要继续执行其余的活动。只支持布尔值(True,False)。 //Specifies that the remaining activities will continue even if the current activity fails. Only Boolean values are supported. //現在のアクティビティが失敗した場合でも、アクティビティの残りを続行するように指定します。 ブール値（True、False）のみがサポートされています。
        public InArgument<bool> ContinueOnError { get; set; }

        [RequiredArgument]
        [Category("From")]
        [Localize.LocalizedDescription("Description95")] //要移动的文件的路径。 //The path to the file to move. //移動するファイルへのパス。
        public InArgument<string> Path { get; set; }

        [Category("Options")]
        public bool Overwrite { get; set; }

        [RequiredArgument]
        [Category("To")]
        [Localize.LocalizedDescription("Description96")] //要移动文件的目标路径。 //The target path to move the file. //ファイルを移動するターゲットパス。
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
