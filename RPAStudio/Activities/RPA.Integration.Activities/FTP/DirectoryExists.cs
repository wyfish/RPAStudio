using Plugins.Shared.Library;
using System;
using System.Activities;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace RPA.Integration.Activities.FTP
{
    [Designer(typeof(FTPActDesigner))]
    public class DirectoryExists : ContinuableAsyncCodeActivity
    {
        [RequiredArgument]
        [Category("输入")]
        [DisplayName("路径")]
        public InArgument<string> RemotePath { get; set; }


        [RequiredArgument]
        [Category("输出")]
        [DisplayName("是否存在")]
        public OutArgument<bool> Exists { get; set; }

        [Browsable(false)]
        public string icoPath
        {
            get { return @"pack://application:,,,/RPA.Integration.Activities;Component/Resources/FTP/FTP.png"; }
        }


        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            PropertyDescriptor ftpSessionProperty = context.DataContext.GetProperties()[WithFtpSession.FtpSessionPropertyName];
            IFtpSession ftpSession = ftpSessionProperty?.GetValue(context.DataContext) as IFtpSession;

            if (ftpSession == null)
            {
                throw new InvalidOperationException("FTPSessionNotFoundException");
            }

            bool exists = await ftpSession.DirectoryExistsAsync(RemotePath.Get(context), cancellationToken);

            return (asyncCodeActivityContext) =>
            {
                Exists.Set(asyncCodeActivityContext, exists);
            };
        }
    }
}
