using System;
using System.Activities;
using System.ComponentModel;
using Plugins.Shared.Library;
using Microsoft.Office.Interop.Word;

namespace RPA.Integration.Activities.WordPlugins
{
    [Designer(typeof(WriteTextDesigner))]
    public sealed class WriteText : AsyncCodeActivity
    {
        public WriteText()
        {
        }

        InArgument<string> _TextContent;
        [Localize.LocalizedCategory("Category4")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName139")] //文本内容 //Text content //テキストコンテンツ
        [Browsable(true)]
        public InArgument<string> TextContent
        {
            get
            {
                return _TextContent;
            }
            set
            {
                _TextContent = value;
            }
        }

        InArgument<Int32> _NewLine = 0;
        [Localize.LocalizedCategory("Category4")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName138")] //换行 //Wrap //ラップ
        [Localize.LocalizedDescription("Description74")] //写入文本前写入的行数 //The number of lines written before writing text //テキストを書き込む前に書き込まれた行数
        [Browsable(true)]
        public InArgument<Int32> NewLine
        {
            get
            {
                return _NewLine;
            }
            set
            {
                _NewLine = value;
            }
        }



        [Browsable(false)]
        public string icoPath { get { return "pack://application:,,,/RPA.Integration.Activities;Component/Resources/Word/text.png"; } }


        [Browsable(false)]
        public string ClassName { get { return "WriteText"; } }
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
                string textContent = TextContent.Get(context);
                Int32 newLine = NewLine.Get(context);

                for (int i = 0; i < newLine; i++)
                {
                    wordApp.Selection.TypeParagraph();
                }
                wordApp.Selection.TypeText(textContent);
            }
            catch (Exception e)
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
