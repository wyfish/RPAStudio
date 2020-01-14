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

        [Localize.LocalizedCategory("Category1")] //连接配置 //Connection configuration //接続構成
        [RequiredArgument]
        [Localize.LocalizedDescription("Description1")] //用于建立数据库连接的连接字符串 //The connection string used to establish a database connection //データベース接続の確立に使用される接続文字列
        [Localize.LocalizedDisplayName("DisplayName1")] //连接字符串 //Connection string //接続文字列
        public InArgument<string> ConnectionString { get; set; }


        [Localize.LocalizedCategory("Category1")] //连接配置 //Connection configuration //接続構成
        [RequiredArgument]
        [Localize.LocalizedDescription("Description2")] //用于访问数据库的数据库提供程序的名称 //The name of the database provider used to access the database //データベースへのアクセスに使用されるデータベースプロバイダーの名前
        [Localize.LocalizedDisplayName("DisplayName2")] //程序名称 //Program name //プログラム名
        public InArgument<string> ProviderName { get; set; }


        [Localize.LocalizedCategory("Category2")] //输出 //Output //出力
        [Localize.LocalizedDescription("Description3")] //数据库连接 //Database Connectivity //データベース接続
        [Localize.LocalizedDisplayName("DisplayName3")] //用于保存数据库连接的变量(仅支持DatabaseConnection) //Variable for saving database connections (DatabaseConnection only) //データベース接続を保存するための変数（DatabaseConnectionのみ）
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
