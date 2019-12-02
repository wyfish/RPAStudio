using System;
using System.Activities;
using System.ComponentModel;
using Plugins.Shared.Library;
using Plugins.Shared.Library.UiAutomation;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Threading;
using FlaUI.Core.AutomationElements;

namespace RPA.UIAutomation.Activities.Control
{
    [Designer(typeof(SelectMultiItemsDesigner))]
    public sealed class SelectMultiItems : CodeActivity
    {
        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "Select Multiltems"; } }

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
        [OverloadGroup("G2")]
        [Browsable(true)]
        [DisplayName("UI元素")]
        [Description("输入UIElement")]
        public InArgument<UiElement> Element { get; set; }

        [Category("输入")]
        [Browsable(true)]
        [DisplayName("添加到现有选择")]
        public bool AddToSelection { get; set; }

        [Category("输入")]
        [Browsable(true)]
        [DisplayName("项目组")]
        public InArgument<string[]> MultipleItems { get; set; }

        [Browsable(false)]
        public string SourceImgPath{ get;set;}

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Control/SelectMultiItems.png";
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
            string[] items = MultipleItems.Get(context);
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
                if(element != null)
                {
                    if (element.IsNativeObjectAutomationElement)
                    {
                        var nativieObject = element.NativeObject as AutomationElement;
                        var comboBox = nativieObject.AsComboBox();
                        for (int i = 0; i < items.Length; i++)
                        {
                            var comboItem = comboBox.Select(items[i]);
                            if (comboItem.Text != items[i])
                            {
                                var lbox = nativieObject.AsListBox();
                                for (int j = 0; j < items.Length; j++)
                                {
                                    var listItem = lbox.Select(items[j]);
                                    if (listItem.Text != items[j])
                                    {
                                        SharedObject.Instance.Output(SharedObject.enOutputType.Error, "SelectMutiItems失败,请检查UI元素!");
                                        break;
                                    }
                                }
                                break;
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
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", e.Message);
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
