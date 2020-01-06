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
        [Localize.LocalizedCategory("Category1")] //公共 //Public //一般公開
        [Localize.LocalizedDisplayName("DisplayName1")] //错误执行 //Error execution //エラー実行
        [Localize.LocalizedDescription("Description40")] //指定即使活动引发错误，自动化是否仍应继续,取值为（True或False） //Specifies whether automation should continue even if the activity raises an error, with a value of (True or False) //アクティビティでエラーが発生した場合でも自動化を続行するかどうかを、値（TrueまたはFalse）で指定します
        public InArgument<bool> ContinueOnError { get; set; }

        [Localize.LocalizedCategory("Category2")] //UI对象 //UI Object //UIオブジェクト
        [OverloadGroup("Selector")]
        [RequiredArgument]
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName2")] //窗口指示器 //Window selector //ウィンドウインジケータ
        [Localize.LocalizedDescription("Description2")] //用于在执行活动时查找特定UI元素的Text属性 //The Text property used to find specific UI elements when performing activities //アクティビティの実行時に特定のUI要素を見つけるために使用されるTextプロパティ
        public InArgument<string> Selector { get; set; }

        [Localize.LocalizedCategory("Category2")] //UI对象 //UI Object //UIオブジェクト
        [OverloadGroup("Element")]
        [RequiredArgument]
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName3")] //UI元素 //UI Element //UI要素
        [Localize.LocalizedDescription("Description43")] //要关闭的窗口。该字段仅接受Window变量 //The window to close.  This field only accepts Window variables //閉じるウィンドウ。 このフィールドはウィンドウ変数のみを受け入れます
        public InArgument<UiElement> Element { get; set; }

        private int _UpLevels = 1;
        [Localize.LocalizedCategory("Category5")] //选项 //Option //オプション
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName45")] //父节点层次 //Parent node level //親ノードレベル
        [Localize.LocalizedDescription("Description50")] //指定要在ui层次结构的哪个级别查找父节点 //Specify which level of the ui hierarchy to find the parent node //親ノードを見つけるためのUI階層のレベルを指定する
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
        

        [Localize.LocalizedCategory("Category4")] //输出 //Output //出力
        [RequiredArgument]
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName46")] //父节点 //Parent node //親ノード
        [Localize.LocalizedDescription("Description51")] //元素的父节点。仅支持UIElement类型变量 //The parent of the element.  Only UIElement type variables are supported //要素の親。  UIElementタイプの変数のみがサポートされています
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
