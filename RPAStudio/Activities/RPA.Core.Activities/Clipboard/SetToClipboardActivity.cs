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
        [Description("指定即使当前活动失败，也要继续执行其余的活动。只支持布尔值(True,False)。")]
        public InArgument<bool> ContinueOnError { get; set; }

        [RequiredArgument]
        [Category("Input")]
        [Description("要复制到剪贴板的文本。")]
        public InArgument<string> Text { get; set; }

        [Category("Input")]
        [Description("拷贝文件或文件夹")]
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
