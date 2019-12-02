using Plugins.Shared.Library;
using System;
using System.Activities;
using System.ComponentModel;
using System.Data;

namespace RPA.Integration.Activities.Database
{
    [Designer(typeof(ExecInsertDesigner))]
    public sealed class ExecInsert : AsyncCodeActivity
    {
        public ExecInsert()
        {
        }

        public new string DisplayName
        {
            get
            {
                return "Insert";
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
        [OverloadGroup("DBConnection")]
        [Description("用于访问数据库的数据库提供程序的名称")]
        [DisplayName("现有数据库连接")]
        public InArgument<DatabaseConnection> DBConnection { get; set; }


        [Category("输入")]
        [Description("插入表中的DataTable变量。DataTable列的名称和描述必须与数据库表中的名称和描述相匹配")]
        [DisplayName("DataTable")]
        public InArgument<DataTable> DataTable { get; set; }

        [Category("输入")]
        [RequiredArgument]
        [Description("要在其中插入数据的SQL表")]
        [DisplayName("表名")]
        public InArgument<string> TableName { get; set; }

        [Category("输出")]
        [DisplayName("执行结果")]
        [Description("将受影响的行数存储到Int32变量中")]
        public OutArgument<Int32> AffectedRecords { get; set; }


        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.Integration.Activities;Component/Resources/DataBase/insert.png";
            }
        }

        DatabaseConnection DbConn;

        [Browsable(false)]
        public string ClassName { get { return "ExecInsert"; } }
        private delegate string runDelegate();
        private runDelegate m_Delegate;
        public string Run()
        {
            return ClassName;
        }



        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            DataTable dataTable = null;
            string connString = null;
            string provName = null;
            string tableName = null;
            try
            {
                DbConn = DBConnection.Get(context);
                connString = ConnectionString.Get(context);
                provName = ProviderName.Get(context);
                tableName = TableName.Get(context);
                dataTable = DataTable.Get(context);

                Func<int> action = () =>
                {
                    DbConn = DbConn ?? new DatabaseConnection().Initialize(connString, provName);
                    if (DbConn == null)
                    {
                        return 0;
                    }
                    return DbConn.InsertDataTable(tableName, dataTable);
                };
                context.UserState = action;
                return action.BeginInvoke(callback, state);
            }
            catch (Exception ex)
            {
                HandleException(ex, ContinueOnError.Get(context));
            }
            //if(dataTable.Rows.Count == 0)
            //{
            //    SharedObject.Instance.Output(SharedObject.enOutputType.Error, "dataTable为空");
            //}
            m_Delegate = new runDelegate(Run);
            return m_Delegate.BeginInvoke(callback, state);
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
                Func<int> action = (Func<int>)context.UserState;
                int affectedRecords = action.EndInvoke(result);
                this.AffectedRecords.Set(context, affectedRecords);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "", e.Message);
            }
            finally
            {
                if (existingConnection == null)
                {
                    DbConn.Dispose();
                }
            }
        }
    }
}