using RPA.UIAutomation.Activities.Mouse;
using Plugins.Shared.Library;
using Plugins.Shared.Library.UiAutomation;
using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows;

namespace RPA.UIAutomation.Activities.Attribute
{
    [Designer(typeof(TakeScreenShotDesigner))]
    public sealed class TakeScreenShot : CodeActivity
    {
        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "Take ScreenShot"; } }

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


        [Browsable(false)]
        public IEnumerable<AttributeEnums> AttrEnums
        {
            get
            {
                return Enum.GetValues(typeof(AttributeEnums)).Cast<AttributeEnums>();
            }
        }

        InArgument<Int32> _WaitBefore = 300;
        [Category("选项")]
        [Browsable(true)]
        [DisplayName("执行等待")]
        [Description("获取指定UI元素的屏幕截图之前的延迟时间（以毫秒为单位）。默认时间为300毫秒。")]
        public InArgument<Int32> WaitBefore
        {
            get
            {
                return _WaitBefore;
            }
            set
            {
                _WaitBefore = 300;
            }
        }

        [Category("输出")]
        [Browsable(true)]
        [DisplayName("截图")]
        [Description("结果截图。该字段仅支持图像变量")]
        public InArgument<Bitmap> Image { get; set; }


        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Attribute/TakeScreenShot.png";
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

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);
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
                Bitmap bit = element.CaptureInformativeScreenshot();
                Image.Set(context, bit);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "屏幕截图失败", e.Message);
                if (ContinueOnError.Get(context))
                {

                }
                else
                {
                    throw e;
                }
            }
        }
    }
}
