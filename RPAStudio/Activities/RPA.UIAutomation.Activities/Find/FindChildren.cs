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


        [Localize.LocalizedCategory("Category1")] //公共 //Public //一般公開
        [Localize.LocalizedDisplayName("DisplayName1")] //错误执行 //Error execution //エラー実行
        [Localize.LocalizedDescription("Description40")] //指定即使活动引发错误，自动化是否仍应继续,取值为（True或False） //Specifies whether automation should continue even if the activity raises an error, with a value of (True or False) //アクティビティでエラーが発生した場合でも自動化を続行するかどうかを、値（TrueまたはFalse）で指定します
        public InArgument<bool> ContinueOnError { get; set; }

        public enum ScopeOption
        {
            Children,
            Descendants,
            Subtree
        }
        
        ScopeOption _Scope = ScopeOption.Children;
        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName38")] //范围 //range //範囲
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

        [Localize.LocalizedCategory("Category2")] //UI对象 //UI Object //UIオブジェクト
        [OverloadGroup("Selector")]
        [RequiredArgument]
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName2")] //窗口指示器 //Window selector //セレクター
        [Localize.LocalizedDescription("Description2")] //用于在执行活动时查找特定UI元素的Text属性 //The Text property used to find specific UI elements when performing activities //アクティビティの実行時に特定のUI要素を見つけるために使用されるTextプロパティ
        public InArgument<string> Selector { get; set; }


        [Localize.LocalizedCategory("Category2")] //UI对象 //UI Object //UIオブジェクト
        [OverloadGroup("Element")]
        [RequiredArgument]
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName3")] //UI元素 //UI Element //UI要素
        [Localize.LocalizedDescription("Description43")] //要关闭的窗口。该字段仅接受Window变量 //The window to close.  This field only accepts Window variables //閉じるウィンドウ。 このフィールドはウィンドウ変数のみを受け入れます
        public InArgument<UiElement> Element { get; set; }

      
        [RequiredArgument]
        [OverloadGroup("FilterText")]
        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName39")] //过滤器 //filter //フィルター
        [Localize.LocalizedDescription("Description44")] //xml字符串指定集合中的所有UI对象应该满足的条件 //The xml string specifies the conditions that all UI Objects in the collection should satisfy. //xml文字列は、コレクション内のすべてのUIオブジェクトが満たすべき条件を指定します。
        public InArgument<string> FilterText { get; set; }

        [Localize.LocalizedCategory("Category4")] //输出 //Output //出力
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName40")] //子元素 //Child element //子要素
        [Localize.LocalizedDescription("Description45")] //所有ui子元素都将按照设置的过滤器和范围。此字段只支持IEnumerable<UIElement>变量 //All ui child elements will follow the set filter and scope.  This field only supports IEnumerable<UIElement> variables //すべてのui子要素は、設定されたフィルターとスコープに従います。 このフィールドはIEnumerable <UIElement>変数のみをサポートします
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
