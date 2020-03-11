using System;
using System.Activities;
using System.ComponentModel;
using System.Activities.Statements;
using System.Windows;
using Plugins.Shared.Library.UiAutomation;
using Plugins.Shared.Library;
using RPA.UIAutomation.Activities.Mouse;

namespace RPA.UIAutomation.Activities.Find
{
    [Designer(typeof(EleScopeDesigner))]
    public sealed class EleScope : NativeActivity
    {
        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "Element Scope"; } }

        public EleScope()
        {
            Body = new ActivityAction<object>
            {
                Argument = new DelegateInArgument<object>(GetEleScope),
                Handler = new Sequence()
                {
                    DisplayName = "Do"
                }
            };
        }

        public static string GetEleScope { get { return "GetEleScope"; } }


        [Browsable(false)]
        public ActivityAction<object> Body { get; set; }


        [Localize.LocalizedCategory("Category1")] //公共 //Public //一般公開
        [Localize.LocalizedDisplayName("DisplayName1")] //错误执行 //Error execution //エラー実行
        [Localize.LocalizedDescription("Description1")] //指定即使活动引发错误，自动化是否仍应继续 //Specifies whether automation should continue even if the activity raises an error //アクティビティでエラーが発生した場合でも自動化を続行するかどうかを指定します
        public InArgument<bool> ContinueOnError { get; set; }

        [Localize.LocalizedCategory("Category2")] //UI对象 //UI Object //UIオブジェクト
        [OverloadGroup("Selector")]
        [RequiredArgument]
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName2")] //窗口指示器 //Window selector //セレクター
        [Localize.LocalizedDescription("Description2")] //用于在执行活动时查找特定UI元素的Text属性 //The Text property used to find specific UI elements when performing activities //アクティビティの実行時に特定のUI要素を見つけるために使用されるTextプロパティ
        public InArgument<string> Selector { get; set; }


        [Localize.LocalizedCategory("Category2")] //UI对象 //UI Object //UIオブジェクト
        [OverloadGroup("G2")]
        [RequiredArgument]
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName3")] //UI元素 //UI Element //UI要素
        [Localize.LocalizedDescription("Description38")] //UiElement变量。此属性不能与Selector属性一起使用。该字段仅支持UiElement变量 //UiElement variable.  This property cannot be used with the Selector property.  This field only supports UiElement variables //UiElement変数。 このプロパティをSelectorプロパティと一緒に使用することはできません。 このフィールドはUiElement変数のみをサポートします
        public InArgument<UiElement> Element { get; set; }


        [Localize.LocalizedCategory("Category4")] //输出 //Output //出力
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName3")] //UI元素 //UI Element //UI要素
        [Localize.LocalizedDescription("Description39")] //在后续子活动中使用的UI元素 //UI elements used in subsequent child activities //後続の子アクティビティで使用されるUI要素
        public OutArgument<UIElement> OutUiElement { get; set; }


        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Find/EleScope.png";
            }
        }

        [Browsable(false)]
        public string SourceImgPath { get; set; }

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

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);
        }

        protected override void Execute(NativeActivityContext context)
        {
            UiElement element = null;
            try
            {
                var selStr = Selector.Get(context);
                element = Common.GetValueOrDefault(context, this.Element, null);
                if (element == null && selStr != null)
                    element = UiElement.FromSelector(selStr);
                if(Body != null)
                    context.ScheduleAction(Body, element, OnCompleted, OnFaulted);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "元素范围流程失败", e.Message);
                if (ContinueOnError.Get(context))
                {
                    context.ScheduleAction(Body, element, OnCompleted, OnFaulted);
                }
                else
                {
                    throw e;
                }
            }
        }

        private void OnFaulted(NativeActivityFaultContext faultContext, Exception propagatedException, ActivityInstance propagatedFrom)
        {
            //TODO
        }

        private void OnCompleted(NativeActivityContext context, ActivityInstance completedInstance)
        {
            //TODO
        }
    }
}
