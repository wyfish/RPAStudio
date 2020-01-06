using Plugins.Shared.Library;
using RPA.Core.Activities.DataTableActivity;
using System;
using System.Activities;
using System.Activities.Presentation;
using System.Activities.Presentation.Metadata;
using System.ComponentModel;
using System.Data;

namespace RPA.Core.Activities.DataTableActivity
{
    [Designer(typeof(AddDataColumnDesigner))]
    public sealed class AddDataColumn<T> : AsyncCodeActivity
    {
        public AddDataColumn()
        {
            AttributeTableBuilder builder = new AttributeTableBuilder();
            Type attrType = Type.GetType("System.Activities.Presentation.FeatureAttribute, System.Activities.Presentation, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
            Type argType = Type.GetType("System.Activities.Presentation.UpdatableGenericArgumentsFeature, System.Activities.Presentation, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");

            Type psType = Type.GetType("");
            builder.AddCustomAttributes(typeof(AddDataColumn<>), new Attribute[] { Activator.CreateInstance(attrType, new object[] { argType, }) as Attribute });
            builder.AddCustomAttributes(typeof(AddDataColumn<>), new DefaultTypeArgumentAttribute(typeof(object)));
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }

        public new string DisplayName
        {
            get
            {
                return "AddDataColumn";
            }
        }

        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [RequiredArgument]
        [OverloadGroup("Column")]
        [DisplayName("Column")]
        [Localize.LocalizedDescription("Description15")] //要附加到DataTable的列集合的DataColumn对象。如果设置了此属性，则会忽略“选项”类别下的所有属性 //The DataColumn object to be attached to the column set of the DataTable.  If this property is set, all properties under the Options category are ignored //DataTableの列セットにアタッチされるDataColumnオブジェクト。 このプロパティが設定されている場合、オプションカテゴリの下のすべてのプロパティは無視されます
        public InArgument<DataColumn> Column { get; set; }

        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [RequiredArgument]
        [OverloadGroup("ColumnName")]
        [DisplayName("ColumnName")]
        [Localize.LocalizedDescription("Description16")] //新列的名称 //The name of the new column //新しい列の名前
        public InArgument<string> ColumnName { get; set; }

        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [RequiredArgument]
        [DisplayName("DataTable")]
        [Localize.LocalizedDescription("Description17")] //要添加列的DataTable对象 //The DataTable object to add the column to //列を追加するDataTableオブジェクト
        public InArgument<DataTable> DataTable { get; set; }


        [Localize.LocalizedCategory("Category4")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName2")] //允许为空 //Allowed to be empty //空にすることができます
        [Localize.LocalizedDescription("Description18")] //指定新列中字段是否允许为空 //Specify whether the fields in the new column are allowed to be empty //新しい列のフィールドを空にすることを許可するかどうかを指定します
        public bool AllowNull { get; set; } = true;

        [Localize.LocalizedCategory("Category4")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName3")] //自动递增 //Auto increment //自動インクリメント
        [Localize.LocalizedDescription("Description19")] // 指定在添加新行时列的值是否自动递增 //Specifies whether the value of the column is automatically incremented when a new row is added //新しい行が追加されたときに列の値を自動的にインクリメントするかどうかを指定します
        public bool AutoIncrement { get; set; }

        [Localize.LocalizedCategory("Category4")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName4")] //唯一约束 //Unique constraint //ユニーク制約
        [Localize.LocalizedDescription("Description20")] // 指定新列的每一行中的值必须是唯一的 //Specify the value in each row of the new column must be unique //新しい列の各行の値は一意でなければなりません
        public bool Unique { get; set; }

        [Localize.LocalizedCategory("Category4")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName5")] //最大长度 //The maximum length //最大長
        [Localize.LocalizedDescription("Description21")] // 指定新列的值的最大长度 //Specify the maximum length of the value of the new column //新しい列の値の最大長を指定します
        public Int32 MaxLength { get; set; }

        [Localize.LocalizedCategory("Category4")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName6")] //默认值 //Defaults //デフォルト値
        [Localize.LocalizedDescription("Description21")] // 指定新列的值的最大长度 //Specify the maximum length of the value of the new column //新しい列の値の最大長を指定します
        public InArgument<object> DefaultValue { get; set; }


        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.Core.Activities;Component/Resources/DataTable/datatable.png";
            }
        }


        

        private delegate string runDelegate();
        private runDelegate m_Delegate;
        public string Run()
        {
            return DisplayName;
        }


        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            Type type = typeof(T);
            DataTable dataTable = DataTable.Get(context);
            DataColumn column = Column.Get(context);
            string columnName = ColumnName.Get(context);
            object defaultValue = DefaultValue.Get(context);
            try
            {
                if (column != null)
                {
                    dataTable.Columns.Add(column);
                }
                else
                {
                    DataColumn newColumn = new DataColumn(columnName);
                    newColumn.DataType = type;
                    if (defaultValue != null)
                        newColumn.DefaultValue = defaultValue;
                    if (MaxLength >= 0)
                        newColumn.MaxLength = MaxLength;
                    newColumn.AllowDBNull = AllowNull;
                    newColumn.Unique = Unique;
                    newColumn.AutoIncrement = AutoIncrement;
                    dataTable.Columns.Add(newColumn);
                }
            }
            catch(Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "增加数据库列失败", e.Message);
                throw e;
            }

            m_Delegate = new runDelegate(Run);
            return m_Delegate.BeginInvoke(callback, state);
        }


        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
            //try
            //{
            //    Func<int> action = (Func<int>)context.UserState;
            //    int affectedRecords = action.EndInvoke(result);
            //}
            //catch (Exception e)
            //{
            //    SharedObject.Instance.Output(SharedObject.enOutputType.Error, "", e.Message);
            //}
        }
    }
}
