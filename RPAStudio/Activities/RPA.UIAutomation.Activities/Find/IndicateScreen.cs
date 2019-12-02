using Plugins.Shared.Library;
using Plugins.Shared.Library.UiAutomation;
using System;
using System.Activities;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;

namespace RPA.UIAutomation.Activities.Find
{
    [Designer(typeof(IndicateScreenDesigner))]
    public sealed class IndicateScreen : CodeActivity
    {
        public  new string DisplayName
        {
            get
            {
                return "Indicate On Screen";
            }
            set
            {

            }
        }

        [Category("公共")]
        [DisplayName("错误执行")]
        [Description("指定即使活动引发错误，自动化是否仍应继续,取值为（True或False）")]
        public InArgument<bool> ContinueOnError { get; set; }

        [Category("选项")]
        [DisplayName("隐藏预览窗口")]
        [Description("当选中一个ui元素的时候，隐藏前一个窗口")]
        public bool HidePreview { get; set; }

        [Category("选项")]
        [DisplayName("选择屏幕区域")]
        [Description("此选项允许在屏幕选中一块区域")]
        public bool SelectScreenRegion { get; set; }

        [Category("输出")]
        [DisplayName("Ui元素")]
        [Description("选择指定ui元素")]
        public OutArgument<UiElement> SelectedElement { get; set; }

        [Browsable(false)]
        public string ClassName { get { return "IndicateScreen"; } }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Find/IndicateScreen.png";
            }
        }

        UiElement uiElement = null;
        bool isSelectedFlag = false;
        bool isCanceledFlag = false;
        protected override void Execute(CodeActivityContext context)
        {
            // Dispatcher.CurrentDispatcher.Invoke(new Action(() => System.Windows.Application.Current.MainWindow.WindowState = System.Windows.WindowState.Minimized));
            //Dispatcher.CurrentDispatcher.Invoke(new Action(delegate
            //{
            //    UiElement.StartElementHighlight();
            //    //System.Windows.Application.Current.MainWindow.WindowState = System.Windows.WindowState.Minimized;
            //}));
            try
            {
                System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    UiElement.OnSelected = UiElement_OnSelected;
                    UiElement.OnCanceled = UiElement_OnCanceled;
                    UiElement.StartElementHighlight();
                });

                while (true)
                {
                    if (isSelectedFlag)
                    {
                        context.SetValue(SelectedElement, uiElement);
                        break;
                    }
                    if (isCanceledFlag)
                    {
                        break;
                    }
                    Thread.Sleep(200);
                }
                isSelectedFlag = false;
                isCanceledFlag = false;
            }
            catch(Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "屏幕截图失败", e.Message);
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


        private void UiElement_OnSelected(UiElement uiElement)
        {
            this.uiElement = uiElement;
            isSelectedFlag = true;
        }
        private void UiElement_OnCanceled()
        {
            isCanceledFlag = true;
        }
    }
}
