using System.Activities;
using System.ComponentModel;
using System.Data;

namespace RPA.Core.Activities.DataTableActivity
{
    [Designer(typeof(MergeDataTableDesigner))]
    public sealed class MergeDataTable : CodeActivity
    {
        public MergeDataTable()
        {
            
        }

        public new string DisplayName
        {
            get
            {
                return "MergeDataTable";
            }
        }


        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [RequiredArgument]
        [Localize.LocalizedDisplayName("DisplayName29")] //目标 //aims //目標
        [Localize.LocalizedDescription("Description58")] //合并源DataTable的DataTable对象 //Merge the DataTable object of the source DataTable //ソースDataTableのDataTableオブジェクトをマージします
        public InArgument<DataTable> Destination { get; set; }

        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [RequiredArgument]
        [Localize.LocalizedDisplayName("DisplayName30")] //源 //source //出所
        [Localize.LocalizedDescription("Description59")] //要添加到目标DataTable的DataTable对象 //The DataTable object to be added to the target DataTable //ターゲットDataTableに追加されるDataTableオブジェクト
        public InArgument<DataTable> Source { get; set; }


        public MissingSchemaAction _MergeType = MissingSchemaAction.Add;
        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [RequiredArgument]
        [Localize.LocalizedDisplayName("DisplayName31")] //合并操作 //Merge operation //マージ操作
        [Localize.LocalizedDescription("Description60")] //指定合并两个DataTable时要执行的操作 //Specify what to do when merging two DataTables //2つのDataTableをマージするときの処理を指定します
        public MissingSchemaAction MergeType
        {
            get
            {
                return _MergeType;
            }
            set
            {
                _MergeType = value;
            }
        }


        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.Core.Activities;Component/Resources/DataTable/datatable.png";
            }
        }

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);
        }


        protected override void Execute(CodeActivityContext context)
        {
            DataTable destination = Destination.Get(context);
            DataTable source = Source.Get(context);

            destination.Merge(source, true, MergeType);
        }
    }
}
