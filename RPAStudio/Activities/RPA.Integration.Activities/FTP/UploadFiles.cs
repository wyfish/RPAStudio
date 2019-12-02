using System;
using System.Activities;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Plugins.Shared.Library;


namespace RPA.Integration.Activities.FTP
{
    [Designer(typeof(FTPActDesigner))]
    public class UploadFiles : ContinuableAsyncCodeActivity
    {
        [RequiredArgument]
        [Category("输入")]
        [DisplayName("本地路径")]
        public InArgument<string> LocalPath { get; set; }

        [RequiredArgument]
        [Category("输入")]
        [DisplayName("FTP路径")]
        public InArgument<string> RemotePath { get; set; }


        [Category("选项")]
        [DisplayName("包含子文件夹")]
        public bool Recursive { get; set; }

        [Category("选项")]
        [DisplayName("创建")]
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

            string localPath = LocalPath.Get(context);
            string remotePath = RemotePath.Get(context);

            if (Directory.Exists(localPath))
            {
                if (string.IsNullOrWhiteSpace(Path.GetExtension(remotePath)))
                {
                    if (!(await ftpSession.DirectoryExistsAsync(remotePath, cancellationToken)))
                    {
                        if (Create)
                        {
                            await ftpSession.CreateDirectoryAsync(remotePath, cancellationToken);
                        }
                        else
                        {
                            throw new ArgumentException("PathNotFoundException", remotePath);
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
                if (File.Exists(localPath))
                {
                    if (string.IsNullOrWhiteSpace(Path.GetExtension(remotePath)))
                    {
                        remotePath = FtpConfiguration.CombinePaths(remotePath, Path.GetFileName(localPath));
                    }

                    string directoryPath = FtpConfiguration.GetDirectoryPath(remotePath);

                    if (!(await ftpSession.DirectoryExistsAsync(directoryPath, cancellationToken)))
                    {
                        if (Create)
                        {
                            await ftpSession.CreateDirectoryAsync(directoryPath, cancellationToken);
                        }
                        else
                        {
                            throw new InvalidOperationException(directoryPath);
                        }
                    }
                }
                else
                {
                    throw new ArgumentException("PathNotFoundException", localPath);
                }
            }

            await ftpSession.UploadAsync(localPath, remotePath, Overwrite, Recursive, cancellationToken);

            return (asyncCodeActivityContext) =>
            {

            };
        }
    }
}
