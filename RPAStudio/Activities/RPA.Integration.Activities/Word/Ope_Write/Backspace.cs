using System;
using System.Activities;
using System.ComponentModel;
using Plugins.Shared.Library;
using Microsoft.Office.Interop.Word;

namespace RPA.Integration.Activities.WordPlugins
{
    [Designer(typeof(BackspaceDesigner))]
    public sealed class Backspace : AsyncCodeActivity
    {
        public Backspace()
        {
        }

        InArgument<Int32> _BackCounts = 1;
        [Category("选项")]
        [DisplayName("退格次数")]
        [Browsable(true)]
        public InArgument<Int32> BackCounts
        {
            get
            {
                return _BackCounts;
            }
            set
            {
                _BackCounts = value;
            }
        }

        [Browsable(false)]
        public string icoPath { get { return "pack://application:,,,/RPA.Integration.Activities;Component/Resources/Word/backspace.png"; } }

        [Browsable(false)]
        public string ClassName { get { return "Backspace"; } }
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
                Int32 backCounts = BackCounts.Get(context);
                for (int i = 0; i < backCounts; i++)
                    wordApp.Selection.TypeBackspace();
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
