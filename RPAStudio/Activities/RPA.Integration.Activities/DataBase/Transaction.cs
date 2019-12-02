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


        [Category("选项")]
        [DisplayName("错误继续执行")]
        [Description("出现错误是否继续执行")]
        public InArgument<bool> ContinueOnError { get; set; }

        [Category("选项")]
        [DisplayName("事务")]
        [Description("指定此活动中的数据库操作是否应包装在数据库事务中")]
        public bool UseTransaction { get; set; }


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


        [Category("输出")]
        [Description("数据库连接")]
        [DisplayName("用于保存数据库连接的变量(仅支持DatabaseConnection)")]
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
