using FlaUI.Core.AutomationElements;
using RPA.UIAutomation.Activities.Mouse;
using Plugins.Shared.Library;
using Plugins.Shared.Library.UiAutomation;
using System;
using System.Activities;
using System.ComponentModel;
using System.Windows;

namespace RPA.UIAutomation.Activities.Find
{
    [Designer(typeof(EleExistDesigner))]
    public sealed class EleExists : CodeActivity
    {
        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "Element Exists"; } }

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
        [Description("输入UIElement")]
        public InArgument<UiElement> Element { get; set; }

        [Category("输出")]
        [Browsable(true)]
        [RequiredArgument]
        [DisplayName("存在性")]
        [Description("指示元素是否存在。该字段只支持布尔变量")]
        public OutArgument<bool> IsExist { get; set; }

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
        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                var selStr = Selector.Get(context);
                UiElement element = Common.GetValueOrDefault(context, this.Element, null);
                if (element == null && selStr != null)
                {
                    element = UiElement.FromSelector(selStr);
                }
                if (element == null)
                    context.SetValue(IsExist, false);
                else
                    context.SetValue(IsExist, true);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "检测元素是否存在过程出错", e.Message);
            }
        }
    }
}
