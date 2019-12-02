using Microsoft.Office.Interop.Word;
using System.Activities;
using System.ComponentModel;
using System;
using Plugins.Shared.Library;

namespace RPA.Integration.Activities.WordPlugins
{
    [Designer(typeof(WholeStoryDesigner))]
    public sealed class WholeStory : AsyncCodeActivity
    {
        public WholeStory()
        {
        }

        [Browsable(false)]
        public string icoPath { get { return "pack://application:,,,/RPA.Integration.Activities;Component/Resources/Word/wholestory.png"; } }

        [Browsable(false)]
        public string ClassName { get { return "WholeStory"; } }
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
