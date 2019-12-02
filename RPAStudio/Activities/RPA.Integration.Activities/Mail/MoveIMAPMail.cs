using System;
using System.Activities;
using System.ComponentModel;
using Plugins.Shared.Library;
using System.Diagnostics;
using MailKit.Security;
using MailKit.Net.Smtp;
using MimeKit;
using System.Collections;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.IO;
using MimeKit.Text;
using MailKit.Net.Pop3;
using System.Net.Mail;
using MailKit.Net.Imap;
using MailKit;
using MailKit.Search;
using System.Linq;

namespace RPA.Integration.Activities.Mail
{
    [Designer(typeof(MoveIMAPMailDesigner))]
    public sealed class MoveIMAPMail : CodeActivity
    {
        public new string DisplayName
        {
            get
            {
                return "MoveIMAPMail";
            }
        }


        [Category("登录")]
        [RequiredArgument]
        [DisplayName("邮件账户")]
        [Description("用于发送邮件的电子邮件帐户")]
        public InArgument<string> Email { get; set; }
        [Category("登录")]
        [RequiredArgument]
        [DisplayName("密码")]
        [Description("用于发送邮件的电子邮件帐户的密码")]
        public InArgument<string> Password { get; set; }


        [Category("主机")]
        [RequiredArgument]
        [DisplayName("服务器")]
        [Description("使用的电子邮件服务器主机")]
        public InArgument<string> Server { get; set; }
        [Category("主机")]
        [DisplayName("端口")]
        [Description("电子邮件将通过的端口")]
        public InArgument<Int32> Port { get; set; }
        [Category("主机")]
        [DisplayName("SSL")]
        [Description("指定是否应使用SSL发送消息")]
        public bool EnableSSL { get; set; }


        [Category("选项")]
        [DisplayName("安全连接")]
        [Description("指定用于连接的SSL和/或TLS加密")]
        public SecureSocketOptions SecureConnection { get; set; }

        [Category("输入")]
        [RequiredArgument]
        [DisplayName("目标文件夹")]
        [Description("要将消息移动到的邮件文件夹")]
        public InArgument<string> MailFolderTo { get; set; }
        public InArgument<string> _MailFolderFrom = "INBOX";
        [Category("输入")]
        [RequiredArgument]
        [DisplayName("源文件夹")]
        [Description("可以找到邮件消息的文件夹")]
        public InArgument<string> MailFolderFrom
        {
            get
            {
                return _MailFolderFrom;
            }
            set
            {
                _MailFolderFrom = value;
            }
        }
        [Category("输入")]
        [DisplayName("邮件消息")]
        [Description("要移动的MailMessage对象")]
        public InArgument<MimeMessage> MailMoveMessage { get; set; }


        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.Integration.Activities;Component/Resources/Mail/GetMail.png";
            }
        }

        protected override void Execute(CodeActivityContext context)
        {
            string username = Email.Get(context);               //发送端账号   
            string password = Password.Get(context);            //发送端密码(这个客户端重置后的密码)
            string server = Server.Get(context);                //邮件服务器
            Int32 port = Port.Get(context);                     //端口号
            string mailFolderTo = MailFolderTo.Get(context);    //目标文件夹
            string mailFolderFrom = MailFolderFrom.Get(context);//源文件夹
            MimeMessage mailMoveMessage = MailMoveMessage.Get(context);

            ImapClient client = new ImapClient();
            SearchQuery query;
            try
            {
                client.Connect(server, port, SecureConnection);
                client.Authenticate(username, password);

                if (EnableSSL)
                    client.SslProtocols = System.Security.Authentication.SslProtocols.Ssl3;

                query = SearchQuery.All;
                List<IMailFolder> mailFolderList = client.GetFolders(client.PersonalNamespaces[0]).ToList();
                IMailFolder fromFolder = client.GetFolder(mailFolderFrom);
                IMailFolder toFolder = client.GetFolder(mailFolderTo);
                fromFolder.Open(FolderAccess.ReadWrite);
                IList<UniqueId> uidss = fromFolder.Search(query);
                List<MailMessage> emails = new List<MailMessage>();
                for (int i = uidss.Count - 1; i >= 0; i--)
                {
                    MimeMessage message = fromFolder.GetMessage(new UniqueId(uidss[i].Id));
                    if (message.Date == mailMoveMessage.Date &&
                        message.MessageId == mailMoveMessage.MessageId &&
                        message.Subject == mailMoveMessage.Subject)
                    {
                        fromFolder.MoveTo(new UniqueId(uidss[i].Id), toFolder);
                        break;
                    }
                }
                client.Disconnect(true);
            }
            catch (Exception e)
            {
                client.Disconnect(true);
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "IMAP移动邮件失败", e.Message);
            }
        }

        private void onComplete(NativeActivityContext context, ActivityInstance completedInstance)
        {
        }
    }
}