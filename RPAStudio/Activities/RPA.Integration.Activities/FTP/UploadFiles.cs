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
        [Localize.LocalizedCategory("Category5")] //输入 //Enter //入力
        [Localize.LocalizedDisplayName("DisplayName47")] //本地路径 //Local path //ローカルパス
        public InArgument<string> LocalPath { get; set; }

        [RequiredArgument]
        [Localize.LocalizedCategory("Category5")] //输入 //Enter //入力
        [Localize.LocalizedDisplayName("DisplayName52")] //FTP路径 //FTP path //FTPパス
        public InArgument<string> RemotePath { get; set; }


        [Localize.LocalizedCategory("Category4")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName49")] //包含子文件夹 //Include subfolders //サブフォルダーを含める
        public bool Recursive { get; set; }

        [Localize.LocalizedCategory("Category4")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName53")] //创建 //create //作成する
        public bool Create { get; set; }

        [Localize.LocalizedCategory("Category4")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName50")] //覆盖 //cover //カバー
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
