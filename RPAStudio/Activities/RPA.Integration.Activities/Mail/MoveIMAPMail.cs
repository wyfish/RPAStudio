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


        [Localize.LocalizedCategory("Category16")] //登录 //Log in //ログイン
        [RequiredArgument]
        [Localize.LocalizedDisplayName("DisplayName64")] //邮件账户 //Mail account //メールアカウント
        [Localize.LocalizedDescription("Description21")] //用于发送邮件的电子邮件帐户 //Email account used to send mail //メールの送信に使用されるメールアカウント
        public InArgument<string> Email { get; set; }
        [Localize.LocalizedCategory("Category16")] //登录 //Log in //ログイン
        [RequiredArgument]
        [Localize.LocalizedDisplayName("DisplayName57")] //密码 //password //パスワード
        [Localize.LocalizedDescription("Description22")] //用于发送邮件的电子邮件帐户的密码 //The password for the email account used to send the message //メッセージの送信に使用される電子メールアカウントのパスワード
        public InArgument<string> Password { get; set; }


        [Localize.LocalizedCategory("DisplayName54")] //主机 //Host //ホスト
        [RequiredArgument]
        [Localize.LocalizedDisplayName("Category13")] //服务器 //server //サーバー
        [Localize.LocalizedDescription("Description23")] //使用的电子邮件服务器主机 //Email server host used //使用されているメールサーバーホスト
        public InArgument<string> Server { get; set; }
        [Localize.LocalizedCategory("DisplayName54")] //主机 //Host //ホスト
        [Localize.LocalizedDisplayName("DisplayName55")] //端口 //port //港
        [Localize.LocalizedDescription("Description24")] //电子邮件将通过的端口 //The port that the email will pass through //電子メールが通過するポート
        public InArgument<Int32> Port { get; set; }
        [Localize.LocalizedCategory("DisplayName54")] //主机 //Host //ホスト
        [DisplayName("SSL")]
        [Localize.LocalizedDescription("Description39")] //指定是否应使用SSL发送消息 //Specifies whether messages should be sent using SSL //SSLを使用してメッセージを送信するかどうかを指定します
        public bool EnableSSL { get; set; }


        [Localize.LocalizedCategory("Category4")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName66")] //安全连接 //Secure connection //安全な接続
        [Localize.LocalizedDescription("Description26")] //指定用于连接的SSL和/或TLS加密 //Specify SSL and/or TLS encryption for the connection //接続にSSLおよび/またはTLS暗号化を指定する
        public SecureSocketOptions SecureConnection { get; set; }

        [Localize.LocalizedCategory("Category5")] //输入 //Enter //入力
        [RequiredArgument]
        [Localize.LocalizedDisplayName("DisplayName79")] //目标文件夹 //Target folder //対象フォルダ
        [Localize.LocalizedDescription("Description40")] //要将消息移动到的邮件文件夹 //The mail folder to which you want to move the message //メッセージの移動先のメールフォルダー
        public InArgument<string> MailFolderTo { get; set; }
        public InArgument<string> _MailFolderFrom = "INBOX";
        [Localize.LocalizedCategory("Category5")] //输入 //Enter //入力
        [RequiredArgument]
        [Localize.LocalizedDisplayName("DisplayName80")] //源文件夹 //Source folder //ソースフォルダー
        [Localize.LocalizedDescription("Description41")] //可以找到邮件消息的文件夹 //A folder where you can find mail messages //メールメッセージを検索できるフォルダー
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
        [Localize.LocalizedCategory("Category5")] //输入 //Enter //入力
        [Localize.LocalizedDisplayName("DisplayName81")] //邮件消息 //Mail message //メールメッセージ
        [Localize.LocalizedDescription("Description42")] //要移动的MailMessage对象 //MailMessage object to move //移動するMailMessageオブジェクト
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
