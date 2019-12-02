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
        [Category("服务器")]
        [DisplayName("主机")]
        public InArgument<string> Host { get; set; }

        [Category("服务器")]
        [DisplayName("端口")]
        public InArgument<int> Port { get; set; }

        [Category("凭据")]
        [DisplayName("用户名")]
        public InArgument<string> Username { get; set; }

        [Category("凭据")]
        [DisplayName("密码")]
        public InArgument<string> Password { get; set; }

        [DefaultValue(false)]
        [Category("凭据")]
        [DisplayName("匿名登录")]
        public bool UseAnonymousLogin { get; set; }

        [DefaultValue(FtpsMode.None)]
        [Category("安全设置")]
        [DisplayName("FTPS模式")]
        public FtpsMode FtpsMode { get; set; }

        [DefaultValue(false)]
        [Category("安全设置")]
        [DisplayName("使用SFTP")]
        public bool UseSftp { get; set; }

        [Category("安全设置")]
        [DisplayName("客户端证书路径")]
        public InArgument<string> ClientCertificatePath { get; set; }

        [Category("安全设置")]
        [DisplayName("客户端证书密码")]
        public InArgument<string> ClientCertificatePassword { get; set; }

        [DefaultValue(false)]
        [Category("安全设置")]
        [DisplayName("接受所有证书")]
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
