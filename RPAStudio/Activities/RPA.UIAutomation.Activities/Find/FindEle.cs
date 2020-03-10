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

        [Localize.LocalizedCategory("Category1")] //公共 //Public //一般公開
        [Localize.LocalizedDisplayName("DisplayName1")] //错误执行 //Error execution //エラー実行
        [Localize.LocalizedDescription("Description40")] //指定即使活动引发错误，自动化是否仍应继续,取值为（True或False） //Specifies whether automation should continue even if the activity raises an error, with a value of (True or False) //アクティビティでエラーが発生した場合でも自動化を続行するかどうかを、値（TrueまたはFalse）で指定します
        public InArgument<bool> ContinueOnError { get; set; }

        [Localize.LocalizedCategory("Category5")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName41")] //等待活动 //Waiting for activity //アクティビティを待っています
        [Localize.LocalizedDescription("Description46")] //当此选项被选中，该活动将等待指定的ui元素成为活动 //When this option is selected, the activity will wait for the specified ui element to become active. //このオプションを選択すると、アクティビティは指定されたui要素がアクティブになるのを待ちます。
        public bool WaitActive { get; set; }

        [Localize.LocalizedCategory("Category5")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName42")] //等待可见 //Waiting for visible //見えるまで待っています
        [Localize.LocalizedDescription("Description47")] //当此选项被选中，该活动将等待指定的ui元素成为可见 //When this option is selected, the activity will wait for the specified ui element to become visible. //このオプションを選択すると、アクティビティは指定されたui要素が表示されるまで待機します。
        public bool WaitVisible { get; set; }

        [Localize.LocalizedCategory("Category2")] //UI对象 //UI Object //UIオブジェクト
        [OverloadGroup("Selector")]
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


        [Localize.LocalizedCategory("Category4")] //输出 //Output //出力
        [RequiredArgument]
        [Browsable(true)]
        [DisplayName("Ui元素")]
        [Localize.LocalizedDescription("Description48")] //找到的ui元素。此字段仅支持UIElement类型变量 //The ui element found.  This field only supports UIElement type variables //見つかったui要素。 このフィールドはUIElementタイプの変数のみをサポートします
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
