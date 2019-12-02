using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Plugins.Shared.Library;
using System.Windows;
using Plugins.Shared.Library.UiAutomation;
using System.Threading;
using FlaUI.Core.AutomationElements;

namespace RPA.UIAutomation.Activities.Control
{
    [Designer(typeof(SelectItemDesigner))]
    public sealed class SelectItem : CodeActivity
    {
        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "Select Item"; } }

        [Category("公共")]
        [DisplayName("错误执行")]
        [Description("指定即使活动引发错误，自动化是否仍应继续")]
        public InArgument<bool> ContinueOnError { get; set; }

        [Category("公共")]
        [DisplayName("延迟时间(结束)")]
        [Description("执行活动后的延迟时间(以毫秒为单位),默认时间为300毫秒")]
        public InArgument<Int32> DelayAfter { get; set; }

        [Category("公共")]
        [DisplayName("延迟时间(开始)")]
        [Description("活动开始执行任何操作之前的延迟时间(以毫秒为单位),默认的时间量是300毫秒。")]
        public InArgument<Int32> DelayBefore { get; set; }

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

        [Category("输入")]
        [RequiredArgument]
        [Browsable(true)]
        [DisplayName("项目")]
        public InArgument<string> Item { get; set; }

        [Browsable(false)]
        public string SourceImgPath{ get;set;}

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Control/SelectItem.png";
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
            string itemName = Item.Get(context);
            Int32 _delayAfter = GetValueOrDefault(context, this.DelayAfter, 300);
            Int32 _delayBefore = GetValueOrDefault(context, this.DelayBefore, 300);
            try
            {
                Thread.Sleep(_delayBefore);
                var selStr = Selector.Get(context);
                UiElement element = GetValueOrDefault(context, this.Element, null);
                if (element == null && selStr != null)
                {
                    element = UiElement.FromSelector(selStr);
                }
                if (element != null)
                {
                    if (element.IsNativeObjectAutomationElement)
                    {
                        var nativieObject = element.NativeObject as AutomationElement;
                        var comboBox = nativieObject.AsComboBox();
                        var comboItem = comboBox.Select(itemName);
                        if (comboItem.Text != itemName)
                        {
                            var lbox = nativieObject.AsListBox();
                            var listItem = lbox.Select(itemName);
                            if (listItem.Text != itemName)
                            {
                                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "SelectItem失败,请检查UI元素!");
                            }
                        }
                    }
                    else
                    {
                        SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", "非Window元素");
                        if (ContinueOnError.Get(context))
                        {
                        }
                        else
                        {
                            throw new NotImplementedException("非Window元素");
                        }
                    }
                }
                else
                {
                    SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", "查找不到元素");
                    if (ContinueOnError.Get(context))
                    {
                    }
                    else
                    {
                        throw new NotImplementedException("查找不到元素");
                    }
                }
                Thread.Sleep(_delayAfter);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "选取组合框或列表框项目失败", e.Message);
                if (ContinueOnError.Get(context))
                {
                }
                else
                {
                    throw;
                }
            }
        }
        private void onComplete(NativeActivityContext context, ActivityInstance completedInstance)
        {
            Console.WriteLine("ClickActivity onComplete");
        }
    }
}
