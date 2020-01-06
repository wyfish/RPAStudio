using System;
using System.Activities;
using System.ComponentModel;
using System.Activities.Statements;

namespace RPA.Integration.Activities.Database
{
    [Designer(typeof(TransactionDesigner))]
    public class Transaction : NativeActivity
    {

        [Browsable(false)]
        public ActivityAction Body { get; set; }


        public Transaction()
        {
            Body = new ActivityAction
            {
                Handler = new Sequence()
                {
                    DisplayName = "Do"
                }
            };
        }


        public static string OpenBrowsersPropertyTag { get { return "Transaction"; } }


        [Localize.LocalizedCategory("Category4")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName5")] //错误继续执行 //Error continues to execute //エラーは引き続き実行されます
        [Localize.LocalizedDescription("Description6")] //出现错误是否继续执行 //Whether an error occurs or not //エラーが発生するかどうか
        public InArgument<bool> ContinueOnError { get; set; }

        [Localize.LocalizedCategory("Category4")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName11")] //事务 //Transaction //取引
        [Localize.LocalizedDescription("Description14")] //指定此活动中的数据库操作是否应包装在数据库事务中 //Specifies whether database operations in this activity should be wrapped in a database transaction //このアクティビティのデータベース操作をデータベーストランザクションでラップするかどうかを指定します
        public bool UseTransaction { get; set; }


        [Localize.LocalizedCategory("Category1")] //连接配置 //Connection configuration //接続構成
        [RequiredArgument]
        [OverloadGroup("新建连接")]
        [Localize.LocalizedDescription("Description1")] //用于建立数据库连接的连接字符串 //The connection string used to establish a database connection //データベース接続の確立に使用される接続文字列
        [Localize.LocalizedDisplayName("DisplayName1")] //连接字符串 //Connection string //接続文字列
        public InArgument<string> ConnectionString { get; set; }


        [Localize.LocalizedCategory("Category1")] //连接配置 //Connection configuration //接続構成
        [RequiredArgument]
        [OverloadGroup("新建连接")]
        [Localize.LocalizedDescription("Description2")] //用于访问数据库的数据库提供程序的名称 //The name of the database provider used to access the database //データベースへのアクセスに使用されるデータベースプロバイダーの名前
        [Localize.LocalizedDisplayName("DisplayName2")] //程序名称 //Program name //プログラム名
        public InArgument<string> ProviderName { get; set; }

        [Localize.LocalizedCategory("Category1")] //连接配置 //Connection configuration //接続構成
        [RequiredArgument]
        [OverloadGroup("现有连接")]
        [Localize.LocalizedDescription("Description2")] //用于访问数据库的数据库提供程序的名称 //The name of the database provider used to access the database //データベースへのアクセスに使用されるデータベースプロバイダーの名前
        [Localize.LocalizedDisplayName("DisplayName6")] //现有数据库连接 //Existing database connection //既存のデータベース接続
        public InArgument<DatabaseConnection> DBConnection { get; set; }


        [Localize.LocalizedCategory("Category2")] //输出 //Output //出力
        [Localize.LocalizedDescription("Description3")] //数据库连接 //Database Connectivity //データベース接続
        [Localize.LocalizedDisplayName("DisplayName3")] //用于保存数据库连接的变量(仅支持DatabaseConnection) //Variable for saving database connections (DatabaseConnection only) //データベース接続を保存するための変数（DatabaseConnectionのみ）
        public OutArgument<DatabaseConnection> DBCONN { get; set; }


        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.Integration.Activities;Component/Resources/DataBase/transaction.png";
            }
        }



        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);
        }


        protected override void Execute(NativeActivityContext context)
        {
            var connString = ConnectionString.Get(context);
            var provName = ProviderName.Get(context);
            var dbConnection = DBConnection.Get(context) ?? new DatabaseConnection().Initialize(connString, provName);


            if (dbConnection == null) return;
            if (UseTransaction)
            {
                dbConnection.BeginTransaction();
            }
            DBCONN.Set(context, dbConnection);

            if (Body != null)
            {
                context.ScheduleAction(Body, OnCompletedCallback, OnFaultedCallback);
            }
        }

        private void OnCompletedCallback(NativeActivityContext context, ActivityInstance activityInstance)
        {
            DatabaseConnection conn = null;
            try
            {
                conn = DBConnection.Get(context);
                if (UseTransaction)
                {
                    conn.Commit();
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, ContinueOnError.Get(context));
            }
            finally
            {
                if (conn != null)
                {
                    conn.Dispose();
                }
            }
        }

        private void OnFaultedCallback(NativeActivityFaultContext faultContext, Exception exception, ActivityInstance source)
        {
            faultContext.CancelChildren();
            DatabaseConnection conn = DBConnection.Get(faultContext);
            if (conn != null)
            {
                try
                {
                    if (UseTransaction)
                    {
                        conn.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    conn.Dispose();
                }
            }

            HandleException(exception, ContinueOnError.Get(faultContext));
            faultContext.HandleFault();
        }

        private void HandleException(Exception ex, bool continueOnError)
        {
            if (continueOnError) return;
            throw ex;
        }


        private void OnFaulted(NativeActivityFaultContext faultContext, Exception propagatedException, ActivityInstance propagatedFrom)
        {
            //TODO
        }
        private void OnCompleted(NativeActivityContext context, ActivityInstance completedInstance)
        {
            //TODO
        }
    }
}
