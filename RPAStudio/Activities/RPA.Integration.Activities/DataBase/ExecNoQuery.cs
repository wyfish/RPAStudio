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
        [OverloadGroup("DatabaseConnection")]
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
        [Localize.LocalizedDescription("Description12")] //对于UPDATE，INSERT和DELETE语句，返回值是受命令影响的行数。对于所有其他类型的语句，返回值为-1 //For UPDATE, INSERT, and DELETE statements, the return value is the number of rows affected by the command.  For all other types of statements, the return value is -1 //UPDATE、INSERT、およびDELETEステートメントの場合、戻り値はコマンドの影響を受ける行の数です。 他のすべてのタイプのステートメントの場合、戻り値は-1です
        [Localize.LocalizedDisplayName("DisplayName8")] //执行结果 //Results of the //実行結果
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
