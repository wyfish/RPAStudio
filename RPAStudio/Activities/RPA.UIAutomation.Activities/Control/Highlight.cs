using System;
using System.Activities;
using System.ComponentModel;
using Plugins.Shared.Library;
using System.Drawing;
using Plugins.Shared.Library.UiAutomation;
using System.Threading;

namespace RPA.UIAutomation.Activities.Control
{
    [Designer(typeof(HighlightDesigner))]
    public sealed class Highlight : CodeActivity
    {
        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "Highlight"; } }

        [Category("公共")]
        [DisplayName("错误执行")]
        [Description("指定即使活动引发错误，自动化是否仍应继续")]
        public InArgument<bool> ContinueOnError { get; set; }

        [Category("UI对象")]
        [OverloadGroup("G1")]
        [RequiredArgument]
        [Browsable(true)]
        [DisplayName("窗口指示器")]
        [Description("用于在执行活动时查找特定UI元素的Text属性")]
        public InArgument<string> Selector { get; set; }

        [Category("UI对象")]
        [OverloadGroup("G1")]
        [Browsable(true)]
        [DisplayName("UI元素")]
        [Description("输入UIElement")]
        public InArgument<UiElement> Element { get; set; }

        [Category("选项")]
        [Browsable(true)]
        [DisplayName("颜色")]
        public InArgument<Color> Value { get; set; }

        [Category("选项")]
        [Browsable(true)]
        [DisplayName("高亮时间(秒)")]
        public InArgument<Int32> HighlightTime { get; set; }

        [Browsable(false)]
        public string SourceImgPath{ get;set;}

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Control/Highlight.png";
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

        public static T GetValueOrDefault<T>(ActivityContext context, InArgument<T> source, T defaultValue)
        {
            T result = defaultValue;
            if (source != null && source.Expression != null)
            {
                result = source.Get(context);
            }
            return result;
        }

        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                Int32 _HighlightTime = GetValueOrDefault(context, HighlightTime, 1);
                Color _Value = GetValueOrDefault(context, Value, Color.Red);
                var selStr = Selector.Get(context);
                UiElement element = GetValueOrDefault(context, this.Element, null);
                if (element == null && selStr != null)
                {
                    element = UiElement.FromSelector(selStr);
                }
                if(element != null)
                {
                    element.DrawHighlight(_Value, TimeSpan.FromSeconds(_HighlightTime), true);
                    Thread.Sleep(_HighlightTime*1);
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
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "UI元素设置高亮失败", e.Message);
                if (ContinueOnError.Get(context))
                {
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
