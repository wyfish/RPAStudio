using System;
using System.Activities;
using System.ComponentModel;
using Plugins.Shared.Library;
using System.Diagnostics;
using MailKit.Security;
using MimeKit;
using System.Collections.Generic;
using System.IO;
using MimeKit.Text;
using System.Activities.Presentation.Metadata;
using Plugins.Shared.Library.Editors;
using Microsoft.Windows.Design.PropertyEditing;
using Plugins.Shared.Library.Extensions;

namespace RPA.Integration.Activities.Mail
{
    [Designer(typeof(SendMailDesigner))]
    public sealed class SendMail : CodeActivity
    {
        public new string DisplayName
        {
            get
            {
                return "SendMail";
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


        [Localize.LocalizedCategory("Category18")] //寄件人 //Sender //送信者
        [Localize.LocalizedDisplayName("DisplayName83")] //名称 //Name //お名前
        [Localize.LocalizedDescription("Description48")] //发件人的显示名称 //Sender's display name //送信者の表示名
        public InArgument<string> Name { get; set; }
        [Localize.LocalizedCategory("Category18")] //寄件人 //Sender //送信者
        [Localize.LocalizedDisplayName("DisplayName84")] //发件人 //Sender //送信者
        [Localize.LocalizedDescription("Description49")] //发件人的电子邮件地址 //Sender's email address //送信者のメールアドレス
        public InArgument<string> From { get; set; }


        [Localize.LocalizedCategory("Category19")] //收件人 //Recipient //受取人
        [RequiredArgument]
        [Localize.LocalizedDisplayName("Category19")] //收件人 //Recipient //受取人
        [Localize.LocalizedDescription("Description50")] //电子邮件的主要收件人 //Primary recipient of the email //電子メールの主要な受信者
        public InArgument<string[]> Receivers_To { get; set; }
        [Localize.LocalizedCategory("Category19")] //收件人 //Recipient //受取人
        [Localize.LocalizedDisplayName("DisplayName85")] //抄送 //Cc //Cc
        [Localize.LocalizedDescription("Description51")] //电子邮件的次要收件人 //Secondary recipient of the email //メールの二次受信者
        public InArgument<string[]> Receivers_Cc { get; set; }
        [Localize.LocalizedCategory("Category19")] //收件人 //Recipient //受取人
        [Localize.LocalizedDisplayName("DisplayName86")] //密件抄送 //BCC //Bcc
        [Localize.LocalizedDescription("Description52")] //电子邮件的隐藏收件人 //Hidden recipient of email //メールの隠された受信者
        public InArgument<string[]> Receivers_Bcc { get; set; }


        [Localize.LocalizedCategory("Category20")] //电子邮件 //e-mail //メール
        [Localize.LocalizedDisplayName("DisplayName87")] //主题 //Theme //テーマ
        [Localize.LocalizedDescription("Description53")] //电子邮件的主题 //Email subject //メールの件名
        public InArgument<string> MailTopic { get; set; }
        [Localize.LocalizedCategory("Category20")] //电子邮件 //e-mail //メール
        [Localize.LocalizedDisplayName("DisplayName88")] //正文 //Text //本体
        [Localize.LocalizedDescription("Description54")] //电子邮件的正文 //The body of the email //メールの本文
        public InArgument<string> MailBody { get; set; }


        [Localize.LocalizedCategory("Category4")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName66")] //安全连接 //Secure connection //安全な接続
        [Localize.LocalizedDescription("Description26")] //指定用于连接的SSL和/或TLS加密 //Specify SSL and/or TLS encryption for the connection //接続にSSLおよび/またはTLS暗号化を指定する
        public SecureSocketOptions SecureConnection { get; set; }
        [Localize.LocalizedCategory("Category4")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName89")] //优先级 //Priority //優先順位
        [Localize.LocalizedDescription("Description55")] //邮件的优先级标志 //Mail priority flag //メール優先フラグ
        public MessagePriority msgProperty { get; set; }
        public bool _IsBodyHtml = true;
        [Localize.LocalizedCategory("Category4")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName90")] //HTML格式 //HTML format //HTML形式
        [Localize.LocalizedDescription("Description56")] // 指定邮件正文是否以HTML格式编写 //Specify whether the body of the message is written in HTML format //メッセージの本文をHTML形式で記述するかどうかを指定します
        public bool IsBodyHtml
        {
            get
            {
                return _IsBodyHtml;
            }
            set
            {
                _IsBodyHtml = value;
            }
        }

        [Localize.LocalizedCategory("Category21")] //附件 //Attachments //添付ファイル
        [Localize.LocalizedDisplayName("DisplayName91")] //文件 //File //ファイル
        public List<InArgument<string>> Files
        {
            get;
            set;
        } = new List<InArgument<string>>();

        [Browsable(false)]
        [DefaultValue(null)]
        [Localize.LocalizedCategory("Category21")] //附件 //Attachments //添付ファイル
        public List<string> Attachments
        {
            get;
            set;
        }

        [Localize.LocalizedCategory("Category21")] //附件 //Attachments //添付ファイル
        [DefaultValue(null)]
        [DisplayName("附件列表")]
        public InArgument<IEnumerable<string>> AttachmentsCollection
        {
            get;
            set;
        }

        //[Localize.LocalizedCategory("Category21")] //附件 //Attachments //添付ファイル
        //[Localize.LocalizedDisplayName("DisplayName91")] //文件 //File //ファイル
        //[Localize.LocalizedDescription("Description57")] //要添加到电子邮件中的附件 //Attachments to add to the email //メールに追加する添付ファイル
        //public InArgument<string[]> AttachFiles { get; set; }


        [Localize.LocalizedCategory("Category22")] //转发 //Forward //進む
        [Localize.LocalizedDisplayName("DisplayName92")] //消息体 //Message body //メッセージ本文
        [Localize.LocalizedDescription("Description58")] //要转发的消息,该字段仅支持MailMessage对象 //For messages to be forwarded, this field only supports MailMessage objects. //転送されるメッセージの場合、このフィールドはMailMessageオブジェクトのみをサポートします。
        public InArgument<MimeMessage> TransMailMessage { get; set; }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.Integration.Activities;Component/Resources/Mail/SendMail.png";
            }
        }

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);

            if (Attachments != null)
            {
                if (Attachments.Count == 0)
                {
                    Attachments = null;
                }
                else
                {
                    if (Files == null)
                    {
                        Files = new List<InArgument<string>>();
                    }
                    foreach (string attachment in Attachments)
                    {
                        Files.Add(new InArgument<string>(attachment));
                    }
                    Attachments = null;
                }
            }

            int num = 1;
            foreach (InArgument<string> file in Files)
            {
                RuntimeArgument argument = new RuntimeArgument("attachmentArg" + ++num, typeof(string), ArgumentDirection.In);
                metadata.Bind(file, argument);
                metadata.AddArgument(argument);
            }
        }

        public SendMail()
        {
            var builder = new AttributeTableBuilder();
            builder.AddCustomAttributes(typeof(SendMail), "Files", new EditorAttribute(typeof(ArgumentCollectionEditor), typeof(DialogPropertyValueEditor)));
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }

        private static void AddAttachments(List<string> attachments, string attPath)
        {
            try
            {
                string text = null;
                if (!Path.IsPathRooted(attPath))
                {
                    text = Path.Combine(Environment.CurrentDirectory, attPath);
                }
                if (File.Exists(text))
                {
                    attachments.Add(text);
                }
                else
                {
                    attachments.Add(attPath);
                }
            }
            catch (Exception ex)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个异常产生", ex.Message);
            }
        }

