using Plugins.Shared.Library.Librarys;
using System;
using System.Activities;
using System.ComponentModel;
using System.Data;

namespace RPA.Core.Activities.DataTableActivity
{
    [Designer(typeof(OutputDataTableDesigner))]
    public sealed class OutputDataTable : CodeActivity
    {
        public OutputDataTable()
        {
            
        }

        public new string DisplayName
        {
            get
            {
                return "OutputDataTable";
            }
        }


        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [RequiredArgument]
        [DisplayName("DataTable")]
        [Localize.LocalizedDescription("Description61")] //要写入字符串的DataTable对象 //The DataTable object to write to the string //文字列に書き込むDataTableオブジェクト
        public InArgument<DataTable> DataTable { get; set; }

        [Localize.LocalizedCategory("Category2")] //输出 //Output //出力
        [RequiredArgument]
        [Localize.LocalizedDisplayName("DisplayName32")] //文本 //Text //テキスト
        [Localize.LocalizedDescription("Description62")] //DataTable作为字符串的输出 //DataTable as a string output //文字列出力としてのDataTable
        public OutArgument<string> Text { get; set; }



        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.Core.Activities;Component/Resources/DataTable/datatable.png";
            }
        }

        protected override void Execute(CodeActivityContext context)
        {
            DataTable dt = DataTable.Get(context);
            string text = DataTableFormatter.FormatTable(dt);
            Text.Set(context, text);
        }
    }
}
