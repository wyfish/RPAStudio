using FlaUI.Core.AutomationElements;
using RPA.UIAutomation.Activities.Mouse;
using Plugins.Shared.Library;
using Plugins.Shared.Library.UiAutomation;
using System;
using System.Activities;
using System.ComponentModel;
using System.Threading;

namespace RPA.UIAutomation.Activities.Find
{
    [Designer(typeof(EleVanishDesigner))]
    public sealed class EleVanish : AsyncCodeActivity
    {
        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "Element Vanish"; } }

        [Localize.LocalizedCategory("Category1")] //公共 //Public //一般公開
        [Localize.LocalizedDisplayName("DisplayName1")] //错误执行 //Error execution //エラー実行
        [Localize.LocalizedDescription("Description40")] //指定即使活动引发错误，自动化是否仍应继续,取值为（True或False） //Specifies whether automation should continue even if the activity raises an error, with a value of (True or False) //アクティビティでエラーが発生した場合でも自動化を続行するかどうかを、値（TrueまたはFalse）で指定します
        public InArgument<bool> ContinueOnError { get; set; }

        [Localize.LocalizedCategory("Category5")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName36")] //等待元素停止 //Waiting for element to stop //要素が停止するのを待っています
        [Localize.LocalizedDescription("Description41")] //当此选项被选中，该活动将在指定活动结束前一直等待 //When this option is selected, the activity will wait until the end of the specified event //このオプションを選択すると、アクティビティは指定されたイベントの終わりまで待機します
        public bool WaitNotActivity { get; set; }

        [Localize.LocalizedCategory("Category5")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName37")] //等待元素消失 //Waiting for element to disappear //要素が消えるのを待っています
        [Localize.LocalizedDescription("Description42")] //当此选项被选中，即使ui元素仍然是活动的，该活动也只会等到ui元素从屏幕上消失 //When this option is selected, even if the ui element is still active, the activity will only wait until the ui element disappears from the screen. //このオプションを選択すると、ui要素がまだアクティブであっても、アクティビティは画面からui要素が消えるまで待機するだけです。
        public bool WaitNotVisible { get; set; }

        [Localize.LocalizedCategory("Category2")] //UI对象 //UI Object //UIオブジェクト
        [OverloadGroup("Selector")]
        [RequiredArgument]
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName2")] //窗口指示器 //Window selector //ウィンドウインジケータ
        [Localize.LocalizedDescription("Description2")] //用于在执行活动时查找特定UI元素的Text属性 //The Text property used to find specific UI elements when performing activities //アクティビティの実行時に特定のUI要素を見つけるために使用されるTextプロパティ
        public InArgument<string> Selector { get; set; }


        [Localize.LocalizedCategory("Category2")] //UI对象 //UI Object //UIオブジェクト
        [OverloadGroup("Element")]
        [RequiredArgument]
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName3")] //UI元素 //UI Element //UI要素
        [Localize.LocalizedDescription("Description43")] //要关闭的窗口。该字段仅接受Window变量 //The window to close.  This field only accepts Window variables //閉じるウィンドウ。 このフィールドはウィンドウ変数のみを受け入れます
        public InArgument<UiElement> Element { get; set; }


        [Browsable(false)]
        public string SourceImgPath { get; set; }
       

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Find/EleVanish.png";
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


        [Browsable(false)]
        public string ClassName { get { return "FindChildren"; } }
        private delegate string runDelegate();
        private runDelegate m_Delegate;
        public string Run()
        {
            return ClassName;
        }


        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            try
            {
                m_Delegate = new runDelegate(Run);

                UiElement element = null;
                var selStr = Selector.Get(context);

                element = Common.GetValueOrDefault(context, this.Element, null);
                if (element == null && selStr != null)
                {
                    element = UiElement.FromSelector(selStr);
                }
                else
                {
                    PropertyDescriptor property = context.DataContext.GetProperties()[EleScope.GetEleScope];
                    element = property.GetValue(context.DataContext) as UiElement;
                }

                AutomationElement autoEle = element.NativeObject as AutomationElement;
                while(true)
                {
                    if(WaitNotActivity && WaitNotVisible)
                    {
                        //不可见 不活动 逻辑
                    }
                    else if(WaitNotActivity)
                    {
                        //不活动逻辑
                    }
                    else if(WaitNotVisible)
                    {
                        //不可见逻辑
                    }
                    else
                    {
                        if (!autoEle.IsEnabled)
                            break;
                    }
                    Thread.Sleep(500);
                }

                return m_Delegate.BeginInvoke(callback, state);

            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "等待元素消失出错", e.Message);
                if (ContinueOnError.Get(context))
                {
                    return m_Delegate.BeginInvoke(callback, state);
                }
                else
                {
                    throw e;
                }
            }
        }

        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
            throw new NotImplementedException();
        }
    }
}
