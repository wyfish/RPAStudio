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

        [Category("输入")]
        [RequiredArgument]
        [OverloadGroup("Column")]
        [DisplayName("Column")]
        [Description("要附加到DataTable的列集合的DataColumn对象。如果设置了此属性，则会忽略“选项”类别下的所有属性")]
        public InArgument<DataColumn> Column { get; set; }

        [Category("输入")]
        [RequiredArgument]
        [OverloadGroup("ColumnName")]
        [DisplayName("ColumnName")]
        [Description("新列的名称")]
        public InArgument<string> ColumnName { get; set; }

        [Category("输入")]
        [RequiredArgument]
        [DisplayName("DataTable")]
        [Description("要添加列的DataTable对象")]
        public InArgument<DataTable> DataTable { get; set; }


        [Category("选项")]
        [DisplayName("允许为空")]
        [Description("指定新列中字段是否允许为空")]
        public bool AllowNull { get; set; } = true;

        [Category("选项")]
        [DisplayName("自动递增")]
        [Description(" 指定在添加新行时列的值是否自动递增")]
        public bool AutoIncrement { get; set; }

        [Category("选项")]
        [DisplayName("唯一约束")]
        [Description(" 指定新列的每一行中的值必须是唯一的")]
        public bool Unique { get; set; }

        [Category("选项")]
        [DisplayName("最大长度")]
        [Description(" 指定新列的值的最大长度")]
        public Int32 MaxLength { get; set; }

        [Category("选项")]
        [DisplayName("默认值")]
        [Description(" 指定新列的值的最大长度")]
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