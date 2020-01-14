using Plugins.Shared.Library;
using System;
using System.Activities;
using System.ComponentModel;
using System.Threading;

namespace RPA.Core.Activities.ClipboardActivity
{
    [Designer(typeof(CopySelectTextDesigner))]
    public sealed class GetFromClipboardActivity : CodeActivity
    {
        public new string DisplayName
        {
            get
            {
                return "Get From Clipboard";
            }
        }

        [Category("Common")]
        [Localize.LocalizedDescription("Description1")] //指定即使当前活动失败，也要继续执行其余的活动。只支持布尔值(True,False)。 //Specifies that the remaining activities will continue even if the current activity fails. Only Boolean values are supported. //現在のアクティビティが失敗した場合でも、アクティビティの残りを続行するように指定します。 ブール値（True、False）のみがサポートされています。
        public InArgument<bool> ContinueOnError { get; set; }

        //[Category("Input")]
        //[Description("指定在抛出错误之前等待活动运行的时间(以毫秒为单位)。默认值为30000毫秒(30秒)。")]
        //public InArgument<Int32> Timeout { get; set; }

        [Category("Options")]
        [Localize.LocalizedDescription("Description12")] //从剪贴板中检索的数据。 //The data retrieved from the clipboard. //クリップボードから取得したデータ。
        public OutArgument<string> Result { get; set; }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.Core.Activities;Component/Resources/Clipboard/gettext.png";
            }
        }

        CountdownEvent latch;
        private void refreshData(CountdownEvent latch)
        {
            latch.Signal();
        }

        private InArgument<Int32> _Timeout = 3000;
        [Category("Options")]
        [Localize.LocalizedDescription("Description7")] //指定在抛出错误之前等待活动运行的时间(以毫秒为单位)。默认值为3000毫秒(3秒)。 //Specifies the amount of time, in milliseconds, to wait for an activity to run before throwing an error.  The default is 3000 milliseconds (3 seconds). //エラーをスローする前にアクティビティの実行を待機する時間をミリ秒単位で指定します。 デフォルトは3000ミリ秒（3秒）です。
        public InArgument<Int32> Timeout
        {
            get
            {
                return _Timeout;
            }
            set
            {
                if (_Timeout != value)
                    _Timeout = value;
            }
        }

        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                Int32 _timeout = Timeout.Get(context);
                string data = "";
                Thread.Sleep(_timeout);
                latch = new CountdownEvent(1);
                Thread td = new Thread(() =>
                {
                    System.Windows.Forms.IDataObject clipBoardBefore = System.Windows.Forms.Clipboard.GetDataObject();
                    if (clipBoardBefore.GetDataPresent(System.Windows.DataFormats.Text))
                    {
                        data = (string)clipBoardBefore.GetData(System.Windows.DataFormats.Text);
                    }
                    refreshData(latch);
                });
                td.TrySetApartmentState(ApartmentState.STA);
                td.IsBackground = true;
                td.Start();
                latch.Wait();
                Result.Set(context, data);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", e.Message);
                if (ContinueOnError.Get(context))
                {
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
