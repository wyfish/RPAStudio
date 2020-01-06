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

        CommandType _CommandType = CommandType.Text;
        [Localize.LocalizedCategory("Category4")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName9")] //命令类型 //Command type //コマンドの種類
        [Localize.LocalizedDescription("Description10")] //指定如何解释命令字符串 //Specify how to interpret the command string //コマンド文字列の解釈方法を指定する
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


        [Localize.LocalizedCategory("Category5")] //输入 //Enter //入力
        [RequiredArgument]
        [Localize.LocalizedDescription("Description11")] //执行的SQL命令语句 //Executed SQL command statement //実行されたSQLコマンドステートメント
        [DisplayName("SQL")]
        public InArgument<string> SQLString { get; set; }

        private Dictionary<string, Argument> parameters;
        [Localize.LocalizedCategory("Category5")] //输入 //Enter //入力
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName10")] //参数 //parameter //パラメータ
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

        [Localize.LocalizedCategory("Category2")] //输出 //Output //出力
        [DisplayName("DataTable")]
        [Localize.LocalizedDescription("Description13")] //将SQL查询的输出存储在DataTable变量中 //Store the output of the SQL query in the DataTable variable //SQLクエリの出力をDataTable変数に保存します
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
