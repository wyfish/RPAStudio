using Plugins.Shared.Library;
using System;
using System.Activities;
using System.ComponentModel;
using System.IO;


namespace RPA.Core.Activities.FileActivity
{
    [Designer(typeof(ReadTextFileDesigner))]
    public sealed class ReadTextFileActivity : CodeActivity
    {
        public new string DisplayName
        {
            get
            {
                return "Read Text File";
            }
        }

        [Category("File")]
        [Description("Enter a VB  expression")]
        public InArgument<string> Encoding { get; set; }

        [RequiredArgument]
        [Category("File")]
        [Description("要读取的文件的路径。")]
        public InArgument<string> FileName { get; set; }

        [Category("输出")]
        [Description("从存储在字符串变量中的文件中提取的文本。")]
        public OutArgument<string> Content { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            string filePath = FileName.Get(context);
            string EncodingName = Encoding.Get(context);

            if (string.IsNullOrWhiteSpace(EncodingName))
            {
                EncodingName = "UTF-8";
            }
            try
            {
                using (StreamReader sr = new StreamReader(filePath, System.Text.Encoding.GetEncoding(EncodingName)))
                {
                    string fileContent = sr.ReadToEnd();
                    Content.Set(context,fileContent);
                }
            }
            catch(Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", e.Message);
            }
        }
    }
}
