using System;
using System.Activities;
using System.ComponentModel;
using System.Windows;

namespace RPA.UIAutomation.Activities.Text
{
    [Designer(typeof(ExtractDataDesigner))]
    public sealed class ExtractDataActivity : CodeActivity
    {
        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "Extract Data"; } }

        [Localize.LocalizedCategory("Category2")] //UI对象 //UI Object //UIオブジェクト
        [OverloadGroup("G1")]
        [RequiredArgument]
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName2")] //窗口指示器 //Window selector //セレクター
        [Localize.LocalizedDescription("Description2")] //用于在执行活动时查找特定UI元素的Text属性 //The Text property used to find specific UI elements when performing activities //アクティビティの実行時に特定のUI要素を見つけるために使用されるTextプロパティ
        public InArgument<string> Selector { get; set; }


        [Localize.LocalizedCategory("Category2")] //UI对象 //UI Object //UIオブジェクト
        [OverloadGroup("G2")]
        [RequiredArgument]
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName3")] //UI元素 //UI Element //UI要素
        [Localize.LocalizedDescription("Description43")] //要关闭的窗口。该字段仅接受Window变量 //The window to close.  This field only accepts Window variables //閉じるウィンドウ。 このフィールドはウィンドウ変数のみを受け入れます
        public InArgument<UIElement> ActiveWindow { get; set; }


        [RequiredArgument]
        [Localize.LocalizedCategory("Category2")] //UI对象 //UI Object //UIオブジェクト
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName34")] //文本 //Text //テキスト
        [Localize.LocalizedDescription("Description63")] //要单击的文本 //The text to click //クリックするテキスト
        public InArgument<String> Text { get; set; }


        [Category("Common")]
        [Localize.LocalizedDescription("Description55")] //指定即使当前活动失败，也要继续执行其余的活动。只支持布尔值(True,False)。 //Specifies that the remaining activities will continue even if the current activity fails. Only Boolean values are supported. //現在のアクティビティが失敗した場合でも、アクティビティの残りを続行するように指定します。 ブール値（True、False）のみがサポートされています。
        public InArgument<bool> ContinueOnError { get; set; }

        private InArgument<Int32> _DelayPage = 300;
        [Localize.LocalizedCategory("Category5")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName51")] //等待时间 //Waiting time //待ち時間
        [Localize.LocalizedDescription("Description64")] //等待加载到下一页的时间量 //The amount of time to wait for loading to the next page //次のページへのロードを待機する時間
        public InArgument<Int32> DelayPage
        {
            get
            {
                return _DelayPage;
            }
            set
            {
                if (_DelayPage == value) return;
                _DelayPage = value;
            }
        }

        private InArgument<Int32> _MaxNumber = 100;
        [Localize.LocalizedCategory("Category5")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName52")] //抽取最大值 //Extract maximum //最大抽出
        [Localize.LocalizedDescription("Description65")] //可以抽取数据的最大值 //The maximum value of the data that can be extracted //抽出できるデータの最大値
        public InArgument<Int32> MaxNumber
        {
            get
            {
                return _MaxNumber;
            }
            set
            {
                if (_MaxNumber == value) return;
                _MaxNumber = value;
            }
        }

        private InArgument<string> _NextSelector;
        [Localize.LocalizedCategory("Category5")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName53")] //下一个连接器 //Next connector //次のコネクタ
        [Localize.LocalizedDescription("Description66")] //选择器标识用于导航到下一页的链接/按钮。应该相对于现有的uielement属性 //The selector identifies the link/button used to navigate to the next page.  Should be relative to the existing uielement attribute //セレクタは、次のページに移動するために使用されるリンク/ボタンを識別します。 既存のuielement属性に関連する必要があります
        public InArgument<string> NextSelector
        {
            get
            {
                return _NextSelector;
            }
            set
            {
                _NextSelector = value;
            }
        }

        [Localize.LocalizedCategory("Category5")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName54")] //发送窗体消息 //Send a form message //フォームメッセージを送信する
        [Localize.LocalizedDescription("Description67")] //如果选中，单击用于导航到下一页的next link/按钮将通过向othe目标应用程序发送特定消息来执行。这种输入方法可以在后台工作，与大多数桌面应用程序兼容，但它不是最快的方法 //If checked, clicking the next link/ button for navigating to the next page will be performed by sending a specific message to the othe target application.  This input method works in the background and is compatible with most desktop applications, but it's not the fastest way //オンにすると、特定のメッセージをターゲットアプリケーションに送信して、次のページに移動するための次のリンク/ボタンをクリックします。 この入力方法はバックグラウンドで機能し、ほとんどのデスクトップアプリケーションと互換性がありますが、最速の方法ではありません
        public bool SendMessage { get; set; }

        private bool _SimulateClick = true;
        [Localize.LocalizedCategory("Category5")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName55")] //模拟点击 //Simulated click //シミュレートされたクリック
        [Localize.LocalizedDescription("Description68")] //如果选中，它将使用目标应用程序的技术模拟单击用于导航下一页的next链接/按钮。这种输入法是三种输入法中速度最快的一种，可以在后台工作 //If checked, it will use the technology simulation of the target application to click the next link/button used to navigate the next page.  This input method is the fastest of the three input methods and works in the background. //オンにすると、ターゲットアプリケーションのテクノロジーシミュレーションを使用して、次のページに移動するために使用される次のリンク/ボタンをクリックします。 この入力メソッドは、3つの入力メソッドの中で最も速く、バックグラウンドで機能します。
        public bool SimulateClick
        {
            get
            {
                return _SimulateClick;
            }
            set
            {
                _SimulateClick = value;
            }
        }

        [Localize.LocalizedCategory("Category5")] //选项 //Option //オプション
        [Localize.LocalizedDisplayName("DisplayName56")] //抽取目标数据 //Extract target data //対象データを抽出する
        [Localize.LocalizedDescription("Description69")] //允许您定义要从指定的web页面提取哪些数据的xml字符串 //An xml string that allows you to define which data to extract from the specified web page //指定したWebページから抽出するデータを定義できるXML文字列
        public InArgument<string> ExtractMetaData { get; set; }

        [Browsable(false)]
        public string SourceImgPath { get; set; }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Text/extract.png";
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

        protected override void Execute(CodeActivityContext context)
        {
        }
    }
}
