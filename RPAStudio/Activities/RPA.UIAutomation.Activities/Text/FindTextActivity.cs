using System;
using System.Activities;
using System.ComponentModel;
using System.Windows;

namespace RPA.UIAutomation.Activities.Text
{
    [Designer(typeof(FindTextDesigner))]
    public sealed class FindTextActivity : CodeActivity
    {
        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "Find Text"; } }

        [Localize.LocalizedCategory("Category2")] //UI对象 //UI Object //UIオブジェクト
        [OverloadGroup("G1")]
        [RequiredArgument]
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName2")] //窗口指示器 //Window selector //ウィンドウインジケータ
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
        [Localize.LocalizedDisplayName("DisplayName49")] //字符串 //String //ひも
        [Localize.LocalizedDescription("Description59")] //要单击的字符串 //The string to click //クリックする文字列
        public InArgument<String> Text { get; set; }

        private InArgument<Int32> _Occurrence = 1;
        [Localize.LocalizedCategory("Category2")] //UI对象 //UI Object //UIオブジェクト
        [Browsable(true)]
        [Localize.LocalizedDisplayName("DisplayName50")] //指定次数 //Specified number //指定された番号
        [Localize.LocalizedDescription("Description61")] //如果文本字段中的字符串在指定的ui元素中出现多次，请在这里指定出现次数，而不是单击次数 //If the string in the text field appears multiple times in the specified ui element, specify the number of occurrences here instead of the number of clicks //テキストフィールドの文字列が指定されたui要素に複数回表示される場合、クリック数の代わりにここに出現回数を指定します
        public InArgument<Int32> Occurrence
        {
            get
            {
                return _Occurrence;
            }
            set
            {
                _Occurrence = value;
            }
        }


        //  [RequiredArgument]
        [Category("Common")]
        [Localize.LocalizedDescription("Description55")] //指定即使当前活动失败，也要继续执行其余的活动。只支持布尔值(True,False)。 //Specifies that the remaining activities will continue even if the current activity fails. Only Boolean values are supported. //現在のアクティビティが失敗した場合でも、アクティビティの残りを続行するように指定します。 ブール値（True、False）のみがサポートされています。
        public InArgument<bool> ContinueOnError { get; set; }

        private InArgument<Int32> _DelayAfter = 300;
        [Category("Common")]
        [Localize.LocalizedDescription("Description56")] //执行活动后的延迟时间(以毫秒为单位)。默认时间为300毫秒。 //The delay time, in milliseconds, after the activity is executed. The default time is 300 milliseconds. //アクティビティが実行された後のミリ秒単位の遅延。 デフォルトの時間は300ミリ秒です。
        public InArgument<Int32> DelayAfter
        {
            get
            {
                return _DelayAfter;
            }
            set
            {
                if (_DelayAfter == value) return;
                _DelayAfter = value;
            }
        }

        private InArgument<Int32> _DelayBefore = 200;
        [Category("Common")]
        [Localize.LocalizedDescription("Description60")] //延迟活动开始执行任何操作之前的时间(以毫秒为单位)。默认时间为200毫秒。 //The time (in milliseconds) before the deferred activity begins any operation.  The default time is 200 milliseconds. //遅延アクティビティが操作を開始するまでの時間（ミリ秒）。 デフォルトの時間は200ミリ秒です。
        public InArgument<Int32> DelayBefore
        {
            get
            {
                return _DelayBefore;
            }
            set
            {
                if (_DelayBefore == value) return;
                _DelayBefore = value;
            }
        }

        [Localize.LocalizedCategory("Category5")] //选项 //Option //オプション
        [Localize.LocalizedDescription("Description62")] //如果此选项被选中,所选文本的屏幕布局将保持不变 //If this option is checked, the screen layout of the selected text will remain unchanged //このオプションをオンにすると、選択したテキストの画面レイアウトは変更されません
        public InArgument<UIElement> Formatted { get; set; }

        [Localize.LocalizedCategory("Category4")] //输出 //Output //出力
        [Localize.LocalizedDisplayName("DisplayName3")] //UI元素 //UI Element //UI要素
        [Localize.LocalizedDescription("Description70")] //您要查找的字符串所在的ui元素。 //The ui element where the string you are looking for is located. //探している文字列が配置されているui要素。
        public bool UIElement { get; set; }

        [Browsable(false)]
        public string SourceImgPath { get; set; }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Text/exist.png";
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
