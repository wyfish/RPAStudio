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
        [Description("指定即使当前活动失败，也要继续执行其余的活动。只支持布尔值(True,False)。")]
        public InArgument<bool> ContinueOnError { get; set; }

        [RequiredArgument]
        [Category("File")]
        [Description("要创建的文件的名称。")]
        public InArgument<string> Name { get; set; }

        [RequiredArgument]
        [Category("File")]
        [Description("要创建的文件的完整路径。")]
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
