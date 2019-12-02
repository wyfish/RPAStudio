using Plugins.Shared.Library;
using System;
using System.Activities;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace RPA.Integration.Activities.ExcelPlugins
{
    [Designer(typeof(ReadCSVDesigner))]
    public sealed class ReadCSV : CodeActivity
    {

        public ReadCSV()
        {
        }

        [Browsable(false)]
        public string icoPath { get { return "pack://application:,,,/RPA.Integration.Activities;Component/Resources/Excel/csv.png"; } }

        InArgument<string> _PathUrl;
        [Category("选项")]
        [RequiredArgument]
        [DisplayName("文件路径")]
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
        [Category("选项")]
        [DisplayName("分隔符")]
        [Browsable(true)]
        public DelimiterEnums Delimiter
        {
            get { return _Delimiter; }
            set { _Delimiter = value; }
        }

        [Category("选项")]
        [DisplayName("字符编码")]
        [Browsable(true)]
        public InArgument<string> EncodingType{ get; set;}

        [Category("输出")]
        [RequiredArgument]
        [DisplayName("DataTable")]
        [Browsable(true)]
        public OutArgument<DataTable> OutDataTable{ get; set;}

        [Category("选项")]
        [DisplayName("列名")]
        [Browsable(true)]
        public bool IncludeColumnNames { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            Encoding csvEncoding;
            string filePath = PathUrl.Get(context);
            string encodingType = EncodingType.Get(context);
            string delimiter = ",";
            if (Delimiter == DelimiterEnums.Caret插入符号)
                delimiter = "^";
            else if(Delimiter == DelimiterEnums.Comma逗号)
                delimiter = ",";
            else if (Delimiter == DelimiterEnums.Pipe竖线)
                delimiter = "|";
            else if (Delimiter == DelimiterEnums.Semicolon分号)
                delimiter = ";";
            else if (Delimiter == DelimiterEnums.Tab制表符)
                delimiter = "	";

            if (!File.Exists(filePath))
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "文件不存在，请检查路径有效性");
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

            /*设置DataTable*/
            try
            {
                DataTable dataTable = ReadCSVFile(filePath, csvEncoding, delimiter);
                OutDataTable.Set(context, dataTable);
                foreach (DataRow dr in dataTable.Rows)
                {
                    for (int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        System.Diagnostics.Debug.WriteLine("dt : " + dr[i]);
                    }
                }
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "EXCEL执行过程出错", e.Message);
                throw e;
            }
        }

        public DataTable ReadCSVFile(string filePath, Encoding encodingType, string delimiter)
        {
            DataTable dt = new DataTable();
            FileStream fs = new FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);

            //StreamReader sr = new StreamReader(fs, Encoding.UTF8);
            StreamReader sr = new StreamReader(fs, encodingType);
            //string fileContent = sr.ReadToEnd();
            //encoding = sr.CurrentEncoding;
            //记录每次读取的一行记录
            string strLine = "";
            //记录每行记录中的各字段内容
            string[] aryLine = null;
            string[] tableHead = null;
            //标示列数
            int columnCount = 0;
            bool headFlag = false;
            //标示是否是读取的第一行
            //bool IsFirst = true;
            //逐行读取CSV中的数据
            while ((strLine = sr.ReadLine()) != null)
            {
                if (IncludeColumnNames == true)
                {
                    if(delimiter == "   ")
                    {
                        tableHead = Regex.Split(strLine, delimiter, RegexOptions.IgnoreCase);
                    }
                    else
                    {
                        char cDelimiter = delimiter[0];
                        tableHead = strLine.Split(cDelimiter);
                    }
                    IncludeColumnNames = false;
                    headFlag = true;
                    columnCount = tableHead.Length;
                    //创建列
                    for (int i = 0; i < columnCount; i++)
                    {
                        DataColumn dc = new DataColumn(tableHead[i]);
                        dt.Columns.Add(dc);
                    }
                }
                else
                {
                    aryLine = strLine.Split(',');
                    columnCount = aryLine.Length;
                    if (headFlag == false)
                    {
                        headFlag = true;
                        string nameBuff = "列";
                        tableHead = new string[columnCount];
                        for (int i = 0; i < columnCount; i++)
                        {
                            string colName = nameBuff + i;
                            tableHead[i] = colName;
                            DataColumn dc = new DataColumn(tableHead[i]);
                            dt.Columns.Add(dc);
                        }
                    }
                    DataRow dr = dt.NewRow();
                    for (int j = 0; j < columnCount; j++)
                    {
                        dr[j] = aryLine[j];
                    }
                    dt.Rows.Add(dr);
                }
            }
            //if (aryLine != null && aryLine.Length > 0)
            //{
            //    dt.DefaultView.Sort = tableHead[0] + " " + "asc";
            //}

            sr.Close();
            fs.Close();
            return dt;
        }
    }
}
