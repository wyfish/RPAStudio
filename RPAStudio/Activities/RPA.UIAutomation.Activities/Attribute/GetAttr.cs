using RPA.UIAutomation.Activities.Mouse;
using Plugins.Shared.Library;
using Plugins.Shared.Library.UiAutomation;
using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Identifiers;
using FlaUI.Core;

namespace RPA.UIAutomation.Activities.Attribute
{
    [Designer(typeof(GetAttrDesigner))]
    public sealed class GetAttr : CodeActivity
    {
        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "Get Attribute"; } }

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

        [Category("输入")]
        [RequiredArgument]
        [Browsable(true)]
        [DisplayName("属性")]
        [Description(" 要检索的属性的名称。该字段仅支持字符串")]
        public InArgument<string> AttrName { get; set; }

        [Category("输出")]
        [Browsable(true)]
        [DisplayName("结果")]
        [Description("指定属性的值")]
        public OutArgument<object> Result { get; set; }


        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Attribute/GetAttr.png";
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
                object attrValue = null;
                string attrName = AttrName.Get(context);

                UiElement element = Common.GetValueOrDefault(context, this.Element, null);
                if (element == null && selStr != null)
                {
                    element = UiElement.FromSelector(selStr);
                }
                AutomationElement autoEle = element.NativeObject as AutomationElement;
                FrameworkAutomationElementBase baseFrame = autoEle.FrameworkAutomationElement;
                PropertyId[] ids = autoEle.GetSupportedPropertiesDirect();
                for (int i=0; i< ids.Length; i++)
                {
                    if(String.Equals(ids[i].Name, attrName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        attrValue = baseFrame.GetPropertyValue(ids[i]);
                        break;
                    }
                }
                Result.Set(context, attrValue);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "获取元素属性失败", e.Message);
                if (ContinueOnError.Get(context))
                {
                    return;
                }
                else
                {
                    throw e;
                }
            }
        }
    }
}
