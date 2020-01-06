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
        [Localize.LocalizedDescription("Description1")] //指定即使当前活动失败，也要继续执行其余的活动。只支持布尔值(True,False)。 //Specifies that the remaining activities will continue even if the current activity fails. Only Boolean values are supported. //現在のアクティビティが失敗した場合でも、アクティビティの残りを続行するように指定します。 ブール値（True、False）のみがサポートされています。
        public InArgument<bool> ContinueOnError { get; set; }

        [RequiredArgument]
        [Category("From")]
        [Localize.LocalizedDescription("Description89")] //要复制的文件的路径。 //The path to the file to be copied. //コピーするファイルへのパス。
        public InArgument<string> Path { get; set; }

        [Category("Options")]
        public bool Overwrite { get; set; }

        [RequiredArgument]
        [Category("To")]
        [Localize.LocalizedDescription("Description90")] //要复制文件的目标路径。 //The target path of the file to be copied. //コピーするファイルのターゲットパス。
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
