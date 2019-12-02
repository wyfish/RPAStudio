using System;
using System.Activities;
using System.ComponentModel;
using Plugins.Shared.Library;
using Plugins.Shared.Library.UiAutomation;
using System.Threading;

namespace RPA.UIAutomation.Activities.Mouse
{
    [Designer(typeof(MouseDesigner))]
    public sealed class HoverClickActivity : CodeActivity
    {
        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "Hover Click"; } }

        [Category("UI对象")]
        [OverloadGroup("G1")]
        [Browsable(true)]
        [DisplayName("窗口指示器")]
        [Description("用于在执行活动时查找特定UI元素的Text属性")]
        public InArgument<string> Selector { get; set; }

        [Category("UI对象")]
        [OverloadGroup("G2")]
        [Browsable(true)]
        [DisplayName("UI元素")]
        [Description("输入UIAutomationInfo")]
        public InArgument<UiElement> Element { get; set; }

        [Category("UI元素矩阵")]
        [Browsable(true)]
        public InArgument<Int32> Left { get; set; }
        [Category("UI元素矩阵")]
        [Browsable(true)]
        public InArgument<Int32> Right { get; set; }
        [Category("UI元素矩阵")]
        [Browsable(true)]
        public InArgument<Int32> Top { get; set; }
        [Category("UI元素矩阵")]
        [Browsable(true)]
        public InArgument<Int32> Bottom { get; set; }

        [Category("Common")]
        [Description("指定即使当前活动失败，也要继续执行其余的活动。只支持布尔值(True,False)。")]
        public InArgument<bool> ContinueOnError { get; set; }

        [Category("Common")]
        [Description("执行活动后的延迟时间(以毫秒为单位)。默认时间为300毫秒。")]
        public InArgument<Int32> DelayAfter { get; set;}

        [Category("Common")]
        [Description("延迟活动开始执行任何操作之前的时间(以毫秒为单位)。默认时间为300毫秒。")]
        public InArgument<Int32> DelayBefore { get; set; }


        [Category("Input")]
        public InArgument<Int32> offsetX { get; set; }
        [Category("Input")]
        public InArgument<Int32> offsetY { get; set; }

        [Category("Input")]
        [DisplayName("使用坐标点")]
        public bool usePoint { get; set; }

        [Browsable(false)]
        public string SourceImgPath { get; set; }
        [Browsable(false)]
        public string icoPath { get { return "pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Mouse/hover.png"; } }

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

        protected override void Execute(CodeActivityContext context)
        {
            Int32 _delayAfter = Common.GetValueOrDefault(context, this.DelayAfter, 300);
            Int32 _delayBefore = Common.GetValueOrDefault(context, this.DelayBefore, 300);
            try
            {
                Thread.Sleep(_delayBefore);
                var selStr = Selector.Get(context);
                UiElement element = Common.GetValueOrDefault(context, this.Element, null);
                if (element == null && selStr != null)
                {
                    element = UiElement.FromSelector(selStr);
                }

                Int32 pointX = 0;
                Int32 pointY = 0;
                if (usePoint)
                {
                    pointX = offsetX.Get(context);
                    pointY = offsetY.Get(context);
                }
                else
                {
                    if (element != null)
                    {
                        pointX = element.GetClickablePoint().X;
                        pointY = element.GetClickablePoint().Y;
                        element.SetForeground();
                    }
                    else
                    {
                        SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", "查找不到元素");
                        if (ContinueOnError.Get(context))
                        {
                            return;
                        }
                        else
                        {
                            throw new NotImplementedException("查找不到元素");
                        }
                    }
                }
                UiElement.MouseMoveTo(pointX, pointY);
                Thread.Sleep(_delayAfter);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", e.Message);
                if (ContinueOnError.Get(context))
                {
                }
                else
                {
                    throw new NotImplementedException("查找不到元素");
                }
            }
        }
        private void onComplete(NativeActivityContext context, ActivityInstance completedInstance)
        {
           
        }
    }
}
