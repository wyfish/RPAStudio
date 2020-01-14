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
        [Localize.LocalizedCategory("Category4")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName4")] //超时时间 //overtime time //タイムアウト
        [Localize.LocalizedDescription("Description5")] //指定浏览器响应超时时间(毫秒) //Specify browser response timeout (ms) //ブラウザーの応答タイムアウト（ミリ秒）を指定する
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

        [Localize.LocalizedCategory("Category4")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName5")] //错误继续执行 //Error continues to execute //エラーは引き続き実行されます
        [Localize.LocalizedDescription("Description6")] //出现错误是否继续执行 //Whether an error occurs or not //エラーが発生するかどうか
        public InArgument<bool> ContinueOnError { get; set; }


        [Localize.LocalizedCategory("Category1")] //连接配置 //Connection configuration //接続構成
        [RequiredArgument]
        [OverloadGroup("ConnectionConfig")]
        [Localize.LocalizedDescription("Description1")] //用于建立数据库连接的连接字符串 //The connection string used to establish a database connection //データベース接続の確立に使用される接続文字列
        [Localize.LocalizedDisplayName("DisplayName1")] //连接字符串 //Connection string //接続文字列
        public InArgument<string> ConnectionString { get; set; }


        [Localize.LocalizedCategory("Category1")] //连接配置 //Connection configuration //接続構成
        [RequiredArgument]
        [OverloadGroup("ConnectionConfig")]
        [Localize.LocalizedDescription("Description2")] //用于访问数据库的数据库提供程序的名称 //The name of the database provider used to access the database //データベースへのアクセスに使用されるデータベースプロバイダーの名前
        [Localize.LocalizedDisplayName("DisplayName2")] //程序名称 //Program name //プログラム名
        public InArgument<string> ProviderName { get; set; }

        [Localize.LocalizedCategory("Category1")] //连接配置 //Connection configuration //接続構成
        [RequiredArgument]
        [OverloadGroup("DBConnection")]
        [Localize.LocalizedDescription("Description2")] //用于访问数据库的数据库提供程序的名称 //The name of the database provider used to access the database //データベースへのアクセスに使用されるデータベースプロバイダーの名前
        [Localize.LocalizedDisplayName("DisplayName6")] //现有数据库连接 //Existing database connection //既存のデータベース接続
        public InArgument<DatabaseConnection> DBConnection { get; set; }


        [Localize.LocalizedCategory("Category5")] //输入 //Enter //入力
        [Localize.LocalizedDescription("Description7")] //插入表中的DataTable变量。DataTable列的名称和描述必须与数据库表中的名称和描述相匹配 //Insert the DataTable variable in the table.  The name and description of the DataTable column must match the name and description in the database table //テーブルにDataTable変数を挿入します。  DataTable列の名前と説明は、データベーステーブルの名前と説明と一致する必要があります
        [DisplayName("DataTable")]
        public InArgument<DataTable> DataTable { get; set; }

        [Localize.LocalizedCategory("Category5")] //输入 //Enter //入力
        [RequiredArgument]
        [Localize.LocalizedDescription("Description8")] //要在其中插入数据的SQL表 //The SQL table in which to insert data //データを挿入するSQLテーブル
        [Localize.LocalizedDisplayName("DisplayName7")] //表名 //Table Name //テーブル名
        public InArgument<string> TableName { get; set; }

        [Localize.LocalizedCategory("Category2")] //输出 //Output //出力
        [Localize.LocalizedDisplayName("DisplayName8")] //执行结果 //Results of the //実行結果
        [Localize.LocalizedDescription("Description9")] //将受影响的行数存储到Int32变量中 //Store the number of affected rows in the Int32 variable //影響を受ける行の数をInt32変数に保存します
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
