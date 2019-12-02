using RPA.UIAutomation.Activities.Mouse;
using Plugins.Shared.Library;
using Plugins.Shared.Library.UiAutomation;
using System;
using System.Activities;
using System.ComponentModel;

namespace RPA.UIAutomation.Activities.Find
{
    [Designer(typeof(FindEleDesigner))]
    public sealed class FindEle : AsyncCodeActivity
    {
        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "Find Element"; } }

        [Category("公共")]
        [DisplayName("错误执行")]
        [Description("指定即使活动引发错误，自动化是否仍应继续,取值为（True或False）")]
        public InArgument<bool> ContinueOnError { get; set; }

        [Category("选项")]
        [DisplayName("等待活动")]
        [Description("当此选项被选中，该活动将等待指定的ui元素成为活动")]
        public bool WaitActive { get; set; }

        [Category("选项")]
        [DisplayName("等待可见")]
        [Description("当此选项被选中，该活动将等待指定的ui元素成为可见")]
        public bool WaitVisible { get; set; }

        [Category("UI对象")]
        [OverloadGroup("Selector")]
        [Browsable(true)]
        [DisplayName("窗口指示器")]
        [Description("用于在执行活动时查找特定UI元素的Text属性")]
        public InArgument<string> Selector { get; set; }


        [Category("UI对象")]
        [OverloadGroup("Element")]
        [RequiredArgument]
        [Browsable(true)]
        [DisplayName("UI元素")]
        [Description("要关闭的窗口。该字段仅接受Window变量")]
        public InArgument<UiElement> Element { get; set; }


        [Category("输出")]
        [RequiredArgument]
        [Browsable(true)]
        [DisplayName("Ui元素")]
        [Description("找到的ui元素。此字段仅支持UIElement类型变量")]
        public OutArgument<UiElement> FoundElement { get; set; }

        [Browsable(false)]
        public string SourceImgPath { get; set; }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Find/EleExists.png";
            }
        }

        private System.Windows.Visibility visi = System.Windows.Visibility.Hidden;
        [Browsable(false)]
        public System.Windows.Visibility visibility
        {
            get
            {
                return visi;
            }
            set
            {
                visi = value;
            }
        }


        [Browsable(false)]
        public string ClassName { get { return "FindEle"; } }
        private delegate string runDelegate();
        private runDelegate m_Delegate;
        public string Run()
        {
            return ClassName;
        }

        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            try
            {
                m_Delegate = new runDelegate(Run);
                var selStr = Selector.Get(context);
                UiElement element = null;
                element = Common.GetValueOrDefault(context, this.Element, null);
                if (element == null && selStr != null)
                {
                    element = UiElement.FromSelector(selStr);
                }
                else
                {
                    PropertyDescriptor property = context.DataContext.GetProperties()[EleScope.GetEleScope];
                    element = property.GetValue(context.DataContext) as UiElement;
                }
                if (WaitActive)
                {
                    //等待元素活动逻辑
                }
                if (WaitVisible)
                {
                    //等待元素可见逻辑
                }
                if(element != null)
                    FoundElement.Set(context, element);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "获取子节点元素失败", e.Message);
                if (ContinueOnError.Get(context))
                {
                    return m_Delegate.BeginInvoke(callback, state);
                }
                else
                {
                    throw e;
                }
            }
            return m_Delegate.BeginInvoke(callback, state);
        }

        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
        }
    }
}
