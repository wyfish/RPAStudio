using Plugins.Shared.Library;
using System.Activities;
using System.ComponentModel;
using System.IO;

namespace RPA.Core.Activities.FileActivity
{
    [Designer(typeof(AppendLineDesigner))]
    public sealed class AppendLineActivity : CodeActivity
    {
        public new string DisplayName
        {
            get
            {
                return "Append Line";
            }
        }

        [Category("File")]
        [Description("Enter a VB  expression")]
        public InArgument<string> Encoding { get; set; }

        [RequiredArgument]
        [Category("File")]
        [Localize.LocalizedDescription("Description87")] //文件的路径。如果路径不完整，则在项目文件夹中创建文件。 //The path to the file.  If the path is incomplete, create a file in the project folder. //ファイルへのパス。 パスが不完全な場合は、プロジェクトフォルダーにファイルを作成します。
        public InArgument<string> FileName { get; set; }

        [RequiredArgument]
        [Category("Input")]
        [Localize.LocalizedDescription("Description88")] //要附加到文件中的文本。 //The text to be attached to the file. //ファイルに添付するテキスト。
        public InArgument<string> Text { get; set; }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.Core.Activities;Component/Resources/File/appline.png";
            }
        }

        protected override void Execute(CodeActivityContext context)
        {
            string filePath = FileName.Get(context);
            string fileContent = Text.Get(context);
            string EncodingName = Encoding.Get(context);

            if(string.IsNullOrWhiteSpace(EncodingName))
            {
                EncodingName = "UTF-8";
            }
            try
            {
                //  byte[] contentByte = System.Text.Encoding.UTF8.GetBytes(fileContent);
                using (FileStream fsWrite = new FileStream(filePath, FileMode.Append))
                {
                    StreamWriter sw = new StreamWriter(fsWrite, System.Text.Encoding.GetEncoding(EncodingName));
                    sw.WriteLine(fileContent);
                    sw.Flush();
                    sw.Close();
                    fsWrite.Close();     
                };
            }
            catch
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", "增加行失败");
            }
        }
    }
}
