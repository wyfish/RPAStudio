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
using System.Activities.Statements;

namespace RPA.Integration.Activities.Mail
{
    [Designer(typeof(GetIMAPMailDesigner))]
    public sealed class GetIMAPMail : NativeActivity
    {
        public new string DisplayName
        {
            get
            {
                return "GetIMAPMail";
            }
        }



        [Browsable(false)]
        public ActivityAction<object, object, object[]> Body { get; set; }


        public GetIMAPMail()
        {
            Body = new ActivityAction<object, object, object[]>
            {
                Argument1 = new DelegateInArgument<object>(GetMailList),
                Argument2 = new DelegateInArgument<object>(GetClient),
                Argument3 = new DelegateInArgument<object[]>(GetConfig),
                Handler = new Sequence()
                {
                    DisplayName = "Do"
                }
            };
        }


        public static string GetMailList { get { return "GetMail"; } }
        public static string GetClient { get { return "GetClient"; } }
        public static string GetConfig { get { return "GetConfig"; } }


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
        public InArgument<string> _MainFolder = "INBOX";
        [Category("主机")]
        [DisplayName("文件夹")]
        [Description("检索消息的邮件文件夹")]
        public InArgument<string> MainFolder
        {
            get
            {
                return _MainFolder;
            }
            set
            {
                _MainFolder = value;
            }
        }


        [Category("选项")]
        [DisplayName("安全连接")]
        [Description("指定用于连接的SSL和/或TLS加密")]
        public SecureSocketOptions SecureConnection { get; set; }
        [Category("选项")]
        [OverloadGroup("MsgCounts")]
        [DisplayName("检索消息数")]
        [Description("从列表顶部开始检索的消息数")]
        public InArgument<Int32> Counts { get; set; }
        [Category("选项")]
        [DisplayName("标记消息删除")]
        [Description(" 指定是否应将读取的消息标记为删除")]
        public bool DeleteMessages { get; set; }
        [Category("选项")]
        [DisplayName("标为已读")]
        [Description("指定是否将检索到的消息标记为已读")]
        public bool MarkAsRead { get; set; }
        [Category("选项")]
        [DisplayName("仅检索未读消息")]
        [Description("指定是否仅检索未读消息")]
        public bool OnlyUnreadMessages { get; set; }

        public bool _AllUnreadMessages;
        [Category("选项")]
        [DisplayName("检索所有未读消息")]
        public bool AllUnreadMessages
        {
            get
            {
                return _AllUnreadMessages;
            }
            set
            {
                _AllUnreadMessages = value;
            }
        }

        [Category("筛选")]
        [DisplayName("主题关键字")]
        [Description("根据邮件主题筛选相应的邮件")]
        public InArgument<String> MailTopicKey { get; set; }


        [Category("筛选")]
        [DisplayName("发件人关键字")]
        [Description("根据发件人地址筛选相应的邮件")]
        public InArgument<String> MailSenderKey { get; set; }

        [Category("筛选")]
        [DisplayName("内容关键字")]
        [Description("根据邮件超文本内容筛选相应的邮件")]
        public InArgument<String> MailTextBodyKey { get; set; }



