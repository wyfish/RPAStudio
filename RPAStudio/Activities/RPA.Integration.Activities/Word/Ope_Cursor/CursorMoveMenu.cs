using System;
using System.Activities;
using System.ComponentModel;
using Plugins.Shared.Library;
using Microsoft.Office.Interop.Word;

namespace RPA.Integration.Activities.WordPlugins
{
    [Designer(typeof(CursorMoveMenuDesigner))]
    public sealed class CursorMoveMenu : AsyncCodeActivity
    {
        [Category("选项")]
        [DisplayName("开头")]
        [Browsable(true)]
        public bool Head
        {
            get;set;
        }

        [Category("选项")]
        [DisplayName("结尾")]
        [Browsable(true)]
        public bool Tail
        {
            get; set;
        }

        [Category("选项")]
        [DisplayName("下一页")]
        [Browsable(true)]
        public bool NextPage
        {
            get; set;
        }

        [Browsable(false)]
        public string icoPath { get { return "pack://application:,,,/RPA.Integration.Activities;Component/Resources/Word/move2.png"; } }


        [Browsable(false)]
        public string ClassName { get { return "CursorMoveMenu"; } }
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
                Selection sel = wordApp.Selection;
                if (Head)
                    sel.GoTo(1, 2, 0, "1");
                if (Tail)
                    sel.EndKey();
                if (NextPage)
                    sel.GoTo(1, 2, 1, "");
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