        protected override void Execute(CodeActivityContext context)
        {
            string username = Email.Get(context);               //发送端账号   
            string password = Password.Get(context);            //发送端密码(这个客户端重置后的密码)
            string server = Server.Get(context);                //邮件服务器
            Int32 port = Port.Get(context);                     //端口号
            string from = From.Get(context);                    //发送者地址（一般与发送账号相同）
            string name = Name.Get(context);                    //发送者名称
            string[] receivers_To = Receivers_To.Get(context);  //收件人
            string[] receivers_Cc = Receivers_Cc.Get(context);  //抄送
            string[] recervers_Bcc = Receivers_Bcc.Get(context);//密送
            string mailTopic = MailTopic.Get(context);          //邮件的主题       
            string mailBody = MailBody.Get(context);            //发送的邮件正文  
            //string[] attachFiles = AttachFiles.Get(context);    //附件列表
            MimeMessage transMsg = TransMailMessage.Get(context);

            try
            {
				List<string> attachments = Attachments ?? new List<string>();
                foreach (InArgument<string> file in Files)
                {
                    AddAttachments(attachments, file.Get(context));
                }
                foreach (string item in AttachmentsCollection.Get(context).EmptyIfNull())
                {
                    AddAttachments(attachments, item);
                }

                MailKit.Net.Smtp.SmtpClient client = new MailKit.Net.Smtp.SmtpClient();
                client.Connect(server, port, SecureConnection);
                client.Authenticate(username, password);

                var multipart = new Multipart("mixed");

                MimeMessage msg = new MimeMessage();
                if (name == null && from == null)
                    msg.From.Add((new MailboxAddress(username)));
                else if (name == null && from != null)
                    msg.From.Add((new MailboxAddress(from)));
                else if (name != null && from == null)
                    msg.From.Add((new MailboxAddress(name, username)));
                else
                    msg.From.Add((new MailboxAddress(name, from)));

                if (receivers_To != null)
                {
                    foreach (string receiver in receivers_To)
                        msg.To.Add((new MailboxAddress(receiver)));
                }
                if (receivers_Cc != null)
                {
                    foreach (string receiver in receivers_Cc)
                        msg.Cc.Add((new MailboxAddress(receiver)));
                }
                if (recervers_Bcc != null)
                {
                    foreach (string receiver in recervers_Bcc)
                        msg.Bcc.Add((new MailboxAddress(receiver)));
                }


                if (mailBody != null)
                {
                    if (!IsBodyHtml)
                    {
                        var plain = new TextPart(TextFormat.Plain)
                        {
                            Text = mailBody
                        };
                        multipart.Add(plain);
                    }
                    else
                    {
                        var Html = new TextPart(TextFormat.Html)
                        {
                            Text = mailBody
                        };
                        multipart.Add(Html);
                    }
                }
                else if (transMsg != null)
                {
                    if (!IsBodyHtml)
                    {
                        var plain = new TextPart(TextFormat.Plain)
                        {
                            Text = transMsg.TextBody
                        };
                        multipart.Add(plain);
                    }
                    else
                    {
                        var Html = new TextPart(TextFormat.Html)
                        {
                            Text = transMsg.HtmlBody
                        };
                        multipart.Add(Html);
                    }
                }
                else
                {
                    var plain = new TextPart(TextFormat.Plain)
                    {
                        Text = ""
                    };
                    multipart.Add(plain);
                }



                if (mailTopic != null)
                    msg.Subject = mailTopic;
                else if (transMsg != null)
                    msg.Subject = transMsg.Subject;

                msg.Priority = msgProperty;

                if (Files != null)
                {
                    foreach (var p in Files)
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(p.Get(context)))
                            {
                                var attimg = new MimePart()
                                {
                                    Content = new MimeContent(File.OpenRead(p.Get(context)), ContentEncoding.Default),
                                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                                    ContentTransferEncoding = ContentEncoding.Base64,
                                    FileName = Path.GetFileName(p.Get(context))
                                };
                                multipart.Add(attimg);
                            }
                        }
                        catch (FileNotFoundException ex)
                        {
                            SharedObject.Instance.Output(SharedObject.enOutputType.Error, "文件未找到，请检查有效路径", ex.Message);
                        }
                    }
                }
                if (transMsg != null)
                {
                    foreach (MimeEntity part in transMsg.Attachments)
                    {
                        multipart.Add(part);
                    }
                }


                msg.Body = multipart;
                try
                {
                    client.Send(msg);
                    Debug.WriteLine("发送成功");
                }
                catch (System.Net.Mail.SmtpException ex)
                {
                    Debug.WriteLine(ex.Message, "发送邮件出错");
                }
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "发送邮件失败", e.Message);
            }
        }
        private void onComplete(NativeActivityContext context, ActivityInstance completedInstance)
        {
        }
    }
}
