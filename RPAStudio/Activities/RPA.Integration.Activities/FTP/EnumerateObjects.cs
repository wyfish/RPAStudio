using Plugins.Shared.Library;
using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace RPA.Integration.Activities.FTP
{
    [Designer(typeof(FTPActDesigner))]
    public class EnumerateObjects : ContinuableAsyncCodeActivity
    {
        [RequiredArgument]
        [Category("输入")]
        [DisplayName("路径")]
        public InArgument<string> RemotePath { get; set; }

        [Category("选项")]
        [DisplayName("包含子文件夹")]
        public bool Recursive { get; set; }

        [Category("输出")]
        [DisplayName("枚举文件")]
        public OutArgument<IEnumerable<FtpObjectInfo>> Files { get; set; }


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

            IEnumerable<FtpObjectInfo> files = await ftpSession.EnumerateObjectsAsync(RemotePath.Get(context), Recursive, cancellationToken);

            foreach(FtpObjectInfo file in files)
            {
                Debug.WriteLine("FtpObjectInfo ---------------- : " + file.FullName);
            }

            return (asyncCodeActivityContext) =>
            {
                Files.Set(asyncCodeActivityContext, files);
            };
        }
    }
}
