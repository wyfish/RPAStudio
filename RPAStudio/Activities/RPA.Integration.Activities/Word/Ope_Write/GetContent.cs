using Microsoft.Office.Interop.Word;
using Plugins.Shared.Library;
using System;
using System.Activities;
using System.ComponentModel;
using System.Diagnostics;

namespace RPA.Integration.Activities.WordPlugins
{
    [Designer(typeof(GetContentDesigner))]
    public sealed class GetContent : AsyncCodeActivity
    {
        public GetContent()
        {
        }


        [Category("输出")]
        [RequiredArgument]
        [DisplayName("文档内容")]
        [Description("获取到Word文档中的全部内容文字")]
        [Browsable(true)]
        public OutArgument<string> WordContent { get; set; }

        [Browsable(false)]
        public string icoPath { get { return "pack://application:,,,/RPA.Integration.Activities;Component/Resources/Word/getcontent.png"; } }


        [Browsable(false)]
        public string ClassName { get { return "GetContent"; } }
        private delegate string runDelegate();
        private runDelegate m_Delegate;
        public string Run()
        {
            return ClassName;
        }

        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            PropertyDescriptor property = context.DataContext.GetProperties()[WordCreate.GetWordAppTag];
            Application wordApp = property.GetValue(context.DataContext) as Application;
            try
            {
                wordApp.Selection.WholeStory();
                string content = wordApp.Selection.Text;
                if (content != null)
                    WordContent.Set(context, content);
            }
            catch(Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "Word执行过程出错", e.Message);
                CommonVariable.realaseProcessExit(wordApp);
            }
         
            m_Delegate = new runDelegate(Run);
            return m_Delegate.BeginInvoke(callback, state);
        }

        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
        }
    }
}