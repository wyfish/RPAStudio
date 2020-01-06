using System;
using System.Activities;
using System.ComponentModel;
using System.Data;

namespace RPA.Core.Activities.DataTableActivity
{
    [Designer(typeof(ForEachRowDesigner))]
    public sealed class ForEachRow : NativeActivity
    {
        public ForEachRow()
        {
            
        }

        public new string DisplayName
        {
            get
            {
                return "ForEachRow";
            }
        }

        public enum filterTypes
        {
            保留,
            删除
        }


        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [DisplayName("DataTable")]
        [Localize.LocalizedDescription("Description31")] //执行单行操作的DataTable变量 //a single-row operation of the DataTable variable //DataTable変数の単一行操作
        public InArgument<DataTable> DataTable { get; set; }


        [Localize.LocalizedCategory("Category2")] //输出 //Output //出力
        [Localize.LocalizedDisplayName("DisplayName9")] //索引 //index //索引
        [Description("")]
        public OutArgument<Int32> Index { get; set; }


        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.Core.Activities;Component/Resources/DataTable/datatable.png";
            }
        }

        private delegate string runDelegate();
        public string Run()
        {
            return DisplayName;
        }

        protected override void Execute(NativeActivityContext context)
        {
            throw new NotImplementedException();
        }
    }
}
