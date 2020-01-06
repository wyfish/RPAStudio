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
        [Localize.LocalizedDescription("Description1")] //指定即使当前活动失败，也要继续执行其余的活动。只支持布尔值(True,False)。 //Specifies that the remaining activities will continue even if the current activity fails. Only Boolean values are supported. //現在のアクティビティが失敗した場合でも、アクティビティの残りを続行するように指定します。 ブール値（True、False）のみがサポートされています。
        public InArgument<bool> ContinueOnError { get; set; }

        [RequiredArgument]
        [Category("File")]
        [Localize.LocalizedDescription("Description94")] //要永久删除的文件或目录的路径。 //The path to the file or directory to be permanently deleted. //完全に削除するファイルまたはディレクトリへのパス。
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
