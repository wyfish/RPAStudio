using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Identifiers;
using RPA.UIAutomation.Activities.Mouse;
using Plugins.Shared.Library;
using Plugins.Shared.Library.UiAutomation;
using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace RPA.UIAutomation.Activities.Attribute
{
    [Designer(typeof(WaitAttrDesigner))]
    public sealed class WaitAttr : CodeActivity
    {
        public new string DisplayName
        {
            get
            {
                return "Wait Attribute";
            }
        }

        [Category("公共")]
        [DisplayName("错误执行")]
        [Description("指定即使活动引发错误，自动化是否仍应继续")]
        public InArgument<bool> ContinueOnError { get; set; }

        InArgument<Int32> _TimeOut = 10000;
        [Category("公共")]
        [Description("指定等待活动运行的时间量（以毫秒为单位）。默认值为10000毫秒（10秒）")]
        public InArgument<Int32> TimeOut
        {
            get
            {
                return _TimeOut;
            }
            set
            {
                _TimeOut = value;
            }
        }


        [Category("输入")]
        [RequiredArgument]
        [Browsable(true)]
        [DisplayName("UI元素")]
        [Description("使用另一个活动返回的UiElement变量。此属性不能与Selector属性一起使用。该字段仅支持UiElement变量")]
        public InArgument<UiElement> Element { get; set; }


        [Category("输入")]
        [RequiredArgument]
        [Browsable(true)]
        [DisplayName("属性")]
        [Description(" 要等待的属性的名称。预定义的属性列表可用作活动中的下拉列表。该字段仅支持String变量")]
        public InArgument<string> AttrName { get; set; }

        [Category("输入")]
        [Browsable(true)]
        [RequiredArgument]
        [DisplayName("属性值")]
        [Description("指定属性的预期值。该字段仅支持String变量")]
        public InArgument<object> AttrValue { get; set; }


        [Browsable(false)]
        public IEnumerable<AttributeEnums> AttrEnums
        {
            get
            {
                return Enum.GetValues(typeof(AttributeEnums)).Cast<AttributeEnums>();
            }
        }



        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Attribute/WaitAttr.png";
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
                string attrName = AttrName.Get(context);
                object attrValue = AttrValue.Get(context);
                Int32 timeOut = TimeOut.Get(context);
                bool isFoundFlag = false;

                UiElement element = Common.GetValueOrDefault(context, this.Element, null);
                AutomationElement autoEle = element.NativeObject as AutomationElement;
                FrameworkAutomationElementBase baseFrame = autoEle.FrameworkAutomationElement;
                PropertyId[] ids = autoEle.GetSupportedPropertiesDirect();
                PropertyId currentId = null;
                for (int i = 0; i < ids.Length; i++)
                {
                    if (String.Equals(ids[i].Name, attrName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        currentId = ids[i];
                        break;
                    }
                }
                for (int i = 0; i < timeOut / 1000; i++)
                {
                    if (attrValue == baseFrame.GetPropertyValue(currentId))
                    {
                        isFoundFlag = true;
                        break;
                    }
                    Thread.Sleep(1000);
                }
                if (!isFoundFlag && !ContinueOnError.Get(context))
                {
                    SharedObject.Instance.Output(SharedObject.enOutputType.Error, "相应元素的属性值未匹配，获取属性失败");
                    throw new Exception("获取属性失败，过程中断");
                }
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "等待获取元素属性过程出错", e.Message);
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
