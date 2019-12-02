using System.Collections.Generic;
using System.Activities;
using System.ComponentModel;
using System.Windows;
using System;
using Plugins.Shared.Library.UiAutomation;
using RPA.UIAutomation.Activities.Mouse;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Conditions;
using Plugins.Shared.Library;

namespace RPA.UIAutomation.Activities.Find
{
    [Designer(typeof(FindChildrenDesigner))]
    public sealed class FindChildren : AsyncCodeActivity
    {
        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "Find Children"; } }


        [Category("公共")]
        [DisplayName("错误执行")]
        [Description("指定即使活动引发错误，自动化是否仍应继续,取值为（True或False）")]
        public InArgument<bool> ContinueOnError { get; set; }

        public enum ScopeOption
        {
            Children,
            Descendants,
            Subtree
        }
        
        ScopeOption _Scope = ScopeOption.Children;
        [Category("输入")]
        [Browsable(true)]
        [DisplayName("范围")]
        public ScopeOption Scope
        {
            get
            {
                return _Scope;
            }
            set
            {
                _Scope = value;
            }
        }

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

      
        [RequiredArgument]
        [OverloadGroup("FilterText")]
        [Category("输入")]
        [Browsable(true)]
        [DisplayName("过滤器")]
        [Description("xml字符串指定集合中的所有UI对象应该满足的条件")]
        public InArgument<string> FilterText { get; set; }

        [Category("输出")]
        [Browsable(true)]
        [DisplayName("子元素")]
        [Description("所有ui子元素都将按照设置的过滤器和范围。此字段只支持IEnumerable<UIElement>变量")]
        public OutArgument<List<UiElement>> UiList { get; set; }

        [Browsable(false)]
        public string SourceImgPath { get; set; }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Find/FindChildren.png";
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
        public string ClassName { get { return "FindChildren"; } }
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
                string filterText = FilterText.Get(context);
                TreeScope treeScope;
                if (Scope == ScopeOption.Children)
                    treeScope = TreeScope.Children;
                else if (Scope == ScopeOption.Descendants)
                    treeScope = TreeScope.Descendants;
                else
                    treeScope = TreeScope.Subtree;
                UiElement element = null;
                var selStr = Selector.Get(context);


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

                List<UiElement> uiList = new List<UiElement>();
                uiList = element.FindAllByFilter(treeScope, TrueCondition.Default, filterText);
                UiList.Set(context, uiList);

                return m_Delegate.BeginInvoke(callback, state);
            }
            catch(Exception e)
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
        }

        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
        }
    }
}
