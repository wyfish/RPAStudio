using System.Collections.Generic;
using System.Activities;
using System.ComponentModel;
using System.Net.Mail;
using Microsoft.Office.Interop.Outlook;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace RPA.Integration.Activities.Mail
{
    [Designer(typeof(GetOutlookMailMessagesDesigner))]
    public sealed class GetOutlookMailMessagesActivity : AsyncCodeActivity
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

        [Category("Options")]
        [DisplayName("获取邮件个数")]
        public InArgument<int> Top
        {
            get;
            set;
        }

        [Category("Output")]
        [DisplayName("输出邮件列表")]
        public OutArgument<List<MailMessage>> Messages
        {
            get;
            set;
        }


        [Category("Input")]
        [DisplayName("邮件目录")]
        [Description("从哪个邮件目录去获取邮件")]
        public InArgument<string> MailFolder
        {
            get;
            set;
        }

        [Category("Input")]
        [DisplayName("账户")]
        [Description("Outlook账户名")]
        public InArgument<string> Account
        {
            get;
            set;
        }

        [Category("Options")]
        [DisplayName("过滤器")]
        [Description("只有符合过滤条件的邮件才会被获取")]
        public InArgument<string> Filter
        {
            get;
            set;
        }

        [Category("Options")]
        [DisplayName("获取附件")]
        [Browsable(false)]
        public bool GetAttachements
        {
            get;
            set;
        }

        [Category("Options")]
        [DisplayName("只获取未读邮件")]
        [Description("是否只获取未读邮件，默认打勾")]
        public bool OnlyUnreadMessages
        {
            get;
            set;
        }

        [Category("Options")]
        [DisplayName("标记为已读")]
        [Description("是否将接收到的邮件自动标记成已读")]
        public bool MarkAsRead
        {
            get;
            set;
        }

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);
        }

        public GetOutlookMailMessagesActivity()
        {
            MailFolder = "收件箱";
            OnlyUnreadMessages = true;
            Top = new InArgument<int>(30);
        }


        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            CancellationTokenSource cancellationTokenSource2 = (CancellationTokenSource)(context.UserState = new CancellationTokenSource());
            int timeout = TimeoutMS.Get(context);
            Task<List<MailMessage>> messages = GetMessages(context, cancellationTokenSource2.Token);
            TaskCompletionSource<List<MailMessage>> taskCompletionSource = new TaskCompletionSource<List<MailMessage>>(state);
            TaskHandler(callback, messages, taskCompletionSource, cancellationTokenSource2.Token, timeout);
            return taskCompletionSource.Task;
        }

        public static void TaskHandler(AsyncCallback callback, Task<List<MailMessage>> task, TaskCompletionSource<List<MailMessage>> tcs, CancellationToken token, int timeout)
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
                    else if (token.IsCancellationRequested || task.IsCanceled)
                    {
                        tcs.TrySetCanceled();
                    }
                    else if (task.IsFaulted)
                    {
                        tcs.TrySetException(task.Exception.InnerExceptions);
                    }
                    else
                    {
                        tcs.TrySetResult(task.Result);
                    }
                }
                catch (System.Exception ex)
                {
                    tcs.TrySetException(ex.InnerException);
                }
                callback?.Invoke(tcs.Task);
            });
        }


        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
            Task<List<MailMessage>> task = (Task<List<MailMessage>>)result;
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
                else
                {
                    Messages.Set(context, task.Result);
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

        public Task<List<MailMessage>> GetMessages(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            string account = Account.Get(context);
            string filter = Filter.Get(context);
            string folderpath = MailFolder.Get(context);
            int top = Top.Get(context);

            return Task.Factory.StartNew(() => OutlookAPI.GetMessages(account, folderpath, top, filter, OnlyUnreadMessages, MarkAsRead, true, cancellationToken));
        }




    }
}
