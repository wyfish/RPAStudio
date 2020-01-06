using System;
using System.Activities;
using System.ComponentModel;
using System.IO;
using Plugins.Shared.Library;
using Microsoft.Office.Interop.Word;

namespace RPA.Integration.Activities.WordPlugins
{
    [Designer(typeof(InsertFileDesigner))]
    public sealed class InsertFile : AsyncCodeActivity
    {
        public InsertFile()
        {
        }

        InArgument<string> _PathUrl;
        [Localize.LocalizedCategory("Category4")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName12")] //文件路径 //File path //ファイルパス
        [RequiredArgument]
        [Browsable(true)]
        public InArgument<string> PathUrl
        {
            get
            {
                return _PathUrl;
            }
            set
            {
                _PathUrl = value;
            }
        }

        [Browsable(false)]
        public string icoPath { get { return "pack://application:,,,/RPA.Integration.Activities;Component/Resources/Word/writefile.png"; } }


        [Browsable(false)]
        public string ClassName { get { return "InsertFile"; } }
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
                string pathUrl = PathUrl.Get(context);
                if (!File.Exists(pathUrl))
                {
                    SharedObject.Instance.Output(SharedObject.enOutputType.Error, "文件不存在，请检查路径有效性!");
                    CommonVariable.realaseProcessExit(wordApp);
                }
                wordApp.Selection.InsertFile(pathUrl);
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
