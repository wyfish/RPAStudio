using Plugins.Shared.Library;
using System;
using System.Activities;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;


namespace RPA.Integration.Activities.ExcelPlugins
{
    [Designer(typeof(WriteCSVDesigner))]
    public sealed class AppendCSV : CodeActivity
    {

        public AppendCSV()
        { 
        }

        [Browsable(false)]
        public string icoPath { get { return "pack://application:,,,/RPA.Integration.Activities;Component/Resources/Excel/csv.png"; } }

        InArgument<string> _PathUrl;
        [Localize.LocalizedCategory("Category4")] //选项 //Option //オプション
        [RequiredArgument]
        [Localize.LocalizedDisplayName("DisplayName12")] //文件路径 //File path //ファイルパス
        [Browsable(true)]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public InArgument<string> PathUrl
        {
            get
            {
                return _PathUrl;
            }
            set
            {
                _PathUrl = value;
            }
        }

        /***
         Tab制表符  Comma逗号, Semicolon分号; Caret插入符号^ Pipe竖线|
         ***/
        public enum DelimiterEnums
        {
            Tab制表符,
            Comma逗号,
            Semicolon分号,
            Caret插入符号,
            Pipe竖线
        }

        DelimiterEnums _Delimiter = DelimiterEnums.Comma逗号;
        [Localize.LocalizedCategory("Category4")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName13")] //分隔符 //Separator //セパレーター
        [Browsable(true)]
        public DelimiterEnums Delimiter
        {
            get { return _Delimiter; }
            set { _Delimiter = value; }
        }

        [Localize.LocalizedCategory("Category4")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName14")] //字符编码 //Character Encoding //文字エンコード
        [Browsable(true)]
        public InArgument<string> EncodingType { get; set;}


        [Localize.LocalizedCategory("Category4")] //选项 //Option //オプション
        [RequiredArgument]
        [DisplayName("DataTable")]
        [Browsable(true)]
        public InArgument<DataTable> InDataTable { get; set; }


        protected override void Execute(CodeActivityContext context)
        {
            Encoding csvEncoding;
            string filePath = PathUrl.Get(context);
            string encodingType = EncodingType.Get(context);
            string delimiter = ",";
            if (Delimiter == DelimiterEnums.Caret插入符号)
                delimiter = "^";
            else if (Delimiter == DelimiterEnums.Comma逗号)
                delimiter = ",";
            else if (Delimiter == DelimiterEnums.Pipe竖线)
                delimiter = "|";
            else if (Delimiter == DelimiterEnums.Semicolon分号)
                delimiter = ";";
            else if (Delimiter == DelimiterEnums.Tab制表符)
                delimiter = "	";

            if (!File.Exists(filePath))
            {
                // 文件不存在，请检查路径有效性
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, Localize.LocalizedResources.GetString("xFileNotExist"));
                return;
            }

            /*取字符编码 如果为空则取文件编码 异常则取系统默认编码*/
            try
            {
                if (encodingType == null)
                    csvEncoding = CSVEncoding.GetEncodingType(filePath);
                else
                    csvEncoding = Encoding.GetEncoding(encodingType);
            }
            catch (Exception)
            {
                csvEncoding = System.Text.Encoding.Default;
            }

            /*将DataTable内容写入CSV文件*/
            try
            {
                DataTable inDataTable = InDataTable.Get(context);
                WriteCSVFile(inDataTable, filePath, csvEncoding, delimiter);
            }
            catch (Exception e)
            {
                // EXCEL执行过程出错
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, Localize.LocalizedResources.GetString("xExcelExecError"), e.Message);
                throw e;
            }
        }

        public void WriteCSVFile(DataTable dt, string fullPath, Encoding encodingType, string delimiter)
        {
            FileInfo fi = new FileInfo(fullPath);
            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }
            FileStream fs = new FileStream(fullPath, System.IO.FileMode.Append , System.IO.FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, encodingType);

            string data = "";
            //写出各行数据
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                data = "";
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string str = dt.Rows[i][j].ToString();
                    str = str.Replace("\"", "\"\"");//替换英文冒号 英文冒号需要换成两个冒号
                    if (str.Contains(',') || str.Contains('"') || str.Contains('\r') || str.Contains('\n'))    
                    //含逗号 冒号 换行符的需要放到引号中
                    {
                        str = string.Format("\"{0}\"", str);
                    }

                    data += str;
                    if (j < dt.Columns.Count - 1)
                    {
                        data += delimiter;
                    }
                }
                sw.WriteLine(data);
            }
            sw.Close();
            fs.Close();
        }
    }
}
