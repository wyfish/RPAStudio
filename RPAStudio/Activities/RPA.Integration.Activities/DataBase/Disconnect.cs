using System;
using System.Activities;
using System.ComponentModel;
using Plugins.Shared.Library;

namespace RPA.Integration.Activities.Database
{
    [Designer(typeof(DisconnectDesigner))]
    public sealed class Disconnect : AsyncCodeActivity
    {
        public new string DisplayName
        {
            get
            {
                return "Disconnect";
            }
        }

        [Category("断开连接")]
        [RequiredArgument]
        [Description("要关闭的数据库连接(DatabaseConnection变量)")]
        [DisplayName("数据库连接")]
        public InArgument<DatabaseConnection> DBConn { get; set; }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.Integration.Activities;Component/Resources/DataBase/disconnect.png";
            }
        }


        private void onComplete(NativeActivityContext context, ActivityInstance completedInstance)
        {
        }

        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            try
            {
                var dbConnection = DBConn.Get(context);
                Action action = () => dbConnection.Dispose();
                context.UserState = action;
                return action.BeginInvoke(callback, state);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "数据库断开连接失败", e.Message);
                throw e;
            }
        }

        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
            Action action = (Action)context.UserState;
            action.EndInvoke(result);
        }
    }
}