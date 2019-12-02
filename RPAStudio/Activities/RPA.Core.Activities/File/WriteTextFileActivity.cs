using Plugins.Shared.Library;
using System;
using System.Activities;
using System.ComponentModel;
using System.IO;

namespace RPA.Core.Activities.FileActivity
{
    [Designer(typeof(AppendLineDesigner))]
    public sealed class WriteTextFileActivity : CodeActivity
    {
        public new string DisplayName
        {
            get
            {
                return "Write Text File";
            }
        }

        [Category("File")]
        [Description("Enter a VB  expression")]
        public InArgument<string> Encoding { get; set; }

        [RequiredArgument]
        [Category("File")]
        [Description("要写入的文件的路径。")]
        public InArgument<string> FileName { get; set; }

        [RequiredArgument]
        [Category("Input")]
        [Description("要写入文件的文本。")]
        public InArgument<string> Text { get; set; }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.Core.Activities;Component/Resources/File/write.png";
            }
        }

        protected override void Execute(CodeActivityContext context)
        {
            string filePath = FileName.Get(context);
            string fileContent = Text.Get(context);
            string EncodingName = Encoding.Get(context);

            if (string.IsNullOrWhiteSpace(EncodingName))
            {
                EncodingName = "UTF-8";
            }
            try
            {
                //  byte[] contentByte = System.Text.Encoding.UTF8.GetBytes(fileContent);
                using (FileStream fsWrite = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    StreamWriter sw = new StreamWriter(fsWrite, System.Text.Encoding.GetEncoding(EncodingName));
                    sw.Write(fileContent);
                    sw.Flush();
                    sw.Close();
                    fsWrite.Close();
                };
            }
            catch(Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", e.Message);
            }
        }
    }
}
