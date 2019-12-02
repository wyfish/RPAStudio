using Plugins.Shared.Library;
using System;
using System.Activities;
using System.ComponentModel;
using System.Threading;

namespace RPA.Core.Activities.DialogActivity
{
    [Designer(typeof(InputDialogDesigner))]
    public sealed class InputDialogActivity : CodeActivity
    {
        public new string DisplayName
        {
            get
            {
                return "Input Dialog";
            }
        }

        [Category("Input")]
        public bool IsPassword { get; set; }

        [Category("Input")]
        [Description("表单字段的标签。")]
        [DisplayNameAttribute("Label")]
        public InArgument<string> LabelTextBox { get; set; }

        [Category("Input")]
        [Description("可供选择的选项数组。如果将其设置为只包含一个元素，则会出现一个文本框来编写文本。如果将其设置为包含2或3个元素，则它们将显示为要从中选择的单选按钮。如果将其设置为包含3个以上的项，则它们将作为组合框显示以供选择。此字段只支持字符串数组变量。")]
        [DisplayNameAttribute("Options")]
        public InArgument<string[]> OptionsTextBox { get; set; }

        [Category("Input")]
        [Description("输入对话框的标题。")]
        [DisplayNameAttribute("Title")]
        public InArgument<string> TitleTextBox { get; set; }

        [Category("输出")]
        [Description("用户在输入对话框中插入的值。")]
        [DisplayNameAttribute("Result")]
        public OutArgument<string> Result { get; set; }


        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.Core.Activities;Component/Resources/Dialog/inputdialog.png";
            }
        }

        CountdownEvent latch;
        private void refreshData(CountdownEvent latch)
        {
            latch.Signal();
        }

        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                string _title = TitleTextBox.Get(context);
                string _textValue = LabelTextBox.Get(context);
                string[] _listItem = OptionsTextBox.Get(context);
                string _resultValue = null;
                latch = new CountdownEvent(1);
                Thread td = new Thread(() =>
                {
                    DialogActivity.Windows.InputDialogWindow dlg = new DialogActivity.Windows.InputDialogWindow();

                    if (_title != null)
                        dlg.Title = _title;

                    if (_textValue != null)
                        dlg.setTextContent(_textValue);

                    if(_listItem != null)
                    {
                        if (_listItem.Length <= 1)
                            dlg.CreateEditBox(IsPassword);
                        else if (_listItem.Length > 1 && _listItem.Length < 4)
                            dlg.CreateCheckBox(_listItem);
                        else
                            dlg.CreateCombobox(_listItem);
                    }
                    else
                    {
                        dlg.CreateEditBox(IsPassword);
                    }
                    dlg.ShowDialog();
                    _resultValue = dlg.getValue();
                    dlg.Close();
                    refreshData(latch);
                });
                td.TrySetApartmentState(ApartmentState.STA);
                td.IsBackground = true;
                td.Start();
                latch.Wait();
                Result.Set(context, _resultValue);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", e.Message);
            }
        }
    }
}