        [Category("输出")]
        [DisplayName("消息")]
        [Description("将检索到的消息作为MailMessage对象的集合")]
        public OutArgument<List<MimeMessage>> MailMsgList { get; set; }


        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.Integration.Activities;Component/Resources/Mail/GetMail.png";
            }
        }

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);
            if ( (_AllUnreadMessages == false) && (Counts==null))
            {
                metadata.AddValidationError("检索消息数不能为空！");
            }
        }

        private void onComplete(NativeActivityContext context, ActivityInstance completedInstance)
        {
        }

        protected override void Execute(NativeActivityContext context)
        {
            string username = Email.Get(context);               //发送端账号   
            string password = Password.Get(context);            //发送端密码(这个客户端重置后的密码)
            string server = Server.Get(context);                //邮件服务器
            Int32 port = Port.Get(context);                     //端口号
            Int32 counts = Counts.Get(context);                 //检索邮件数
            string mainFolder = MainFolder.Get(context);        //邮件文件夹

            string mailTopicKey = MailTopicKey.Get(context);
            string mailSenderKey = MailSenderKey.Get(context);
            string mailTextBodyKey = MailTextBodyKey.Get(context);

            List<object> configList = new List<object>();
            List<MimeMessage> emails = new List<MimeMessage>();
            ImapClient client = new ImapClient();
            SearchQuery query;
            IList<UniqueId> uidss;
            try
            {
                client.CheckCertificateRevocation = false;
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                client.Connect(server, port, SecureConnection);
                client.Authenticate(username, password);

                if (OnlyUnreadMessages || _AllUnreadMessages)
                    query = SearchQuery.NotSeen;
                else
                    query = SearchQuery.All;

                List<IMailFolder> mailFolderList = client.GetFolders(client.PersonalNamespaces[0]).ToList();
                IMailFolder folder = client.GetFolder(mainFolder);
                folder.Open(FolderAccess.ReadWrite);
                emails = new List<MimeMessage>();
                if(_AllUnreadMessages)
                {
                    query = SearchQuery.NotSeen;
                    uidss = folder.Search(query);
                    for (int i = uidss.Count - 1; i >= 0; i--)
                    {
                        MimeMessage message = folder.GetMessage(new UniqueId(uidss[i].Id));
                        emails.Add(message);
                        if (MarkAsRead)
                            folder.AddFlags(new UniqueId(uidss[i].Id), MessageFlags.Seen, true);//如果设置为true，则不会发出MessageFlagsChanged事件
                        if (DeleteMessages)
                            folder.AddFlags(new UniqueId(uidss[i].Id), MessageFlags.Deleted, true);//如果设置为true，则不会发出MessageFlagsChanged事件
                    }
                }
                else
                {
                    uidss = folder.Search(query);
                    for (int i = uidss.Count - 1, j = 0; i >= 0 && j < counts; i--, j++)
                    {
                        MimeMessage message = folder.GetMessage(new UniqueId(uidss[i].Id));

                        InternetAddressList Sender = message.From;
                        string SenderStr = Sender.Mailboxes.First().Address;
                        string Topic = message.Subject;
                        if(mailTopicKey != null && mailTopicKey != "")
                        {
                            if (Topic == null || Topic == "")
                            {
                                j--;
                                continue;
                            }
                            if (!Topic.Contains(mailTopicKey))
                            {
                                j--;
                                continue;
                            }
                        }
                        if (mailSenderKey != null && mailSenderKey != "")
                        {
                            if (SenderStr == null || SenderStr == "")
                            {
                                j--;
                                continue;
                            }
                            if (!SenderStr.Contains(mailSenderKey))
                            {
                                j--;
                                continue;
                            }
                        }
                        if (mailTextBodyKey != null && mailTextBodyKey != "")
                        {
                            if (message.TextBody == null || message.TextBody == "")
                            {
                                j--;
                                continue;
                            }
                            if (!message.TextBody.Contains(mailTextBodyKey))
                            {
                                j--;
                                continue;
                            }
                        }

                        emails.Add(message);
                        if (MarkAsRead)
                            folder.AddFlags(new UniqueId(uidss[i].Id), MessageFlags.Seen, true);//如果设置为true，则不会发出MessageFlagsChanged事件
                        if (DeleteMessages)
                            folder.AddFlags(new UniqueId(uidss[i].Id), MessageFlags.Deleted, true);//如果设置为true，则不会发出MessageFlagsChanged事件
                    }
                }
           
                //获取搜索结果的摘要信息（我们需要UID和BODYSTRUCTURE每条消息，以便我们可以提取文本正文和附件）(获取全部邮件)
                //var items = folder.Fetch(uidss, MessageSummaryItems.UniqueId | MessageSummaryItems.BodyStructure);
                MailMsgList.Set(context, emails);
                client.Disconnect(true);

                configList.Add(server);
                configList.Add(port);
                configList.Add(SecureConnection);
                configList.Add(username);
                configList.Add(password);
                configList.Add(mainFolder);
            }
            catch (Exception e)
            {
                client.Disconnect(true);
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "获取IMAP邮件失败", e.Message);
            }
            if (Body != null)
            {
                object[] buff = configList.ToArray();
                context.ScheduleAction(Body, emails, client, buff);
            }
        }
    }
}