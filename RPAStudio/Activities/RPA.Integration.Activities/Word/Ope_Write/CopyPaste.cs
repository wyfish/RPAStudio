using System;
using System.Activities;
using System.ComponentModel;
using Plugins.Shared.Library;
using Microsoft.Office.Interop.Word;

namespace RPA.Integration.Activities.WordPlugins
{
    [Designer(typeof(CopyPasteDesigner))]
    public sealed class CopyPaste : AsyncCodeActivity
    {
        public CopyPaste()
        {
        }

        bool _Copy = true;
        [Category("选项")]
        [DisplayName("复制")]
        [Browsable(true)]
        public bool Copy
        {
            get
            {
                return _Copy;
            }
            set
            {
                _Copy = value;
            }
        }

        bool _Paste;
        [Category("选项")]
        [DisplayName("粘贴")]
        [Browsable(true)]
        public bool Paste
        {
            get
            {
                return _Paste;
            }
            set
            {
                _Paste = value;
            }
        }

        [Browsable(false)]
        public string icoPath { get { return "pack://application:,,,/RPA.Integration.Activities;Component/Resources/Word/copy.png"; } }

					
					
		[Browsable(false)]
        public string ClassName { get { return "CopyPaste"; } }
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
                if (_Copy)
                    wordApp.Selection.Copy();
                if (_Paste)
                    wordApp.Selection.Paste();
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
