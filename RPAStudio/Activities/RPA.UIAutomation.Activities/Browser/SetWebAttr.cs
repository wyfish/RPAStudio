using System;
using System.Activities;
using System.ComponentModel;
using Plugins.Shared.Library;
using System.Threading;
using System.Windows;
using Plugins.Shared.Library.UiAutomation;

namespace RPA.UIAutomation.Activities.Browser
{
    [Designer(typeof(SetWebAttrDesigner))]
    public sealed class SetWebAttr : CodeActivity
    {
        [Localize.LocalizedCategory("Category1")] //公共 //Public //一般公開
        [Localize.LocalizedDisplayName("DisplayName1")] //错误执行 //Error execution //エラー実行
        [Localize.LocalizedDescription("Description1")] //指定即使活动引发错误，自动化是否仍应继续 //Specifies whether automation should continue even if the activity raises an error //アクティビティでエラーが発生した場合でも自動化を続行するかどうかを指定します
        public InArgument<bool> ContinueOnError { get; set; }

        [Localize.LocalizedCategory("Category1")] //公共 //Public //一般公開
        [Localize.LocalizedDisplayName("DisplayName17")] //延迟时间(结束) //Delay time (end) //遅延時間（終了）
        [Localize.LocalizedDescription("Description30")] //执行活动后的延迟时间(以毫秒为单位),默认时间为300毫秒 //The delay in milliseconds after the activity is executed. The default time is 300 milliseconds. //アクティビティが実行された後のミリ秒単位の遅延デフォルトの時間は300ミリ秒です。
        public InArgument<Int32> DelayAfter { get; set; }

        [Localize.LocalizedCategory("Category1")] //公共 //Public //一般公開
        [Localize.LocalizedDisplayName("DisplayName18")] //延迟时间(开始) //Delay time (start) //遅延時間（開始）
        [Localize.LocalizedDescription("Description31")] //活动开始执行任何操作之前的延迟时间(以毫秒为单位),默认的时间量是200毫秒。 //The delay (in milliseconds) before the activity begins any operation, the default amount of time is 200 milliseconds. //アクティビティが操作を開始するまでの遅延（ミリ秒）。デフォルトの時間は200ミリ秒です。
        public InArgument<Int32> DelayBefore { get; set; }


        [Localize.LocalizedCategory("Category2")] //UI对象 //UI Object //UIオブジェクト
        [OverloadGroup("G1")]
        [RequiredArgument]
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName2")] //窗口指示器 //Window selector //ウィンドウインジケータ
        [Localize.LocalizedDescription("Description2")] //用于在执行活动时查找特定UI元素的Text属性 //The Text property used to find specific UI elements when performing activities //アクティビティの実行時に特定のUI要素を見つけるために使用されるTextプロパティ
        public InArgument<string> Selector { get; set; }

        [Localize.LocalizedCategory("Category2")] //UI对象 //UI Object //UIオブジェクト
        [Localize.LocalizedDisplayName("DisplayName12")] //超时时间 //overtime time //タイムアウト
        [Localize.LocalizedDescription("Description15")] //指定在引发错误之前等待活动运行的时间量（以毫秒为单位）。默认值为30000毫秒（30秒） //Specifies the amount of time, in milliseconds, to wait for an activity to run before an error is raised.  The default is 30000 milliseconds (30 seconds) //エラーが発生する前にアクティビティの実行を待機する時間をミリ秒単位で指定します。 デフォルトは30000ミリ秒（30秒）です
        public InArgument<Int32> TimeoutMS { get; set; }

        [Localize.LocalizedCategory("Category2")] //UI对象 //UI Object //UIオブジェクト
        [OverloadGroup("G1")]
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName3")] //UI元素 //UI Element //UI要素
        [Localize.LocalizedDescription("Description3")] //输入UIElement //Enter UIElement //UIElementを入力
        public InArgument<UiElement> Element { get; set; }

        InArgument<string> _Attribute;
        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [Localize.LocalizedDisplayName("DisplayName26")] //HTML属性 //HTML attribute //HTML属性
        [Localize.LocalizedDescription("Description32")] //要更改的HTML属性的名称 //The name of the HTML attribute to change //変更するHTML属性の名前
        [RequiredArgument]
        [Browsable(true)]
        public InArgument<string> Attribute
        {
            get
            {
                return _Attribute;
            }
            set
            {
                _Attribute = value;
            }
        }

        InArgument<string> _Value;
        [Localize.LocalizedCategory("Category3")] //输入 //Enter //入力
        [Localize.LocalizedDisplayName("DisplayName27")] //值 //value //価値
        [RequiredArgument]
        [Localize.LocalizedDescription("Description33")] //要设置为指定属性的值。仅支持字符串变量 //The value to be set to the specified property.  Only string variables are supported //指定したプロパティに設定する値。 文字列変数のみがサポートされています
        [Browsable(true)]
        public InArgument<string> Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
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

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Browser/SetWebAttr.png";
            }
        }

        CountdownEvent latch;
        private void refreshData(CountdownEvent latch)
        {
            latch.Signal();
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
            string attribute_str = Attribute.Get(context);
            string attribute_value = Value.Get(context);
            try
            {
                Int32 _timeout = TimeoutMS.Get(context);
                Thread.Sleep(_timeout);
                latch = new CountdownEvent(1);
                Thread td = new Thread(() =>
                {
                    if (Selector.Expression == null)
                    {
                        //ActiveElement处理
                    }
                    else
                    {
                        var selStr = Selector.Get(context);
                        UiElement element = GetValueOrDefault(context, this.Element, null);
                        if (element == null && selStr != null)
                        {
                            element = UiElement.FromSelector(selStr);
                        }
                        if(element != null)
                        {
                            element.SetForeground();
                            mshtml.IHTMLDocument2 currDoc = null;
                            SHDocVw.InternetExplorer ieBrowser = GetIEFromHWndClass.GetIEFromHWnd((int)element.WindowHandle, out currDoc);
                            mshtml.IHTMLElement currEle = GetIEFromHWndClass.GetEleFromDoc(
                                element.GetClickablePoint(), (int)element.WindowHandle, currDoc);
                            currEle.setAttribute(attribute_str, attribute_value);
                        }
                        else
                        {
                            SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", "查找不到元素");
                            if (ContinueOnError.Get(context))
                            {
                                return;
                            }
                            else
                            {
                                throw new NotImplementedException("查找不到元素");
                            }
                        }
                    }
                    refreshData(latch);
                });
                td.TrySetApartmentState(ApartmentState.STA);
                td.IsBackground = true;
                td.Start();
                latch.Wait();
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "出现异常", e.Message);
                if (ContinueOnError.Get(context))
                {

                }
            }
        }
    }
}
