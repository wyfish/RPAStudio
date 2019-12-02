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


        [Category("输入")]
        [DisplayName("DataTable")]
        [Description("执行单行操作的DataTable变量")]
        public InArgument<DataTable> DataTable { get; set; }


        [Category("输出")]
        [DisplayName("索引")]
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