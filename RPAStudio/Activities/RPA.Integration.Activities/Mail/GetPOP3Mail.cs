using System;
using System.Activities;
using System.ComponentModel;
using Plugins.Shared.Library;
using MailKit.Security;
using MimeKit;
using System.Collections.Generic;
using MailKit.Net.Pop3;
using System.Activities.Statements;

namespace RPA.Integration.Activities.Mail
{
    [Designer(typeof(GetPOP3MailDesigner))]
    public sealed class GetPOP3Mail : NativeActivity
    {
        public new string DisplayName
        {
            get
            {
                return "GetPOP3Mail";
            }
        }

        [Browsable(false)]
        public ActivityAction<object, object, object[]> Body { get; set; }


        public GetPOP3Mail()
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



        [Category("选项")]
        [DisplayName("安全连接")]
        [Description("指定用于连接的SSL和/或TLS加密")]
        public SecureSocketOptions SecureConnection { get; set; }
        [Category("选项")]
        [RequiredArgument]
        [DisplayName("检索消息数")]
        [Description("从列表顶部开始检索的消息数")]
        public InArgument<Int32> Counts { get; set; }
        [Category("选项")]
        [DisplayName("标记消息删除")]
        [Description(" 指定是否应将读取的消息标记为删除")]
        public bool DeleteMessages { get; set; }


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


        protected override void Execute(NativeActivityContext context)
        {
            string username = Email.Get(context);               //发送端账号   
            string password = Password.Get(context);            //发送端密码(这个客户端重置后的密码)
            string server = Server.Get(context);                //邮件服务器
            Int32 port = Port.Get(context);                     //端口号
            Int32 counts = Counts.Get(context);
            List<object> configList = new List<object>();
            Pop3Client emailClient = new Pop3Client();
            List<MimeMessage> emails = new List<MimeMessage>();


            string mailTopicKey = MailTopicKey.Get(context);
            string mailSenderKey = MailSenderKey.Get(context);
            string mailTextBodyKey = MailTextBodyKey.Get(context);

            try
            {
                emailClient.Connect(server, port, SecureConnection);
                emailClient.Authenticate(username, password);
                for (int i = emailClient.Count - 1, j = 0; i >= 0 && j < counts; i--, j++)
                {
                    MimeMessage message = emailClient.GetMessage(i);

                    InternetAddressList Sender = message.From;
                    string SenderStr = Sender[0].Name;
                    string Topic = message.Subject;


                    if (mailTopicKey != null && mailTopicKey != "")
                    {
                        if(Topic == null || Topic == "")
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
                    if (DeleteMessages)
                        emailClient.DeleteMessage(i);
                }
                MailMsgList.Set(context, emails);
                emailClient.Disconnect(true);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "获取POP3邮件失败", e.Message);
                emailClient.Disconnect(true);
            }


            configList.Add(server);
            configList.Add(port);
            configList.Add(SecureConnection);
            configList.Add(username);
            configList.Add(password);
            configList.Add("");

            if (Body != null)
            {
                object[] buff = configList.ToArray();
                context.ScheduleAction(Body, emails, emailClient, buff);
            }
        }
    }
}