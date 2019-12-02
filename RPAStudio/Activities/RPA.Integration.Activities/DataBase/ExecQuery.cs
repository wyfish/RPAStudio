using System;
using System.Activities;
using System.ComponentModel;
using System.Collections.Generic;
using System.Activities.Presentation.Metadata;
using System.Activities.Presentation.PropertyEditing;
using Plugins.Shared.Library.Editors;
using System.Data;

namespace RPA.Integration.Activities.Database
{
    [Designer(typeof(ExecDesigner))]
    public sealed class ExecQuery : AsyncCodeActivity
    {
        public ExecQuery()
        {
            var builder = new AttributeTableBuilder();
            builder.AddCustomAttributes(typeof(ExecQuery), nameof(ExecQuery.Parameters), new EditorAttribute(typeof(DictionaryArgumentEditor), typeof(DialogPropertyValueEditor)));
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }

        public new string DisplayName
        {
            get
            {
                return "ExecQuery";
            }
        }

        InArgument<Int32> _OverTime = 10 * 1000;
        [Category("选项")]
        [DisplayName("超时时间")]
        [Description("指定浏览器响应超时时间(毫秒)")]
        public InArgument<Int32> OverTime
        {
            get
            {
                return _OverTime;
            }
            set
            {
                _OverTime = value;
            }
        }

        [Category("选项")]
        [DisplayName("错误继续执行")]
        [Description("出现错误是否继续执行")]
        public InArgument<bool> ContinueOnError { get; set; }

        CommandType _CommandType = CommandType.Text;
        [Category("选项")]
        [DisplayName("命令类型")]
        [Description("指定如何解释命令字符串")]
        public CommandType CommandType
        {
            get
            {
                return _CommandType;
            }
            set
            {
                _CommandType = value;
            }

        }

        [Category("连接配置")]
        [RequiredArgument]
        [OverloadGroup("新建连接")]
        [Description("用于建立数据库连接的连接字符串")]
        [DisplayName("连接字符串")]
        public InArgument<string> ConnectionString { get; set; }


        [Category("连接配置")]
        [RequiredArgument]
        [OverloadGroup("新建连接")]
        [Description("用于访问数据库的数据库提供程序的名称")]
        [DisplayName("程序名称")]
        public InArgument<string> ProviderName { get; set; }

        [Category("连接配置")]
        [RequiredArgument]
        [OverloadGroup("现有连接")]
        [Description("用于访问数据库的数据库提供程序的名称")]
        [DisplayName("现有数据库连接")]
        public InArgument<DatabaseConnection> DBConnection { get; set; }


        [Category("输入")]
        [RequiredArgument]
        [Description("执行的SQL命令语句")]
        [DisplayName("SQL")]
        public InArgument<string> SQLString { get; set; }

        private Dictionary<string, Argument> parameters;
        [Category("输入")]
        [Browsable(true)]
        [DisplayName("参数")]
        public Dictionary<string, Argument> Parameters
        {
            get
            {
                if (this.parameters == null)
                {
                    this.parameters = new Dictionary<string, Argument>();
                }
                return this.parameters;
            }
            set
            {
                parameters = value;
            }
        }

        [Category("输出")]
        [DisplayName("DataTable")]
        [Description("将SQL查询的输出存储在DataTable变量中")]
        public OutArgument<DataTable> DataTable { get; set; }


        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.Integration.Activities;Component/Resources/DataBase/query.png";
            }
        }

        DatabaseConnection DBConn;


        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            var dataTable = DataTable.Get(context);
            string connString = null;
            string provName = null;
            string sql = string.Empty;
            int commandTimeout = OverTime.Get(context);
            if (commandTimeout < 0)
            {
                throw new ArgumentException("TimeoutMS");
            }
            Dictionary<string, Tuple<object, ArgumentDirection>> parameters = null;
            try
            {
                DBConn = DBConnection.Get(context);
                connString = ConnectionString.Get(context);
                provName = ProviderName.Get(context);
                sql = SQLString.Get(context);
                if (Parameters != null)
                {
                    parameters = new Dictionary<string, Tuple<object, ArgumentDirection>>();
                    foreach (var param in Parameters)
                    {
                        parameters.Add(param.Key, new Tuple<object, ArgumentDirection>(param.Value.Get(context), param.Value.Direction));
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, ContinueOnError.Get(context));
            }

            // create the action for doing the actual work
            Func<DataTable> action = () =>
            {
                if (DBConn == null)
                {
                    DBConn = new DatabaseConnection().Initialize(connString, provName);
                }
                if (DBConn == null)
                {
                    return null;
                }
                return DBConn.ExecuteQuery(sql, parameters, commandTimeout, CommandType);
            };

            context.UserState = action;

            return action.BeginInvoke(callback, state);
        }

        private void HandleException(Exception ex, bool continueOnError)
        {
            if (continueOnError) return;
            throw ex;
        }

        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
            DatabaseConnection existingConnection = DBConnection.Get(context);
            try
            {
                Func<DataTable> action = (Func<DataTable>)context.UserState;
                DataTable dt = action.EndInvoke(result);
                if (dt == null) return;

                DataTable.Set(context, dt);
            }
            catch (Exception ex)
            {
                HandleException(ex, ContinueOnError.Get(context));
            }
            finally
            {
                if (existingConnection == null)
                {
                    DBConn.Dispose();
                }
            }
        }
    }
}