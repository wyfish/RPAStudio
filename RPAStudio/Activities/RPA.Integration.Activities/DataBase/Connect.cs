using System;
using System.Activities;
using System.ComponentModel;
using Plugins.Shared.Library;
using System.Data.SqlClient;

namespace RPA.Integration.Activities.Database
{
    [Designer(typeof(ConnectDesigner))]
    public sealed class Connect : AsyncCodeActivity
    {
        public new string DisplayName
        {
            get
            {
                return "Connect";
            }
        }

        [Category("连接配置")]
        [RequiredArgument]
        [Description("用于建立数据库连接的连接字符串")]
        [DisplayName("连接字符串")]
        public InArgument<string> ConnectionString { get; set; }


        [Category("连接配置")]
        [RequiredArgument]
        [Description("用于访问数据库的数据库提供程序的名称")]
        [DisplayName("程序名称")]
        public InArgument<string> ProviderName { get; set; }


        [Category("输出")]
        [Description("数据库连接")]
        [DisplayName("用于保存数据库连接的变量(仅支持DatabaseConnection)")]
        public OutArgument<DatabaseConnection> DatabaseConnection { get; set; }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.Integration.Activities;Component/Resources/DataBase/connect.png";
            }
        }

        private void onComplete(NativeActivityContext context, ActivityInstance completedInstance)
        {
        }

        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            string connStr = ConnectionString.Get(context);
            string providerName = ProviderName.Get(context);
            SqlConnection sqlConn = new SqlConnection();
            try
            {
                IDBConnectionFactory _connectionFactory = new DBConnectionFactory();
                Func<DatabaseConnection> action = () => _connectionFactory.Create(connStr, providerName);
                context.UserState = action;
                return action.BeginInvoke(callback, state);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "数据库连接失败:", e.Message);
                throw e;
            }
        }

        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
            Func<DatabaseConnection> action = (Func<DatabaseConnection>)context.UserState;
            var dbConn = action.EndInvoke(result);
            DatabaseConnection.Set(context, dbConn);
        }
    }
}