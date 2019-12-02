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


        [Category("公共")]
        [DisplayName("错误执行")]
        [Description("指定即使活动引发错误，自动化是否仍应继续")]
        public InArgument<bool> ContinueOnError { get; set; }

        [Category("UI对象")]
        [OverloadGroup("Selector")]
        [RequiredArgument]
        [Browsable(true)]
        [DisplayName("窗口指示器")]
        [Description("用于在执行活动时查找特定UI元素的Text属性")]
        public InArgument<string> Selector { get; set; }


        [Category("UI对象")]
        [OverloadGroup("G2")]
        [RequiredArgument]
        [Browsable(true)]
        [DisplayName("UI元素")]
        [Description("UiElement变量。此属性不能与Selector属性一起使用。该字段仅支持UiElement变量")]
        public InArgument<UiElement> Element { get; set; }


        [Category("输出")]
        [Browsable(true)]
        [DisplayName("UI元素")]
        [Description("在后续子活动中使用的UI元素")]
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
