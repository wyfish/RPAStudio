using Plugins.Shared.Library;
using System;
using System.Activities;
using System.Activities.Statements;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace RPA.Integration.Activities.FTP
{
    [Designer(typeof(SessionDesigner))]
    public class WithFtpSession : ContinuableAsyncNativeActivity
    {
        private IFtpSession _ftpSession;

        public static readonly string FtpSessionPropertyName = "FtpSession";

        [Browsable(false)]
        public ActivityAction<IFtpSession> Body { get; set; }

        [RequiredArgument]
        [Localize.LocalizedCategory("Category13")] //服务器 //server //サーバー
        [Localize.LocalizedDisplayName("DisplayName54")] //主机 //Host //ホスト
        public InArgument<string> Host { get; set; }

        [Localize.LocalizedCategory("Category13")] //服务器 //server //サーバー
        [Localize.LocalizedDisplayName("DisplayName55")] //端口 //port //港
        public InArgument<int> Port { get; set; }

        [Localize.LocalizedCategory("Category14")] //凭据 //Credential //クレデンシャル
        [Localize.LocalizedDisplayName("DisplayName56")] //用户名 //username //ユーザー名
        public InArgument<string> Username { get; set; }

        [Localize.LocalizedCategory("Category14")] //凭据 //Credential //クレデンシャル
        [Localize.LocalizedDisplayName("DisplayName57")] //密码 //password //パスワード
        public InArgument<string> Password { get; set; }

        [DefaultValue(false)]
        [Localize.LocalizedCategory("Category14")] //凭据 //Credential //クレデンシャル
        [Localize.LocalizedDisplayName("DisplayName58")] //匿名登录 //Anonymous login //匿名ログイン
        public bool UseAnonymousLogin { get; set; }

        [DefaultValue(FtpsMode.None)]
        [Localize.LocalizedCategory("Category15")] //安全设置 //Security Settings //セキュリティ設定
        [Localize.LocalizedDisplayName("DisplayName59")] //FTPS模式 //FTPS mode //FTPSモード
        public FtpsMode FtpsMode { get; set; }

        [DefaultValue(false)]
        [Localize.LocalizedCategory("Category15")] //安全设置 //Security Settings //セキュリティ設定
        [Localize.LocalizedDisplayName("DisplayName60")] //使用SFTP //Use SFTP //SFTPを使用する
        public bool UseSftp { get; set; }

        [Localize.LocalizedCategory("Category15")] //安全设置 //Security Settings //セキュリティ設定
        [Localize.LocalizedDisplayName("DisplayName61")] //客户端证书路径 //Client certificate path //クライアント証明書のパス
        public InArgument<string> ClientCertificatePath { get; set; }

        [Localize.LocalizedCategory("Category15")] //安全设置 //Security Settings //セキュリティ設定
        [Localize.LocalizedDisplayName("DisplayName62")] //客户端证书密码 //Client certificate password //クライアント証明書のパスワード
        public InArgument<string> ClientCertificatePassword { get; set; }

        [DefaultValue(false)]
        [Localize.LocalizedCategory("Category15")] //安全设置 //Security Settings //セキュリティ設定
        [Localize.LocalizedDisplayName("DisplayName63")] //接受所有证书 //Accept all certificates //すべての証明書を受け入れます
        public bool AcceptAllCertificates { get; set; }


        [Browsable(false)]
        public string icoPath
        {
            get { return @"pack://application:,,,/RPA.Integration.Activities;Component/Resources/FTP/FTP.png"; }
        }

        public WithFtpSession()
        {
            FtpsMode = FtpsMode.None;
            Body = new ActivityAction<IFtpSession>()
            {
                Argument = new DelegateInArgument<IFtpSession>(FtpSessionPropertyName),
                Handler = new Sequence() { DisplayName = "Do" }
            };
        }

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            // TODO: Validation code here.

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<NativeActivityContext>> ExecuteAsync(NativeActivityContext context, CancellationToken cancellationToken)
        {
            IFtpSession ftpSession = null;
            FtpConfiguration ftpConfiguration = new FtpConfiguration(Host.Get(context));
            ftpConfiguration.Port = Port.Expression == null ? null : (int?)Port.Get(context);
            ftpConfiguration.UseAnonymousLogin = UseAnonymousLogin;
            ftpConfiguration.ClientCertificatePath = ClientCertificatePath.Get(context);
            ftpConfiguration.ClientCertificatePassword = ClientCertificatePassword.Get(context);
            ftpConfiguration.AcceptAllCertificates = AcceptAllCertificates;

            if (ftpConfiguration.UseAnonymousLogin == false)
            {
                ftpConfiguration.Username = Username.Get(context);
                ftpConfiguration.Password = Password.Get(context);

                if (string.IsNullOrWhiteSpace(ftpConfiguration.Username))
                {
                    throw new ArgumentNullException("EmptyUsernameException");
                }
            }

            if (UseSftp)
            {
                ftpSession = new SftpSession(ftpConfiguration);
            }
            else
            {
                ftpSession = new FtpSession(ftpConfiguration, FtpsMode);
            }

            await ftpSession.OpenAsync(cancellationToken);

            return (nativeActivityContext) =>
            {
                if (Body != null)
                {
                    _ftpSession = ftpSession;
                    nativeActivityContext.ScheduleAction(Body, ftpSession, OnCompleted, OnFaulted);
                }
            };
        }

        private void OnCompleted(NativeActivityContext context, ActivityInstance completedInstance)
        {
            if (_ftpSession == null)
            {
                throw new InvalidOperationException("FTPSessionNotFoundException");
            }

            _ftpSession.Close();
            _ftpSession.Dispose();
        }

        private void OnFaulted(NativeActivityFaultContext faultContext, Exception propagatedException, ActivityInstance propagatedFrom)
        {
            PropertyDescriptor ftpSessionProperty = faultContext.DataContext.GetProperties()[WithFtpSession.FtpSessionPropertyName];
            IFtpSession ftpSession = ftpSessionProperty?.GetValue(faultContext.DataContext) as IFtpSession;

            ftpSession?.Close();
            ftpSession?.Dispose();
        }
    }
}
