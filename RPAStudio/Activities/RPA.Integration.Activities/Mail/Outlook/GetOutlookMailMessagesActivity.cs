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
        [Localize.LocalizedDisplayName("DisplayName96")] //超时时间(毫秒) //Timeout (ms) //タイムアウト（ミリ秒）
        [Localize.LocalizedDescription("Description62")] // 指定在引发错误之前等待活动运行的时间量（以毫秒为单位）。默认值为30000毫秒（30秒） //Specifies the amount of time, in milliseconds, to wait for an activity to run before an error is raised.  The default is 30000 milliseconds (30 seconds) //エラーが発生する前にアクティビティの実行を待機する時間をミリ秒単位で指定します。 デフォルトは30000ミリ秒（30秒）です
        public InArgument<int> TimeoutMS
        {
            get;
            set;
        }

        [Category("Options")]
        [Localize.LocalizedDisplayName("DisplayName97")] //获取邮件个数 //Get the number of messages //メッセージの数を取得する
        public InArgument<int> Top
        {
            get;
            set;
        }

        [Category("Output")]
        [Localize.LocalizedDisplayName("DisplayName98")] //输出邮件列表 //Output mailing list //メーリングリストを出力する
        public OutArgument<List<MailMessage>> Messages
        {
            get;
            set;
        }


        [Category("Input")]
        [Localize.LocalizedDisplayName("DisplayName99")] //邮件目录 //Mail directory //メールディレクトリ
        [Localize.LocalizedDescription("Description63")] //从哪个邮件目录去获取邮件 //From which mail directory to get mail //どのメールディレクトリからメールを取得するか
        public InArgument<string> MailFolder
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

        [Category("Options")]
        [Localize.LocalizedDisplayName("DisplayName101")] //过滤器 //filter //フィルター
        [Localize.LocalizedDescription("Description64")] //只有符合过滤条件的邮件才会被获取 //Only messages that match the filter will be retrieved. //フィルターに一致するメッセージのみが取得されます。
        public InArgument<string> Filter
        {
            get;
            set;
        }

        [Category("Options")]
        [Localize.LocalizedDisplayName("DisplayName102")] //获取附件 //Get attachment //添付ファイルを取得
        [Browsable(false)]
        public bool GetAttachements
        {
            get;
            set;
        }

        [Category("Options")]
        [Localize.LocalizedDisplayName("DisplayName103")] //只获取未读邮件 //Get only unread messages //未読メッセージのみを取得する
        [Localize.LocalizedDescription("Description65")] //是否只获取未读邮件，默认打勾 //Whether to get only unread mail, check by default //未読メールのみを取得するかどうか、デフォルトでチェックする
        public bool OnlyUnreadMessages
        {
            get;
            set;
        }

        [Category("Options")]
        [Localize.LocalizedDisplayName("DisplayName104")] //标记为已读 //Mark as read //既読にする
        [Localize.LocalizedDescription("Description66")] //是否将接收到的邮件自动标记成已读 //Whether to automatically mark received messages as read //受信したメッセージを自動的に既読にするかどうか
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
