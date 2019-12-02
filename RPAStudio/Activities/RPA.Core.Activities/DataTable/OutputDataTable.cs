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


        [Category("输入")]
        [RequiredArgument]
        [DisplayName("DataTable")]
        [Description("要写入字符串的DataTable对象")]
        public InArgument<DataTable> DataTable { get; set; }

        [Category("输出")]
        [RequiredArgument]
        [DisplayName("文本")]
        [Description("DataTable作为字符串的输出")]
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