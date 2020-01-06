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
        [Localize.LocalizedDescription("Description78")] //这表示HTML文件的完整路径或外部URL。 //This represents the full path or external URL of the HTML file. //これは、HTMLファイルの完全パスまたは外部URLを表します。
        [DisplayNameAttribute("URL")]
        public InArgument<string> URL { get; set; }

        [Localize.LocalizedCategory("Category2")] //输出 //Output //出力
        [Localize.LocalizedDescription("Description79")] //提供的HTML页面返回的字符串。 //The string returned by the provided HTML page. //指定されたHTMLページによって返される文字列。
        [DisplayNameAttribute("Result")]
        public OutArgument<string> ResultValue { get; set; }

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
