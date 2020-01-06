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

        [Localize.LocalizedCategory("Category1")] //公共 //Public //一般公開
        [Localize.LocalizedDisplayName("DisplayName1")] //错误执行 //Error execution //エラー実行
        [Localize.LocalizedDescription("Description40")] //指定即使活动引发错误，自动化是否仍应继续,取值为（True或False） //Specifies whether automation should continue even if the activity raises an error, with a value of (True or False) //アクティビティでエラーが発生した場合でも自動化を続行するかどうかを、値（TrueまたはFalse）で指定します
        public InArgument<bool> ContinueOnError { get; set; }

        [Localize.LocalizedCategory("Category5")] //选项 //Option //オプション
        [DisplayName("隐藏预览窗口")]
        [Localize.LocalizedDescription("Description52")] //当选中一个ui元素的时候，隐藏前一个窗口 //Hide the previous window when a ui element is selected //UI要素が選択されたときに前のウィンドウを非表示にする
        public bool HidePreview { get; set; }

        [Localize.LocalizedCategory("Category5")] //选项 //Option //オプション
        [DisplayName("选择屏幕区域")]
        [Localize.LocalizedDescription("Description53")] //此选项允许在屏幕选中一块区域 //This option allows an area to be selected on the screen. //このオプションを使用すると、画面上の領域を選択できます。
        public bool SelectScreenRegion { get; set; }

        [Localize.LocalizedCategory("Category4")] //输出 //Output //出力
        [DisplayName("Ui元素")]
        [Localize.LocalizedDescription("Description54")] //选择指定ui元素 //Select the specified ui element //指定されたUI要素を選択します
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
