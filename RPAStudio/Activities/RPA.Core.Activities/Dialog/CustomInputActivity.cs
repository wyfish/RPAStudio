using RPA.Core.Activities.DialogActivity.Windows;
using Plugins.Shared.Library;
using System;
using System.Activities;
using System.ComponentModel;
using System.Threading;

namespace RPA.Core.Activities.DialogActivity
{
    [Designer(typeof(CustomInputDesigner))]
    public sealed class CustomInputActivity : CodeActivity
    {
        public new string DisplayName
        {
            get
            {
                return "Custom Input";
            }
        }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.Core.Activities;Component/Resources/Dialog/custom_input.png";
            }
        }

        [Category("Input")]
        [RequiredArgument]
        [Description("这表示HTML文件的完整路径或外部URL。")]
        [DisplayNameAttribute("URL")]
        public InArgument<string> URL { get; set; }

        [Category("输出")]
        [Description("提供的HTML页面返回的字符串。")]
        [DisplayNameAttribute("Result")]
        public InOutArgument<string> ResultValue { get; set; }

        CountdownEvent latch;
        private void refreshData(CountdownEvent latch)
        {
            latch.Signal();
        }

        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                string _url = URL.Get(context);
                string _resultValue = null;
                latch = new CountdownEvent(1);
                Thread td = new Thread(() =>
                {
                    CustomInputWindow dlg = new CustomInputWindow();
                    dlg.setUrl(_url);
                    dlg.ShowDialog();
                    _resultValue = dlg.getResult();
                    dlg.Close();
                    refreshData(latch);
                });
                td.TrySetApartmentState(ApartmentState.STA);
                td.IsBackground = true;
                td.Start();
                latch.Wait();
                ResultValue.Set(context,_resultValue);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", e.Message);
            }
        }
    }
}
