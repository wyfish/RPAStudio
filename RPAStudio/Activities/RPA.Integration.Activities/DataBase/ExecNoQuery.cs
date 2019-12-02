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
    public sealed class ExecNoQuery : AsyncCodeActivity
    {
        public ExecNoQuery()
        {
            var builder = new AttributeTableBuilder();
            builder.AddCustomAttributes(typeof(ExecNoQuery), nameof(ExecNoQuery.Parameters), new EditorAttribute(typeof(DictionaryArgumentEditor), typeof(DialogPropertyValueEditor)));
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }

        public new string DisplayName
        {
            get
            {
                return "ExecNoQuery";
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
        [OverloadGroup("ConnectionConfig")]
        [Description("用于建立数据库连接的连接字符串")]
        [DisplayName("连接字符串")]
        public InArgument<string> ConnectionString { get; set; }


        [Category("连接配置")]
        [RequiredArgument]
        [OverloadGroup("ConnectionConfig")]
        [Description("用于访问数据库的数据库提供程序的名称")]
        [DisplayName("程序名称")]
        public InArgument<string> ProviderName { get; set; }

        [Category("连接配置")]
        [RequiredArgument]
        [OverloadGroup("DatabaseConnection")]
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
        [Description("对于UPDATE，INSERT和DELETE语句，返回值是受命令影响的行数。对于所有其他类型的语句，返回值为-1")]
        [DisplayName("执行结果")]
        public OutArgument<Int32> AffectedRecords { get; set; }


        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.Integration.Activities;Component/Resources/DataBase/noquery.png";
            }
        }

        DatabaseConnection DBConn;


        protected override System.IAsyncResult BeginExecute(AsyncCodeActivityContext context, System.AsyncCallback callback, object state)
        {
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
                sql = SQLString.Get(context);
                DBConn = DBConnection.Get(context);
                connString = ConnectionString.Get(context);
                provName = ProviderName.Get(context);
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
            Func<DBExecuteCommandResult> action = () =>
            {
                DBExecuteCommandResult executeResult = new DBExecuteCommandResult();
                if (DBConn == null)
                {
                    DBConn = new DatabaseConnection().Initialize(connString, provName);
                }
                if (DBConn == null)
                {
                    return executeResult;
                }
                executeResult = new DBExecuteCommandResult(DBConn.Execute(sql, parameters, commandTimeout, CommandType), parameters);
                return executeResult;
            };

            context.UserState = action;

            return action.BeginInvoke(callback, state);
        }

        private void HandleException(Exception ex, bool continueOnError)
        {
            if (continueOnError) return;
            throw ex;
        }

        protected override void EndExecute(AsyncCodeActivityContext context, System.IAsyncResult result)
        {
            DatabaseConnection existingConnection = DBConnection.Get(context);
            try
            {
                Func<DBExecuteCommandResult> action = (Func<DBExecuteCommandResult>)context.UserState;
                DBExecuteCommandResult commandResult = action.EndInvoke(result);
                this.AffectedRecords.Set(context, commandResult.Result);
                foreach (var param in commandResult.ParametersBind)
                {
                    var currentParam = Parameters[param.Key];
                    if (currentParam.Direction == ArgumentDirection.Out || currentParam.Direction == ArgumentDirection.InOut)
                    {
                        currentParam.Set(context, param.Value.Item1);
                    }
                }
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

        private class DBExecuteCommandResult
        {
            public int Result { get; }
            public Dictionary<string, Tuple<object, ArgumentDirection>> ParametersBind { get; }

            public DBExecuteCommandResult()
            {
                this.Result = 0;
                this.ParametersBind = new Dictionary<string, Tuple<object, ArgumentDirection>>();
            }

            public DBExecuteCommandResult(int result, Dictionary<string, Tuple<object, ArgumentDirection>> parametersBind)
            {
                this.Result = result;
                this.ParametersBind = parametersBind;
            }
        }
    }
}