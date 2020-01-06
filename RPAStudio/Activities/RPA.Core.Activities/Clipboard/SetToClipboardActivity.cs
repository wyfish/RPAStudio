using Plugins.Shared.Library;
using System;
using System.Activities;
using System.ComponentModel;
using System.Threading;
using System.Windows;

namespace RPA.Core.Activities.ClipboardActivity
{
    [Designer(typeof(SetToClipboardDesigner))]
    public sealed class SetToClipboardActivity : CodeActivity
    {
        public new string DisplayName
        {
            get
            {
                return "Set To Clipboard";
            }
        }

        [Category("Common")]
        [Localize.LocalizedDescription("Description1")] //指定即使当前活动失败，也要继续执行其余的活动。只支持布尔值(True,False)。 //Specifies that the remaining activities will continue even if the current activity fails. Only Boolean values are supported. //現在のアクティビティが失敗した場合でも、アクティビティの残りを続行するように指定します。 ブール値（True、False）のみがサポートされています。
        public InArgument<bool> ContinueOnError { get; set; }

        [RequiredArgument]
        [Category("Input")]
        [Localize.LocalizedDescription("Description13")] //要复制到剪贴板的文本。 //The text to be copied to the clipboard. //クリップボードにコピーされるテキスト。
        public InArgument<string> Text { get; set; }

        [Category("Input")]
        [Localize.LocalizedDescription("Description14")] //拷贝文件或文件夹 //Copy files or folders //ファイルまたはフォルダーをコピーする
        public bool IsFile { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                string _text = Text.Get(context);
                if (!IsFile)
                {
                    
                    Thread td = new Thread(() =>
                    {
                        Clipboard.SetDataObject(_text, true);
                    });
                    td.TrySetApartmentState(ApartmentState.STA);
                    td.IsBackground = true;
                    td.Start();
                }
                else
                {
                    Thread td = new Thread(() =>
                    {
                        System.Collections.Specialized.StringCollection strcoll = new System.Collections.Specialized.StringCollection();
                        strcoll.Add(_text);
                        Clipboard.SetFileDropList(strcoll);
                    });
                    td.TrySetApartmentState(ApartmentState.STA);
                    td.IsBackground = true;
                    td.Start();
                }
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
