using Plugins.Shared.Library;
using System.Activities;
using System.ComponentModel;
using System.IO;


namespace RPA.Core.Activities.FileActivity
{
    [Designer(typeof(CreateDirectoryDesigner))]
    public sealed class CreateFileActivity : CodeActivity
    {
        public new string DisplayName
        {
            get
            {
                return "Create File";
            }
        }

        [Category("Common")]
        [Localize.LocalizedDescription("Description1")] //指定即使当前活动失败，也要继续执行其余的活动。只支持布尔值(True,False)。 //Specifies that the remaining activities will continue even if the current activity fails. Only Boolean values are supported. //現在のアクティビティが失敗した場合でも、アクティビティの残りを続行するように指定します。 ブール値（True、False）のみがサポートされています。
        public InArgument<bool> ContinueOnError { get; set; }

        [RequiredArgument]
        [Category("File")]
        [Localize.LocalizedDescription("Description92")] //要创建的文件的名称。 //The name of the file to create. //作成するファイルの名前。
        public InArgument<string> Name { get; set; }

        [RequiredArgument]
        [Category("File")]
        [Localize.LocalizedDescription("Description93")] //要创建的文件的完整路径。 //The full path to the file to be created. //作成するファイルへのフルパス。
        public InArgument<string> Path { get; set; }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.Core.Activities;Component/Resources/File/newfile.png";
            }
        }

        protected override void Execute(CodeActivityContext context)
        {
            FileStream fileStream = null;
            try
            {
                string filePath = Path.Get(context);
                string fileName = Name.Get(context);

                if (string.IsNullOrEmpty(filePath))
                {
                    fileStream = File.Create(fileName);
                }
                else
                {
                    if (string.IsNullOrEmpty(fileName))
                    {
                        fileStream = File.Create(filePath);
                    }
                    else
                    {
                        fileStream = File.Create(System.IO.Path.Combine(filePath, fileName));
                    }
                }
            }
            catch
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", "创建文件失败!");
                if (ContinueOnError.Get(context))
                {
                }
                else
                {
                    throw;
                }
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
        }
    }
}
