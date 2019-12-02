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
        [DisplayName("超时时间(毫秒)")]
        [Description(" 指定在引发错误之前等待活动运行的时间量（以毫秒为单位）。默认值为30000毫秒（30秒）")]
        public InArgument<int> TimeoutMS
        {
            get;
            set;
        }

        [Category("Input")]
        [DisplayName("邮件对象")]
        [RequiredArgument]
        public InArgument<MailMessage> MailMessage
        {
            get;
            set;
        }

        [Category("Input")]
        [DisplayName("回复所有人")]
        [Description("不勾选的话只回复给发送者，勾选的话不仅给发送者，还会给所有抄送者回复邮件")]
        public bool ReplyAll
        {
            get;
            set;
        }

        [Category("Email")]
        [DisplayName("正文")]
        public InArgument<string> Body
        {
            get;
            set;
        }


        [Category("Attachments")]
        [DisplayName("附件")]
        public List<InArgument<string>> Files
        {
            get;
            set;
        } = new List<InArgument<string>>();


        [Category("Attachments")]
        [DisplayName("附件列表")]
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
