using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.ComponentModel;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Plugins.Shared.Library.Extensions;
using System.IO;
using Plugins.Shared.Library;
using System.Activities.Presentation.Metadata;
using Plugins.Shared.Library.Editors;
using System.Activities.Presentation.PropertyEditing;

namespace RPA.Integration.Activities.Mail
{
    [Designer(typeof(SendOutlookMailDesigner))]
    public sealed class SendOutlookMailActivity : AsyncCodeActivity
    {
        [Category("Receiver")]
        [Localize.LocalizedDisplayName("Category19")] //收件人 //Recipient //受取人
        [Localize.LocalizedDescription("Description50")] //电子邮件的主要收件人 //Primary recipient of the email //電子メールの主要な受信者
        [RequiredArgument]
        public InArgument<string> To
        {
            get;
            set;
        }

        [Category("Receiver")]
        [Localize.LocalizedDisplayName("DisplayName85")] //抄送 //Cc //Cc
        [Localize.LocalizedDescription("Description51")] //电子邮件的次要收件人 //Secondary recipient of the email //メールの二次受信者
        public InArgument<string> Cc
        {
            get;
            set;
        }

        [Category("Receiver")]
        [Localize.LocalizedDisplayName("DisplayName107")] //密送 //Secret delivery //秘密の配達
        [Localize.LocalizedDescription("Description52")] //电子邮件的隐藏收件人 //Hidden recipient of email //メールの隠された受信者
        public InArgument<string> Bcc
        {
            get;
            set;
        }

        [Category("Email")]
        [Localize.LocalizedDisplayName("DisplayName87")] //主题 //Theme //テーマ
        [Localize.LocalizedDescription("Description53")] //电子邮件的主题 //Email subject //メールの件名
        public InArgument<string> Subject
        {
            get;
            set;
        }

        [Category("Email")]
        [Localize.LocalizedDisplayName("DisplayName88")] //正文 //Text //本体
        [Localize.LocalizedDescription("Description54")] //电子邮件的正文 //The body of the email //メールの本文
        public InArgument<string> Body
        {
            get;
            set;
        }

        [Category("Options")]
        [Localize.LocalizedDisplayName("DisplayName90")] //HTML格式 //HTML format //HTML形式
        [Localize.LocalizedDescription("Description56")] // 指定邮件正文是否以HTML格式编写 //Specify whether the body of the message is written in HTML format //メッセージの本文をHTML形式で記述するかどうかを指定します
        public bool IsBodyHtml
        {
            get;
            set;
        }

        [Category("Common")]
        [Localize.LocalizedDisplayName("DisplayName96")] //超时时间(毫秒) //Timeout (ms) //タイムアウト（ミリ秒）
        [Localize.LocalizedDescription("Description62")] // 指定在引发错误之前等待活动运行的时间量（以毫秒为单位）。默认值为30000毫秒（30秒） //Specifies the amount of time, in milliseconds, to wait for an activity to run before an error is raised.  The default is 30000 milliseconds (30 seconds) //エラーが発生する前にアクティビティの実行を待機する時間をミリ秒単位で指定します。 デフォルトは30000ミリ秒（30秒）です
        public InArgument<int> TimeoutMS
        {
            get;
            set;
        }


        [Category("Attachments")]
        [Localize.LocalizedDisplayName("Category21")] //附件 //Attachments //添付ファイル
        public List<InArgument<string>> Files
        {
            get;
            set;
        } = new List<InArgument<string>>();


        [Category("Attachments")]
        [DefaultValue(null)]
        [Localize.LocalizedDisplayName("DisplayName82")] //附件列表 //Attachment list //添付リスト
        public InArgument<IEnumerable<string>> AttachmentsCollection
        {
            get;
            set;
        }

        [Category("Forward")]
        [Localize.LocalizedDisplayName("DisplayName95")] //邮件对象 //Mail object //メールオブジェクト
        [OverloadGroup("Forward")]
        [Localize.LocalizedDescription("Description58")] //要转发的消息,该字段仅支持MailMessage对象 //For messages to be forwarded, this field only supports MailMessage objects. //転送されるメッセージの場合、このフィールドはMailMessageオブジェクトのみをサポートします。
        public InArgument<MailMessage> MailMessage
        {
            get;
            set;
        }


        [Browsable(false)]
        [DefaultValue(null)]
        [Localize.LocalizedDisplayName("Category21")] //附件 //Attachments //添付ファイル
        public List<string> Attachments
        {
            get;
            set;
        }

