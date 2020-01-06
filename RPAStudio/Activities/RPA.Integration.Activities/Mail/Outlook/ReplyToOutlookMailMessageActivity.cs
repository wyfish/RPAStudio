using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.ComponentModel;
using Plugins.Shared.Library;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Threading;
using Plugins.Shared.Library.Extensions;
using System.IO;
using Plugins.Shared.Library.Editors;
using System.Activities.Presentation.Metadata;
using System.Activities.Presentation.PropertyEditing;

namespace RPA.Integration.Activities.Mail
{
    [Designer(typeof(ReplyToOutlookMailMessageDesigner))]
    public sealed class ReplyToOutlookMailMessageActivity : AsyncTaskCodeActivity
    {
        private const int DefaultTimeoutMS = 30000;

        [Category("Common")]
        [Localize.LocalizedDisplayName("DisplayName96")] //超时时间(毫秒) //Timeout (ms) //タイムアウト（ミリ秒）
        [Localize.LocalizedDescription("Description62")] // 指定在引发错误之前等待活动运行的时间量（以毫秒为单位）。默认值为30000毫秒（30秒） //Specifies the amount of time, in milliseconds, to wait for an activity to run before an error is raised.  The default is 30000 milliseconds (30 seconds) //エラーが発生する前にアクティビティの実行を待機する時間をミリ秒単位で指定します。 デフォルトは30000ミリ秒（30秒）です
        public InArgument<int> TimeoutMS
        {
            get;
            set;
        }

        [Category("Input")]
        [Localize.LocalizedDisplayName("DisplayName95")] //邮件对象 //Mail object //メールオブジェクト
        [RequiredArgument]
        public InArgument<MailMessage> MailMessage
        {
            get;
            set;
        }

        [Category("Input")]
        [Localize.LocalizedDisplayName("DisplayName105")] //回复所有人 //Reply to everyone //全員に返信
        [Localize.LocalizedDescription("Description68")] //不勾选的话只回复给发送者，勾选的话不仅给发送者，还会给所有抄送者回复邮件 //If you don't check it, you will only reply to the sender. If you check it, you will not only give the sender, but also reply to all the CCs. //チェックしない場合は送信者に返信するだけで、チェックする場合は送信者だけでなくすべてのCCに返信します。
        public bool ReplyAll
        {
            get;
            set;
        }

        [Category("Email")]
        [Localize.LocalizedDisplayName("DisplayName88")] //正文 //Text //本体
        public InArgument<string> Body
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
        [Localize.LocalizedDisplayName("DisplayName82")] //附件列表 //Attachment list //添付リスト
        [DefaultValue(null)]
        public InArgument<IEnumerable<string>> AttachmentsCollection
        {
            get;
            set;
        }

        public ReplyToOutlookMailMessageActivity()
        {
            var builder = new AttributeTableBuilder();
            builder.AddCustomAttributes(typeof(ReplyToOutlookMailMessageActivity), "Files", new EditorAttribute(typeof(ArgumentCollectionEditor), typeof(DialogPropertyValueEditor)));

            MetadataStore.AddAttributeTable(builder.CreateTable());
        }

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);
            int num = 1;
            foreach (InArgument<string> file in Files)
            {
                RuntimeArgument argument = new RuntimeArgument("attachmentArg" + ++num, typeof(string), ArgumentDirection.In);
                metadata.Bind(file, argument);
                metadata.AddArgument(argument);
            }
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            int num = TimeoutMS.Get(context);
            string body = Body.Get(context);
            MailMessage mailMessage = MailMessage.Get(context);
            if (mailMessage == null)
            {
                throw new ArgumentNullException("MailMessage对象不能为空！");
            }
            MailMessage message = mailMessage;
            List<string> attachments = new List<string>();
            foreach (InArgument<string> file in Files)
            {
                AddAttachments(attachments, file.Get(context));
            }
            foreach (string item in AttachmentsCollection.Get(context).EmptyIfNull())
            {
                AddAttachments(attachments, item);
            }
            num = ((num <= 0) ? DefaultTimeoutMS : num);
            using (CancellationTokenSource timeoutCts = new CancellationTokenSource(num))
            {
                using (CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(timeoutCts.Token, cancellationToken))
                {
                    try
                    {
                        await OutlookAPI.ReplyToAsync(message, body, attachments, ReplyAll, cts.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        if (timeoutCts.IsCancellationRequested)
                        {
                            throw new TimeoutException();
                        }
                        throw;
                    }
                }
            }
            return delegate
            {
            };
        }

        private static void AddAttachments(List<string> attachments, string attPath)
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
    }
}
