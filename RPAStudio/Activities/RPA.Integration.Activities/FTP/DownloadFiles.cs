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
        [Localize.LocalizedCategory("Category5")] //输入 //Enter //入力
        [Localize.LocalizedDisplayName("DisplayName45")] //路径 //path //パス
        public InArgument<string> RemotePath { get; set; }


        [RequiredArgument]
        [Localize.LocalizedCategory("Category5")] //输入 //Enter //入力
        [Localize.LocalizedDisplayName("DisplayName47")] //本地路径 //Local path //ローカルパス
        public InArgument<string> LocalPath { get; set; }

        [Localize.LocalizedCategory("Category4")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName48")] //创建文件夹 //Create folder //フォルダーを作成
        public bool Recursive { get; set; }

        [Localize.LocalizedCategory("Category4")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName49")] //包含子文件夹 //Include subfolders //サブフォルダーを含める
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