        [Category("Input")]
        [Localize.LocalizedDisplayName("DisplayName100")] //账户 //Account //アカウント
        [Localize.LocalizedDescription("Description60")] //Outlook账户名 //Outlook account name //Outlookアカウント名
        public InArgument<string> Account
        {
            get;
            set;
        }

        [Category("Input")]
        [Localize.LocalizedDisplayName("DisplayName108")] //显示名 //Display name //表示名
        [Localize.LocalizedDescription("Description72")] //预期的显示名称，邮件系统必须支持该功能才能使用 //The expected display name that the messaging system must support in order to use //メッセージングシステムが使用するためにサポートする必要がある表示名
        [DefaultValue(null)]
        public InArgument<string> SentOnBehalfOfName
        {
            get;
            set;
        }

        [Category("Options")]
        [Localize.LocalizedDisplayName("DisplayName109")] //是否保存成草稿 //Whether to save as a draft //下書きとして保存するかどうか
        public bool IsDraft
        {
            get;
            set;
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

        public SendOutlookMailActivity()
        {
            var builder = new AttributeTableBuilder();
            builder.AddCustomAttributes(typeof(SendOutlookMailActivity), "Files", new EditorAttribute(typeof(ArgumentCollectionEditor), typeof(DialogPropertyValueEditor)));

            MetadataStore.AddAttributeTable(builder.CreateTable());
        }

        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            CancellationTokenSource cancellationTokenSource2 = (CancellationTokenSource)(context.UserState = new CancellationTokenSource());
            Receiver reciever = new Receiver(To.Get(context), Cc.Get(context), Bcc.Get(context));
            bool isNewMessage = false;
            MailMessage mailMessage = MailMessage.Get(context);
            string body = Body.Get(context);
            if (mailMessage == null)
            {
                mailMessage = new MailMessage();
                mailMessage.Subject = Subject.Get(context);
                mailMessage.Body = body;
                isNewMessage = true;
            }
            mailMessage.IsBodyHtml = IsBodyHtml;
            int timeout = TimeoutMS.Get(context);
            Task task = SendMailTask(context, reciever, mailMessage, cancellationTokenSource2.Token, isNewMessage, body);
            TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>(state);
            TaskHandler(callback, task, taskCompletionSource, cancellationTokenSource2.Token, mailMessage, isNewMessage, timeout);
            return taskCompletionSource.Task;
        }

        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
            Task task = (Task)result;
            try
            {
                if (task.IsFaulted)
                {
                    throw task.Exception.InnerException;
                }
                if (task.IsCanceled || context.IsCancellationRequested)
                {
                    context.MarkCanceled();
                }
            }
            catch (OperationCanceledException)
            {
                context.MarkCanceled();
            }
        }

        protected override void Cancel(AsyncCodeActivityContext context)
        {
            ((CancellationTokenSource)context.UserState).Cancel();
        }



        public Task SendMailTask(AsyncCodeActivityContext context, Receiver reciever, MailMessage mailMessage, CancellationToken cancellationToken, bool isNewMessage, string body)
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
            string to = To.Get(context);
            string cc = Cc.Get(context);
            string bcc = Bcc.Get(context);
            string sentOnBehalfOfName = SentOnBehalfOfName.Get(context);
            string account = Account.Get(context);
            return Task.Factory.StartNew(delegate
            {
                OutlookAPI.SendMail(mailMessage, account, sentOnBehalfOfName, to, cc, bcc, attachments, IsDraft, IsBodyHtml);
            });
        }

        public static void TaskHandler(AsyncCallback callback, Task task, TaskCompletionSource<object> tcs, CancellationToken token, MailMessage msg, bool isNewMessage, int timeout)
        {
            timeout = ((timeout <= 0) ? 30000 : timeout);
            Task.Run(delegate
            {
                try
                {
                    if (!task.Wait(timeout, token))
                    {
                        tcs.TrySetException(new TimeoutException());
                    }
                    else
                    {
                        if (isNewMessage)
                        {
                            msg.Dispose();
                        }
                        if (token.IsCancellationRequested || task.IsCanceled)
                        {
                            tcs.TrySetCanceled();
                        }
                        else if (task.IsFaulted)
                        {
                            tcs.TrySetException(task.Exception.InnerExceptions);
                        }
                    }
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex.InnerException);
                }
                callback?.Invoke(tcs.Task);
            });
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




    }







}
