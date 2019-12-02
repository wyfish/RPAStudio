using FluentFTP;
using Plugins.Shared.Library;
using System;
using System.Activities;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace RPA.Integration.Activities.FTP
{
    [Designer(typeof(FTPActDesigner))]
    public class DownloadFiles : ContinuableAsyncCodeActivity
    {
        [RequiredArgument]
        [Category("输入")]
        [DisplayName("路径")]
        public InArgument<string> RemotePath { get; set; }


        [RequiredArgument]
        [Category("输入")]
        [DisplayName("本地路径")]
        public InArgument<string> LocalPath { get; set; }

        [Category("选项")]
        [DisplayName("创建文件夹")]
        public bool Recursive { get; set; }

        [Category("选项")]
        [DisplayName("包含子文件夹")]
        public bool Create { get; set; }

        [Category("选项")]
        [DisplayName("覆盖")]
        public bool Overwrite { get; set; }

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

            string remotePath = RemotePath.Get(context);
            string localPath = LocalPath.Get(context);

            FtpObjectType objectType = await ftpSession.GetObjectTypeAsync(remotePath, cancellationToken);
            if (objectType == FtpObjectType.Directory)
            {
                if (string.IsNullOrWhiteSpace(Path.GetExtension(localPath)))
                {
                    if (!Directory.Exists(localPath))
                    {
                        if (Create)
                        {
                            Directory.CreateDirectory(localPath);
                        }
                        else
                        {
                            throw new ArgumentException("PathNotFoundException", localPath);
                        }
                    }
                }
                else
                {
                    throw new InvalidOperationException("IncompatiblePathsException");
                }
            }
            else
            {
                if (objectType == FtpObjectType.File)
                {
                    if (string.IsNullOrWhiteSpace(Path.GetExtension(localPath)))
                    {
                        localPath = Path.Combine(localPath, Path.GetFileName(remotePath));
                    }

                    string directoryPath = Path.GetDirectoryName(localPath);

                    if (!Directory.Exists(directoryPath))
                    {
                        if (Create)
                        {
                            Directory.CreateDirectory(directoryPath);
                        }
                        else
                        {
                            throw new InvalidOperationException("PathNotFoundException");
                        }
                    }
                }
                else
                {
                    throw new NotImplementedException("UnsupportedObjectTypeException");
                }
            }

            if(Overwrite) 
                await ftpSession.DownloadAsync(remotePath, localPath, FtpLocalExists.Overwrite, Recursive, cancellationToken);
            else
                await ftpSession.DownloadAsync(remotePath, localPath, FtpLocalExists.Append, Recursive, cancellationToken);

            return (asyncCodeActivityContext) =>
            {
                
            };
        }
    }
}
