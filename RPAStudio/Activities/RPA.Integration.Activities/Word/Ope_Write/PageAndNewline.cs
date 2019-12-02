using System;
using System.Activities;
using System.ComponentModel;
using Plugins.Shared.Library;
using Microsoft.Office.Interop.Word;

namespace RPA.Integration.Activities.WordPlugins
{
    [Designer(typeof(PageAndNewlineDesigner))]
    public sealed class PageAndNewline : AsyncCodeActivity
    {
        public PageAndNewline()
        {
        }


        InArgument<Int32> _OperateCount = 1;
        [Category("选项")]
        [DisplayName("分页/换行次数")]
        [Browsable(true)]
        public InArgument<Int32> OperateCount
        {
            get
            {
                return _OperateCount;
            }
            set
            {
                _OperateCount = value;
            }
        }

        bool _PageBreak;
        [Category("选项")]
        [DisplayName("分页")]
        [Browsable(true)]
        public bool PageBreak
        {
            get
            {
                return _PageBreak;
            }
            set
            {
                _PageBreak = value;
            }
        }

        bool _NewLine;
        [Category("选项")]
        [DisplayName("换行")]
        [Browsable(true)]
        public bool NewLine
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
        public string icoPath { get { return "pack://application:,,,/RPA.Integration.Activities;Component/Resources/Word/newline.png"; } }

        [Browsable(false)]
        public string ClassName { get { return "PageAndNewline"; } }
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
                Int32 operateCount = OperateCount.Get(context);

                if (_NewLine)
                {
                    for (int i = 0; i < operateCount; i++)
                    {
                        wordApp.Selection.TypeParagraph();
                    }
                }

                if (_PageBreak)
                {
                    for (int i = 0; i < operateCount; i++)
                    {
                        wordApp.Selection.InsertBreak();
                    }
                }
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
