using System;
using System.Activities;
using System.ComponentModel;
using Plugins.Shared.Library;
using Plugins.Shared.Library.UiAutomation;
using FlaUI.Core.AutomationElements;

namespace RPA.UIAutomation.Activities.Control
{
    [Designer(typeof(GetTextDesigner))]
    public sealed class GetText : CodeActivity
    {
        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "Get Text"; } }
       
        [Category("选项")]
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


        [Category("输出")]
        [RequiredArgument]
        [Browsable(true)]
        [DisplayName("值")]
        public OutArgument<string> Value { get; set; }

        [Browsable(false)]
        public string SourceImgPath{ get;set;}

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Control/GetText.png";
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
            string buff = "";
            try
            {
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
                        var comboBox = nativieObject.AsTextBox();
                        buff = comboBox.Text;
                        if (Value != null)
                            Value.Set(context, buff);
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
            }
            //catch(Exception)
            //{
            //    SharedObject.Instance.Output(SharedObject.enOutputType.Error, "PatternNotSupportedException");
            //}
            //catch (FlaUI.Core.Exceptions.MethodNotSupportedException)
            //{
            //    DataGridView gridView = element.AsDataGridView();
            //    AutomationElement[] child = gridView.FindAllChildren();
            //    for (int i = 0; i < child.Length; i++)
            //    {
            //        string nameStr = child[i].Name;
            //        buff += nameStr;
            //    }
            //    if (Value != null)
            //        Value.Set(context, buff);
            //}
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "获取UI元素文本失败", e.Message);
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
