using FlaUI.Core.AutomationElements;
using RPA.UIAutomation.Activities.Mouse;
using Plugins.Shared.Library;
using Plugins.Shared.Library.UiAutomation;
using System;
using System.Activities;
using System.ComponentModel;
using System.Drawing;

namespace RPA.UIAutomation.Activities.Find
{
    [Designer(typeof(GetAncestorDesigner))]
    public sealed class GetAncestor : AsyncCodeActivity
    {
        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "Get Ancestor"; } }
        [Category("公共")]
        [DisplayName("错误执行")]
        [Description("指定即使活动引发错误，自动化是否仍应继续,取值为（True或False）")]
        public InArgument<bool> ContinueOnError { get; set; }

        [Category("UI对象")]
        [OverloadGroup("Selector")]
        [RequiredArgument]
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

        private int _UpLevels = 1;
        [Category("选项")]
        [Browsable(true)]
        [DisplayName("父节点层次")]
        [Description("指定要在ui层次结构的哪个级别查找父节点")]
        public int UpLevels
        {
            get
            {
                return _UpLevels;
            }
            set
            {
                _UpLevels = value;
            }
        }
        

        [Category("输出")]
        [RequiredArgument]
        [Browsable(true)]
        [DisplayName("父节点")]
        [Description("元素的父节点。仅支持UIElement类型变量")]
        public OutArgument<UiElement> AncestorElement { get; set; }
        
        [Browsable(false)]
        public string SourceImgPath { get; set; }
       
        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Find/GetAncestor.png";
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
        public string ClassName { get { return "GetAncestor"; } }
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
                UiElement parentEle = null;
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
                parentEle = element;
                for (int i = 0; i < UpLevels; i++)
                {
                    parentEle = parentEle.AutomationElementParent;
                }
                AncestorElement.Set(context, parentEle);

                return m_Delegate.BeginInvoke(callback, state);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "获取父节点元素失败", e.Message);
                if (ContinueOnError.Get(context))
                {
                    return m_Delegate.BeginInvoke(callback, state);
                }
                else
                {
                    throw e;
                }
            }
        }

        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
        }
    }
}
